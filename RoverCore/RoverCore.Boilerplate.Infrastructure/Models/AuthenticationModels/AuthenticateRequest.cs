﻿using System.ComponentModel.DataAnnotations;

namespace RoverCore.Boilerplate.Infrastructure.Models.AuthenticationModels;

public class AuthenticateRequest
{
    [Required] public string Username { get; set; } = string.Empty;

    [Required] public string Password { get; set; } = string.Empty;
}