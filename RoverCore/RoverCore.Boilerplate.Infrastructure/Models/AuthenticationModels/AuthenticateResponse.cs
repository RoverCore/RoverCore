﻿using RoverCore.Boilerplate.Domain.Entities;
using RoverCore.Boilerplate.Domain.Entities.Identity;

namespace RoverCore.Boilerplate.Infrastructure.Models.AuthenticationModels;

public class AuthenticateResponse
{
    public AuthenticateResponse(ApplicationUser user, string token)
    {
        Id = user.Id;
        Username = user.UserName;
        Token = token;
    }

    public string Id { get; set; }
    public string Username { get; set; }
    public string Token { get; set; }
}