using System;
using System.Collections.Generic;
using System.Linq;

namespace TopMethods.Dapper
{
    /// <summary>
    /// Dapper扩展方法
    /// </summary>
    public sealed class DapperExtensions
    {
        #region 获取sql语句

        private const string INSERT_TABLE_ITEM_VALUE = "INSERT OR REPLACE INTO T_{0} ({1}) VALUES ({2})";

        /// <summary>
        /// 获取参数形式的sql语句
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="model">对象</param>
        /// <param name="exceptParams">排除的字段名</param>
        /// <returns></returns>
        public static string GetInsertParamSql<T>(T model, params string[] exceptParams)
        {
            Type t = typeof(T);

            try
            {

                List<string> exceptList = exceptParams?.ToList() ?? new List<string>();

                var propertyInfo = t.GetProperties().Where(pi => !Attribute.IsDefined(pi, typeof(DapperIgnore))).ToArray();

                var proDic = propertyInfo.Where(s => !exceptList.Contains(s.Name))
                     .Select(s => new
                     {
                         key = s.Name,
                         value = $"@{s.Name}"
                     })
                     .ToDictionary(s => s.key, s => s.value);

                return string.Format(INSERT_TABLE_ITEM_VALUE, t.Name, string.Join(",", proDic.Keys), string.Join(",", proDic.Values));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion
    }
}
