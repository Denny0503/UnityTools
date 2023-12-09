using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection;

namespace UnityMethods.Extend
{
    public static partial class Ext
    {
        /// <summary>
        /// 通过反射获取某一个类的属性字段
        /// </summary>
        /// <param name="info"></param>
        /// <param name="field"></param>
        /// <returns></returns>
        public static object GetPropertyValue(object info, string field)
        {
            if (info == null)
            {
                return null;
            }

            Type t = info.GetType();
            IEnumerable<System.Reflection.PropertyInfo> property = from pi in t.GetProperties() where pi.Name.ToLower() == field.ToLower() select pi;
            return property.First().GetValue(info, null);
        }

        public static Dictionary<string, string> CreateDictionary<T>()
        {
            var result = new Dictionary<string, string>();

            Type t = typeof(T);
            Array arrays = Enum.GetValues(t);
            for (int i = 0; i < arrays.LongLength; i++)
            {
                T test = (T)arrays.GetValue(i);

                FieldInfo fieldInfo = test.GetType().GetField(test.ToString());

                object[] attribArray = fieldInfo.GetCustomAttributes(false);

                if (attribArray.Length > 0 && !attribArray.ToList().Exists(ch => ch.GetType() == typeof(NotMappedAttribute)))
                {
                    for (int j = 0; j < attribArray.LongLength; j++)
                    {
                        if (attribArray[j].GetType() == typeof(DescriptionAttribute))
                        {
                            DescriptionAttribute attrib = (DescriptionAttribute)attribArray[j];

                            result.Add(fieldInfo.Name, attrib.Description);
                            break;
                        }
                    }
                }
            }

            return result;
        }

        public static string GetDescriptionAttribute<T>(T test)
        {
            FieldInfo fieldInfo = test.GetType().GetField(test.ToString());

            if (null != fieldInfo)
            {
                object[] attribArray = fieldInfo.GetCustomAttributes(false);

                return ((DescriptionAttribute)attribArray[0]).Description;
            }
            else
            {
                return "";
            }
        }
    }
}
