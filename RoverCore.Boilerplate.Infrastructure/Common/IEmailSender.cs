namespace RoverCore.Boilerplate.Infrastructure.Common;

public interface IEmailSender
{
    Task SendEmailAsync(string email, string subject, string message);
}