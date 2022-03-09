using System.Text.Encodings.Web;
using System.Threading.Tasks;
using RoverCore.Boilerplate.Infrastructure.Common;
using RoverCore.Boilerplate.Infrastructure.Common.Email.Models.EmailViewModels;
using RoverCore.Boilerplate.Infrastructure.Common.Email.Services;

namespace RoverCore.Boilerplate.Web.Extensions;

public static class EmailSenderExtensions
{
    public static Task SendEmailConfirmationAsync(this IEmailSender emailSender, string email, string link)
    {
        //ndEmailConfirmationAsync(EmailVerificationViewModel viewModel)
        var vm = new EmailVerificationViewModel
        {
            
            Link = link
        };
        //return emailSender.SendEmailAsync(email, "Confirm your email", $"Please confirm your account by clicking this link: <a href='{HtmlEncoder.Default.Encode(link)}'>link</a>");
        return emailSender.SendEmailConfirmationAsync(vm);
    }
}