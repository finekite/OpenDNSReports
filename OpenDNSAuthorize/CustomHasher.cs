using Microsoft.AspNet.Identity;
using System;
using System.Configuration;
using System.Security.Cryptography;
using System.Text;

namespace OpenDNSAuthorize
{
    public class CustomHasher : IPasswordHasher
    {
        internal string userId { get; set; }

        public string HashPassword(string password)
        {
            return OpenDnsCryptography.Encrypt(password, userId);
        }

        public PasswordVerificationResult VerifyHashedPassword(string hashedPassword, string providedPassword)
        {
            if (hashedPassword.Equals(HashPassword(providedPassword)))
                return PasswordVerificationResult.Success;
            return PasswordVerificationResult.Failed;
        }
    }
}
