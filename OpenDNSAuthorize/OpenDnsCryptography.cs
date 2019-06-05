using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace OpenDNSAuthorize
{
    public class OpenDnsCryptography
    {
        public static string Encrypt(string password, string userId)
        {
            var cipher = CreateCipher(userId);
            var encryptor = cipher.CreateEncryptor();
            var encryptedPassword = encryptor.TransformFinalBlock(Encoding.UTF8.GetBytes(password), 0, password.Length);
            return Convert.ToBase64String(encryptedPassword);
        }

        public static string Decrypt(string hashedPassword, string userId)
        {
            var cipher = CreateCipher(userId);
            var decryptor = cipher.CreateDecryptor();
            var hashedPassowrdBytes = Convert.FromBase64String(hashedPassword);
            var password = decryptor.TransformFinalBlock(hashedPassowrdBytes, 0, hashedPassowrdBytes.Length);
            return Encoding.UTF8.GetString(password);
        }

        private static RijndaelManaged CreateCipher(string userId)
        {
            var start = Convert.ToInt32(ConfigurationManager.AppSettings["Start"]);
            var end = Convert.ToInt32(ConfigurationManager.AppSettings["End"]);

            var cipher = new RijndaelManaged();
            cipher.KeySize = Convert.ToInt32(ConfigurationManager.AppSettings["KeySize"]);
            cipher.BlockSize = Convert.ToInt32(ConfigurationManager.AppSettings["IVSize"]);
            cipher.Padding = PaddingMode.ISO10126;
            cipher.Mode = CipherMode.CBC;

            cipher.Key = Encoding.ASCII.GetBytes(ConfigurationManager.AppSettings["Venahapoch"] + userId.Substring(start, end));
            cipher.IV = Encoding.ASCII.GetBytes(userId.Substring(start, end) + ConfigurationManager.AppSettings["Venahapoch"]);

            return cipher;
        }
    }
}
