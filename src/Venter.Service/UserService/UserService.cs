using IdentityModel;
using Microsoft.ApplicationInsights;
using Microsoft.Extensions.Logging;
using Vivius.Interface.Repositories;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Vivius.Model.User;
using Vivius.Services.Mail;
using Vivus.Model.Mail;

namespace Venter.Service.UserService
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly ILogger<UserService> _logger;
        private readonly IMailService _mailService;

        private readonly TelemetryClient _telemetry = new TelemetryClient();

        public UserService(IUserRepository userRepository,
            ILogger<UserService> logger,
            IMailService mailService)
        {
            _userRepository = userRepository;
            _logger = logger;
            _mailService = mailService;
        }

        public async Task<string> CreateAsync(UserItem user, bool sendEmail = true)
        {
            if (user == null)
                throw new ArgumentNullException("User argument cant be null");
                        
            string userId = await _userRepository.CreateAsync(user);

            if (userId != null)
            {
                if (sendEmail)
                {
                    await _mailService.SendVerifyMail(new VerifyItem()
                    {
                        Email = user.Email,
                        UserId = userId,
                        FirstName = user.FirstName,
                        FullName = $"{user?.FirstName ?? ""} {user?.LastName ?? ""}"
                    });
                }
            }

            return userId;
        }
         
        public async Task<bool> CheckIfExistAsync(string email)
        {
            return await _userRepository.CheckIfExistAsync(email);
        }
        
        public async Task<string> GetIdByEmailAsync(string id)
        {
            return await _userRepository.GetIdByEmailAsync(id);
        }
        
        public Task<UserProfileItem> GetProfileAsync(string sub)
        {
            return _userRepository.GetProfileAsync(sub);
        }
        
        public async Task<bool> DeleteAccountAsync(string userId)
        {
            return await _userRepository.DeleteAccountAsync(userId);
        }

        public async Task<UserEmailItem> FindByExternalProviderAsync(string provider, string userId)
        {
            return await _userRepository.FindByExternalProviderAsync(provider, userId);
        }

        public async Task<bool> SetVerifyEmailAsync(string userId)
        {
            return await _userRepository.SetVerifyEmailAsync(userId);
        }

        public async Task RequestEmailChangeAsync(string newEmail, string oldEmail, string code)
        {
            await _userRepository.RequestEmailChangeAsync(newEmail, oldEmail, code);
        }

        public async Task<string[]> CheckEmailChangeAsync(string code)
        {
            return await _userRepository.CheckEmailChangeAsync(code);
        }

        public async Task<bool> CommitEmailChange(string NewEmail, string OldEmail)
        {
            return await _userRepository.CommitEmailChange(NewEmail, OldEmail);
        }

        public async Task UpdateLastLogin(string userId)
        {
            await _userRepository.UpdateLastLoginAsync(userId);
        }
    }
}
