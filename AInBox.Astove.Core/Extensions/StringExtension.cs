using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace AInBox.Astove.Core.Extensions
{
    public static class StringExtension
    {
        public static string ToCamelCase(this string input)
        {
            string firstLetter = input.Substring(0, 1).ToLower();
            string rest = input.Substring(1, input.Length - 1);
            
            return string.Format("{0}{1}", firstLetter, rest);
        }

        public static string ToPascalCase(this string input)
        {
            string firstLetter = input.Substring(0, 1).ToUpper();
            string rest = input.Substring(1, input.Length - 1);

            return string.Format("{0}{1}", firstLetter, rest);
        }

        public static string ToSlug(this string phrase)
        {
            string str = phrase.RemoveAccent().ToLower();

            str = Regex.Replace(str, @"[^a-z0-9\s-]", ""); // invalid chars           
            str = Regex.Replace(str, @"\s+", " ").Trim(); // convert multiple spaces into one space   
            //str = str.Substring(0, str.Length <= 45 ? str.Length : 45).Trim(); // cut and trim it   
            str = str.Trim(); // cut and trim it   
            str = Regex.Replace(str, @"\s", "-"); // hyphens   

            return str;
        }

        public static string RemoveAccent(this string txt)
        {
            byte[] bytes = System.Text.Encoding.GetEncoding("Cyrillic").GetBytes(txt);
            return System.Text.Encoding.ASCII.GetString(bytes);
        }

        private static readonly string cryptoKey = "@InB0x6!35";
        private static readonly byte[] IV = new byte[8] { 240, 3, 45, 29, 0, 76, 173, 59 };

        public static string Encrypt(this string s)
        {
            return Encrypt(s, cryptoKey);
        }

        public static string Decrypt(this string s)
        {
            return Decrypt(s, cryptoKey);
        }

        public static string Encrypt(this string s, string key)
        {
            byte[] buffer = Encoding.ASCII.GetBytes(s);
            TripleDESCryptoServiceProvider des = new TripleDESCryptoServiceProvider();
            MD5CryptoServiceProvider MD5 = new MD5CryptoServiceProvider();
            des.Key = MD5.ComputeHash(ASCIIEncoding.ASCII.GetBytes(key));
            des.IV = IV;
            return Convert.ToBase64String(
                des.CreateEncryptor().TransformFinalBlock(
                    buffer,
                    0,
                    buffer.Length
                )
            );
        }

        public static string Decrypt(this string s, string key)
        {
            if (string.IsNullOrEmpty(s))
                return null;

            byte[] buffer = Convert.FromBase64String(s);
            TripleDESCryptoServiceProvider des = new TripleDESCryptoServiceProvider();
            MD5CryptoServiceProvider MD5 = new MD5CryptoServiceProvider();
            des.Key = MD5.ComputeHash(ASCIIEncoding.ASCII.GetBytes(key));
            des.IV = IV;
            return Encoding.ASCII.GetString(
                des.CreateDecryptor().TransformFinalBlock(
                    buffer,
                    0,
                    buffer.Length
                )
            );
        }

    }
}
