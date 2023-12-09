using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Management;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using UnityMethods.Logs;

namespace UnityMethods.Extend
{
    /// <summary>
    /// 系统扩展方法
    /// </summary>
    public static partial class Ext
    {
        #region 获取机器识别码

        /// <summary>
        /// 硬件设备类型
        /// </summary>
        internal enum DeviceType
        {
            /// <summary>
            /// （CPU）处理器
            /// </summary>
            Win32_Processor,
            /// <summary>
            /// 主板
            /// </summary>
            Win32_BaseBoard,
            /// <summary>
            /// 声卡
            /// </summary>
            Win32_SoundDevice,
            /// <summary>
            /// 主板BIOS
            /// </summary>
            Win32_BIOS
        }

        /// <summary>
        /// 设备对应信息
        /// </summary>
        internal enum DeviceSubType
        {
            /// <summary>
            /// CPU ID
            /// </summary>
            ProcessorId,
            /// <summary>
            /// 序列号
            /// </summary>
            SerialNumber,
            /// <summary>
            /// 设备ID
            /// </summary>
            DeviceID,
            /// <summary>
            /// 制造商
            /// </summary>
            Manufacturer,
        }

        /// <summary>
        /// 硬件信息
        /// </summary>
        private static Dictionary<string, ManagementObjectCollection> DeviceInfoDict = new Dictionary<string, ManagementObjectCollection>();

        /// <summary>
        /// BIOS ID
        /// </summary>
        private static string BIOSID { set; get; } = "";
        /// <summary>
        /// CPU ID
        /// </summary>
        private static string CPUID { set; get; } = "";
        /// <summary>
        /// 主板 ID
        /// </summary>
        private static string BASEID { set; get; } = "";

        /// <summary>
        /// 声卡设备ID
        /// </summary>
        private static string SoundDeviceID { set; get; } = "";

        /// <summary>
        /// 唯一机器码hash值
        /// </summary>
        private static string UniqueUUID_Hash { set; get; } = "";

        /// <summary>
        /// 读取硬件信息
        /// </summary>
        private static void LoadDeviceInfo()
        {
            try
            {
                DeviceInfoDict.Clear();
                var names = Enum.GetNames(typeof(DeviceType));
                foreach (string name in names)
                {
                    DeviceInfoDict.Add(name, new ManagementObjectSearcher("SELECT * FROM " + name).Get());
                }

                BIOSID = GetDeviceInfoByName(DeviceType.Win32_BIOS, DeviceSubType.Manufacturer);
                CPUID = GetDeviceInfoByName(DeviceType.Win32_Processor, DeviceSubType.ProcessorId);
                BASEID = GetDeviceInfoByName(DeviceType.Win32_BaseBoard, DeviceSubType.SerialNumber);
                SoundDeviceID = GetDeviceInfoByName(DeviceType.Win32_SoundDevice, DeviceSubType.DeviceID);

                UniqueUUID_Hash = GetHash($"Win32_BIOS:{BIOSID}|Win32_Processor:{CPUID}|Win32_BaseBoard:{BASEID}|Win32_SoundDevice:{SoundDeviceID}");
            }
            catch (Exception ex)
            {
                LogFactory.SystemLog(new System.Diagnostics.StackTrace().GetFrame(0).GetMethod().Name, LogLevelType.Exception, ex);
            }
        }

        /// <summary>
        /// 获取设备对应信息
        /// </summary>
        /// <param name="subType"></param>
        /// <returns></returns>
        private static string GetDeviceInfoByName(DeviceType deviceType, DeviceSubType subType)
        {
            string result = string.Empty;

            if (DeviceInfoDict.ContainsKey(deviceType.ToString()))
            {
                var query = DeviceInfoDict[deviceType.ToString()];

                foreach (var item in query)
                {
                    using (item)
                    {
                        result = item[subType.ToString()].ToString();
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// 获取唯一机器识别码
        /// </summary>
        /// <returns></returns>
        public static string GetUniqueUUID()
        {
            if (DeviceInfoDict.Count == 0)
            {
                LoadDeviceInfo();
            }

            return UniqueUUID_Hash.IsEmpty() ? GetHash($"windows:{BIOSID}|CPU:{CPUID}|BASE:{BASEID}") : UniqueUUID_Hash;
        }

        /// <summary>
        /// 获取字符串的哈希值
        /// </summary>
        /// <param name="srcData"></param>
        /// <returns></returns>
        private static string GetHash(string srcData)
        {
            StringBuilder str = new StringBuilder();
            byte[] data = Encoding.GetEncoding("utf-8").GetBytes((string)srcData);
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] bytes = md5.ComputeHash(data);
            for (int i = 0; i < bytes.Length; i++)
            {
                str.Append(bytes[i].ToString("X2"));
                if ((i + 1) != bytes.Length && (i + 1) % 2 == 0)
                {
                    str.Append("-");
                }
            }
            return str.ToString();
        }

        #endregion

        #region Windows凭据

        /// <summary>         
        /// 凭据类型         
        /// </summary> 
        public enum CRED_TYPE : uint
        {
            /// <summary>
            /// 普通凭据
            /// </summary>
            GENERIC = 1,
            /// <summary>
            /// 域密码 
            /// </summary>
            DOMAIN_PASSWORD = 2,
            /// <summary>
            /// 域证书
            /// </summary>
            DOMAIN_CERTIFICATE = 3,
            /// <summary>
            /// 域可见密码 
            /// </summary>
            DOMAIN_VISIBLE_PASSWORD = 4,
            /// <summary>
            /// 一般证书
            /// </summary>
            GENERIC_CERTIFICATE = 5,
            /// <summary>
            /// 域扩展 
            /// </summary>
            DOMAIN_EXTENDED = 6,
            /// <summary>
            /// 最大 
            /// </summary>
            MAXIMUM = 7,
            /// <summary>
            /// Maximum supported cred type 
            /// </summary>
            MAXIMUM_EX = (MAXIMUM + 1000),  // Allow new applications to run on old OSes 
        }

        /// <summary>
        /// 时效
        /// </summary>
        public enum CRED_PERSIST : uint
        {
            /// <summary>
            /// 会话
            /// </summary>
            SESSION = 1,
            /// <summary>
            /// 本地计算机 
            /// </summary>
            LOCAL_MACHINE = 2,
            /// <summary>
            /// 企业 
            /// </summary>
            ENTERPRISE = 3,
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct Credential
        {
            public UInt32 Flags;
            public CRED_TYPE Type;
            public string TargetName;
            public string Comment;
            public System.Runtime.InteropServices.ComTypes.FILETIME LastWritten;
            public UInt32 CredentialBlobSize;
            public string CredentialBlob;
            public CRED_PERSIST Persist;
            public UInt32 AttributeCount;
            public IntPtr Attributes;
            public string TargetAlias;
            public string UserName;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        private struct NativeCredential
        {
            public UInt32 Flags;
            public CRED_TYPE Type;
            public IntPtr TargetName;
            public IntPtr Comment;
            public System.Runtime.InteropServices.ComTypes.FILETIME LastWritten;
            public UInt32 CredentialBlobSize;
            public IntPtr CredentialBlob;
            public UInt32 Persist;
            public UInt32 AttributeCount;
            public IntPtr Attributes;
            public IntPtr TargetAlias;
            public IntPtr UserName;

            internal static NativeCredential GetNativeCredential(Credential cred)
            {
                NativeCredential ncred = new NativeCredential
                {
                    AttributeCount = 0,
                    Attributes = IntPtr.Zero,
                    Comment = IntPtr.Zero,
                    TargetAlias = IntPtr.Zero,
                    Type = cred.Type,
                    Persist = (UInt32)cred.Persist,
                    CredentialBlobSize = (UInt32)cred.CredentialBlobSize,
                    TargetName = Marshal.StringToCoTaskMemUni(cred.TargetName),
                    CredentialBlob = Marshal.StringToCoTaskMemUni(cred.CredentialBlob),
                    UserName = Marshal.StringToCoTaskMemUni(cred.UserName),
                };
                return ncred;
            }
        }

        internal class NativeCredMan
        {
            /// <summary>
            /// 读取凭据信息 
            /// </summary>
            /// <param name="target"></param>
            /// <param name="type"></param>
            /// <param name="reservedFlag"></param>
            /// <param name="CredentialPtr"></param>
            /// <returns></returns>
            [DllImport("Advapi32.dll", EntryPoint = "CredReadW", CharSet = CharSet.Unicode, SetLastError = true)]
            private static extern bool CredRead(string target, CRED_TYPE type, int reservedFlag, out IntPtr CredentialPtr);

            /// <summary>
            /// 增加凭据
            /// </summary>
            /// <param name="userCredential"></param>
            /// <param name="flags"></param>
            /// <returns></returns>
            [DllImport("Advapi32.dll", EntryPoint = "CredWriteW", CharSet = CharSet.Unicode, SetLastError = true)]
            private static extern bool CredWrite([In] ref NativeCredential userCredential, [In] UInt32 flags);

            [DllImport("Advapi32.dll", EntryPoint = "CredFree", SetLastError = true)]
            private static extern bool CredFree([In] IntPtr cred);

            /// <summary>
            /// 删除凭据 
            /// </summary>
            /// <param name="target"></param>
            /// <param name="type"></param>
            /// <param name="flags"></param>
            /// <returns></returns>
            [DllImport("Advapi32.dll", EntryPoint = "CredDeleteW", CharSet = CharSet.Unicode)]
            private static extern bool CredDelete(string target, CRED_TYPE type, int flags);

            //[DllImport("advapi32", SetLastError = true, CharSet = CharSet.Unicode)] 
            //static extern bool CredEnumerateold(string filter, int flag, out int count, out IntPtr pCredentials); 

            [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
            public static extern bool CredEnumerate(string filter, uint flag, out uint count, out IntPtr pCredentials);

            /// <summary> 
            /// 向添加计算机的凭据管理其中添加凭据             
            /// </summary> 
            /// <param name="key">internet地址或者网络地址</param>             
            /// <param name="userName">用户名</param>             
            /// <param name="secret">密码</param>             
            /// <param name="type">密码类型</param>             
            /// <param name="credPersist"></param>             
            /// <returns></returns> 
            public static bool WriteCred(string key, string userName, string secret, CRED_TYPE type, CRED_PERSIST credPersist)
            {
                byte[] byteArray = Encoding.Unicode.GetBytes(secret);
                if (byteArray.Length > 512)
                {
                    throw new ArgumentOutOfRangeException("The secret message has exceeded 512 bytes.");
                }

                Credential cred = new Credential
                {
                    TargetName = key,
                    CredentialBlob = secret,
                    CredentialBlobSize = (UInt32)Encoding.Unicode.GetBytes(secret).Length,
                    AttributeCount = 0,
                    Attributes = IntPtr.Zero,
                    UserName = userName,
                    Comment = null,
                    TargetAlias = null,
                    Type = type,
                    Persist = credPersist
                };

                NativeCredential ncred = NativeCredential.GetNativeCredential(cred);
                bool written = CredWrite(ref ncred, 0);

                if (!written)
                {
                    int lastError = Marshal.GetLastWin32Error();
                    if (lastError == 1312)
                    {
                        throw new Exception((string.Format(String.Format("Failed to save {0} with error code {{0}}.", key), lastError)
                        + "  This error typically occurrs on home editions of Windows XP and Vista.  Verify the version of Windows is Pro/Business or higher."));
                    }
                    else
                    {
                        throw new Exception(string.Format(String.Format("Failed to save {0} with error code {{0}}.", key), lastError));
                    }
                }

                return written;
            }

            /// <summary>            
            /// 读取凭据            
            /// </summary> 
            /// <param name="targetName"></param>            
            /// <param name="credType"></param>            
            /// <param name="reservedFlag"></param> 
            /// <param name="intPtr"></param>            
            /// <returns></returns> 
            public static bool WReadCred(string targetName, CRED_TYPE credType, int reservedFlag, out IntPtr intPtr)
            {
                return CredRead(targetName, credType, reservedFlag, out intPtr);
            }

            /// <summary>             
            /// 删除凭据             
            /// </summary> 
            /// <param name="target"></param>             
            /// <param name="type"></param>             
            /// <param name="flags"></param>             
            /// <returns></returns> 
            public static bool DeleteCred(string target, CRED_TYPE type, int flags)
            {
                return CredDelete(target, type, flags);
            }
        }

        /// <summary>
        /// 创建Windows凭据
        /// </summary>
        /// <param name="key">IP或者网络地址</param>
        /// <param name="userName">用户名</param>
        /// <param name="password">用户密码</param>
        /// <returns></returns>
        public static bool CreateCred(string key, string userName, string password)
        {
            bool flag = false;
            try
            {
                flag = NativeCredMan.WriteCred(key, userName, password, CRED_TYPE.GENERIC, CRED_PERSIST.LOCAL_MACHINE);
            }
            catch (Exception ex)
            {
                LogFactory.SystemLog(new System.Diagnostics.StackTrace().GetFrame(0).GetMethod().Name, LogLevelType.Exception, ex);
            }
            return flag;
        }

        /// <summary> 
        /// 查询凭据是否存在         
        /// </summary> 
        /// <param name="targetName">IP或者网络地址</param>   
        /// <returns>是否存在</returns>
        public static bool QueryCred(string targetName)
        {
            IntPtr intPtr = new IntPtr();
            return NativeCredMan.WReadCred(targetName, CRED_TYPE.GENERIC, 1, out intPtr);
        }

        /// <summary>
        /// 读取windows凭据
        /// </summary>
        /// <param name="targetName"></param>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public static bool ReadCred(string targetName, out string userName, out string password)
        {
            userName = "";
            password = "";
            IntPtr intPtr = new IntPtr();
            bool flag = false;
            try
            {
                flag = NativeCredMan.WReadCred(targetName, CRED_TYPE.GENERIC, 1, out intPtr);
                if (flag)
                {
                    NativeCredential credential = Marshal.PtrToStructure<NativeCredential>(intPtr);
                    userName = Marshal.PtrToStringAuto(credential.UserName);
                    password = Marshal.PtrToStringAuto(credential.CredentialBlob);
                }
            }
            catch (Exception ex)
            {
                LogFactory.SystemLog(new System.Diagnostics.StackTrace().GetFrame(0).GetMethod().Name, LogLevelType.Exception, ex);
            }
            return flag;
        }

        /// <summary>
        /// 删除凭据
        /// </summary>
        /// <param name="targetName">IP或者网络地址</param>
        /// <returns>是否删除成功</returns>
        public static bool DeleteCred(string targetName)
        {
            bool flag = false;
            try
            {
                IntPtr intPtr = new IntPtr();
                if (flag = NativeCredMan.WReadCred(targetName, CRED_TYPE.GENERIC, 1, out intPtr))
                {
                    flag = NativeCredMan.DeleteCred(targetName.Trim(), CRED_TYPE.GENERIC, 0);
                }
            }
            catch (Exception ex)
            {
                LogFactory.SystemLog(new System.Diagnostics.StackTrace().GetFrame(0).GetMethod().Name, LogLevelType.Exception, ex);
            }

            return flag;
        }

        #endregion

        #region 注册表操作

        /// <summary>
        /// 删除注册表
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public static void RemoveRegistryKey(string key, string subKey)
        {
            if (!subKey.IsEmpty() && !key.IsEmpty())
            {
                try
                {
                    RegistryKey rsg = null;
                    if (null == Registry.CurrentUser.OpenSubKey("SOFTWARE\\" + subKey))
                    {
                        Registry.CurrentUser.CreateSubKey("SOFTWARE\\" + subKey);
                    }

                    rsg = Registry.CurrentUser.OpenSubKey("SOFTWARE\\" + subKey, true);
                    if (null != rsg)
                    {
                        rsg.DeleteValue(key, false);
                        rsg.Close();
                    }
                }
                catch (Exception ex) { LogFactory.SystemLog(new System.Diagnostics.StackTrace().GetFrame(0).GetMethod().Name, LogLevelType.Exception, ex); }
            }
        }

        /// <summary>
        /// 写入注册表
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public static void AddRegistryKey(string key, string value, string subKey)
        {
            if (!subKey.IsEmpty() && !key.IsEmpty())
            {
                try
                {
                    RegistryKey rsg = null;
                    if (null == Registry.CurrentUser.OpenSubKey("SOFTWARE\\" + subKey))
                    {
                        Registry.CurrentUser.CreateSubKey("SOFTWARE\\" + subKey);
                    }

                    rsg = Registry.CurrentUser.OpenSubKey("SOFTWARE\\" + subKey, true);
                    if (null != rsg)
                    {
                        rsg.SetValue(key, value);
                        rsg.Close();
                    }
                }
                catch (Exception ex) { LogFactory.SystemLog(new System.Diagnostics.StackTrace().GetFrame(0).GetMethod().Name, LogLevelType.Exception, ex); }
            }
        }

        /// <summary>
        /// 读取注册表信息
        /// </summary>
        /// <param name="subKey"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string ReadRegistryKey(string key, string subKey)
        {
            string result = "";
            if (!subKey.IsEmpty() && !key.IsEmpty())
            {
                try
                {
                    RegistryKey rsg = null;
                    if (null != Registry.CurrentUser.OpenSubKey("SOFTWARE\\" + subKey))
                    {
                        rsg = Registry.CurrentUser.OpenSubKey("SOFTWARE\\" + subKey, true);

                        if (null != rsg && rsg.GetValue(key) != null)
                        {
                            result = rsg.GetValue(key).ToString();
                            rsg.Close();
                        }
                    }
                }
                catch (Exception ex) { LogFactory.SystemLog(new System.Diagnostics.StackTrace().GetFrame(0).GetMethod().Name, LogLevelType.Exception, ex); }
            }

            return result;
        }

        #endregion
              
    }
}
