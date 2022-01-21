using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Identity;

namespace RoverCore.Domain.Entities.Identity;

// Add profile data for application users by adding properties to the ApplicationUser class
public class ApplicationUser : IdentityUser<int>
{
    [Required]
    [StringLength(50, ErrorMessage = "First name cannot be longer than 50 characters.")]
    [Display(Name = "First Name")]
    public string FirstName { get; set; } = String.Empty;

    [Display(Name = "Last Name")] 
    public string LastName { get; set; } = String.Empty;
    public List<ApplicationRole> Roles { get; set; }

    public virtual string FullName => FirstName.Trim() + " " + LastName?.Trim();

    public virtual string GravitarHash()
    {
        MD5 md5Hasher = MD5.Create();

        byte[] data = md5Hasher.ComputeHash(Encoding.Default.GetBytes(Email?.ToLower() ?? ""));

        StringBuilder sBuilder = new StringBuilder();

        for (int i = 0; i < data.Length; i++)
        {
            sBuilder.Append(data[i].ToString("x2"));
        }

        return sBuilder.ToString();
    }
}