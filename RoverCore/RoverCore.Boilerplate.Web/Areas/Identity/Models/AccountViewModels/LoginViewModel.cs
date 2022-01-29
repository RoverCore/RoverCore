﻿using System.ComponentModel.DataAnnotations;

namespace RoverCore.Boilerplate.Web.Areas.Identity.Models.AccountViewModels;

public class LoginViewModel
{
    [Required]
    public string Username { get; set; }

    [Required]
    [DataType(DataType.Password)]
    public string Password { get; set; }

    [Display(Name = "Remember me?")]
    public bool RememberMe { get; set; }
}