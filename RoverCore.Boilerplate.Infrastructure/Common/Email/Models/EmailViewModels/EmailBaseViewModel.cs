namespace RoverCore.Boilerplate.Infrastructure.Common.Email.Models.EmailViewModels
{
    public class EmailBaseViewModel
    {
        public string SenderName { get; set; } = string.Empty;
        public string SenderEmailAddress { get; set; } = string.Empty;
        public string ReceiverName { get; set; } = string.Empty;

    }
}
