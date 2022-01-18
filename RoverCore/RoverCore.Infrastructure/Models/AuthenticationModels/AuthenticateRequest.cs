using System.ComponentModel.DataAnnotations;

namespace RoverCore.Infrastructure.Models.AuthenticationModels;

public class AuthenticateRequest
{
    [Required]
    public string Email { get; set; }

    [Required]
    public string Password { get; set; }
}