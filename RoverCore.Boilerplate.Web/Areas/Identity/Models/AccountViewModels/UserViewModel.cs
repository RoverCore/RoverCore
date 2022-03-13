using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RoverCore.Boilerplate.Web.Areas.Identity.Models.AccountViewModels;

public class UserViewModel
{
    public string Id { get; set; }

    [Required]
    [DataType(DataType.EmailAddress)]
    public string Email { get; set; }

    [Display(Name = "First Name")]
    [Required]
    public string FirstName { get; set; }

    [Display(Name = "Last Name")]
    public string LastName { get; set; }

    [Required]
    public List<string> Roles { get; set; }

    [DataType(DataType.Password)]
    public string Password { get; set; }

    [DataType(DataType.Password)]
    [Display(Name = "Confirm Password")]
    [Compare("Password", ErrorMessage = "Passwords don't match")]
    public string ConfirmPassword { get; set; }
    [Display(Name = "Email Confirmed")]
    public bool EmailConfirmed { get; set; }
    [Display(Name = "Lockout Enabled")]
    public bool LockoutEnabled { get; set; }
    [Display(Name = "Phone Number")]
    [DataType(DataType.PhoneNumber)]
    public string PhoneNumber { get; set; }
    [Display(Name = "Phone Number Confirmed")]
    public bool PhoneNumberConfirmed { get; set; }

}