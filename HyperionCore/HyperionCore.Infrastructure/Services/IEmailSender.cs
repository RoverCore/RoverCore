﻿namespace HyperionCore.Infrastructure.Services;

public interface IEmailSender
{
    Task SendEmailAsync(string email, string subject, string message);
}