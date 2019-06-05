using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace OpenDnsLogs.Tests
{
    public static class Crypto
    {
        public static void GetRandomBytes(byte[] buffer)
        {
            RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();

            rng.GetBytes(buffer);
        }
    }
}
