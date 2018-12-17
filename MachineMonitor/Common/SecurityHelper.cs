using System;
using System.Collections.Generic;
using System.Text;
using System.Security.Cryptography;

namespace Common
{
    public class SecurityHelper
    {
        /// <summary>
        /// HMACSHA256加密
        /// </summary>
        /// <param name="key">加密密钥</param>
        /// <param name="message">要加密的字符串（API_KEY + message）</param>
        /// <returns></returns>
        public static string Hmacsha256(string Hmacsha256Key, string message)
        {
            //key = key ?? "";
            var encoding = new System.Text.ASCIIEncoding();
            byte[] keyByte = encoding.GetBytes(Hmacsha256Key);
            byte[] messageBytes = encoding.GetBytes(message);
            using (var hmacsha256 = new HMACSHA256(keyByte))
            {
                byte[] hashmessage = hmacsha256.ComputeHash(messageBytes);
                return Convert.ToBase64String(hashmessage);
            }
        }
    }
}
