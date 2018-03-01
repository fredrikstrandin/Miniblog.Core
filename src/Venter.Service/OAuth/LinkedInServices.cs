using IdentityServer4.MongoDB.Model.OAuthClient;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using Multiblog.Service.Interface;
using Multiblog.Core.Model.User;

namespace Multiblog.Service
{
    public class LinkedInServices : ILinkedInServices
    {
        private readonly ILogger<LinkedInServices> _logger;

        public LinkedInServices(ILogger<LinkedInServices> logger)
        {
            _logger = logger;
        }

        public async Task<UserItem> UpdatePersonDataAsync(string accesstoken, UserItem user)
        {
            var message = new HttpRequestMessage();

            message.Method = HttpMethod.Get;
            message.RequestUri = new Uri($"https://api.linkedin.com/v1/people/~:(id,first-name,last-name,maiden-name,formatted-name,phonetic-first-name,phonetic-last-name,formatted-phonetic-name,headline,location,industry,current-share,num-connections,num-connections-capped,summary,specialties,positions,picture-url,picture-urls::(original),site-standard-profile-request,api-standard-profile-request,public-profile-url,educations)?oauth2_access_token={ accesstoken }&format=json");

            //last-modified-timestamp,proposal-comments,associations,interests,publications,patents,languages,skills,certifications,educations,courses,volunteer,three-current-positions,three-past-positions,num-recommenders,recommendations-received,following,job-bookmarks,suggestions,date-of-birth,member-url-resources,related-profile-views,honors-awards


            HttpClient client = new HttpClient();

            HttpResponseMessage response = await client.SendAsync(message);
            if (response.IsSuccessStatusCode)
            {
                string json = await response.Content.ReadAsStringAsync();

                if (!string.IsNullOrEmpty(json))
                {
                    LinkedInPersonData linkedIn = JsonConvert.DeserializeObject<LinkedInPersonData>(json);
                    if (linkedIn != null)
                    {
                        user.FirstName = linkedIn.firstName;
                        user.LastName = linkedIn.lastName;
                        user.ProfileImageUrl = linkedIn.pictureUrl;
                        user.EmailVerified = true;
                        user.Summary = linkedIn.summary;
                        user.Headline = linkedIn.headline;
                        user.City = linkedIn.location.name;
                    }
                }
            }

            return user;
        }
    }
}
