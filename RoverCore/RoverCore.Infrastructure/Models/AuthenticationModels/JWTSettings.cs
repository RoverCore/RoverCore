﻿namespace RoverCore.Infrastructure.Models.AuthenticationModels;

public class JWTSettings
{
    public string TokenSecret { get; set; } = String.Empty;
}