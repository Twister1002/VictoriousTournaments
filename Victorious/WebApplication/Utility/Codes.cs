using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace WebApplication.Utility
{
    public static class Codes
    {
        public static String GenerateInviteCode(int length = 7)
        {
            Random random = new Random();
            String characters = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
            StringBuilder builder = new StringBuilder(length);

            for (int i = 0; i < length; i++)
            {
                builder.Append(characters[random.Next(characters.Length)]);
            }

            return builder.ToString();
        }
    }
}