using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using KunevDevPortfolio.ViewModels;

namespace KunevDevPortfolio.Services
{
    public interface IEmailService
    {
        Task<bool> SendContactEmailAsync(ContactFormModel model);
    }

    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<EmailService> _logger;

        public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<bool> SendContactEmailAsync(ContactFormModel model)
        {
            try
            {
                var smtpSettings = _configuration.GetSection("SmtpSettings");

                // Check if SMTP settings are configured
                var host = smtpSettings["Host"];
                var portStr = smtpSettings["Port"];
                var username = smtpSettings["Username"];
                var password = smtpSettings["Password"];
                var fromEmail = smtpSettings["FromEmail"];

                if (string.IsNullOrEmpty(host) || string.IsNullOrEmpty(portStr) ||
                    string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password) ||
                    string.IsNullOrEmpty(fromEmail))
                {
                    _logger.LogError("SMTP settings are not properly configured. Missing: Host={Host}, Port={Port}, Username={Username}, FromEmail={FromEmail}",
                        string.IsNullOrEmpty(host) ? "MISSING" : "OK",
                        string.IsNullOrEmpty(portStr) ? "MISSING" : "OK",
                        string.IsNullOrEmpty(username) ? "MISSING" : "OK",
                        string.IsNullOrEmpty(fromEmail) ? "MISSING" : "OK");
                    return false;
                }

                var message = new MimeMessage();

                // From address
                message.From.Add(new MailboxAddress("kunev.dev Contact Form", fromEmail));

                // To address
                message.To.Add(new MailboxAddress("Yavor Kunev", "yavor@kunev.dev"));

                // Reply-To address (sender's email)
                message.ReplyTo.Add(new MailboxAddress(model.Name, model.Email));

                // Subject
                message.Subject = $"New inquiry from {model.Name} - {model.ProjectType}";

                // Body
                var bodyBuilder = new BodyBuilder
                {
                    HtmlBody = GenerateEmailBody(model)
                };
                message.Body = bodyBuilder.ToMessageBody();

                using var client = new SmtpClient();

                // Connect to SMTP server with SSL (port 465)
                await client.ConnectAsync(
                    host,
                    int.Parse(portStr),
                    SecureSocketOptions.SslOnConnect);

                // Authenticate
                await client.AuthenticateAsync(username, password);

                // Send message
                await client.SendAsync(message);

                // Disconnect
                await client.DisconnectAsync(true);

                _logger.LogInformation("Contact email sent successfully from {Email}", model.Email);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send contact email from {Email}", model.Email);
                return false;
            }
        }

        private static string GenerateEmailBody(ContactFormModel model)
        {
            var company = !string.IsNullOrEmpty(model.Company) ? $"<p><strong>Company:</strong> {model.Company}</p>" : "";
            var phone = !string.IsNullOrEmpty(model.Phone) ? $"<p><strong>Phone:</strong> {model.Phone}</p>" : "";
            var budget = !string.IsNullOrEmpty(model.Budget) ? $"<p><strong>Budget:</strong> {model.Budget}</p>" : "";
            var timeline = !string.IsNullOrEmpty(model.Timeline) ? $"<p><strong>Timeline:</strong> {model.Timeline}</p>" : "";

            return $@"
                <html>
                <body style='font-family: Arial, sans-serif; line-height: 1.6;'>
                    <div style='max-width: 600px; margin: 0 auto; padding: 20px;'>
                        <h2 style='color: #1e30f3; border-bottom: 2px solid #e21e80; padding-bottom: 10px;'>
                            New inquiry from kunev.dev
                        </h2>
                        
                        <div style='background-color: #f8f9fa; padding: 20px; border-radius: 8px; margin: 20px 0;'>
                            <h3 style='color: #333; margin-top: 0;'>Contact Information:</h3>
                            <p><strong>Name:</strong> {model.Name}</p>
                            <p><strong>Email:</strong> {model.Email}</p>
                            {phone}
                            {company}
                        </div>
                        
                        <div style='background-color: #fff; padding: 20px; border: 1px solid #dee2e6; border-radius: 8px; margin: 20px 0;'>
                            <h3 style='color: #333; margin-top: 0;'>Project Details:</h3>
                            <p><strong>Project Type:</strong> {model.ProjectType}</p>
                            {budget}
                            {timeline}
                        </div>
                        
                        <div style='background-color: #f8f9fa; padding: 20px; border-radius: 8px; margin: 20px 0;'>
                            <h3 style='color: #333; margin-top: 0;'>Message:</h3>
                            <p style='white-space: pre-line;'>{model.Message}</p>
                        </div>
                        
                        <div style='margin-top: 30px; padding-top: 20px; border-top: 1px solid #dee2e6; color: #666; font-size: 14px;'>
                            <p>This message was sent from the contact form on kunev.dev</p>
                            <p>Date: {DateTime.Now:dd.MM.yyyy HH:mm}</p>
                        </div>
                    </div>
                </body>
                </html>";
        }
    }
}