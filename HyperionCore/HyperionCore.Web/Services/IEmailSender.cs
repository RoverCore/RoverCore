using System.Threading.Tasks;

namespace Hyperion.Web.Services
{
    public interface IEmailSender
    {
        Task SendEmailAsync(string email, string subject, string message);
    }
}
