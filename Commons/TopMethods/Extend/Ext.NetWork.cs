using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using TopMethods.Logs;

namespace TopMethods.Extend
{
    public static partial class Ext
    {
        /// <summary>
        /// 获取本机IP地址列表
        /// </summary>
        /// <returns>本机IP地址</returns>
        public static List<string> GetLocalIPList()
        {
            List<string> result = new List<string>();
            try
            {
                string HostName = Dns.GetHostName(); /*得到主机名*/
                IPHostEntry IpEntry = Dns.GetHostEntry(HostName);
                for (int i = 0; i < IpEntry.AddressList.Length; i++)
                {
                    /*从IP地址列表中筛选出IPv4类型的IP地址*/
                    if (IpEntry.AddressList[i].AddressFamily == AddressFamily.InterNetwork)
                    {
                        result.Add(IpEntry.AddressList[i].ToString());
                    }
                }
                return result;
            }
            catch (Exception ex) { LogFactory.SystemLog(new System.Diagnostics.StackTrace().GetFrame(0).GetMethod().Name, LogLevelType.Exception, ex); return result; }
        }

        /// <summary>
        /// 获取本地第一个ip地址
        /// </summary>
        /// <param name="isPing">是否可ping通</param>
        /// <returns></returns>
        public static string GetFirstLocalIP(bool isPing = false)
        {
            string result = "";
            try
            {
                List<string> localIPs = GetLocalIPList();

                if (isPing)
                {
                    foreach (string key in localIPs)
                    {
                        if (IsPingIP(key, 120))
                        {
                            result = key;
                            break;
                        }
                    }
                }

                if (result.IsEmpty())
                {
                    result = localIPs.Count > 0 ? localIPs[0] : "";
                }
            }
            catch (Exception ex) { LogFactory.SystemLog(new System.Diagnostics.StackTrace().GetFrame(0).GetMethod().Name, LogLevelType.Exception, ex); }
            return result;
        }

        /// <summary>
        /// 获取本地第一个ip地址，并指定远程ip，判断是否可以连通
        /// </summary>
        /// <returns></returns>
        public static string GetFirstLocalIP(string remoteIP)
        {
            string result = "";
            try
            {
                List<string> localIPs = GetLocalIPList();

                if (!remoteIP.IsEmpty() && localIPs.Count > 1)
                {
                    Process p = new Process();
                    p.StartInfo.FileName = "cmd.exe";
                    p.StartInfo.UseShellExecute = false; //关闭Shell的使用
                    p.StartInfo.RedirectStandardInput = true;//重定向标准输入
                    p.StartInfo.RedirectStandardOutput = true;//重定向标准输出
                    p.StartInfo.RedirectStandardError = true;//重定向错误输出
                    p.StartInfo.CreateNoWindow = true;//设置不显示窗口                    

                    foreach (string key in localIPs)
                    {
                        p.Start();
                        p.StandardInput.WriteLine($"ping -S {key} {remoteIP} ");
                        p.StandardInput.WriteLine("exit");
                        string strRst = p.StandardOutput.ReadToEnd();

                        if (strRst.IndexOf("(0% loss)") != -1 || strRst.IndexOf("(0% 丢失)") != -1)
                        {
                            result = key;
                            break;
                        }
                    }

                    p.StandardInput.WriteLine("exit");
                    p.Close();
                }

                if (result.IsEmpty())
                {
                    result = localIPs.Count == 1 ? localIPs[0] : "";
                }
            }
            catch (Exception ex) { LogFactory.SystemLog(new System.Diagnostics.StackTrace().GetFrame(0).GetMethod().Name, LogLevelType.Exception, ex); }
            return result;
        }

        /// <summary>
        /// 是否能ping通指定ip
        /// </summary>
        /// <param name="ip"></param>
        /// <returns></returns>
        private static bool IsPingIP(string ip, int timeout = 120)
        {
            Ping pingSender = new Ping();
            PingReply reply = pingSender.Send(ip, timeout);
            return reply.Status == IPStatus.Success;
        }

        /// <summary>
        /// 是否能ping通指定ip
        /// </summary>
        /// <param name="ip"></param>
        /// <returns></returns>
        private static bool IsPingIP(string remoteIP, string localIP, int timeout = 60)
        {
            bool pingResult = false;

            Process p = new Process();
            p.StartInfo.FileName = "cmd.exe";
            p.StartInfo.UseShellExecute = false; //关闭Shell的使用
            p.StartInfo.RedirectStandardInput = true;//重定向标准输入
            p.StartInfo.RedirectStandardOutput = true;//重定向标准输出
            p.StartInfo.RedirectStandardError = true;//重定向错误输出
            p.StartInfo.CreateNoWindow = true;//设置不显示窗口

            p.Start();
            p.StandardInput.WriteLine($"ping -S {localIP} {remoteIP} ");
            p.StandardInput.WriteLine("exit");
            string strRst = p.StandardOutput.ReadToEnd();
            pingResult = (strRst.IndexOf("(0% loss)") != -1 || strRst.IndexOf("(0% 丢失)") != -1);
            p.Close();

            return pingResult;
        }
    }
}
