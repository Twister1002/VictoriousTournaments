using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Security.Cryptography;
using System.Text;

namespace WebApplication.Utility
{
    public class HashManager
    {
        SHA512CryptoServiceProvider service;
        RNGCryptoServiceProvider random;
        

        public HashManager()
        {
            service = new SHA512CryptoServiceProvider();
            random = new RNGCryptoServiceProvider();
        }

        public String GetSalt()
        {
            Byte[] salt = new Byte[32];
            return Convert.ToBase64String(salt);
        }

        public String HashPassword(String pass)
        {
            var bytes = new UTF8Encoding().GetBytes(pass);
            var hashedBytes = service.ComputeHash(bytes);
        }
    }
}