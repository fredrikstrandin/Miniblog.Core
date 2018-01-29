using System.Security.Cryptography;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System;
using IdentityServer4.MongoDB.Model.Setting;
using Microsoft.Extensions.Options;

namespace IdentityServer4.MongoDB.Service
{
    public class PasswordService : IPasswordService
    {
        private readonly PasswordSetting _passwordSetting;

        public PasswordService(IOptions<PasswordSetting> passwordSetting)
        {
            _passwordSetting = passwordSetting.Value;
        }

        public byte[] GenerateSalt()
        {
            // generate a 128-bit salt using a secure PRNG
            byte[] salt = new byte[128 / 8];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }

            return salt;
        }

        public string CreateHash(string password, byte[] salt)
        {
            // derive a 512-bit subkey (use HMACSHA1 with 10,000 iterations)
            return Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: password,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: _passwordSetting.HaschCount,
                numBytesRequested: 512 / 8));
        }

        public bool CompareHash(string password, string hash, byte[] salt)
        {
            string compareHashe = CreateHash(password, salt);
            return hash == compareHashe;
        }
    }
}
