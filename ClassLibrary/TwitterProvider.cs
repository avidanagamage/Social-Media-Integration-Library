using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Tweetinvi;
using Tweetinvi.Models;
using Tweetinvi.Parameters;

namespace ClassLibrary
{
    public class TwitterProvider : IProvider
    {
        private readonly TwitterOptions _options;

        public TwitterProvider(TwitterOptions options)
        {
            _options = options;
        }
        public string Name { get; set; } = "Twitter";

        public async Task Post(MessageOptions messageOptions, Dictionary<string, string> providerParams)
        {
            Auth.SetUserCredentials(_options.AppId,
                _options.AppSecret, _options.Token,
                _options.TokenSecret);

            var webClient = new WebClient();
            var image = webClient.DownloadData(messageOptions.ImageUrl);
            var media = Upload.UploadBinary(image);
            try
            {
                Tweet.PublishTweet(messageOptions.Message, new PublishTweetOptionalParameters
                {
                    Medias = new List<IMedia> { media },
                });
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}