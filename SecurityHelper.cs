using System;
using System.Security.Cryptography;
using System.Text;

namespace HastaneYonetim
{
    public static class SecurityHelper
    {
        public static string HashPassword(string plainTextPassword)
        {
            if (string.IsNullOrWhiteSpace(plainTextPassword))
                return null;

            using (SHA256 sha256Hash = SHA256.Create())
            {
                // ComputeHash returns byte array, convert it to a string
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(plainTextPassword));

                // Return Base64 string which is 44 characters long and fits in varchar(50)
                return Convert.ToBase64String(bytes);
            }
        }
    }
}
