using FluentEmail.Core;
using FluentEmail.Core.Models;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;
using RoverCore.Abstractions.Templates;
using RoverCore.Boilerplate.Domain.Entities.Settings;
using RoverCore.Boilerplate.Infrastructure.Common.Email.Models.EmailViewModels;
using RoverCore.Boilerplate.Infrastructure.Common.Templates.Models;

namespace RoverCore.Boilerplate.Infrastructure.Common.Email.Services;
// This class is used by the application to send email for account confirmation and password reset.
// For more details see https://go.microsoft.com/fwlink/?LinkID=532713

public class EmailSender : IEmailSender
{
    private readonly IFluentEmail _email;
    private readonly ApplicationSettings _settings;
    private readonly ITemplateService _templateService;
    private readonly ILogger _logger;
    private readonly LinkGenerator _linkGenerator;

    public EmailSender(ApplicationSettings settings, IFluentEmail fluentEmail, ITemplateService templateService, ILogger<EmailSender> logger, LinkGenerator linkGenerator)
    {
        _settings = settings;
        _email = fluentEmail;
        _templateService = templateService;
        _logger = logger;
        _linkGenerator = linkGenerator;
    }

    public async Task SendEmailConfirmationAsync(EmailVerificationViewModel viewModel)
    {
        ConfigureEmailDefaults(viewModel);

        await SendFluidEmailAsync(TemplateSlugs.VerificationEmail, viewModel);
    }

    /// <summary>
    /// Configures all the default email viewmodel settings (particularly in regards to the sender)
    /// </summary>
    /// <param name="viewModel"></param>
    public void ConfigureEmailDefaults(EmailBaseViewModel viewModel)
    {
        if (string.IsNullOrWhiteSpace(viewModel.SenderAddress))
            viewModel.SenderAddress = _settings.Email.DefaultSenderAddress;

        if (string.IsNullOrWhiteSpace(viewModel.SenderName))
            viewModel.SenderName = _settings.Email.DefaultSenderName;

        if (string.IsNullOrWhiteSpace(viewModel.SiteName))
            viewModel.SiteName = _settings.SiteName;

        if (string.IsNullOrWhiteSpace(viewModel.Company))
            viewModel.Company = _settings.Company;

        /*
    public string SiteUrl { get; set; } = string.Empty;
    public string Company { get; set; } = string.Empty;
    public string LogoImageUrlSmall { get; set; } = string.Empty;
    public string UnsubscribeUrl { get; set; } = string.Empty;*/
    }

    public async Task<SendResponse?> SendFluidEmailAsync(string templateSlug, EmailBaseViewModel viewModel)
    {
        List<string> errors = new List<string>();

        // Quick sanity checks to see if this is minimally enough to send
        if (string.IsNullOrWhiteSpace(viewModel.SenderAddress))
            errors.Add("Sender email address cannot be empty");

        if (string.IsNullOrWhiteSpace(viewModel.ReceiverAddress))
            errors.Add("Receiver email address cannot be empty");

        if (string.IsNullOrWhiteSpace(viewModel.Subject))
            errors.Add("Subject cannot be empty");

        if (errors.Count > 0)
        {
            return new SendResponse { ErrorMessages = errors };
        }
        //var template = @"
        //{% layout '_layout' %}
        //Click this link: {{ Link }}.";

        var template = await _templateService.FindTemplateBySlug(templateSlug);

        if (template == null)
        {
            _logger.LogError("Email template slug {templateSlug} is missing from the database.", templateSlug);

            errors.Add($"Email template slug {templateSlug} is missing from the database.");

            return new SendResponse { ErrorMessages = errors };
        }

        template.Body ??= "";

        if (string.IsNullOrWhiteSpace(viewModel.PreHeaderText))
        {
            viewModel.PreHeaderText = template.PreHeader ?? string.Empty;
        }

        var email = await _email
            .SetFrom(viewModel.SenderAddress, string.IsNullOrWhiteSpace(viewModel.SenderName) ? null : viewModel.SenderName)
            .To(viewModel.ReceiverAddress, string.IsNullOrWhiteSpace(viewModel.ReceiverName) ? null : viewModel.ReceiverName)
            .Subject(viewModel.Subject)
            .UsingTemplate(template.Body, viewModel)
            .SendAsync();

        return email;
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

        var template = @"
        {% layout '_layout' %}
        {{ Message }}";

        try
        {
            await _email.SetFrom(_settings.Email.DefaultSenderAddress, _settings.Email.DefaultSenderName)
                .To(email)
                .Subject(subject)
                .UsingTemplate(template, new { Message = message })
                .SendAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unable to send email to {email}", email);
        }
    }
}