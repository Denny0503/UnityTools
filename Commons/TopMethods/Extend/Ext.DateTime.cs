using System;

namespace TopMethods.Extend
{
    public static partial class Ext
    {
        /// <summary>
        /// 返回自 1970 年 1 经过的毫秒数-01-01T00:00:00.000Z。
        /// </summary>
        /// <returns></returns>
        public static string GetTimeStampMilliseconds()
        {
            return DateTimeOffset.Now.ToUnixTimeMilliseconds().ToString();
        }

        /// <summary>
        /// 根据毫秒获取时间
        /// </summary>
        /// <param name="millisecond"></param>
        /// <returns></returns>
        public static string FormatMillisecond(this long millisecond)
        {
            DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0);
            dateTime = dateTime.AddMilliseconds(millisecond);
            return dateTime.ToString("HH:mm:ss");
        }
    }
}
