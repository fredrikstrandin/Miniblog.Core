using System.Threading.Tasks;
using IdentityModel;
using IdentityServer4.MongoDB.Service;
using IdentityServer4.Validation;
using IdentityServer4.MongoDB.Model.Enum;

namespace IdentityServer4.MongoDB.Validator
{
    public class MongoDBUserResourceOwnerPasswordValidator : IResourceOwnerPasswordValidator
    {
        private readonly IUserMongoDBService _users;

        public MongoDBUserResourceOwnerPasswordValidator(IUserMongoDBService users)
        {
            _users = users;
        }

        public async Task ValidateAsync(ResourceOwnerPasswordValidationContext context)
        {
            if (await _users.ValidateCredentialsAsync(context.UserName, context.Password))
            {
                var user = await _users.FindByEmailAsync(context.UserName);
                //context.Result = new GrantValidationResult(user.Id.ToString(), OidcConstants.AuthenticationMethods.Password, user.Claims);
                context.Result = new GrantValidationResult(user.Id.ToString(), OidcConstants.AuthenticationMethods.Password);
            }

            return;
        }
    }
}
