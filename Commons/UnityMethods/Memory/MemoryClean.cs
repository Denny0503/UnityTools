//using System;
//using System.Runtime.InteropServices;
//using System.Timers;
//using UnityMethods.Logs;

//namespace UnityMethods.Memory
//{
//    public class MemoryClean : IDisposable
//    {
//        #region 内存回收

//        [DllImport("kernel32.dll", EntryPoint = "SetProcessWorkingSetSize")]
//        public static extern int SetProcessWorkingSetSize(IntPtr process, int minSize, int maxSize);
//        /// <summary>
//        /// 释放内存
//        /// </summary>
//        private void ClearMemory()
//        {
//            GC.Collect();
//            GC.WaitForPendingFinalizers();
//            if (Environment.OSVersion.Platform == PlatformID.Win32NT)
//            {
//                SetProcessWorkingSetSize(System.Diagnostics.Process.GetCurrentProcess().Handle, -1, -1);
//            }
//        }

//        #endregion

//        /// <summary>
//        /// 内存清理定时器
//        /// </summary>
//        private Timer MemoryCleanTimer;

//        /// <summary>
//        /// 默认每30秒回收内存
//        /// </summary>
//        public MemoryClean()
//        {
//            try
//            {
//                MemoryCleanTimer = new Timer();
//                MemoryCleanTimer.Interval = 30000;
//                MemoryCleanTimer.AutoReset = true;
//                MemoryCleanTimer.Enabled = false;
//                MemoryCleanTimer.Elapsed += MemoryCleanTimer_Elapsed;

//            }
//            catch (Exception ex) { LogFactory.SystemLog(new System.Diagnostics.StackTrace().GetFrame(0).GetMethod().Name, LogLevelType.Fatal, ex); }
//        }

//        public MemoryClean(int second)
//        {
//            try
//            {
//                MemoryCleanTimer = new Timer(second * 1000);
//                MemoryCleanTimer.Interval = second * 1000;
//                MemoryCleanTimer.AutoReset = true;
//                MemoryCleanTimer.Enabled = false;
//                MemoryCleanTimer.Elapsed += MemoryCleanTimer_Elapsed;

//            }
//            catch (Exception ex) { LogFactory.SystemLog(new System.Diagnostics.StackTrace().GetFrame(0).GetMethod().Name, LogLevelType.Fatal, ex); }
//        }

//        private void MemoryCleanTimer_Elapsed(object sender, ElapsedEventArgs e)
//        {
//            ClearMemory();
//        }

//        /// <summary>
//        /// 开启内存清理任务
//        /// </summary>
//        /// <param name="second"></param>
//        public void StartMemoryTask(int second = 30)
//        {
//            if (MemoryCleanTimer != null)
//            {
//                MemoryCleanTimer.Interval = second * 1000;
//                MemoryCleanTimer.Start();
//            }
//        }

//        #region IDisposable Support
//        private bool disposedValue = false; // 要检测冗余调用

//        protected virtual void Dispose(bool disposing)
//        {
//            if (!disposedValue)
//            {
//                if (disposing)
//                {
//                    // TODO: 释放托管状态(托管对象)。
//                    if (null != MemoryCleanTimer)
//                    {
//                        MemoryCleanTimer.Stop();
//                        MemoryCleanTimer.Dispose();
//                    }
//                }

//                // TODO: 释放未托管的资源(未托管的对象)并在以下内容中替代终结器。
//                // TODO: 将大型字段设置为 null。

//                disposedValue = true;
//            }
//        }

//        // TODO: 仅当以上 Dispose(bool disposing) 拥有用于释放未托管资源的代码时才替代终结器。
//        // ~MiniMemoryCleanCore() {
//        //   // 请勿更改此代码。将清理代码放入以上 Dispose(bool disposing) 中。
//        //   Dispose(false);
//        // }

//        // 添加此代码以正确实现可处置模式。
//        public void Dispose()
//        {
//            // 请勿更改此代码。将清理代码放入以上 Dispose(bool disposing) 中。
//            Dispose(true);
//            // TODO: 如果在以上内容中替代了终结器，则取消注释以下行。
//            // GC.SuppressFinalize(this);
//        }
//        #endregion
//    }
//}
