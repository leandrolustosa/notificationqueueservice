using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
using System.IO;

namespace AInBox.Astove.Core.Security
{
    public class RijndaelManagedClass
    {
        private const string PasswordHash = "PT0qOASjSlQ5OcmwWU5x9tXMZfmneZ5YpzT5azgITujmtn7aXkPlOE3+JI2jCbhkcbWkjfQEjBQf9ViocKs1NnCtOBG7krjrHf3n9w7EumWHBEh+lk=";
        private const string SaltKey = "Nh0vzO72lQYH9kuo";

        public static string Encrypt(string plainText)
        {
            byte[] plainTextBytes = Encoding.UTF8.GetBytes(plainText);
            Rijndael rijndael = Rijndael.Create();
            Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(PasswordHash, Encoding.ASCII.GetBytes(SaltKey));
            rijndael.Key = pdb.GetBytes(32);
            rijndael.IV = pdb.GetBytes(16);

            byte[] cipherTextBytes;
            using (var memoryStream = new MemoryStream())
            {
                using (var cryptoStream = new CryptoStream(memoryStream, rijndael.CreateEncryptor(), CryptoStreamMode.Write))
                {
                    cryptoStream.Write(plainTextBytes, 0, plainTextBytes.Length);
                    cryptoStream.FlushFinalBlock();
                    cipherTextBytes = memoryStream.ToArray();
                    cryptoStream.Close();
                }
                memoryStream.Close();
            }
            return Convert.ToBase64String(cipherTextBytes);
        }        

        public static string Decrypt(string encryptedText)
        {
            byte[] cipherTextBytes = Convert.FromBase64String(encryptedText);
            Rijndael rijndael = Rijndael.Create();
            Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(PasswordHash, Encoding.ASCII.GetBytes(SaltKey));
            rijndael.Key = pdb.GetBytes(32);
            rijndael.IV = pdb.GetBytes(16);

            var decryptor = rijndael.CreateDecryptor();
            var memoryStream = new MemoryStream(cipherTextBytes);
            var cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read);
            byte[] plainTextBytes = new byte[cipherTextBytes.Length];

            int decryptedByteCount = cryptoStream.Read(plainTextBytes, 0, plainTextBytes.Length);
            memoryStream.Close();
            cryptoStream.Close();
            return Encoding.UTF8.GetString(plainTextBytes, 0, decryptedByteCount).TrimEnd("\0".ToCharArray());
        }

        public static string UrlTokenEncode(string token)
        {
            return System.Web.HttpServerUtility.UrlTokenEncode(System.Text.Encoding.UTF8.GetBytes(token));
        }

        public static string UrlTokenDecode(string encodedCredentials)
        {
            return Encoding.UTF8.GetString(System.Web.HttpServerUtility.UrlTokenDecode(encodedCredentials));
        }
    }
}
