using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
using System.IO;

namespace Hx.Tools
{
    public class EncryptString
    {
        //加密
        public static String Encrypt(String data)
        {
            byte[] bytes = Encoding.Unicode.GetBytes(data);
            byte[] key = { 23, 136, 56, 87, 172, 215, 242, 114 };
            byte[] iv = { 125, 217, 69, 74, 131, 225, 52, 160 };
            DESCryptoServiceProvider desc = new DESCryptoServiceProvider();
            ICryptoTransform entrans = desc.CreateEncryptor(key, iv);
            byte[] en = entrans.TransformFinalBlock(bytes, 0, bytes.Length);
            String res = Encoding.Unicode.GetString(en);
            return res;
        }

        //解密   
        public static String Decrypt(String data)
        {
            byte[] bytes = Encoding.Unicode.GetBytes(data);
            byte[] key = { 23, 136, 56, 87, 172, 215, 242, 114 };
            byte[] iv = { 125, 217, 69, 74, 131, 225, 52, 160 };
            DESCryptoServiceProvider desc = new DESCryptoServiceProvider();
            ICryptoTransform detrans = desc.CreateDecryptor(key, iv);
            byte[] de = detrans.TransformFinalBlock(bytes, 0, bytes.Length);
            String res = Encoding.Unicode.GetString(de);
            return res;
        }

        /// <summary>
        /// MD5加密字符串处理
        /// </summary>
        /// <param name="Half">加密是16位还是32位；如果为true为16位</param>
        /// <param name="Input">待加密码字符串</param>
        /// <returns></returns>
        public static string MD5(string Input, bool Half)
        {
            string output = System.Web.Security.FormsAuthentication.HashPasswordForStoringInConfigFile(Input, "MD5").ToLower();
            if (Half)//16位MD5加密（取32位加密的9~25字符）
                output = output.Substring(8, 16);
            return output;
        }

        public static string MD5(string Input)
        {
            return MD5(Input, true);
        }
        //默认密钥向量
        private static byte[] Keys = { 0x12, 0x34, 0x56, 0x78, 0x90, 0xAB, 0xCD, 0xEF };
        /// <summary>
        /// DES加密字符串
        /// </summary>
        /// <param name="encryptString">待加密的字符串</param>
        /// <param name="encryptKey">加密密钥,要求为8位</param>
        /// <returns>加密成功返回加密后的字符串,失败返回源串</returns>
        public static string DESEncode(string encryptString, string encryptKey)
        {
            encryptKey = StrHelper.GetSubString(encryptKey, 8, "");
            encryptKey = encryptKey.PadRight(8, ' ');
            byte[] rgbKey = Encoding.UTF8.GetBytes(encryptKey.Substring(0, 8));
            byte[] rgbIV = Keys;
            byte[] inputByteArray = Encoding.UTF8.GetBytes(encryptString);
            DESCryptoServiceProvider dCSP = new DESCryptoServiceProvider();
            MemoryStream mStream = new MemoryStream();
            CryptoStream cStream = new CryptoStream(mStream, dCSP.CreateEncryptor(rgbKey, rgbIV), CryptoStreamMode.Write);
            cStream.Write(inputByteArray, 0, inputByteArray.Length);
            cStream.FlushFinalBlock();
            return Convert.ToBase64String(mStream.ToArray());

        }

        /// <summary>
        /// DES解密字符串
        /// </summary>
        /// <param name="decryptString">待解密的字符串</param>
        /// <param name="decryptKey">解密密钥,要求为8位,和加密密钥相同</param>
        /// <returns>解密成功返回解密后的字符串,失败返源串</returns>
        public static string DESDecode(string decryptString, string decryptKey)
        {
            try
            {
                decryptKey = StrHelper.GetSubString(decryptKey, 8, "");
                decryptKey = decryptKey.PadRight(8, ' ');
                byte[] rgbKey = Encoding.UTF8.GetBytes(decryptKey);
                byte[] rgbIV = Keys;
                byte[] inputByteArray = Convert.FromBase64String(decryptString);
                DESCryptoServiceProvider DCSP = new DESCryptoServiceProvider();

                MemoryStream mStream = new MemoryStream();
                CryptoStream cStream = new CryptoStream(mStream, DCSP.CreateDecryptor(rgbKey, rgbIV), CryptoStreamMode.Write);
                cStream.Write(inputByteArray, 0, inputByteArray.Length);
                cStream.FlushFinalBlock();
                return Encoding.UTF8.GetString(mStream.ToArray());
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// SHA256函数
        /// </summary>
        /// /// <param name="str">原始字符串</param>
        /// <returns>SHA256结果</returns>
        public static string SHA256(string str)
        {
            byte[] SHA256Data = Encoding.UTF8.GetBytes(str);
            SHA256Managed Sha256 = new SHA256Managed();
            byte[] Result = Sha256.ComputeHash(SHA256Data);
            return Convert.ToBase64String(Result);  //返回长度为44字节的字符串
        }

        //SHA1
        public static string SHA1_Hash(string str_sha1_in)
        {
            SHA1 sha1 = new SHA1CryptoServiceProvider();
            byte[] bytes_sha1_in = UTF8Encoding.Default.GetBytes(str_sha1_in);
            byte[] bytes_sha1_out = sha1.ComputeHash(bytes_sha1_in);
            string str_sha1_out = BitConverter.ToString(bytes_sha1_out).Replace("-",string.Empty);
            //str_sha1_out = str_sha1_out.Replace("-", "");
            return str_sha1_out;
        }
    }
}
