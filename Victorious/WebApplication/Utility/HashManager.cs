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
        public const int SaltByteSize = 32;
        public const int HashByteSize = 32; // to match the size of the PBKDF2-HMAC-SHA-1 hash 
        public const int Pbkdf2Iterations = 1000;
        public const int IterationIndex = 0;
        public const int SaltIndex = 1;
        public const int Pbkdf2Index = 2;

        public static String GetSalt()
        {
            RNGCryptoServiceProvider crypto = new RNGCryptoServiceProvider();
            Byte[] salt = new Byte[SaltByteSize];
            crypto.GetBytes(salt);
            return Convert.ToBase64String(salt);
        }

        public static String HashPassword(String pass, String saltString)
        {
            byte[] salt = Convert.FromBase64String(saltString);
            byte[] hash = GetPbkdf2Bytes(pass, salt, Pbkdf2Iterations, HashByteSize);
            return Pbkdf2Iterations + ":" + Convert.ToBase64String(salt) + ":"+ Convert.ToBase64String(hash);
        }

        public static bool ValidatePassword(String password, String correctHash)
        {
            char[] delimiter = { ':' };
            var split = correctHash.Split(delimiter);
            var iterations = Int32.Parse(split[IterationIndex]);
            var salt = Convert.FromBase64String(split[SaltIndex]);
            var hash = Convert.FromBase64String(split[Pbkdf2Index]);

            var testHash = GetPbkdf2Bytes(password, salt, iterations, hash.Length);
            return SlowEquals(hash, testHash);
        }

        private static byte[] GetPbkdf2Bytes(String pass, byte[] salt, int iterations, int outputBytes)
        {
            Rfc2898DeriveBytes pbkdf2 = new Rfc2898DeriveBytes(pass, salt);
            pbkdf2.IterationCount = iterations;
            return pbkdf2.GetBytes(outputBytes);
        }

        private static bool SlowEquals(byte[] a, byte[] b)
        {
            var diff = (uint)a.Length ^ (uint)b.Length;
            for (int i = 0; i < a.Length && i < b.Length; i++)
            {
                diff |= (uint)(a[i] ^ b[i]);
            }
            return diff == 0;
        }
    }
}