namespace RoverCore.Boilerplate.Infrastructure.Common.Email.Services;

public interface IEmailSender
{
    Task SendEmailAsync(string email, string subject, string message);
}