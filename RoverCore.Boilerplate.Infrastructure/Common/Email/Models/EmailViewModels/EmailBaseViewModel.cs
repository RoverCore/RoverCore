namespace RoverCore.Boilerplate.Infrastructure.Common.Email.Models.EmailViewModels
{
    public class EmailBaseViewModel
    {
        public string Subject { get; set; } = string.Empty;
        public string SenderName { get; set; } = string.Empty;
        public string SenderAddress { get; set; } = string.Empty;
        public string ReceiverName { get; set; } = string.Empty;
        public string ReceiverAddress { get; set; } = string.Empty;
        /// <summary>
        /// The first bit of text that appears after the subject in most email viewers.  Typically
        /// this comes from the template configuration.
        /// </summary>
        public string PreHeaderText { get; set; } = string.Empty;
        /// <summary>
        /// You received this email because we received a request for [ActionType] for your account.
        /// If you didn't request [ActionType] you can safely delete this email.
        ///
        /// Examples: email verification, a password change, etc.
        /// </summary>
        public string ActionType { get; set; } = string.Empty;
        public string SiteName { get; set; } = string.Empty;
        public string SiteUrl { get; set; } = string.Empty;
        public string Company { get; set; } = string.Empty;
        public string LogoImageUrlSmall { get; set; } = string.Empty;
        public string UnsubscribeUrl { get; set; } = string.Empty;
    }
}
