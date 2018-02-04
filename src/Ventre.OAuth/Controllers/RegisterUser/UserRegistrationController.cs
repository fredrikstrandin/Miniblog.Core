using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using IdentityServer4.MongoDB.Service;
using IdentityServer4.Services;
using System.Security.Claims;
using IdentityServer4.MongoDB.Model;
using Microsoft.AspNetCore.Http;
using IdentityModel;
using Venter.Service.Queue;
using Vivus.Model.User;
using Microsoft.Extensions.Options;
using Vivus.Model.Setting;
using Venter.Service.UserService;
using Miniblog.Core.Model.User;

namespace Vivus.OAuth.Controllers
{
    [Route("[controller]")]
    public class AccountRegistrationController : Controller
    {
        private readonly IUserMongoDBService _oauthUserServices;
        private readonly IIdentityServerInteractionService _interaction;
        private readonly IQeueMessageServices _qeueMessageServices;
        private readonly ApplicationSetting _applicationSetting;
        private readonly IUserService _userService;

        public AccountRegistrationController(IUserMongoDBService oauthUserServices,
             IIdentityServerInteractionService interaction,
             IQeueMessageServices qeueMessageServices,
             IOptions<ApplicationSetting> applicationSetting,
             IUserService userService)
        {
            _oauthUserServices = oauthUserServices;
            _interaction = interaction;
            _qeueMessageServices = qeueMessageServices;
            _applicationSetting = applicationSetting.Value;
            _userService = userService;
        }

        [HttpGet("index")]
        public async Task<IActionResult> Index(RegistrationInputModel registrationInputModel)
        {
            List<Claim> claims = await _oauthUserServices.GetTempOAuthUserClaimsAsync(registrationInputModel.Provider, registrationInputModel.ProviderUserId);

            string profileUrl = null;

            if (registrationInputModel != null)
            {
                if (registrationInputModel.Provider == "Facebook")
                {
                    profileUrl = $"https://graph.facebook.com/v2.10/{registrationInputModel.ProviderUserId}/picture?redirect=true&type=large";
                }

                var vm = new RegisterUserViewModel()
                {
                    ProfileImage = profileUrl,
                    Email = claims?.FirstOrDefault(x => x.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress")?.Value,
                    Firstname = claims?.FirstOrDefault(x => x.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/givenname")?.Value,
                    Lastname = claims?.FirstOrDefault(x => x.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/surname")?.Value,
                    ReturnUrl = registrationInputModel?.ReturnUrl,
                    Provider = registrationInputModel?.Provider,
                    ProviderUserId = registrationInputModel?.ProviderUserId
                };

                return View(vm);
            }

            return View();
        }

        [HttpPost]
        [Route("index")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(RegisterUserViewModel model)
        {
            TempOAuthUserAccessTokenItem accessTokenItem = await _oauthUserServices.GetTempOAuthUserAccessTokenAsync(model.Provider, model.ProviderUserId);

            if (ModelState.IsValid)
            {
                if(model.IsProvisioningFromExternal == false && string.IsNullOrEmpty(model.Password))
                {
                    ModelState.AddModelError("NoPassword", "You need to provide password.");

                    return View(model);
                }

                // create user + claims
                var userToCreate = new RegisterUserModel()
                {
                    Firstname = model.Firstname,
                    Lastname = model.Lastname,
                    Username = model.Username,
                    ProfileImage = model.ProfileImage,
                    CountryCodes = model.Country,
                    Password = model.Password,
                    Email = model.Email
                };

                userToCreate.EmailVerified = model.IsProvisioningFromExternal;

                userToCreate.Roles = new List<string>() { "user" };

                userToCreate.Claims = new List<Claim>()
                {
                    new Claim("subscriptionlevel", "FreeUser", ClaimValueTypes.String, _applicationSetting.Issuer)
                };

                if (!string.IsNullOrEmpty(model.Country))
                {
                    userToCreate.Claims.Add(new Claim("country", model.Country, ClaimValueTypes.String, _applicationSetting.Issuer));
                }

                if (!string.IsNullOrEmpty(model.Firstname))
                {
                    userToCreate.Claims.Add(new Claim("given_name", model.Firstname, ClaimValueTypes.String, _applicationSetting.Issuer));
                }

                if (!string.IsNullOrEmpty(model.Lastname))
                {
                    userToCreate.Claims.Add(new Claim("family_name", model.Lastname, ClaimValueTypes.String, _applicationSetting.Issuer));
                }

                if (!string.IsNullOrEmpty(model.Username))
                {
                    userToCreate.Claims.Add(new Claim("nickname", model.Username, ClaimValueTypes.String, _applicationSetting.Issuer));
                }

                if (!string.IsNullOrEmpty(model.Email))
                {
                    userToCreate.Claims.Add(new Claim("email", model.Email, ClaimValueTypes.Email, _applicationSetting.Issuer));
                }

                if (!string.IsNullOrEmpty(model.ProfileImage))
                {
                    userToCreate.Claims.Add(new Claim("picture", model.ProfileImage, ClaimValueTypes.String, _applicationSetting.Issuer));
                }

                // if we're provisioning a user via external login, we must add the provider &
                // user id at the provider to this user's logins
                if (model.IsProvisioningFromExternal)
                {
                    userToCreate.ProviderName = model.Provider;
                    userToCreate.ProviderSubjectId = model.ProviderUserId;
                }

                // add it through the repository
                OAuthUserCreate oauthUserCreate = new OAuthUserCreate()
                {
                    Email = userToCreate.Email,
                    EmailVerified = userToCreate.EmailVerified,
                    Password = userToCreate.Password,
                    Claims = userToCreate.Claims,
                    Roles = userToCreate.Roles
                };

                if (!string.IsNullOrEmpty(userToCreate.ProviderName) &&
                    !string.IsNullOrEmpty(userToCreate.ProviderSubjectId) &&
                    accessTokenItem.AccessToken != null)
                {
                    oauthUserCreate.Providers = new List<Provider>()
                    {
                        new Provider()
                        {
                            ProviderName = userToCreate.ProviderName,
                            ProviderSubjectId = userToCreate.ProviderSubjectId,
                            AccessToken = accessTokenItem.AccessToken,
                            AccessTokenExpiresAt = accessTokenItem.CreatedOn
                        }
                    };
                }

                userToCreate.Id = await _oauthUserServices.CreateLoginAsync(oauthUserCreate);
                
                if (string.IsNullOrEmpty(userToCreate.Id))
                {
                    throw new Exception($"Creating a user failed.");
                }

                await _userService.CreateAsync(new UserItem()
                {
                    Id = userToCreate.Id,
                    Email = model.Email,
                    Country = model.Country,
                    EmailVerified = model.IsProvisioningFromExternal,
                    FirstName = model.Firstname,
                    LastName = model.Lastname,
                    ProfileImageUrl = model.ProfileImage
                });

                if (!model.IsProvisioningFromExternal)
                {
                    // log the user in
                    await HttpContext.SignInAsync(userToCreate.Id, userToCreate.Claims.ToArray());
                }


                await _qeueMessageServices.SendCreateUser(userToCreate);

                // continue with the flow     
                if (_interaction.IsValidReturnUrl(model.ReturnUrl) || Url.IsLocalUrl(model.ReturnUrl))
                {
                    return Redirect(model.ReturnUrl);
                }

                return Redirect("~/");
            }
            
            // ModelState invalid, return the view with the passed-in model
            // so changes can be made
            return View(model);
        }
    }
}