using Microsoft.AspNet.Identity;

namespace OpenDNSAuthorize
{
    public class CustomHasher : IPasswordHasher
    {
        public string HashPassword(string password)
        {
            // Encrypt here
            return password;
        }

        public PasswordVerificationResult VerifyHashedPassword(string hashedPassword, string providedPassword)
        {
            // Decrypt here 
            if (hashedPassword.Equals(providedPassword))
                return PasswordVerificationResult.Success;
            return PasswordVerificationResult.Failed;
        }
    }
}
