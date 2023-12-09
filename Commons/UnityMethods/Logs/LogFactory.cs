using log4net;
using System;
using System.IO;

namespace UnityMethods.Logs
{
    /// <summary>
    /// 日志等级
    /// </summary>
    public enum LogLevelType
    {
        /// <summary>
        /// 调试
        /// </summary>
        Debug,
        /// <summary>
        /// 致命的
        /// </summary>
        Fatal,
        /// <summary>
        /// 提示
        /// </summary>
        Info,
        /// <summary>
        /// 警告
        /// </summary>
        Warn,
        /// <summary>
        /// 错误
        /// </summary>
        Error,
        /// <summary>
        /// 异常
        /// </summary>
        Exception,
    }

    public static class LogFactory
    {
        static LogFactory()
        {
            FileInfo configFile = new FileInfo(System.IO.Directory.GetCurrentDirectory() + "/Configs/log4net.config");
            log4net.Config.XmlConfigurator.Configure(configFile);
        }

        /// <summary>
        /// 系统运行信息日志
        /// </summary>
        private static readonly ILog SystemLogs = LogManager.GetLogger("ErrorAppender");

        /// <summary>
        /// 记录系统日志
        /// </summary>
        /// <param name="message">日志内容</param>
        /// <param name="category">日志等级</param>
        public static void SystemLog(string message, LogLevelType category, Exception ex = null)
        {
            if (string.IsNullOrEmpty(message))
            {
                return;
            }

            switch (category)
            {
#if DEBUG
                case LogLevelType.Debug:
                    SystemLogs.Debug(message, ex);
                    break;
#endif
                case LogLevelType.Exception:
                case LogLevelType.Fatal:
                    SystemLogs.Fatal(message, ex);
                    break;
                case LogLevelType.Info:
                    SystemLogs.Info(message, ex);
                    break;
                case LogLevelType.Warn:
                    SystemLogs.Warn(message, ex);
                    break;
                case LogLevelType.Error:
                    SystemLogs.Error(message, ex);
                    break;
            }
        }
    }
}
