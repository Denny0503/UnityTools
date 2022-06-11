//using Dapper;
//using System;
//using System.Collections.Generic;
//using System.ComponentModel.DataAnnotations.Schema;
//using System.Data;
//using System.Data.SQLite;
//using System.IO;
//using System.Linq;
//using TopMethods.Dapper;
//using TopMethods.Extend;

//namespace TopMethods.Data
//{
//    public sealed class SqliteDataHelper : ISqlDataBase, IDisposable
//    {
//        #region 数据库操作常量字段

//        /// <summary>
//        /// 清空数据表
//        /// </summary>
//        private const string CLEAN_UP_THE_DATA_SHEET = "DELETE FROM T_{0} ";

//        private const string UPDATE_TABLE_EDITITEM = "UPDATE T_{0} SET {1}";



//        /// <summary>
//        /// 查询表单所有数据
//        /// </summary>
//        private const string Query_ITEM_TABLE_ALL = "SELECT {0} FROM T_{1} ";
//        /// <summary>
//        /// 指定顺序查找数据，分页查找
//        /// </summary>
//        private const string QUERY_ALL_ORDER_BY_LIMIT = "SELECT {0} FROM T_{1}  WHERE {2} ORDER BY {3} {4} LIMIT {5} ";
//        /// <summary>
//        /// 分页查找数据
//        /// </summary>
//        private const string QUERY_ITEM_TABLE_WHERE_BY_PAGE = "SELECT {0} FROM T_{1} WHERE {2} LIMIT {3},{4} ";
//        /// <summary>
//        /// 查询不重复数据
//        /// </summary>
//        private const string QUERY_ITEM_SINGLE_GROUP = "SELECT {0} FROM T_{1} WHERE {2} GROUP BY {3} LIMIT {4} ";
//        /// <summary>
//        /// 查询不重复数据，指定排序规则
//        /// </summary>
//        private const string QUERY_ITEM_SINGLE_GROUP_ORDER = "SELECT {0} FROM T_{1} WHERE {2} GROUP BY {3} ORDER BY {4}  {5} LIMIT {6} ";



//        /// <summary>
//        /// 根据某字段查询个数
//        /// </summary>
//        private const string QUERY_ITEM_COUNT = "SELECT COUNT(*) FROM T_{0} WHERE {1}";

//        #endregion

//        /// <summary>
//        /// 本地缓存数据库
//        /// </summary>
//        public string SqliteDataBaseSrc { private set; get; }

//        /// <summary>
//        /// 数据库表文件
//        /// </summary>
//        public string DataBaseSql { private set; get; }

//        /// <summary>
//        /// 数据库连接，内存数据库连接
//        /// </summary>
//        private IDbConnection SQLiteConnect_Memory;

//        public SqliteDataHelper() { }

//        /// <summary>
//        /// 
//        /// </summary>
//        /// <param name="dataUrl">数据库文件路径</param>
//        /// <param name="sqlUrl">数据库脚本路径</param>
//        public SqliteDataHelper(string dataUrl, string sqlUrl)
//        {
//            SqliteDataBaseSrc = dataUrl;

//            DataBaseSql = sqlUrl;

//            InitDataBase(dataUrl);
//        }

//        /// <summary>
//        /// 释放数据库连接资源
//        /// </summary>
//        public void Dispose()
//        {
//            SQLiteConnect_Memory.Close();

//            SQLiteConnect_Memory.Dispose();
//        }

//        #region 创建本地数据库

//        /// <summary>
//        /// 初始化数据库
//        /// </summary>
//        public void InitDataBase(string dataSource)
//        {
//            OpenDataConnection(dataSource);
//        }

//        /// <summary>
//        /// 打开数据库链接
//        /// </summary>
//        /// <returns></returns>
//        private void OpenDataConnection(string dataSource)
//        {
//            SQLiteConnect_Memory = SimpleDbConnection(dataSource);

//            if (SQLiteConnect_Memory.State == ConnectionState.Closed)
//            {
//                SQLiteConnect_Memory.Open();
//            }
//        }

//        /// <summary>
//        /// 创建数据库连接
//        /// </summary>
//        /// <param name="dataSource"></param>
//        /// <returns></returns>
//        private SQLiteConnection SimpleDbConnection(string dataSource)
//        {
//            if (SqliteDataBaseSrc.IsEmpty()) { throw new ArgumentNullException(); }

//            //数据库加密密码： Password=Password$Password&666
//            /* 可以通过定义SQLITE_THREADSAFE宏来指定线程模式。如果没有指定，默认为串行模式。定义宏SQLITE_THREADSAFE=1指定使用串行模式;=0使用单线程模式；＝2使用多线程模式。*/
//            //string connString = string.Format("Data Source={0};temp_store=2;auto_vacuum=0;case_sensitive_like = 0;count_changes = 1;synchronous = 0; 
//            //Version=3;SQLITE_THREADSAFE=2;cache_size=8000", SqliteDataBaseSrc);

//            Directory.CreateDirectory(Path.GetDirectoryName(SqliteDataBaseSrc));

//            SQLiteConnectionStringBuilder scBuilder = new SQLiteConnectionStringBuilder
//            {
//                DataSource = dataSource,
//                CacheSize = 4194304,
//                Version = 3,
//                JournalMode = SQLiteJournalModeEnum.Memory,
//                SyncMode = SynchronizationModes.Full,
//                Pooling = true,
//                PageSize = 50,
//            };

//            return new SQLiteConnection(scBuilder.ToString());
//        }

//        /// <summary>
//        /// 通过sql数据库文件创建数据库表
//        /// </summary>
//        /// <param name="sqlFilePath">数据库定义文件路径</param>
//        public void InitTableBySqlFile(string sqlFilePath)
//        {
//            if (!sqlFilePath.IsEmpty() && File.Exists(sqlFilePath))
//            {
//                FileStream fs = null;
//                StreamReader rs = null;
//                try
//                {
//                    fs = new FileStream(sqlFilePath, FileMode.Open, FileAccess.Read);
//                    rs = new StreamReader(fs);
//                    SQLiteConnect_Memory.Execute(rs.ReadToEnd());
//                }
//                catch (Exception ex) { throw ex; }
//                finally
//                {
//                    if (rs != null) { rs.Close(); }
//                    if (fs != null) { fs.Close(); }
//                }
//            }
//        }

//        /// <summary>
//        /// 执行sql语句
//        /// </summary>
//        /// <param name="sql">sql语句</param>
//        public void ExecuteSQL(string sql)
//        {
//            if (!sql.IsEmpty())
//            {
//                try
//                {
//                    SQLiteConnect_Memory.Execute(sql);
//                }
//                catch (Exception ex) { throw ex; }
//            }
//        }

//        #endregion

//        /// <summary>
//        /// 查询数据记录数目
//        /// </summary>
//        /// <typeparam name="T">数据表名</typeparam>
//        /// <param name="where">查询条件</param>
//        /// <returns></returns>
//        public int GetItemCount<T>(string where)
//        {
//            try
//            {
//                var type = typeof(T);
//                string sqlStr = string.Format(QUERY_ITEM_COUNT, type.Name, where);
//                return SQLiteConnect_Memory.ExecuteScalar(sqlStr).ToInt();
//            }
//            catch (Exception ex) { throw ex; }
//        }

//        #region 插入和读取键值对数据

//        /// <summary>
//        /// 以键值对的形式插入数据
//        /// </summary>
//        /// <typeparam name="T"></typeparam>
//        /// <param name="model"></param>
//        /// <param name="exceptKey">忽略的字段名，多个字段以"|"分隔</param>
//        /// <returns></returns>
//        public int InsertToKeyValue<T>(T model, params string[] exceptParams)
//        {
//            Type t = typeof(T);

//            List<string> exceptList = exceptParams?.ToList() ?? new List<string>();

//            var propertyInfo = t.GetProperties().Where(pi => !Attribute.IsDefined(pi, typeof(NotMappedAttribute))).ToArray();

//            var proDic = propertyInfo.Where(s => !exceptList.Contains(s.Name))
//                .Select(s => new KeyValueDataPair
//                {
//                    Key = s.Name,
//                    Value = null == s.GetValue(model) ? "" : s.GetValue(model).ToString()
//                })
//                .ToList();

//            return Adds(proDic);
//        }

//        /// <summary>
//        /// 获取键值对的对象
//        /// </summary>
//        /// <typeparam name="T"></typeparam>
//        /// <param name="loginUser"></param>
//        /// <returns></returns>
//        public T GetKeyValueToObject<T>() where T : class
//        {
//            var result = Activator.CreateInstance(typeof(T));

//            List<KeyValueDataPair> queryRes = QueryMultiAll<KeyValueDataPair>();

//            try
//            {
//                if (null != queryRes && queryRes.Count > 0)
//                {
//                    queryRes.ForEach(child =>
//                    {
//                        if (!child.Key.IsEmpty() && !child.Value.IsEmpty())
//                        {
//                            var property = result.GetType().GetProperty(child.Key);
//                            if (null != property)
//                            {
//                                Type type = property.PropertyType;
//                                if (type == typeof(string))
//                                {
//                                    property.SetValue(result, child.Value, null);
//                                }
//                                else if (type == typeof(int))
//                                {
//                                    if (!int.TryParse(child.Value, out int v))
//                                    {
//                                        v = 0;
//                                    }
//                                    property.SetValue(result, v, null);
//                                }
//                                else if (type.BaseType == typeof(Enum))
//                                {
//                                    if (!int.TryParse(child.Value, out int v))
//                                    {
//                                        v = 0;
//                                    }
//                                    property.SetValue(result, v, null);
//                                }
//                                else if (type == typeof(bool))
//                                {
//                                    if (!bool.TryParse(child.Value, out bool v))
//                                    {
//                                        v = false;
//                                    }
//                                    property.SetValue(result, v, null);
//                                }
//                                else if (type == typeof(DateTime))
//                                {
//                                    if (!DateTime.TryParse(child.Value, out DateTime v))
//                                    {
//                                        v = DateTime.Now;
//                                    }
//                                    property.SetValue(result, v, null);
//                                }
//                                else if (type == typeof(Guid))
//                                {
//                                    if (!Guid.TryParse(child.Value, out Guid v))
//                                    {
//                                        v = Guid.Empty;
//                                    }
//                                    property.SetValue(result, v, null);
//                                }
//                            }
//                        }
//                    });
//                }
//            }
//            catch (Exception ex) { throw ex; }

//            return result as T;
//        }

//        #endregion

//        #region 添加数据

//        public int Add<T>(T model, params string[] exceptParams)
//        {
//            try
//            {
//                var insertSql = DapperExtensions.GetInsertParamSql<T>(model, exceptParams);

//                return SQLiteConnect_Memory.Execute(insertSql, model);
//            }
//            catch (Exception ex) { throw ex; }
//        }

//        /// <summary>
//        /// 添加保存数据，参数可选忽略字段
//        /// </summary>
//        /// <typeparam name="T"></typeparam>
//        /// <param name="models"></param>
//        /// <param name="exceptParams"></param>
//        /// <returns></returns>
//        public int Adds<T>(List<T> models, params string[] exceptParams)
//        {
//            int resultN = 0;
//            string insertSql = "";
//            //var transaction = SQLiteConnect.BeginTransaction();
//            try
//            {
//                models.ForEach(d =>
//                {
//                    insertSql = DapperExtensions.GetInsertParamSql<T>(d, exceptParams);
//                    resultN += SQLiteConnect_Memory.Execute(insertSql, d);
//                });
//                //transaction.Commit();
//            }
//            catch (Exception ex) { throw ex; }

//            return resultN;
//        }

//        #endregion

//        #region 删除

//        /// <summary>
//        /// 删除数据
//        /// </summary>
//        private const string DELETE_DATA_FROM_TABLE = "DELETE FROM T_{0} WHERE {1}";

//        /// <summary>
//        /// 删除数据
//        /// </summary>
//        /// <typeparam name="T"></typeparam>
//        /// <param name="where">形式：data=@data</param>
//        /// <returns></returns>
//        public int Delete<T>(T model, string where)
//        {
//            try
//            {
//                var type = typeof(T);

//                string sqlStr = string.Format(DELETE_DATA_FROM_TABLE, type.Name, where);

//                return SQLiteConnect_Memory.Execute(sqlStr, model);
//            }
//            catch (Exception ex) { throw ex; }
//        }

//        /// <summary>
//        /// 删除数据
//        /// </summary>
//        /// <typeparam name="T"></typeparam>
//        /// <returns></returns>
//        public int Delete<T>(string where)
//        {
//            try
//            {
//                var type = typeof(T);

//                string sqlStr = string.Format(DELETE_DATA_FROM_TABLE, type.Name, where);

//                return SQLiteConnect_Memory.Execute(sqlStr);
//            }
//            catch (Exception ex) { throw ex; }
//        }

//        #endregion

//        #region 修改、更新

//        private const string UPDATE_TABLE_EDITITEM_WHERE = "UPDATE T_{0} SET {1} WHERE {2} ";

//        /// <summary>
//        /// 修改更新某一项数据
//        /// </summary>
//        /// <typeparam name="T"></typeparam>
//        /// <param name="where"></param>
//        /// <param name="setValue">形式：data=@data</param>
//        /// <returns></returns>
//        public int UpdateItem<T>(T model, string where, string setValue)
//        {
//            try
//            {
//                Type t = typeof(T);
//                var sqlStr = string.Format(UPDATE_TABLE_EDITITEM_WHERE, t.Name, setValue, where);

//                return SQLiteConnect_Memory.Execute(sqlStr, model);
//            }
//            catch (Exception ex) { throw ex; }
//        }

//        /// <summary>
//        /// 更新所有字段
//        /// </summary>
//        /// <typeparam name="T"></typeparam>
//        /// <param name="model"></param>
//        /// <param name="where"></param>
//        /// <param name="exceptParams">排除更新的字段名</param>
//        /// <returns></returns>
//        public int UpdateAllItem<T>(T model, string where, params string[] exceptParams)
//        {
//            try
//            {
//                Type t = typeof(T);

//                List<string> exceptList = exceptParams?.ToList() ?? new List<string>();

//                var propertyInfo = t.GetProperties().Where(pi => !Attribute.IsDefined(pi, typeof(DapperIgnore))).ToArray();

//                var proList = propertyInfo.Where(s => !exceptList.Contains(s.Name)).Select(s => $"{s.Name}=@{s.Name}").ToList();

//                var sqlStr = string.Format(UPDATE_TABLE_EDITITEM_WHERE, t.Name, string.Join(",", proList), where);

//                return SQLiteConnect_Memory.Execute(sqlStr, model);
//            }
//            catch (Exception ex) { throw ex; }
//        }

//        #endregion

//        #region 查询

//        /// <summary>
//        /// 根据指定排序获取数据
//        /// </summary>
//        private const string QUERY_ITEM_TABLE_ORDER_BY_LIMIT = "SELECT {0} FROM T_{1} WHERE {2} ORDER BY {3}  {4}  LIMIT {5}";

//        /// <summary>
//        /// 指定顺序分页查找数据
//        /// </summary>
//        /// <typeparam name="T"></typeparam>
//        /// <param name="where"></param>
//        /// <param name="orderItem"></param>
//        /// <param name="sort_Enum"></param>
//        /// <param name="limit"></param>
//        /// <param name="attrs"></param>
//        /// <returns></returns>
//        public T QueryOrderByWhere<T>(string where, string orderItem, QuerySortType sort_Enum, string limit, params string[] attrs)
//        {
//            try
//            {
//                var type = typeof(T);

//                string item = string.Join(",", attrs?.ToList() ?? new List<string>());

//                string sqlStr = string.Format(QUERY_ITEM_TABLE_ORDER_BY_LIMIT, item, type.Name, where, orderItem, sort_Enum.ToString(), limit);

//                return SQLiteConnect_Memory.Query<T>(sqlStr).FirstOrDefault();
//            }
//            catch (Exception ex) { throw ex; }
//        }

//        /// <summary>
//        /// 根据指定排序获取数据
//        /// </summary>
//        private const string QUERY_ITEM_TABLE_ORDER_BY = "SELECT {0} FROM T_{1} WHERE {2} ORDER BY {3}  {4}  ";

//        /// <summary>
//        /// 指定顺序查找所有数据
//        /// </summary>
//        /// <returns></returns>
//        public T QueryOrderByWhere<T>(string where, string orderItem, QuerySortType sort_Enum, params string[] attrs)
//        {
//            try
//            {
//                var type = typeof(T);

//                string item = string.Join(",", attrs?.ToList() ?? new List<string>());

//                string sqlStr = string.Format(QUERY_ITEM_TABLE_ORDER_BY, item, type.Name, where, orderItem, sort_Enum.ToString());

//                return SQLiteConnect_Memory.Query<T>(sqlStr).FirstOrDefault();
//            }
//            catch (Exception ex) { throw ex; }
//        }

//        private const string Query_ITEM_TABLE_WHERE = "SELECT {0} FROM T_{1} WHERE {2} ";

//        /// <summary>
//        /// 按条件查询单条数据
//        /// </summary>
//        /// <typeparam name="T"></typeparam>
//        /// <param name="where"></param>
//        /// <param name="attrs"></param>
//        /// <returns></returns>
//        public T QueryByWhere<T>(string where, params string[] attrs)
//        {
//            try
//            {
//                Type type = typeof(T);

//                string item = string.Join(",", attrs?.ToList() ?? new List<string>());

//                var sqlStr = string.Format(Query_ITEM_TABLE_WHERE, item, type.Name, where);

//                return SQLiteConnect_Memory.Query<T>(sqlStr).FirstOrDefault();
//            }
//            catch (Exception ex) { throw ex; }
//        }

//        /// <summary>
//        /// 查询不重复数据
//        /// </summary>
//        /// <returns></returns>
//        public List<T> QuerySingle<T>(string where, string groupItem, string limit, params string[] attrs)
//        {
//            try
//            {
//                Type type = typeof(T);

//                string item = string.Join(",", attrs?.ToList() ?? new List<string>());

//                var sqlStr = string.Format(QUERY_ITEM_SINGLE_GROUP, item, type.Name, where, groupItem, limit);

//                return SQLiteConnect_Memory.Query<T>(sqlStr).ToList();
//            }
//            catch (Exception ex) { throw ex; }
//        }

//        /// <summary>
//        /// 查询不重复数据，进行排序
//        /// </summary>
//        /// <typeparam name="T"></typeparam>
//        /// <param name="where"></param>
//        /// <param name="groupItem">组合数据字段</param>
//        /// <param name="orderItem">指定排序字段</param>
//        /// <param name="sort_Enum">排序类型</param>
//        /// <param name="limit">分页</param>
//        /// <param name="attrs">指定字段</param>
//        /// <returns></returns>
//        public List<T> QuerySingle_Order<T>(string where, string groupItem, string orderItem, QuerySortType sort_Enum, string limit, params string[] attrs)
//        {
//            try
//            {
//                Type type = typeof(T);

//                string item = string.Join(",", attrs?.ToList() ?? new List<string>());

//                var sqlStr = string.Format(QUERY_ITEM_SINGLE_GROUP_ORDER, item, type.Name, where, groupItem, orderItem, sort_Enum.ToString(), limit);

//                return SQLiteConnect_Memory.Query<T>(sqlStr).ToList();
//            }
//            catch (Exception ex) { throw ex; }
//        }

//        /// <summary>
//        /// 按条件查询多条数据
//        /// </summary>
//        /// <typeparam name="T"></typeparam>
//        /// <param name="where"></param>
//        /// <returns></returns>
//        public List<T> QueryMultiByWhere<T>(string where)
//        {
//            try
//            {
//                Type type = typeof(T);

//                var sqlStr = string.Format(Query_ITEM_TABLE_WHERE, "*", type.Name, where);

//                return SQLiteConnect_Memory.Query<T>(sqlStr).ToList();
//            }
//            catch (Exception ex) { throw ex; }
//        }

//        /// <summary>
//        /// 按条件查询多条数据
//        /// </summary>
//        /// <typeparam name="T"></typeparam>
//        /// <param name="where"></param>
//        /// <param name="attrs"></param>
//        /// <returns></returns>
//        public List<T> QueryMultiByWhere<T>(string where, params string[] attrs)
//        {
//            try
//            {
//                Type type = typeof(T);

//                string item = string.Join(",", attrs?.ToList() ?? new List<string>());

//                var sqlStr = string.Format(Query_ITEM_TABLE_WHERE, item, type.Name, where);

//                return SQLiteConnect_Memory.Query<T>(sqlStr).ToList();
//            }
//            catch (Exception ex) { throw ex; }
//        }

//        /// <summary>
//        /// 分页查找数据
//        /// </summary>
//        /// <typeparam name="T"></typeparam>
//        /// <param name="where"></param>
//        /// <param name="pageIndex"></param>
//        /// <param name="pageSize"></param>
//        /// <returns></returns>
//        public List<T> QueryMultiByWhere_Page<T>(string where, int pageIndex = 0, int pageSize = 25)
//        {
//            try
//            {
//                Type type = typeof(T);

//                var sqlStr = string.Format(QUERY_ITEM_TABLE_WHERE_BY_PAGE, "*", type.Name, where, pageIndex, pageSize);

//                return SQLiteConnect_Memory.Query<T>(sqlStr).ToList();
//            }
//            catch (Exception ex) { throw ex; }
//        }

//        /// <summary>
//        /// 查询所有记录
//        /// </summary>
//        /// <typeparam name="T"></typeparam>
//        /// <returns></returns>
//        public List<T> QueryMultiAll<T>()
//        {
//            try
//            {
//                Type type = typeof(T);

//                var sqlStr = string.Format(Query_ITEM_TABLE_ALL, "*", type.Name);

//                return SQLiteConnect_Memory.Query<T>(sqlStr).ToList();
//            }
//            catch (Exception ex) { throw ex; }
//        }

//        /// <summary>
//        /// 按指定顺序分页查找数据
//        /// </summary>
//        /// <typeparam name="T"></typeparam>
//        /// <param name="orderItem"></param>
//        /// <param name="sort_Enum"></param>
//        /// <param name="limit"></param>
//        /// <param name="attrs"></param>
//        /// <returns></returns>
//        public List<T> QueryMultiAll_Order<T>(string where, string orderItem, QuerySortType sort_Enum, string limit, params string[] attrs)
//        {
//            try
//            {
//                Type type = typeof(T);

//                string item = string.Join(",", attrs?.ToList() ?? new List<string>());

//                var sqlStr = string.Format(QUERY_ALL_ORDER_BY_LIMIT, item, type.Name, where, orderItem, sort_Enum.ToString(), limit);

//                return SQLiteConnect_Memory.Query<T>(sqlStr).ToList();
//            }
//            catch (Exception ex) { throw ex; }
//        }

//        /// <summary>
//        /// 查找某些字段的所有信息
//        /// </summary>
//        /// <typeparam name="T"></typeparam>
//        /// <param name="where"></param>
//        /// <param name="attrs"></param>
//        /// <returns></returns>
//        public List<T> QueryMultiAll<T>(params string[] attrs)
//        {
//            try
//            {
//                Type type = typeof(T);

//                string item = string.Join(",", attrs?.ToList() ?? new List<string>());

//                var sqlStr = string.Format(Query_ITEM_TABLE_ALL, item, type.Name);

//                return SQLiteConnect_Memory.Query<T>(sqlStr).ToList();
//            }
//            catch (Exception ex) { throw ex; }
//        }

//        #endregion

//        /// <summary>
//        /// 清空数据表
//        /// </summary>
//        /// <typeparam name="T"></typeparam>
//        public int ClearTable<T>()
//        {
//            try
//            {
//                var type = typeof(T);
//                string sqlStr = string.Format(CLEAN_UP_THE_DATA_SHEET, type.Name);
//                return SQLiteConnect_Memory.Execute(sqlStr);
//            }
//            catch (Exception ex) { throw ex; }
//        }
//    }
//}
