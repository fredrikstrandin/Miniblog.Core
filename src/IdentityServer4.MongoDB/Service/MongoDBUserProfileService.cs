using IdentityModel;
using IdentityServer4.Extensions;
using IdentityServer4.Models;
using IdentityServer4.Services;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace IdentityServer4.MongoDB.Service
{
    public class MongoDBUserProfileService : IProfileService
    {
        private readonly ILogger<MongoDBUserProfileService> _logger;
        private readonly IUserMongoDBService _userMongoDBService;
        
        public MongoDBUserProfileService(IUserMongoDBService userMongoDBService,
            ILogger<MongoDBUserProfileService> logger)
        {
            _userMongoDBService = userMongoDBService;
            _logger = logger;
        }


        public async Task GetProfileDataAsync(ProfileDataRequestContext context)
        {
            _logger.LogDebug("Get profile called for subject {subject} from client {client} with claim types{claimTypes} via {caller}",
                context.Subject.GetSubjectId(),
                context.Client.ClientName ?? context.Client.ClientId,
                context.RequestedClaimTypes,
                context.Caller);

            var subject = context.Subject;

            if (subject == null)
            {
                throw new ArgumentNullException(nameof(context.Subject));
            }

            List<Claim> claims = await _userMongoDBService.FindClaimsBySubjectIdAsync(subject.GetSubjectId());
                        
            if (claims != null)
            {
                if (context.RequestedClaimTypes.Any())
                {
                    //foreach (var item in context.Client.AllowedScopes)
                    {
                        claims = claims.Where(claim => context.RequestedClaimTypes.Contains(claim.Type)).ToList();
                    }

                    //context.AddFilteredClaims(claims);
                    foreach (var item in claims)
                    {
                        context.IssuedClaims.Add(item);
                    }
                }
            }
        }

        public async Task IsActiveAsync(IsActiveContext context)
        {
            string subjectId = context.Subject.GetSubjectId();
            context.IsActive = await _userMongoDBService.IsActiveAsync(subjectId);            
        }
    }
}
