using MailKit.Net.Smtp;
using Microsoft.Extensions.Options;
using MimeKit;

namespace V1.Utils.EmailSender
{
    /// <summary>
    /// Service for sending emails using MailKit and SMTP.
    /// </summary>
    /// <remarks>
    /// Initializes a new instance of the <see cref="EmailSender"/> class.
    /// </remarks>
    /// <param name="emailSettings">The email settings configuration injected via IOptions.</param>
    public class EmailSender(IOptions<EmailSettings> emailSettings) : IEmailSender
    {
        private readonly EmailSettings _emailSettings = emailSettings.Value;

        /// <summary>
        /// Sends an email asynchronously to the specified target email address with the given subject and body.
        /// </summary>
        /// <param name="targetEmail">The recipient's email address.</param>
        /// <param name="subject">The subject of the email.</param>
        /// <param name="body">The body of the email.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        /// <exception cref="SmtpCommandException">Thrown if an error occurs during the SMTP communication.</exception>
        /// <exception cref="FileNotFoundException">Thrown if the email template file is missing.</exception>
        private async Task SendEmail(string targetEmail, string body)
        {
            // Create the MIME message.
            MimeMessage message = new();
            message.From.Add(new MailboxAddress(_emailSettings.SenderName, _emailSettings.SenderEmail));
            message.To.Add(MailboxAddress.Parse(targetEmail));

            message.Body = new TextPart("html")
            {
                Text = body
            };

            // Create and configure the SMTP client.
            using SmtpClient client = new();

            // Connect to the SMTP server.
            await client.ConnectAsync(_emailSettings.SmtpServer, _emailSettings.SmtpPort, false);

            // Authenticate using the sender's credentials.
            await client.AuthenticateAsync(_emailSettings.SenderEmail, _emailSettings.Password);

            // Send the email.
            await client.SendAsync(message);

            // Disconnect from the SMTP server.
            await client.DisconnectAsync(true);
        }

        /// <summary>
        /// Generates the HTML email body by reading and replacing placeholders in the template.
        /// </summary>
        /// <param name="subject">The subject of the email, to be inserted into the template.</param>
        /// <param name="body">The body of the email, to be inserted into the template.</param>
        /// <returns>A task that represents the asynchronous operation and contains the generated email body as a string.</returns>
        /// <exception cref="FileNotFoundException">Thrown if the email template file is not found.</exception>
        private static async Task<string> GenerateEmailBody(string templateName)
        {
            // Define the path to the email template file.
            string templatePath = Path.Combine(Directory.GetCurrentDirectory(), "Templates", templateName + ".html");

            // Read the template file as a string.
            string template = await File.ReadAllTextAsync(templatePath);

            return template;
        }

        public async Task SendUserVerificationEmail(string targetEmail, string token)
        {
            // Generate the body of the email using a template.
            string body = await GenerateEmailBody("UserVerificationTemplate");

            // Replace placeholders in the template with the actual subject and body.
            body = body.Replace("@Model.Url", $"{Environment.GetEnvironmentVariable("FRONTEND_URL")}/verify?token={token}");

            await SendEmail(targetEmail, body);
        }
    }
}
