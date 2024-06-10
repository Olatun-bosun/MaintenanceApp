using SendGrid;
using SendGrid.Helpers.Mail;

namespace MaintenanceLog.MyHelpers
{
    public class EmailSender
    {
        public static async Task SendEmail(string toEmail, string username,
            string subject, string message)
        {
            string apiKey = "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";
            var client = new SendGridClient(apiKey);

            var from = new EmailAddress("boostmytool@gmail.com", "MaintenanceLog.com");
            var to = new EmailAddress(toEmail, username);
            var plainTextContent = message;
            var htmlContent = "";

            var msg = MailHelper.CreateSingleEmail(
                from, to, subject, plainTextContent, htmlContent);

            var response = await client.SendEmailAsync(msg);
        }
    }
}
