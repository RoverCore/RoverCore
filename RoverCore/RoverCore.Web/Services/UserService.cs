using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using Rover.Web.Helpers;
using Rover.Web.Models;
using Rover.Web.Models.AuthenticationModels;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using RoverCore.Domain.Entities;
using RoverCore.Infrastructure.DbContexts;

namespace Rover.Web.Services;

public interface IUserService
{
    Task<AuthenticateResponse> Authenticate(AuthenticateRequest model);
    Task<Member> GetById(int id);
}

public class UserService : IUserService
{
    private readonly ApplicationDbContext _context;
    private readonly AppSettings _appSettings;

    public UserService(IOptions<AppSettings> appSettings, ApplicationDbContext context)
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
        if (member.Password != PasswordHasher.Hash(model.Password, member.PasswordSalt).HashedPassword)
            return null;

        // authentication successful so generate jwt token
        var token = generateJwtToken(member);

        return new AuthenticateResponse(member, token);
    }

    public async Task<Member> GetById(int id)
    {
        return await _context.Member.FirstOrDefaultAsync(x => x.MemberId == id);
    }

    // helper methods

    private string generateJwtToken(Member user)
    {
        // generate token that is valid for 7 days
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_appSettings.JWTTokenSecret);
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