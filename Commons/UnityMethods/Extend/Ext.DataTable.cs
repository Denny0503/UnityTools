﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using UnityMethods.Excel;

namespace UnityMethods.Extend
{
    public static partial class Ext
    {
        /// <summary>
        /// DataTable转成List
        /// </summary>
        public static List<T> ToDataList<T>(this DataTable dt)
        {
            var list = new List<T>();
            var plist = new List<PropertyInfo>(typeof(T).GetProperties());

            if (dt == null || dt.Rows.Count == 0)
            {
                return null;
            }

            foreach (DataRow item in dt.Rows)
            {
                T s = Activator.CreateInstance<T>();
                for (int i = 0; i < dt.Columns.Count; i++)
                {
                    PropertyInfo info = plist.Find(p => p.Name == dt.Columns[i].ColumnName);
                    if (info != null)
                    {
                        try
                        {
                            if (!Convert.IsDBNull(item[i]))
                            {
                                object v = null;
                                if (info.PropertyType.ToString().Contains("System.Nullable"))
                                {
                                    v = Convert.ChangeType(item[i], Nullable.GetUnderlyingType(info.PropertyType));
                                }
                                else
                                {
                                    v = Convert.ChangeType(item[i], info.PropertyType);
                                }
                                info.SetValue(s, v, null);
                            }
                        }
                        catch (Exception ex)
                        {
                            throw new Exception($"字段[{info.Name}]转换出错！错误信息：{ex.Message}");
                        }
                    }
                }
                list.Add(s);
            }
            return list;
        }

        /// <summary>
        /// DataTable转成实体对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static T ToDataEntity<T>(this DataTable dt)
        {
            T s = Activator.CreateInstance<T>();
            if (dt == null || dt.Rows.Count == 0)
            {
                return default(T);
            }
            var plist = new List<PropertyInfo>(typeof(T).GetProperties());
            for (int i = 0; i < dt.Columns.Count; i++)
            {
                PropertyInfo info = plist.Find(p => p.Name == dt.Columns[i].ColumnName);
                if (info != null)
                {
                    try
                    {
                        if (!Convert.IsDBNull(dt.Rows[0][i]))
                        {
                            object v = null;
                            if (info.PropertyType.ToString().Contains("System.Nullable"))
                            {
                                v = Convert.ChangeType(dt.Rows[0][i], Nullable.GetUnderlyingType(info.PropertyType));
                            }
                            else
                            {
                                v = Convert.ChangeType(dt.Rows[0][i], info.PropertyType);
                            }
                            info.SetValue(s, v, null);
                        }
                    }
                    catch (Exception ex)
                    {
                        throw new Exception($"字段[{info.Name}]转换出错！错误信息：{ex.Message}");
                    }
                }
            }
            return s;
        }

        /// <summary>
        /// List转成DataTable
        /// </summary>
        /// <typeparam name="T">实体类型</typeparam>
        /// <param name="entities">实体集合</param>
        public static DataTable ToDataTable<T>(this List<T> entities)
        {
            if (entities == null || entities.Count == 0)
            {
                return null;
            }
            var result = CreateTable<T>();

            FillData(result, entities);

            return result;
        }

        /// <summary>
        /// 属性列表
        /// </summary>
        private static List<PropertyInfo> PropertyInfos = new List<PropertyInfo>();

        /// <summary>
        /// 创建表
        /// </summary>
        private static DataTable CreateTable<T>()
        {
            var result = new DataTable();
            var type = typeof(T);

            PropertyInfos.Clear();
            PropertyInfos.AddRange(type.GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance).Where(pi => Attribute.IsDefined(pi, typeof(ExcelColumnAttribute))).ToList());

            foreach (var property in PropertyInfos)
            {
                var propertyType = property.PropertyType;
                if ((propertyType.IsGenericType) && (propertyType.GetGenericTypeDefinition() == typeof(Nullable<>)))
                {
                    propertyType = propertyType.GetGenericArguments()[0];
                }

                var attribute = property.GetCustomAttributes(typeof(ExcelColumnAttribute), true).FirstOrDefault();
                if (attribute == null)
                {
                    result.Columns.Add(property.Name, propertyType);
                }
                else
                {
                    result.Columns.Add((attribute as ExcelColumnAttribute).ExcelColumn, propertyType);
                }
            }
            return result;
        }

        /// <summary>
        /// 填充数据
        /// </summary>
        private static void FillData<T>(DataTable dt, IEnumerable<T> entities)
        {
            foreach (var entity in entities)
            {
                dt.Rows.Add(CreateRow(dt, entity));
            }
        }

        /// <summary>
        /// 创建行
        /// </summary>
        private static DataRow CreateRow<T>(DataTable dt, T entity)
        {
            DataRow row = dt.NewRow();
            var type = typeof(T);

            if (PropertyInfos.Count == 0)
            {
                PropertyInfos.AddRange(type.GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance).Where(pi => Attribute.IsDefined(pi, typeof(ExcelColumnAttribute))).ToList());
            }

            foreach (var property in PropertyInfos)
            {
                var attribute = property.GetCustomAttributes(typeof(ExcelColumnAttribute), true).FirstOrDefault();

                if (attribute == null)
                {
                    row[property.Name] = property.GetValue(entity) ?? DBNull.Value;
                }
                else
                {
                    row[(attribute as ExcelColumnAttribute).ExcelColumn] = property.GetValue(entity) ?? DBNull.Value;
                }

            }
            return row;
        }
    }
}
