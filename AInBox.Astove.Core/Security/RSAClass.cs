using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace AInBox.Astove.Core.Security
{
    public class RSAClass
    {
        private static string _privateKey = "<RSAKeyValue><Modulus>5WYl5bY6M4ulT/PT0qOASjSlQ5OcmwWU5x9tXMZfmneZ5YpzT5azgITujmtn7aXkPl/OE3+JI2jCbhkcbWkjfQ==</Modulus><Exponent>AQAB</Exponent><P>6me0v67IiW1Hfr6J3jGPMWlzf2DlmUF4/1QFwLAf3xk=</P><Q>+ohgbMklFDFsjRszeJL7gteJmK1BF/0lFCiEBEN5CAU=</Q><DP>z+FjCe+vBzm0AzJlwHkBPdgARwIe/Nh0vzO72lQYH9k=</DP><DQ>EjBQf9ViocKs1NnCtOBG7krjrHf3n9w7EumWHBEh+lk=</DQ><InverseQ>P+GG+EjJKeeKIhSjC3TN+BNwet2cxjgpSdgvVlSpKk8=</InverseQ><D>IyKW1dzoA6qVFo6YKDg6J0Nyd4v1jcnYUWD/IVlR3ci+/2RnxhLEAvr5KibKApODMWpLyjxeDtEbY+yqP4JNKQ==</D></RSAKeyValue>";
        private static string _publicKey = "<RSAKeyValue><Modulus>5WYl5bY6M4ulT/PT0qOASjSlQ5OcmwWU5x9tXMZfmneZ5YpzT5azgITujmtn7aXkPl/OE3+JI2jCbhkcbWkjfQ==</Modulus><Exponent>AQAB</Exponent></RSAKeyValue>";
        private static UnicodeEncoding _encoder = new UnicodeEncoding();

        public static string Decrypt(string data)
        {
            var rsa = new RSACryptoServiceProvider();
            var dataArray = data.Split(new char[] { ',' });
            byte[] dataByte = new byte[dataArray.Length];
            for (int i = 0; i < dataArray.Length; i++)
            {
                dataByte[i] = Convert.ToByte(dataArray[i]);
            }

            rsa.FromXmlString(_privateKey);
            var decryptedByte = rsa.Decrypt(dataByte, false);
            return _encoder.GetString(decryptedByte);
        }

        public static string Encrypt(string data)
        {
            var rsa = new RSACryptoServiceProvider();
            rsa.FromXmlString(_publicKey);
            var dataToEncrypt = _encoder.GetBytes(data);
            var encryptedByteArray = rsa.Encrypt(dataToEncrypt, false).ToArray();
            var length = encryptedByteArray.Count();
            var item = 0;
            var sb = new StringBuilder();
            foreach (var x in encryptedByteArray)
            {
                item++;
                sb.Append(x);

                if (item < length)
                    sb.Append(",");
            }

            return sb.ToString();
        }
    }
}
