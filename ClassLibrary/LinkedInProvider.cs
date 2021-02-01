using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Net;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Sparkle.LinkedInNET;

namespace ClassLibrary
{
    public class LinkedInProvider : IProvider
    {
        private readonly LinkedInOptions _options;

        public LinkedInProvider(LinkedInOptions options)
        {
            _options = options;
        }
        public string Name { get; set; } = "LinkedIn";

        public async Task Post(MessageOptions messageOptions, Dictionary<string, string> providerParams)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls
                                                   | SecurityProtocolType.Tls11
                                                   | SecurityProtocolType.Tls12;
            string authCode = null;
            foreach (var param in providerParams)
            {
                if (param.Key == Name)
                {
                    authCode = param.Value;
                }
            }
            
            var config = new LinkedInApiConfiguration(_options.AppId, _options.AppSecret);
            var api = new LinkedInApi(config);
           
            if (authCode != null)
            {
                var accessTokenResult = await  api.OAuth2.GetAccessTokenAsync(authCode, _options.RedirectUrl);
                var accessToken = accessTokenResult.AccessToken;
                var requestUrl = $"https://api.linkedin.com/v2/shares?oauth2_access_token={accessToken}";
                PostToLinkedIn(requestUrl, messageOptions);
            }
        }
        private static void PostToLinkedIn(string url, MessageOptions messageOptions)
        {
            var jsonContent = new
            {
                content = new
                {
                    contentEntities = new dynamic[]
                    {
                        new
                        {
                            entityLocation = messageOptions.Link,
                            thumbnails = new dynamic[]
                            {
                                new
                                {
                                    resolvedUrl = messageOptions.ImageUrl
                                }
                            }
                        }
                    },
                    title = messageOptions.Title
                },
                distribution = new
                {
                    linkedInDistributionTarget = new { }
                },
                owner = $"urn:li:organization:{messageOptions.OwnerId}",
                subject = messageOptions.Subject,
                text = new
                {
                    text = messageOptions.Message
                }
            };
            var jsonFormatted = JsonConvert.SerializeObject(jsonContent);
            var client = new WebClient();
            var requestHeaders = new NameValueCollection
            {
                {"Content-Type", "application/json"},
            };
            client.Headers.Add(requestHeaders);
            client.UploadString(url, "POST", jsonFormatted);
        }
    }
}