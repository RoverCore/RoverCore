using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using RoverCore.Domain.Entities;
using RoverCore.Infrastructure.DbContexts;
using RoverCore.Infrastructure.Extensions;
using RoverCore.Infrastructure.Models.AuthenticationModels;

namespace RoverCore.Infrastructure.Services;

public interface IUserService
{
    Task<AuthenticateResponse> Authenticate(AuthenticateRequest model);
    Task<Member> GetById(int id);
}

public class UserService : IUserService
{
    private readonly ApplicationDbContext _context;
    private readonly JWTSettings _appSettings;

    public UserService(IOptions<JWTSettings> appSettings, ApplicationDbContext context)
    {
        _appSettings = appSettings.Value;
        _context = context;
    }

    public async Task<AuthenticateResponse> Authenticate(AuthenticateRequest model)
    {
        var member = await _context.Member.FirstOrDefaultAsync(x => x.Email == model.Email);
                        
        // return null if user not found
        if (member == null) 
            return null;

        // Check password
        if (member.Password != model.Password.Hash(member.PasswordSalt).HashedPassword)
            return null;

        // authentication successful so generate jwt token
        var token = GenerateJwtToken(member);

        return new AuthenticateResponse(member, token);
    }

    public async Task<Member> GetById(int id)
    {
        return await _context.Member.FirstOrDefaultAsync(x => x.MemberId == id);
    }

    // helper methods

    private string GenerateJwtToken(Member user)
    {
        // generate token that is valid for 7 days
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_appSettings.TokenSecret);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[] { new Claim("id", user.MemberId.ToString()) }),
            Expires = DateTime.UtcNow.AddDays(7),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
}