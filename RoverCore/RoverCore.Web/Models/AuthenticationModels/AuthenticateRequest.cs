using System.ComponentModel.DataAnnotations;

namespace Rover.Web.Models.AuthenticationModels;

public class AuthenticateRequest
{
    [Required]
    public string Email { get; set; }

    [Required]
    public string Password { get; set; }
}