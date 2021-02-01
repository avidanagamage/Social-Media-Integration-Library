using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ClassLibrary;

namespace ConsoleApp
{
    internal class Program
    {
        private static async Task Main()
        {
            var providerParams = new Dictionary<string, string>()
            {
                {"Twitter", "CodeTW"},
                {"Facebook", "CodeFB"},
                {"LinkedIn", "CodeLI"}
            };

            var smIntegration = new SocialMediaIntegration();

            smIntegration.AddProvider(new FacebookProvider(new FacebookOptions
            {
                AppSecret = "AppSecret",
                AppId = "AppId",
                PageId = "PageId",
                PageName = "PageName",
                RedirectUrl = "http://localhost:54372/",
                Scope = "publish_pages,manage_pages"
            }));

            smIntegration.AddProvider(new LinkedInProvider(new LinkedInOptions
            {
                AppSecret = "AppSecret",
                AppId = "AppId",
                OrgNumber = "OrgNumber",
                RedirectUrl = "http://localhost:54372/",
                Scope = "w_member_social w_organization_social rw_organization_admin"
            }));

            smIntegration.AddProvider(new TwitterProvider(new TwitterOptions
            {
                AppSecret = "AppSecret",
                AppId = "AppId",
                Token = "Token",
                TokenSecret = "TokenSecret"
            }));

            var messageOptions = new MessageOptions
            {
                Message = "Tested  By Amila for Social media sharing",
                Link = "https://www.xyz.com/",
                ImageUrl = "https://media-exp1.licdn.com/dms/image/C4D0BAQGEOxV8RpyOZA/company-logo_200_200/0?e=1594252800&v=beta&t=fW9q2zvx9smGLsVrqMrOi1_UnWYGl7kn6x3c1qvj1JE",
                OwnerId = "2414183",
                Subject = "Amila Vidanagamage",
                Title = "Tested By Amila",
                Description = "This library create social media auto post with the auth handling"
            };

            await smIntegration.Post(providerParams, messageOptions);
        }
    }
}