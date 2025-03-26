using Azure;
using Azure.Communication.Email;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace WebApplication1.Services
{
    public class EmailService
    {
        private readonly EmailClient _emailClient;

        public EmailService()
        {
            // Retrieve the connection string from the environment variable
            string connectionString = Environment.GetEnvironmentVariable("COMMUNICATION_SERVICES_CONNECTION_STRING")
                ?? throw new InvalidOperationException("COMMUNICATION_SERVICES_CONNECTION_STRING is not set.");
            _emailClient = new EmailClient(connectionString);
        }

               public async Task SendPackageEmailAsync(string toEmail, string subject, int itemCount, string shippingProvider, string additionalInfo)
        {
            try
            {
                // Format the email body with package details
                var emailBody = $@"
                <html>
                    <body>
                        <h1>{subject}</h1>
                        <p><strong>Lecturer Email:</strong> {toEmail}</p>
                        <p><strong>Item Count:</strong> {itemCount}</p>
                        <p><strong>Shipping Provider:</strong> {shippingProvider}</p>
                        <p><strong>Additional Information:</strong> {additionalInfo}</p>
                    </body>
                </html>";

                // Create the email content
                var emailContent = new EmailContent(subject)
                {
                    PlainText = $"Lecturer Email: {toEmail}\nItem Count: {itemCount}\nShipping Provider: {shippingProvider}\nAdditional Information: {additionalInfo}",
                    Html = emailBody
                };

                // Create the email message
                var emailMessage = new EmailMessage(
                    senderAddress: "DoNotReply@c82bcbff-b02e-4e6f-af44-059a9fd518f9.azurecomm.net", // Replace with your verified sender address
                    content: emailContent,
                    recipients: new EmailRecipients(new List<EmailAddress> { new EmailAddress(toEmail) })
                );

                // Send the email
                EmailSendOperation emailSendOperation = await _emailClient.SendAsync(WaitUntil.Completed, emailMessage);
                Console.WriteLine($"Email sent successfully. Message ID: {emailSendOperation.Id}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to send email: {ex.Message}");
                throw;
            }
        }
    }
}