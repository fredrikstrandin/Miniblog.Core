// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using IdentityModel;
using IdentityServer4.MongoDB.Model;
using IdentityServer4.MongoDB.Model.Enum;
using IdentityServer4.MongoDB.Model.Setting;
using IdentityServer4.MongoDB.Repository;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace IdentityServer4.MongoDB.Service
{
    public class UserMongoDBService : IUserMongoDBService
    {
        private readonly IUserMongoDBRepository _userMongoDBRepository;
        private readonly IPasswordService _passwordService;
        private readonly PasswordSetting _passwordSetting;
        
        public UserMongoDBService(IUserMongoDBRepository userMongoDBRepository,
            IPasswordService passwordService,
            IOptions<PasswordSetting> passwordSetting)
        {
            _userMongoDBRepository = userMongoDBRepository;
            _passwordService = passwordService;
            _passwordSetting = passwordSetting.Value;
        }

        public async Task<bool> ValidateCredentialsAsync(string email, string password)
        {
            if (password.Length > _passwordSetting.PasswordMaxLength)
            {
                return false;
            }

            var user = await _userMongoDBRepository.FindByEmailAsync(email);

            if (user != null)
            {                
                if (_passwordService.CompareHash(password, user.Password, user.Salt))
                {
                    return true;
                }                
            }

            return false;
        }

        public async Task<string> FindIdByEmailAsync(string email)
        {
            return await _userMongoDBRepository.FindIdByEmailAsync(email);
        }

        public async Task<OAuthUserEntity> FindByEmailAsync(string email)
        {
            return await _userMongoDBRepository.FindByEmailAsync(email);
        }
        
        public async Task<OAuthUserEntity> FindBySubjectIdAsync(string id)
        {
            return await _userMongoDBRepository.FindBySubjectIdAsync(id);
        }

        public async Task<List<Claim>> FindClaimsBySubjectIdAsync(string id)
        {
            return await _userMongoDBRepository.FindClaimsBySubjectIdAsync(id);
        }

        public async Task<bool> SetPasswordAsync(string userId, string newPassword, string email)
        {
            return await _userMongoDBRepository.SetPasswordAsync(userId, newPassword, email);
        }
        
        public Task<string> CreateLoginAsync(OAuthUserCreate item)
        {
            if(item == null)
            {
                return null;
            }
            
            OAuthUserItem entity = new OAuthUserItem()
            {
                Email = item.Email,
                EmailVerified = item.EmailVerified,
                Password = item.Password,
                Providers = item.Providers,
                Claims = item.Claims,
                Roles = item.Roles,
                IsActive = true                
            };

            if (!string.IsNullOrEmpty(item.Password))
            {
                entity.Salt = _passwordService.GenerateSalt();
                entity.Password = _passwordService.CreateHash(item.Password, entity.Salt);
            }

            return _userMongoDBRepository.CreateLoginAsync(entity);

        }

        public Task<bool> GetIfPasswordExists(string userId)
        {
            return _userMongoDBRepository.GetIfPasswordExists(userId);
        }

        public async Task<OAuthUserItem> FindByExternalProviderAsync(string provider, string subjectId)
        {
            return await _userMongoDBRepository.FindByExternalProviderAsync(provider, subjectId);
        }
        
        public async Task<bool> IsActiveAsync(string id)
        {
            return await _userMongoDBRepository.IsActiveAsync(id);
        }

        public async Task CreateTempOAuthUserAsync(string provider, string subjectId, List<Claim> claims, string access_token, DateTime? expiresAt)
        {
            await _userMongoDBRepository.UpsertTempOAuthUserAsync(provider, subjectId, claims, access_token, expiresAt);
        }

        public async Task<List<Claim>> GetTempOAuthUserClaimsAsync(string provider, string subjectId)
        {
            return await _userMongoDBRepository.GetTempOAuthUserClaimsAsync(provider, subjectId);
        }

        public async Task<TempOAuthUserAccessTokenItem> GetTempOAuthUserAccessTokenAsync(string provider, string subjectId)
        {
            return await _userMongoDBRepository.GetTempOAuthUserAccessTokenAsync(provider, subjectId);
        }

        public async Task UpdateAccessToken(string provider, string subjectId, string access_token, DateTime? expiresAt)
        {
            await _userMongoDBRepository.UpdateAccessToken(subjectId, provider, access_token, expiresAt);
        }
    }
}