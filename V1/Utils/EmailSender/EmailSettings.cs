namespace V1.Utils.EmailSender
{
    /// <summary>
    /// Represents the configuration settings required for sending emails.
    /// These settings include the SMTP server details, sender information, and credentials.
    /// </summary>
    public class EmailSettings
    {
        /// <summary>
        /// Gets or sets the SMTP server address.
        /// This is the address of the mail server used to send emails.
        /// </summary>
        public required string SmtpServer { get; set; }

        /// <summary>
        /// Gets or sets the SMTP port number.
        /// This is the port number used to connect to the SMTP server (e.g., 587 for TLS).
        /// </summary>
        public int SmtpPort { get; set; }

        /// <summary>
        /// Gets or sets the sender's name.
        /// This name will appear as the sender in the email.
        /// </summary>
        public required string SenderName { get; set; }

        /// <summary>
        /// Gets or sets the sender's email address.
        /// This email will appear as the "From" address in the sent email.
        /// </summary>
        public required string SenderEmail { get; set; }

        /// <summary>
        /// Gets or sets the password for SMTP authentication.
        /// This password is used to authenticate the sender with the SMTP server.
        /// </summary>
        public required string Password { get; set; }
    }
}
