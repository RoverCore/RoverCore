using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;

namespace RoverCore.Boilerplate.Infrastructure.Common;
// This class is used by the application to send email for account confirmation and password reset.
// For more details see https://go.microsoft.com/fwlink/?LinkID=532713

/// <summary>
/// OBSOLETE : TO BE REPLACED WITH FLUENT EMAIL
/// </summary>
public class EmailSender : IEmailSender
{
    public EmailSender(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    private IConfiguration Configuration { get; }

    public Task SendEmailAsync(string email, string subject, string message)
    {
        var client = new SmtpClient(Configuration["SMTP_HOST"])
        {
            UseDefaultCredentials = false,
            Port = int.Parse(Configuration["SMTP_PORT"]),
            Credentials = new NetworkCredential(Configuration["SMTP_USER"], Configuration["SMTP_PASSWORD"])
        };

        var mailMessage = new MailMessage
        {
            IsBodyHtml = true,
            From = new MailAddress(Configuration["SMTP_USER"], "ADS Backend"),
            Body = message,
            Subject = subject
        };
        mailMessage.To.Add(email);

        return client.SendMailAsync(mailMessage);
    }
}