﻿using System;
using System.Text.RegularExpressions;

namespace TopMethods.Extend
{
    public static partial class Ext
    {
        /// <summary>
        /// 使用正则表达式修正文件路径
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static string CorrectFilePath(this string fileName)
        {
            if (!fileName.IsEmpty())
            {
                Regex regex = new Regex(@" ^[^\/\:\*\?\""\<\>\|\,]+$");
                return regex.Replace(fileName, "_");
            }
            else
            {
                return "";
            }
        }

        /// <summary>   
        /// 匹配3位或4位区号的电话号码，其中区号可以用小括号括起来，   
        /// 也可以不用，区号与本地号间可以用连字号或空格间隔，   
        /// 也可以没有间隔   
        /// \(0\d{2}\)[- ]?\d{8}|0\d{2}[- ]?\d{8}|\(0\d{3}\)[- ]?\d{7}|0\d{3}[- ]?\d{7}   
        /// </summary>   
        /// <param name="input"></param>   
        /// <returns></returns>   
        public static bool IsPhone(string input)
        {
            string pattern = "^\\(0\\d{2}\\)[- ]?\\d{8}$|^0\\d{2}[- ]?\\d{8}$|^\\(0\\d{3}\\)[- ]?\\d{7}$|^0\\d{3}[- ]?\\d{7}$";
            Regex regex = new Regex(pattern);
            return regex.IsMatch(input);
        }

        /// <summary>   
        /// 判断输入的字符串是否是一个合法的手机号   
        /// </summary>   
        /// <param name="input"></param>   
        /// <returns></returns>   
        public static bool IsMobilePhone(string input)
        {
            Regex regex = new Regex("^13\\d{9}$");
            return regex.IsMatch(input);
        }

        /// <summary>   
        /// 判断输入的字符串只包含数字   
        /// 可以匹配整数和浮点数   
        /// ^-?\d+$|^(-?\d+)(\.\d+)?$   
        /// </summary>   
        /// <param name="input"></param>   
        /// <returns></returns>   
        public static bool IsNumber(string input)
        {
            string pattern = "^-?\\d+$|^(-?\\d+)(\\.\\d+)?$";
            Regex regex = new Regex(pattern);
            return regex.IsMatch(input);
        }

        /// <summary>   
        /// 匹配非负整数
        /// </summary>   
        /// <param name="input"></param>   
        /// <returns></returns>   
        public static bool IsNotNagtive(string input)
        {
            Regex regex = new Regex(@"^\d+$");
            return regex.IsMatch(input);
        }

        /// <summary>   
        /// 匹配正整数   
        /// </summary>   
        /// <param name="input"></param>   
        /// <returns></returns>   
        public static bool IsUint(string input)
        {
            Regex regex = new Regex("^[0-9]*[1-9][0-9]*$");
            return regex.IsMatch(input);
        }

        /// <summary>   
        /// 判断输入的字符串字包含英文字母   
        /// </summary>   
        /// <param name="input"></param>   
        /// <returns></returns>   
        public static bool IsEnglish(string input)
        {
            Regex regex = new Regex("^[A-Za-z]+$");
            return regex.IsMatch(input);
        }


        /// <summary>   
        /// 判断输入的字符串是否是一个合法的Email地址   
        /// </summary>   
        /// <param name="input"></param>   
        /// <returns></returns>   
        public static bool IsEmail(string input)
        {
            string pattern = @"^([\w-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$";
            Regex regex = new Regex(pattern);
            return regex.IsMatch(input);
        }


        /// <summary>   
        /// 判断输入的字符串是否只包含数字和英文字母   
        /// </summary>   
        /// <param name="input"></param>   
        /// <returns></returns>   
        public static bool IsNumAndEnCh(string input)
        {
            string pattern = @"^[A-Za-z0-9]+$";
            Regex regex = new Regex(pattern);
            return regex.IsMatch(input);
        }


        /// <summary>   
        /// 判断输入的字符串是否是一个超链接   
        /// </summary>   
        /// <param name="input"></param>   
        /// <returns></returns>   
        public static bool IsURL(string input)
        {
            //string pattern = @"http://([\w-]+\.)+[\w-]+(/[\w- ./?%&=]*)?";   
            string pattern = @"^[a-zA-Z]+://(\w+(-\w+)*)(\.(\w+(-\w+)*))*(\?\S*)?$";
            Regex regex = new Regex(pattern);
            return regex.IsMatch(input);
        }


        /// <summary>   
        /// 判断输入的字符串是否是表示一个IP地址   
        /// </summary>   
        /// <param name="input">被比较的字符串</param>   
        /// <returns>是IP地址则为True</returns>   
        public static bool IsIPv4(string input)
        {

            string[] IPs = input.Split('.');
            Regex regex = new Regex(@"^\d+$");
            for (int i = 0; i < IPs.Length; i++)
            {
                if (!regex.IsMatch(IPs[i]))
                {
                    return false;
                }
                if (Convert.ToUInt16(IPs[i]) > 255)
                {
                    return false;
                }
            }
            return true;
        }


        /// <summary>   
        /// 计算字符串的字符长度，一个汉字字符将被计算为两个字符   
        /// </summary>   
        /// <param name="input">需要计算的字符串</param>   
        /// <returns>返回字符串的长度</returns>   
        public static int GetCount(string input)
        {
            return Regex.Replace(input, @"[\一-\龥/g]", "aa").Length;
        }

        /// <summary>   
        /// 调用Regex中IsMatch函数实现一般的正则表达式匹配   
        /// </summary>   
        /// <param name="pattern">要匹配的正则表达式模式。</param>   
        /// <param name="input">要搜索匹配项的字符串</param>   
        /// <returns>如果正则表达式找到匹配项，则为 true；否则，为 false。</returns>   
        public static bool IsMatch(string pattern, string input)
        {
            Regex regex = new Regex(pattern);
            return regex.IsMatch(input);
        }

        /// <summary>   
        /// 从输入字符串中的第一个字符开始，用替换字符串替换指定的正则表达式模式的所有匹配项。   
        /// </summary>   
        /// <param name="pattern">模式字符串</param>   
        /// <param name="input">输入字符串</param>   
        /// <param name="replacement">用于替换的字符串</param>   
        /// <returns>返回被替换后的结果</returns>   
        public static string Replace(string pattern, string input, string replacement)
        {
            Regex regex = new Regex(pattern);
            return regex.Replace(input, replacement);
        }

        /// <summary>   
        /// 在由正则表达式模式定义的位置拆分输入字符串。   
        /// </summary>   
        /// <param name="pattern">模式字符串</param>   
        /// <param name="input">输入字符串</param>   
        /// <returns></returns>   
        public static string[] Split(string pattern, string input)
        {
            Regex regex = new Regex(pattern);
            return regex.Split(input);
        }

        /// <summary>   
        /// 判断输入的字符串是否是合法的IPV6 地址   
        /// </summary>   
        /// <param name="input"></param>   
        /// <returns></returns>   
        public static bool IsIPV6(string input)
        {
            string pattern = "";
            string temp = input;
            string[] strs = temp.Split(':');
            if (strs.Length > 8)
            {
                return false;
            }
            int count = GetStringCount(input, "::");
            if (count > 1)
            {
                return false;
            }
            else if (count == 0)
            {
                pattern = @"^([\da-f]{1,4}:){7}[\da-f]{1,4}$";

                Regex regex = new Regex(pattern);
                return regex.IsMatch(input);
            }
            else
            {
                pattern = @"^([\da-f]{1,4}:){0,5}::([\da-f]{1,4}:){0,5}[\da-f]{1,4}$";
                Regex regex1 = new Regex(pattern);
                return regex1.IsMatch(input);
            }

        }
        /* ******************************************************************* 
        * 1、通过“:”来分割字符串看得到的字符串数组长度是否小于等于8 
        * 2、判断输入的IPV6字符串中是否有“::”。 
        * 3、如果没有“::”采用 ^([\da-f]{1,4}:){7}[\da-f]{1,4}$ 来判断 
        * 4、如果有“::” ，判断"::"是否止出现一次 
        * 5、如果出现一次以上 返回false 
        * 6、^([\da-f]{1,4}:){0,5}::([\da-f]{1,4}:){0,5}[\da-f]{1,4}$ 
        * ******************************************************************/
        /// <summary>   
        /// 判断字符串compare 在 input字符串中出现的次数   
        /// </summary>   
        /// <param name="input">源字符串</param>   
        /// <param name="compare">用于比较的字符串</param>   
        /// <returns>字符串compare 在 input字符串中出现的次数</returns>   
        private static int GetStringCount(string input, string compare)
        {
            int index = input.IndexOf(compare);
            if (index != -1)
            {
                return 1 + GetStringCount(input.Substring(index + compare.Length), compare);
            }
            else
            {
                return 0;
            }

        }
       
       /// <summary>
        /// 截取字符串中开始和结束字符串中间的字符串
        /// </summary>
        /// <param name="source">源字符串</param>
        /// <param name="startStr">开始字符串</param>
        /// <param name="endStr">结束字符串</param>
        /// <returns>中间字符串</returns>
        public static string SubstringSingle(string source, string startStr, string endStr)
        {
            Regex rg = new Regex("(?<=(" + startStr + "))[.\\s\\S]*?(?=(" + endStr + "))", RegexOptions.Multiline | RegexOptions.Singleline);
            return rg.Match(source).Value;
        }
 
        /// <summary>
        /// （批量）截取字符串中开始和结束字符串中间的字符串
        /// </summary>
        /// <param name="source">源字符串</param>
        /// <param name="startStr">开始字符串</param>
        /// <param name="endStr">结束字符串</param>
        /// <returns>中间字符串</returns>
        public static List<string> SubstringMultiple(string source, string startStr, string endStr)
        {
            Regex rg = new Regex("(?<=(" + startStr + "))[.\\s\\S]*?(?=(" + endStr + "))", RegexOptions.Multiline | RegexOptions.Singleline);
            
            MatchCollection matches = rg.Matches(source);
 
            List<string> resList=new List<string>();
 
            foreach (Match item in matches)
                resList.Add(item.Value);
 
            return resList;
        }
    }
}
