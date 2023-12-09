using System;

namespace UnityMethods.Extend
{
    public static partial class Ext
    {
        #region 是否为空

        /// <summary>
        /// 是否为空
        /// </summary>
        /// <param name="value">值</param>
        public static bool IsEmpty(this string value)
        {
            return string.IsNullOrWhiteSpace(value);
        }

        /// <summary>
        /// 是否为空
        /// </summary>
        /// <param name="value">值</param>
        public static bool IsEmpty(this Guid? value)
        {
            if (value == null)
            {
                return true;
            }

            return IsEmpty(value.Value);
        }

        /// <summary>
        /// 是否为空
        /// </summary>
        /// <param name="value">值</param>
        public static bool IsEmpty(this Guid value)
        {
            if (value == Guid.Empty)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// 是否为空
        /// </summary>
        /// <param name="value">值</param>
        public static bool IsEmpty(this object value)
        {
            if (value != null && !string.IsNullOrEmpty(value.ToString()))
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        #endregion

        #region 判断字符串是否是数字

        /// <summary>
        /// 判断是否是数字
        /// </summary>
        /// <param name="message"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public static bool IsNumberic(this string message)
        {
            System.Text.RegularExpressions.Regex rex = new System.Text.RegularExpressions.Regex(@"^\d+$");
            if (rex.IsMatch(message))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 从字符串中提取数字
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public static string GetNumbericFromStr(this string message)
        {
            return System.Text.RegularExpressions.Regex.Replace(message, @"[^0-9]+", "");
        }

        #endregion

        #region 数值转换
        /// <summary>
        /// object 转换为整型
        /// </summary>
        /// <param name="data">数据</param>
        public static int ToInt(this object data)
        {
            int result = -1;
            if (data == null)
            {
                return result;
            }

            var success = int.TryParse(data.ToString(), out result);
            if (success)
            {
                return result;
            }
            else
            {
                try
                {
                    return Convert.ToInt32(ToDouble(data, 0));
                }
                catch (Exception)
                {
                    return result;
                }
            }
        }

        /// <summary>
        /// object 转换为整型
        /// </summary>
        /// <param name="data">数据</param>
        public static long ToLong(this object data)
        {
            long result = -1;
            if (data == null)
            {
                return result;
            }

            var success = long.TryParse(data.ToString(), out result);
            if (success)
            {
                return result;
            }
            else
            {
                try
                {
                    return Convert.ToInt64(ToDouble(data, 0));
                }
                catch (Exception)
                {
                    return result;
                }
            }
        }

        /// <summary>
        /// object 转换为可空整型
        /// </summary>
        /// <param name="data">数据</param>
        public static int? ToIntOrNull(this object data)
        {
            if (data == null)
            {
                return null;
            }

            bool isValid = int.TryParse(data.ToString(), out int result);
            if (isValid)
            {
                return result;
            }

            return null;
        }

        /// <summary>
        /// object 转换为单精度浮点数
        /// </summary>
        /// <param name="data">数据</param>
        public static float ToFloat(this object data)
        {
            if (data == null)
            {
                return 0;
            }

            return float.TryParse(data.ToString(), out float result) ? result : 0;
        }

        /// <summary>
        /// object 转换为双精度浮点数
        /// </summary>
        /// <param name="data">数据</param>
        public static double ToDouble(this object data)
        {
            if (data == null)
            {
                return 0;
            }

            return double.TryParse(data.ToString(), out double result) ? result : 0;
        }

        /// <summary>
        /// object 转换为双精度浮点数,并按指定的小数位4舍5入
        /// </summary>
        /// <param name="data">数据</param>
        /// <param name="digits">小数位数</param>
        public static double ToDouble(this object data, int digits)
        {
            return Math.Round(ToDouble(data), digits);
        }

        /// <summary>
        /// object 转换为可空双精度浮点数
        /// </summary>
        /// <param name="data">数据</param>
        public static double? ToDoubleOrNull(this object data)
        {
            if (data == null)
            {
                return null;
            }

            bool isValid = double.TryParse(data.ToString(), out double result);
            if (isValid)
            {
                return result;
            }

            return null;
        }

        /// <summary>
        /// object 转换为高精度浮点数
        /// </summary>
        /// <param name="data">数据</param>
        public static decimal ToDecimal(this object data)
        {
            if (data == null)
            {
                return 0;
            }

            return decimal.TryParse(data.ToString(), out decimal result) ? result : 0;
        }

        /// <summary>
        /// object 转换为高精度浮点数,并按指定的小数位4舍5入
        /// </summary>
        /// <param name="data">数据</param>
        /// <param name="digits">小数位数</param>
        public static decimal ToDecimal(this object data, int digits)
        {
            return Math.Round(ToDecimal(data), digits);
        }

        /// <summary>
        /// object 转换为可空高精度浮点数
        /// </summary>
        /// <param name="data">数据</param>
        public static decimal? ToDecimalOrNull(this object data)
        {
            if (data == null)
            {
                return null;
            }

            bool isValid = decimal.TryParse(data.ToString(), out decimal result);
            if (isValid)
            {
                return result;
            }

            return null;
        }

        /// <summary>
        /// object 转换为可空高精度浮点数,并按指定的小数位4舍5入
        /// </summary>
        /// <param name="data">数据</param>
        /// <param name="digits">小数位数</param>
        public static decimal? ToDecimalOrNull(this object data, int digits)
        {
            var result = ToDecimalOrNull(data);
            if (result == null)
            {
                return null;
            }

            return Math.Round(result.Value, digits);
        }

        #endregion

        public static bool StrEquals(this string data, string data2)
        {
            if (data.IsEmpty() || data2.IsEmpty())
            {
                return false;
            }

            return data.Equals(data2);
        }

    }
}
