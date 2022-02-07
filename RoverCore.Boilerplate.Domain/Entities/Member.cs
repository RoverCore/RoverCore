using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace RoverCore.Boilerplate.Domain.Entities;

public class Member
{
    [Key]
    public int MemberId { get; set; }

    [Required]
    [StringLength(32, MinimumLength = 1, ErrorMessage = "First name is required")]  // Max 32 characters, min 1 character
    [Display(Name = "First Name")]
    public string FirstName { get; set; }

    [Required]
    [StringLength(32, MinimumLength = 1, ErrorMessage = "Last name is required")]  // Max 32 characters, min 1 character
    [Display(Name = "Last Name")]
    public string LastName { get; set; }

    [Required, DataType(DataType.EmailAddress, ErrorMessage = "Enter a valid email address")]
    public string Email { get; set; }

    [Required]
    [ScaffoldColumn(false)]
    [JsonIgnore]
    [MinLength(8, ErrorMessage = "Password must be at least 8 characters long")]
    public string Password { get; set; }

    [JsonIgnore]
    [ScaffoldColumn(false)]
    public string PasswordSalt { get; set; }
}