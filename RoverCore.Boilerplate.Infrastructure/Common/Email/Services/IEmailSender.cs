using RoverCore.Boilerplate.Infrastructure.Common.Email.Models.EmailViewModels;

namespace RoverCore.Boilerplate.Infrastructure.Common.Email.Services;

public interface IEmailSender
{
    Task SendEmailConfirmationAsync(EmailVerificationViewModel viewModel);

    Task SendEmailAsync(string email, string subject, string message);
}