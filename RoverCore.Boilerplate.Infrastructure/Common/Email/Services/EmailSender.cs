using FluentEmail.Core;
using Microsoft.Extensions.Logging;
using RoverCore.Abstractions.Templates;
using RoverCore.Boilerplate.Domain.Entities.Settings;
using RoverCore.Boilerplate.Infrastructure.Common.Email.Models.EmailViewModels;

namespace RoverCore.Boilerplate.Infrastructure.Common.Email.Services;
// This class is used by the application to send email for account confirmation and password reset.
// For more details see https://go.microsoft.com/fwlink/?LinkID=532713

public class EmailSender : IEmailSender
{
    private readonly IFluentEmail _email;
    private readonly ApplicationSettings _settings;
    private readonly ITemplateService _templateService;
    private readonly ILogger _logger;

    public EmailSender(ApplicationSettings settings, IFluentEmail fluentEmail, ITemplateService templateService, ILogger<EmailSender> logger)
    {
        _settings = settings;
        _email = fluentEmail;
        _templateService = templateService;
        _logger = logger;
    }

    public async Task SendEmailConfirmationAsync(EmailVerificationViewModel viewModel)
    {
        await SendFluidEmailAsync(viewModel);
    }

    public async Task SendFluidEmailAsync(EmailBaseViewModel viewModel)
    {
        var template = @"
        {% layout '_layout' %}
        Click this link: {{ Link }}.";

        var email = _email
            .SetFrom("bob@hotmail.com")
            .To("somedude@gmail.com")
            .Subject("woo nuget")
            .UsingTemplate(template, viewModel)
            .SendAsync();
    }

    /// <summary>
    /// Legacy email sender for sending simple emails
    /// </summary>
    /// <param name="email"></param>
    /// <param name="subject"></param>
    /// <param name="message"></param>
    /// <returns></returns>
    public async Task SendEmailAsync(string email, string subject, string message)
    {
        if (string.IsNullOrWhiteSpace(email)) return;
        
        try
        {
            await _email.SetFrom(_settings.Email.DefaultSenderAddress, _settings.Email.DefaultSenderName)
                .To(email)
                .Subject(subject)
                .Body(message)
                .SendAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unable to send email to {email}", email);
        }
    }
}