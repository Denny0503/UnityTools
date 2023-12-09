using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using UnityMethods.Logs;

namespace UnityMethods.Extend
{
    public static partial class Ext
    {
        /// <summary>
        /// 全局设置json
        /// </summary>
        public static void JsonDefaultSettings()
        {
            var timeConverter = new IsoDateTimeConverter { DateTimeFormat = "yyyy-MM-dd HH:mm:ss" };
            JsonSerializerSettings settings = new JsonSerializerSettings()
            {
                /*日期类型默认格式化处理*/
                DateFormatHandling = DateFormatHandling.IsoDateFormat,
                DateFormatString = "yyyy-MM-dd HH:mm:ss",
                DateTimeZoneHandling = DateTimeZoneHandling.Utc,

                /*控制成员缺失的方式*/
                MissingMemberHandling = MissingMemberHandling.Ignore,

                /*控制在序列化期间如何处理.NET对象上的空值以及在反序列化期间如何处理JSON中的空值。*/
                NullValueHandling = NullValueHandling.Ignore,

                ///*控制在反序列化期间如何创建和反序列化对象。*/
                //ObjectCreationHandling = ObjectCreationHandling.Auto,

                ///*获取或设置在序列化和反序列化期间如何处理默认值。
                // * 默认值为newtonsoft.json.defaultvaluehandling.include。*/
                //DefaultValueHandling = DefaultValueHandling.Include,
            };

            JsonConvert.DefaultSettings = new Func<JsonSerializerSettings>(() =>
            {
                settings.Converters.Add(timeConverter);
                return settings;
            });
        }

        /// <summary>
        /// 对象转为json字符串
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string GetJson(this object obj)
        {
            var timeConverter = new IsoDateTimeConverter { DateTimeFormat = "yyyy-MM-dd HH:mm:ss" };
            return JsonConvert.SerializeObject(obj, timeConverter);
        }

        /// <summary>
        /// json反序列化为object
        /// </summary>
        /// <param name="Json"></param>
        /// <returns></returns>
        public static object JsonToObject(this string Json)
        {
            return Json.IsEmpty() ? null : JsonConvert.DeserializeObject(Json);
        }

        /// <summary>
        /// json字符串反序列化为对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Json"></param>
        /// <returns></returns>
        public static T JsonToObject<T>(this string Json)
        {
            try
            {
                return Json.IsEmpty() ? default(T) : JsonConvert.DeserializeObject<T>(Json);
            }
            catch (Exception ex)
            {
                LogFactory.SystemLog(new System.Diagnostics.StackTrace().GetFrame(0).GetMethod().Name, LogLevelType.Exception, ex);
                return default(T);
            }
        }
        /// <summary>
        /// json字符串反序列化为list
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Json"></param>
        /// <returns></returns>
        public static List<T> JsonToList<T>(this string Json)
        {
            return Json.IsEmpty() ? default(List<T>) : JsonConvert.DeserializeObject<List<T>>(Json);
        }

        /// <summary>
        /// 反序列化为JObject
        /// </summary>
        /// <param name="Json"></param>
        /// <returns></returns>
        public static JObject JsonToJObject(this string Json)
        {
            return Json.IsEmpty() ? JObject.Parse("{}") : JObject.Parse(Json.Replace("&nbsp;", ""));
        }

        /// <summary>
        /// 根据key获取数值
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        public static string GetValueFromKey(string json, string key)
        {
            string result = "";

            try
            {
                if (!json.IsEmpty())
                {
                    JObject jObject = JObject.Parse(json);
                    if (jObject.ContainsKey(key))
                    {
                        result = jObject[key].ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                LogFactory.SystemLog(new System.Diagnostics.StackTrace().GetFrame(0).GetMethod().Name, LogLevelType.Exception, ex);
            }

            return result;
        }

        /// <summary>
        /// 根据key获取数值
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        public static string GetValueFromKey(string json, string rootKey, string valueKey)
        {
            string result = "";
            try
            {
                if (!json.IsEmpty())
                {
                    JObject rootjObject = JObject.Parse(json);
                    if (rootjObject.ContainsKey(rootKey))
                    {
                        JObject valueObj = (JObject)rootjObject[rootKey];
                        result = valueObj.ContainsKey(valueKey) ? valueObj.GetValue(valueKey).ToString() : "";
                    }
                }
            }
            catch (Exception ex)
            {
                LogFactory.SystemLog(new System.Diagnostics.StackTrace().GetFrame(0).GetMethod().Name, LogLevelType.Exception, ex);
            }

            return result;
        }
    }
}
