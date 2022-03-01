using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using RoverCore.Boilerplate.Domain.DTOs.Authentication;
using RoverCore.Boilerplate.Domain.Entities.Identity;
using RoverCore.Boilerplate.Infrastructure.Persistence.DbContexts;
using Serviced;

namespace RoverCore.Boilerplate.Infrastructure.Authentication.Services;

public class UserService : IUserService, IScoped<IUserService>
{
    private readonly JWTSettings _appSettings;
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public UserService(IOptions<JWTSettings> appSettings, ApplicationDbContext context, UserManager<ApplicationUser> userManager)
    {
        _appSettings = appSettings.Value;
        _context = context;
        _userManager = userManager;
    }

    public async Task<AuthenticateResponse?> Authenticate(AuthenticateRequest model)
    {
        var user = await _userManager.FindByNameAsync(model.Username);

        if (user != null)
        {
            if (await _userManager.CheckPasswordAsync(user, model.Password))
            {
                // authentication successful so generate jwt token
                var token = await GenerateJwtToken(user);

                return new AuthenticateResponse(user, token);
            }
        }

        return null;
    }

    public async Task<ApplicationUser?> GetById(string id)
    {
        return await _userManager.FindByIdAsync(id);
    }

    // helper methods

    private async Task<string> GenerateJwtToken(ApplicationUser user)
    {
        // generate token that is valid for 7 days
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_appSettings.TokenSecret);

        var userRoles = await _userManager.GetRolesAsync(user);

        var authClaims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id),
            new Claim(ClaimTypes.Name, user.UserName),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        };

        foreach (var userRole in userRoles)
        {
            authClaims.Add(new Claim(ClaimTypes.Role, userRole));
        }

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(authClaims),
            Expires = DateTime.UtcNow.AddDays(7),
            SigningCredentials =
                new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);

        return tokenHandler.WriteToken(token);
    }
}