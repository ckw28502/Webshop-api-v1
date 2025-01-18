namespace V1.Utils.EmailSender
{
    /// <summary>
    /// Defines the contract for the email sending functionality.
    /// This interface provides a method for sending emails to a target email address.
    /// </summary>
    public interface IEmailSender
    {
        /// <summary>
        /// Sends an email asynchronously to the specified target email address.
        /// </summary>
        /// <param name="targetEmail">The recipient's email address.</param>
        /// <param name="subject">The subject of the email.</param>
        /// <param name="body">The body content of the email.</param>
        /// <returns>A task representing the asynchronous operation of sending the email.</returns>
        Task SendEmail(string targetEmail, string subject, string body);
    }
}
