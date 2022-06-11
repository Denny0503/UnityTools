namespace TopMethods.Data
{
    internal class Common
    {
    }

    /// <summary>
    /// 键值对表
    /// </summary>
    public class KeyValueDataPair
    {
        public string Key { set; get; }
        public string Value { set; get; }
    }


    ///// <summary>
    ///// 升级本地数据库
    ///// </summary>
    //private void UpgradeSqliteData()
    //{
    //    StringBuilder str = new StringBuilder();
    //    if (SqliteDataTableInfoDict.Count > 0)
    //    {
    //        foreach (SqliteTableType key in SqliteDataTableInfoDict.Keys)
    //        {
    //            SqliteDataTableInfoDict[key].UUID = SqliteDataTableInfoDict[key].CreateTableSql.ToMD5_32();
    //            str.Append(SqliteDataTableInfoDict[key].UUID);
    //            str.Append("|");

    //            /*初始化数据库表，数据库表不存在时有效*/
    //            SQLiteConnect_Memory?.Execute(SqliteDataTableInfoDict[key].CreateTableSql);
    //        }
    //    }

    //    //本地数据库信息的标识信息
    //    string localDataUUID = str.ToString();

    //    string localDataUUID_Old = CoreKernelRegistration.ReadRegistryKey("LocalDataUUID", "SmartConsole");
    //    if (localDataUUID_Old.IsEmpty() || !localDataUUID_Old.Equals(localDataUUID))
    //    {
    //        /*获取数据库需要更新的列表*/
    //        List<string> data = localDataUUID_Old?.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries).ToList();
    //        if (null != data && data.Count > 0)
    //        {
    //            foreach (SqliteTableType key in SqliteDataTableInfoDict.Keys)
    //            {
    //                if (!data.Contains(SqliteDataTableInfoDict[key].UUID))
    //                {
    //                    SqliteDataTableInfoDict[key].IsUpdate = true;
    //                }
    //            }
    //        }

    //        /*升级数据库*/
    //        List<string> columnsOld = new List<string>(); /*旧表列名*/
    //        List<string> columnsNew = new List<string>(); /*新表列名*/
    //        foreach (SqliteTableType key in SqliteDataTableInfoDict.Keys)
    //        {
    //            if (SqliteDataTableInfoDict[key].IsUpdate)
    //            {
    //                columnsOld.Clear();
    //                columnsNew.Clear();

    //                /*创建临时表，并复制数据*/
    //                SQLiteConnect_Memory?.Execute($"CREATE TABLE IF NOT EXISTS {SqliteDataTableInfoDict[key].TableName}_temp AS SELECT * FROM {SqliteDataTableInfoDict[key].TableName}; ");

    //                /*查询原表的字段名*/
    //                using (SQLiteDataReader dr = (SQLiteDataReader)((IWrappedDataReader)SQLiteConnect_Memory?.ExecuteReader($"PRAGMA table_info({SqliteDataTableInfoDict[key].TableName});")).Reader)
    //                {
    //                    while (null != dr && dr.Read())
    //                    {
    //                        columnsOld.Add(dr["Name"].ToString());
    //                    }
    //                }

    //                /*删除原表*/
    //                SQLiteConnect_Memory?.Execute(SqliteDataTableInfoDict[key].DropTableSql);

    //                /*创建新数据表*/
    //                SQLiteConnect_Memory?.Execute(SqliteDataTableInfoDict[key].CreateTableSql);

    //                /*查询新数据表的字段名*/
    //                using (SQLiteDataReader dr = (SQLiteDataReader)((IWrappedDataReader)SQLiteConnect_Memory?.ExecuteReader($"PRAGMA table_info({SqliteDataTableInfoDict[key].TableName});")).Reader)
    //                {
    //                    while (null != dr && dr.Read())
    //                    {
    //                        columnsNew.Add(dr["Name"].ToString());
    //                    }
    //                }

    //                /*删除不存在的字段*/
    //                for (int i = 0; i < columnsOld.Count; i++)
    //                {
    //                    if (!columnsNew.Contains(columnsOld[i]))
    //                    {
    //                        columnsOld.RemoveAt(i);
    //                        --i;
    //                    }
    //                }

    //                /*复制原数据到新表*/
    //                SQLiteConnect_Memory?.Execute($"INSERT INTO {SqliteDataTableInfoDict[key].TableName}({string.Join(",", columnsOld)}) SELECT {string.Join(",", columnsOld)} FROM {SqliteDataTableInfoDict[key].TableName}_temp;");

    //                /*删除临时表*/
    //                SQLiteConnect_Memory?.Execute($"DROP TABLE IF EXISTS \"{SqliteDataTableInfoDict[key].TableName}_temp\"");
    //            }
    //        }

    //        /*保存数据库标记信息到注册表*/
    //        CoreKernelRegistration.AddRegistryKey("LocalDataUUID", localDataUUID, "SmartConsole");
    //    }

    //}

}
