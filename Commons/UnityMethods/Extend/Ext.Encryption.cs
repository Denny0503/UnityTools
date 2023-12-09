using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Xml;
using UnityMethods.Logs;

namespace UnityMethods.Extend
{
    public static partial class Ext
    {
        /// <summary>
        /// 计算字符串的MD5值  ，小写形式
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>        
        public static string ToMD5_32(this string srcData)
        {
            try
            {
                if (!srcData.IsEmpty())
                {
                    //StringBuilder str = new StringBuilder();
                    //byte[] data = Encoding.GetEncoding("utf-8").GetBytes(srcData);
                    //MD5 md5 = new MD5CryptoServiceProvider();
                    //byte[] bytes = md5.ComputeHash(data);
                    //for (int i = 0; i < bytes.Length; i++)
                    //{
                    //    str.Append(bytes[i].ToString("x2"));
                    //}
                    //return str.ToString();

                    MD5CryptoServiceProvider md52 = new MD5CryptoServiceProvider();
                    return BitConverter.ToString(md52.ComputeHash(Encoding.GetEncoding("utf-8").GetBytes(srcData))).Replace("-", "").ToLower();
                }
                else
                {
                    return "";
                }

            }
            catch (Exception ex)
            {
                LogFactory.SystemLog(new System.Diagnostics.StackTrace().GetFrame(0).GetMethod().Name, LogLevelType.Exception, ex);
                return "";
            }
        }

        /// <summary>
        /// 获取16位的md5值
        /// </summary>
        /// <param name="srcData"></param>
        /// <returns></returns>
        public static string ToMD5_16(this string srcData)
        {
            try
            {
                if (!srcData.IsEmpty())
                {
                    MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
                    return BitConverter.ToString(md5.ComputeHash(UTF8Encoding.Default.GetBytes(srcData)), 4, 8).Replace("-", "");
                }
                else
                {
                    return "";
                }
            }
            catch (Exception ex)
            {
                LogFactory.SystemLog(new System.Diagnostics.StackTrace().GetFrame(0).GetMethod().Name, LogLevelType.Exception, ex);
                return "";
            }
        }

        /// <summary>
        /// 获取des加密字符串
        /// </summary>
        /// <param name="srcData"></param>
        /// <returns></returns>
        public static string GetDES_Encrypt(this object srcData)
        {
            return DES_Encrypt(srcData.ToString());
        }

        /// <summary>
        /// 获取des加密字符串
        /// </summary>
        /// <param name="srcData"></param>
        /// <returns></returns>
        public static string GetDES_Encrypt(this object srcData, string encryptKey)
        {
            return DES_Encrypt(srcData.ToString(), encryptKey);
        }

        /// <summary>
        /// DES加密
        /// </summary>
        /// <param name="str">需要加密的字符</param>
        /// <param name="sKey">密匙</param>
        /// <returns></returns>
        public static string DES_Encrypt(string str, string sKey = "desencrypt")
        {
            DESCryptoServiceProvider des = new DESCryptoServiceProvider();
            byte[] inputByteArray = Encoding.Default.GetBytes(str);
            des.Key = ASCIIEncoding.ASCII.GetBytes(sKey.ToMD5_32().Substring(0, 8));// 密匙
            des.IV = ASCIIEncoding.ASCII.GetBytes(sKey.ToMD5_32().Substring(0, 8));// 初始化向量
            MemoryStream ms = new MemoryStream();
            CryptoStream cs = new CryptoStream(ms, des.CreateEncryptor(), CryptoStreamMode.Write);
            cs.Write(inputByteArray, 0, inputByteArray.Length);
            cs.FlushFinalBlock();
            //var retB = Convert.ToBase64String(ms.ToArray());
            var retB = ToHEX(ms.ToArray());
            return retB;
        }

        /// <summary>
        /// 字符串转为16进制
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string ToHEX(byte[] values)
        {
            string result = "";
            foreach (byte letter in values)
            {
                int value = Convert.ToInt32(letter);
                result += String.Format("{0:x2}", value);
            }
            return result;
        }

        private static byte[] HEXToByte(string hexStr)
        {
            byte[] result = new byte[hexStr.Length / 2];

            for (int i = 0; i < hexStr.Length; i += 2)
            {
                result[i / 2] = (byte)Convert.ToInt32(string.Format("{0}{1}", hexStr[i], hexStr[i + 1]), 16);
            }

            return result;
        }

        /// <summary>
        /// 获取解密后的字符
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string GetDES_Decrypt(this object obj)
        {
            return DES_Decrypt(obj.ToString());
        }

        /// <summary>
        /// DES解密
        /// </summary>
        /// <param name="pToDecrypt">需要解密的字符</param>
        /// <param name="sKey">密匙</param>
        /// <returns></returns>
        public static string DES_Decrypt(string pToDecrypt, string sKey = "desencrypt")
        {
            try
            {
                DESCryptoServiceProvider des = new DESCryptoServiceProvider();
                //byte[] inputByteArray = Convert.FromBase64String(pToDecrypt);
                byte[] inputByteArray = HEXToByte(pToDecrypt);
                des.Key = ASCIIEncoding.UTF8.GetBytes(sKey.ToMD5_32().Substring(0, 8));
                des.IV = ASCIIEncoding.UTF8.GetBytes(sKey.ToMD5_32().Substring(0, 8));
                MemoryStream ms = new MemoryStream();
                CryptoStream cs = new CryptoStream(ms, des.CreateDecryptor(), CryptoStreamMode.Write);
                cs.Write(inputByteArray, 0, inputByteArray.Length);
                // 如果两次密匙不一样，这一步可能会引发异常
                cs.FlushFinalBlock();
                return Encoding.UTF8.GetString(ms.ToArray());
            }
            catch (Exception ex)
            {
                LogFactory.SystemLog(new System.Diagnostics.StackTrace().GetFrame(0).GetMethod().Name, LogLevelType.Exception, ex);
                return "";
            }
        }

        public static RSACryptoServiceProvider CreateRSADEEncryptProvider(String privateKeyFile)
        {
            RSAParameters parameters1;
            parameters1 = new RSAParameters();
            StreamReader reader1 = new StreamReader(privateKeyFile);
            XmlDocument document1 = new XmlDocument();
            document1.LoadXml(reader1.ReadToEnd());
            XmlElement element1 = (XmlElement)document1.SelectSingleNode("root");
            parameters1.Modulus = ReadChild(element1, "Modulus");
            parameters1.Exponent = ReadChild(element1, "Exponent");
            parameters1.D = ReadChild(element1, "D");
            parameters1.DP = ReadChild(element1, "DP");
            parameters1.DQ = ReadChild(element1, "DQ");
            parameters1.P = ReadChild(element1, "P");
            parameters1.Q = ReadChild(element1, "Q");
            parameters1.InverseQ = ReadChild(element1, "InverseQ");
            CspParameters parameters2 = new CspParameters();
            parameters2.Flags = CspProviderFlags.UseMachineKeyStore;
            RSACryptoServiceProvider provider1 = new RSACryptoServiceProvider(parameters2);
            provider1.ImportParameters(parameters1);
            reader1.Close();
            return provider1;
        }
        public static RSACryptoServiceProvider CreateRSAEncryptProvider(String publicKeyFile)
        {
            RSAParameters parameters1;
            parameters1 = new RSAParameters();
            StreamReader reader1 = new StreamReader(publicKeyFile);
            XmlDocument document1 = new XmlDocument();
            document1.LoadXml(reader1.ReadToEnd());
            XmlElement element1 = (XmlElement)document1.SelectSingleNode("RSAKeyValue");
            parameters1.Modulus = ReadChild(element1, "Modulus");
            parameters1.Exponent = ReadChild(element1, "Exponent");
            CspParameters parameters2 = new CspParameters();
            parameters2.Flags = CspProviderFlags.UseMachineKeyStore;
            RSACryptoServiceProvider provider1 = new RSACryptoServiceProvider(parameters2);
            provider1.ImportParameters(parameters1);
            reader1.Close();
            return provider1;
        }

        public static byte[] ReadChild(XmlElement parent, string name)
        {
            XmlElement element1 = (XmlElement)parent.SelectSingleNode(name);
            //return hexToBytes(element1.InnerText);
            return Convert.FromBase64String(element1.InnerText);
        }

        public static string ToHexString(byte[] bytes) // 0xae00cf => "AE00CF "
        {
            string hexString = string.Empty;
            if (bytes != null)
            {
                StringBuilder strB = new StringBuilder();

                for (int i = 0; i < bytes.Length; i++)
                {
                    strB.Append(bytes[i].ToString("X2"));
                }
                hexString = strB.ToString();
            }
            return hexString;
        }
        public static byte[] hexToBytes(String src)
        {
            int l = src.Length / 2;
            String str;
            byte[] ret = new byte[l];

            for (int i = 0; i < l; i++)
            {
                str = src.Substring(i * 2, 2);
                ret[i] = Convert.ToByte(str, 16);
            }
            return ret;
        }

        public static void SaveToFile(String filename, String data)
        {
            System.IO.StreamWriter sw = System.IO.File.CreateText(filename);
            sw.WriteLine(data);
            sw.Close();
        }

        public static string EnCrypt(string str)
        {
            RSACryptoServiceProvider rsaencrype = CreateRSAEncryptProvider("publickey.xml");
            String text = str;
            byte[] data = new UnicodeEncoding().GetBytes(text);
            byte[] endata = rsaencrype.Encrypt(data, true);
            return ToHexString(endata);
        }

        public static bool VerifyData(string desStr, string key)
        {
            RSACryptoServiceProvider rsaencrype = CreateRSAEncryptProvider("publickey.xml");
            byte[] data = new UnicodeEncoding().GetBytes(desStr);
            byte[] endata = new UnicodeEncoding().GetBytes(key);
            return rsaencrype.VerifyData(data, "SHA1", endata);
        }

        public static string DoEncrypt(string hexstr)
        {
            RSACryptoServiceProvider rsadeencrypt = CreateRSADEEncryptProvider("privatekey.xml");

            byte[] miwen = hexToBytes(hexstr);

            byte[] dedata = rsadeencrypt.Decrypt(miwen, true);

            return System.Text.UnicodeEncoding.Unicode.GetString(dedata);
        }

        /// <summary>
        /// RSA签名验证
        /// </summary>
        /// <param name="strKeyPublic">公钥</param>
        /// <param name="HashbyteDeformatter">Hash描述</param>
        /// <param name="DeformatterData">签名后的结果</param>
        /// <returns></returns>
        public static bool SignatureDeformatter(string strKeyPublic, byte[] HashbyteDeformatter, byte[] DeformatterData)
        {
            bool result = false;
            try
            {
                if (!strKeyPublic.IsEmpty() && null != HashbyteDeformatter && null != DeformatterData)
                {
                    RSACryptoServiceProvider RSA = new RSACryptoServiceProvider();
                    RSA.FromXmlString(strKeyPublic);

                    //指定解密的时候HASH算法为MD5 
                    if (RSA.VerifyData(HashbyteDeformatter, "SHA1", DeformatterData))
                    {
                        result = true;
                    }
                }
            }
            catch (ArgumentNullException ex) { LogFactory.SystemLog(new System.Diagnostics.StackTrace().GetFrame(0).GetMethod().Name, LogLevelType.Exception, ex); }

            return result;
        }
    }
}
