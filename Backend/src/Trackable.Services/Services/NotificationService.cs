using Microsoft.Extensions.Configuration;
using SendGrid;
using SendGrid.Helpers.Mail;
using System.Threading.Tasks;

namespace Trackable.Services
{
    class NotificationService : INotificationService
    {
        private readonly IConfiguration configuration;

        public NotificationService(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public async Task<bool> NotifyViaEmail(string email, string subject, string textContent, string htmlContent)
        {
            var client = new SendGridClient(configuration["SendGrid:ApiKey"]);

            var mail = MailHelper.CreateSingleEmail(
                new EmailAddress(
                    configuration["SendGrid:FromEmailAddress"],
                    configuration["SendGrid:FromEmailName"]),
                new EmailAddress(email, configuration["SendGrid:ToEmailName"]),
                subject,
                textContent,
                htmlContent);

            var response = await client.SendEmailAsync(mail);
            return response.StatusCode == System.Net.HttpStatusCode.Accepted;
        }
    }
}
