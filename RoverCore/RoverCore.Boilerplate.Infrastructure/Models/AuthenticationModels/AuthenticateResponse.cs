using RoverCore.Boilerplate.Domain.Entities;

namespace RoverCore.Boilerplate.Infrastructure.Models.AuthenticationModels;

public class AuthenticateResponse
{
    public AuthenticateResponse(Member member, string token)
    {
        MemberId = member.MemberId;
        Email = member.Email;
        Token = token;
    }

    public int MemberId { get; set; }
    public string Email { get; set; }
    public string Token { get; set; }
}