using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Notify.Client;
using Notify.Interfaces;
using Notify.Models.Responses;

namespace GovNotify
{
    public class GovNotifyClient
    {
        private readonly IAsyncNotificationClient _client;

        private readonly string _emailAddress;
        private readonly string _templateId;
        private readonly Dictionary<string, dynamic> _templateValues = new Dictionary<string, dynamic>();

        public GovNotifyClient(IAsyncNotificationClient client)
        {
            _client = client;
        }

        private GovNotifyClient(IAsyncNotificationClient client, string emailAddress, string templateId)
        {
            _client = client;
            _emailAddress = emailAddress;
            _templateId = templateId;
        }
        
        public GovNotifyClient EmailTo(string emailAddress, string templateId)
        {
            return new GovNotifyClient(_client, emailAddress, templateId);
        }

        public GovNotifyClient WithTemplate(params (string Name, dynamic Value)[] templateValues)
        {
            foreach (var (name, value) in templateValues)
            {
                _templateValues.TryAdd(name, value);
            }
            
            return this;
        }

        public GovNotifyClient WithAttachment(string name, string base64String)
        {
            return WithAttachment(name, Convert.FromBase64String(base64String));
        }
        
        public GovNotifyClient WithAttachment(string name, byte[] attachmentBytes)
        {
            _templateValues.TryAdd(name, NotificationClient.PrepareUpload(attachmentBytes));
            return this;
        }
        
        public Task<EmailNotificationResponse> SendAsync()
        {
            return _client.SendEmailAsync(_emailAddress, _templateId, _templateValues);
        }
    }
}