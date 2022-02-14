using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Notify.Client;
using Notify.Models.Responses;

namespace GovNotify
{
    static class Program
    {
        static async Task Main(string[] args)
        {
            var apiKey = args[0];
            var templateId = args[1];;
            var recipient = args[2];

            var fileContent = "SGVsbG8gd29ybGQh"; // Hello world
            
            var response = await SendWithPlainCode(apiKey, templateId, recipient, fileContent);
            Console.WriteLine(response.content.body);

            response = await SendWithReusableComponent(apiKey, templateId, recipient, fileContent);
            Console.WriteLine(response.content.body);
            Console.ReadKey();
        }

        private static async Task<EmailNotificationResponse> SendWithPlainCode(string apiKey, string templateId, string recipient, string base64Content)
        {
            var client = new NotificationClient(apiKey);
            var documentContents = Convert.FromBase64String(base64Content);

            var templateValues = new Dictionary<string, dynamic>()
            {
                {"name", "John Doe"},
                {"title", "Mr."},
                {"link1", NotificationClient.PrepareUpload(documentContents)}
            };

            return await client.SendEmailAsync(recipient, templateId, templateValues);
        }
        
        private static async Task<EmailNotificationResponse> SendWithReusableComponent(string apiKey, string templateId, string recipient, string base64Content)
        {
            var notificationClient = new NotificationClient(apiKey);
            var govNotifyClient = new GovNotifyClient(notificationClient);

            return await govNotifyClient
                .EmailTo(recipient, templateId)
                .WithTemplate(
                    ("name", "John Doe"),
                    ("title", "Mr."))
                .WithAttachment("link1", base64Content)
                .SendAsync();
        }
    }
}