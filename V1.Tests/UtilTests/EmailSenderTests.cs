using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using V1.Utils.EmailSender;

namespace V1.Tests.UtilTests
{
    /// <summary>
    /// Tests for the EmailSender utility class, using an in-memory SMTP server to validate email sending.
    /// </summary>
    public class EmailSenderTests : IAsyncLifetime
    {
        /// <summary>
        /// Stores the email configuration used for the EmailSender.
        /// This includes SMTP server address, port, sender details, and authentication credentials.
        /// </summary>
        private readonly EmailSettings _emailSettings;

        /// <summary>
        /// Instance of the EmailSender utility being tested.
        /// This class uses the provided email settings to send emails via the configured SMTP server.
        /// </summary>
        private readonly EmailSender _emailSender;

        private const string BASE_PATH = "http://localhost:8080/api/messages"; // Base URL for accessing the SMTP test server API.

        /// <summary>
        /// DTO for deserializing the response from the SMTP server's "Get Messages" API.
        /// </summary>
        private class GetMessagesDTO
        {
            /// <summary>
            /// List of email messages stored on the SMTP server.
            /// </summary>
            public required List<GetMessagesResult> Results { get; set; }
        }

        /// <summary>
        /// Represents an individual email message result from the SMTP server.
        /// </summary>
        private class GetMessagesResult
        {
            /// <summary>
            /// The unique ID of the email message.
            /// </summary>
            public required string Id { get; set; }

            /// <summary>
            /// List of recipients for the email message.
            /// </summary>
            public required List<string> To { get; set; }

            /// <summary>
            /// The subject of the email message.
            /// </summary>
            public required string Subject { get; set; }
        }

        /// <summary>
        /// Initializes the test with default email settings and an EmailSender instance.
        /// </summary>
        public EmailSenderTests()
        {
            // Configuration for connecting to the test SMTP server.
            _emailSettings = new()
            {
                SmtpServer = "localhost",
                SmtpPort = 25,
                SenderName = "Test Sender",
                SenderEmail = "test@sender.com",
                Password = "testPassword",
            };

            // Instantiate the EmailSender with the configured settings.
            _emailSender = new EmailSender(Options.Create(_emailSettings));
        }

        /// <summary>
        /// Runs before each test to clear any existing messages on the SMTP test server.
        /// </summary>
        public async Task InitializeAsync()
        {
            await ClearMessages();
        }

        /// <summary>
        /// Runs after all tests are complete. Does nothing in this case.
        /// </summary>
        public Task DisposeAsync()
        {
            return Task.CompletedTask;
        }

        /// <summary>
        /// Sends a GET HTTP request to the specified path of the SMTP test server API.
        /// </summary>
        /// <param name="path">The relative path of the API endpoint.</param>
        /// <returns>The response as a string.</returns>
        private static async Task<string> SendGetHttpRequest(string path = "")
        {
            using HttpClient httpClient = new();
            return await httpClient.GetStringAsync(BASE_PATH + path);
        }

        /// <summary>
        /// Clears all email messages stored on the SMTP test server.
        /// </summary>
        private static async Task ClearMessages()
        {
            using HttpClient httpClient = new();
            await httpClient.DeleteAsync(BASE_PATH);
        }

        /// <summary>
        /// Retrieves the first email message stored on the SMTP test server.
        /// </summary>
        /// <returns>The first email message as a <see cref="GetMessagesResult"/>.</returns>
        private static async Task<GetMessagesResult> GetMessages()
        {
            string messagesJson = await SendGetHttpRequest();
            GetMessagesDTO? messages = JsonConvert.DeserializeObject<GetMessagesDTO>(messagesJson);

            Assert.NotNull(messages); // Ensure that the response is not null.

            return messages.Results.First();
        }

        /// <summary>
        /// Retrieves the HTML body of an email message from the SMTP test server.
        /// </summary>
        /// <param name="id">The unique ID of the email message.</param>
        /// <returns>The HTML body of the email as a string.</returns>
        private static async Task<string> GetMesssageBody(string id)
        {
            return await SendGetHttpRequest($"/{id}/html");
        }

        /// <summary>
        /// Verifies that the EmailSender correctly sends an email with the expected details.
        /// </summary>
        [Fact]
        public async Task EmailSender_ShouldSendEmailWithCorrectDetails()
        {
            // Arrange
            const string targetEmail = "test2@sender.com";
            const string subject = "Test Subject";
            const string body = "Email body";

            // Act
            await _emailSender.SendEmail(targetEmail, subject, body);

            // Retrieve the sent message from the SMTP test server.
            GetMessagesResult message = await GetMessages();

            // Assert
            Assert.Contains(targetEmail, message.To); // Verify recipient.
            Assert.Equal(subject, message.Subject);   // Verify subject.

            // Verify body content.
            string messageBody = await GetMesssageBody(message.Id);
            Assert.Contains(body, messageBody);
        }
    }
}
