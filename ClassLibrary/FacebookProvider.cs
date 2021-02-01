using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Facebook;
using Newtonsoft.Json.Linq;

namespace ClassLibrary
{
    public class FacebookProvider : IProvider
    {
        private readonly FacebookOptions _options;

        public FacebookProvider(FacebookOptions options)
        {
            _options = options;
        }
        public string Name { get; set; } = "Facebook";

        public async Task Post(MessageOptions messageOptions, Dictionary<string, string> providerParams)
        {
            string authCode = null;
            foreach (var param in providerParams)
            {
                if (param.Key == Name)
                {
                    authCode = param.Value;
                }
            }
            var fbClient = new FacebookClient();
            if (authCode != null)
            {
                var fbParams = new Dictionary<string, object>
                {
                    ["client_id"] = _options.AppId,
                    ["Scope"] = _options.Scope,
                    ["redirect_uri"] = _options.RedirectUrl,
                    ["client_secret"] = _options.AppSecret,
                    ["code"] = authCode
                };
                var publishedResponse = (JsonObject)fbClient.Get("/oauth/access_token", fbParams);
                if (publishedResponse == null) return;
                var userAccessToken = publishedResponse["access_token"].ToString();
                var pageAccessToken = GetPageAccessToken(userAccessToken, _options.PageName);
                PostToFacebook(pageAccessToken, _options.PageId, messageOptions);
            }
        }

        public string GetPageAccessToken(string userAccessToken, string pageName)
        {
            var fbClient = new FacebookClient(userAccessToken);
            var publishedResponse = fbClient.Get("/me/accounts") as JsonObject;
            var data = JArray.Parse(publishedResponse?["data"].ToString());

            foreach (var account in data)
                if (account["name"].ToString().Equals(pageName))
                    return account["access_token"].ToString();

            return string.Empty;
        }

        public void PostToFacebook(string pageAccessToken,string pageId, MessageOptions messageOptions)
        {
            var fbClient = new FacebookClient(pageAccessToken);

            var webClient = new WebClient();
            var image = webClient.DownloadData(messageOptions.ImageUrl);
            var facebookUploader = new FacebookMediaObject { FileName = Path.GetFileName(messageOptions.ImageUrl), ContentType = "image/png,image/jpg" };
            facebookUploader.SetValue(image);


            var fbParams = new Dictionary<string, object>
            {
                ["description"] = messageOptions.Description,
                ["picture"] = facebookUploader,
                ["caption"] = messageOptions.Title,
                ["link"] = messageOptions.Link,
                ["no_story"] = true
            };

            try
            {
                var result = fbClient.Post($"/{pageId}/photos", fbParams);

                var response = (IDictionary<string, object>)result;
                if (response == null) throw new ArgumentNullException(nameof(response));
                var fbPostParams = new Dictionary<string, object>()
                {
                    ["object_attachment"] = response["id"],
                    ["message"] = messageOptions.Message,
                };

                fbClient.Post($"/{pageId}/feed", fbPostParams);
            }
            catch (FacebookOAuthException e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}