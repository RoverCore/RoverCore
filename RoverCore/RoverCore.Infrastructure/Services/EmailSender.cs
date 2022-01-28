using Microsoft.Extensions.Configuration;
using System.Net;
using System.Net.Mail;

namespace RoverCore.Infrastructure.Services;
// This class is used by the application to send email for account confirmation and password reset.
// For more details see https://go.microsoft.com/fwlink/?LinkID=532713

public class EmailSender : IEmailSender
{
    private IConfiguration Configuration { get; set; }

    public EmailSender(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public Task SendEmailAsync(string email, string subject, string message)
    {
        SmtpClient client = new SmtpClient(Configuration["SMTP_HOST"])
        {
            UseDefaultCredentials = false,
            Port = int.Parse(Configuration["SMTP_PORT"]),
            Credentials = new NetworkCredential(Configuration["SMTP_USER"], Configuration["SMTP_PASSWORD"])
        };

        MailMessage mailMessage = new MailMessage
        {
            IsBodyHtml = true,
            From = new MailAddress(Configuration["SMTP_USER"], "ADS Backend"),
            Body = message,
            Subject = subject,
        };
        mailMessage.To.Add(email);

        return client.SendMailAsync(mailMessage);
    }
}