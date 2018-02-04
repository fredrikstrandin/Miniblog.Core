using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Venter.Service.Interface;
using Miniblog.Core.Repository.Mail;
using Miniblog.Core.Repository.VerifyUser;

namespace Miniblog.Core.Services.VerifyUser
{
    public class VerifyUserServices : IVerifyUserServices
    {
        private readonly IVerifyUserRepository _verifyUserRepository;
        private readonly IMailRepository _mailRepository;

        public VerifyUserServices(IVerifyUserRepository verifyUserRepository,
            IMailRepository mailRepository)
        {
            _verifyUserRepository = verifyUserRepository;
            _mailRepository = mailRepository;
        }

        public async Task<string> CreateCodeAsync(string userId)
        {
            return await _verifyUserRepository.CreateCodeAsync(userId);
        }

        public async Task DeleteVerifyCodeAsync(string code)
        {
            await _verifyUserRepository.DeleteVerifyCodeAsync(code);
        }

        public async Task<string> GetUserFromVerifyCodeAsync(string code)
        {
            return await _verifyUserRepository.GetUserFromVerifyCodeAsync(code);

        }
        
        public async Task<bool> VerifyCodeAsync(string code, string userId)
        {
            return await _verifyUserRepository.VerifyCodeAsync(code, userId);
        }
    }
}
