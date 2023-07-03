using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace OpenDnsLogs.Tests
{
    [TestClass]
    public class KeyGenerate
    {
        [TestMethod]
        public void KeyGenerator()
        {
            byte[] key = new byte[18];
            Crypto.GetRandomBytes(key);

            var keyToBase64 = Convert.ToBase64String(key);
        }
    }
}
