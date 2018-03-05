// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using SendGrid;
using SendGrid.Helpers.Mail;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Trackable.Services
{
    class NotificationService : INotificationService
    {
        private readonly IConfiguration configuration;
        private static HttpClient httpClient = new HttpClient();

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

        public async Task<bool> NotifyViaWebhook(string webhookUrl, GeofenceWebhookNotification notification)
        {
            var response = await httpClient.PostAsync(
                webhookUrl,
                new StringContent(
                    JsonConvert.SerializeObject(notification),
                    Encoding.UTF8,
                    "application/json"));
            return response.IsSuccessStatusCode;
        }
    }
}
