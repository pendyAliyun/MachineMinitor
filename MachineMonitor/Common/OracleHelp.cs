using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;

namespace Common
{
    public static class OracleHelper
    {

        public static List<T> ExecuteReaderList<T>(string connString, string cmdText, Func<OracleDataReader, T> funcReader) where T : new()
        {
            return ExecuteReaderList(connString, cmdText, null, funcReader);
        }
        public static T ExecuteReaderEntity<T>(string connString, string cmdText, Func<OracleDataReader, T> funcReader) where T : new()
        {
            return ExecuteReaderEntity(connString, cmdText, null, funcReader);
        }
        public static int ExecuteNonQuery(string connString, string cmdText)
        {
            return ExecuteNonQuery(connString, cmdText, null);
        }
        public static object ExecuteScalar(string connString, string cmdText)
        {
            return ExecuteScalar(connString, cmdText, null);
        }
        public static DataSet ExecuteFillDataSet(string connString, string cmdText)
        {
            return ExecuteFillDataSet(connString, cmdText, null);
        }


        public static List<T> ExecuteReaderList<T>(string connString, string cmdText, Action<OracleCommand> cmdAction, Func<OracleDataReader, T> funcReader) where T : new()
        {
            using (OracleConnection conn = new OracleConnection(connString))
            {
                TryOpenSqlConnection(conn);
                return ExecuteReaderList<T>(conn, null, cmdText, cmdAction, funcReader);
            }
        }
        public static T ExecuteReaderEntity<T>(string connString, string cmdText, Action<OracleCommand> cmdAction, Func<OracleDataReader, T> funcReader) where T : new()
        {
            using (OracleConnection conn = new OracleConnection(connString))
            {
                TryOpenSqlConnection(conn);
                return ExecuteReaderEntity<T>(conn, null, cmdText, cmdAction, funcReader);
            }
        }
        public static int ExecuteNonQuery(string connString, string cmdText, Action<OracleCommand> cmdAction)
        {
            using (OracleConnection conn = new OracleConnection(connString))
            {
                TryOpenSqlConnection(conn);
                return ExecuteNonQuery(conn, null, cmdText, cmdAction);
            }
        }
        public static object ExecuteScalar(string connString, string cmdText, Action<OracleCommand> cmdAction)
        {
            using (OracleConnection conn = new OracleConnection(connString))
            {
                TryOpenSqlConnection(conn);
                return ExecuteScalar(conn, null, cmdText, cmdAction);
            }
        }
        public static DataSet ExecuteFillDataSet(string connString, string cmdText, Action<OracleCommand> cmdAction)
        {
            using (OracleConnection conn = new OracleConnection(connString))
            {
                TryOpenSqlConnection(conn);
                return ExecuteFillDataSet(conn, null, cmdText, cmdAction);
            }
        }


        public static List<T> ExecuteReaderList<T>(OracleConnection conn, OracleTransaction tran, string cmdText, Action<OracleCommand> cmdAction, Func<OracleDataReader, T> funcReader) where T : new()
        {
            List<OracleSingleScript> listScript = SplitOracleScript(cmdText);
            if (listScript == null || listScript.Count <= 0 || listScript.Count == 1)
            {
                return SingleExecuteReaderList<T>(conn, tran, cmdText, cmdAction, funcReader);
            }
            else
            {
                OracleBatchResult<T> result = ExecuteBatchScript<T>(conn, tran, listScript, false, cmdAction, funcReader);
                return result.ExecuteReaderList();
            }
        }
        public static T ExecuteReaderEntity<T>(OracleConnection conn, OracleTransaction tran, string cmdText, Action<OracleCommand> cmdAction, Func<OracleDataReader, T> funcReader) where T : new()
        {
            List<OracleSingleScript> listScript = SplitOracleScript(cmdText);
            if (listScript == null || listScript.Count <= 0 || listScript.Count == 1)
            {
                return SingleExecuteReaderEntity<T>(conn, tran, cmdText, cmdAction, funcReader);
            }
            else
            {
                OracleBatchResult<T> result = ExecuteBatchScript<T>(conn, tran, listScript, false, cmdAction, funcReader);
                return result.ExecuteReaderEntity();
            }
        }
        public static int ExecuteNonQuery(OracleConnection conn, OracleTransaction tran, string cmdText, Action<OracleCommand> cmdAction)
        {
            List<OracleSingleScript> listScript = SplitOracleScript(cmdText);
            if (listScript == null || listScript.Count <= 0 || listScript.Count == 1)
            {
                return SingleExecuteNonQuery(conn, tran, cmdText, cmdAction);
            }
            else
            {
                OracleBatchResult<object> result = ExecuteBatchScript<object>(conn, tran, listScript, false, cmdAction, null);
                return result.ExecuteNonQuery();
            }
        }
        public static object ExecuteScalar(OracleConnection conn, OracleTransaction tran, string cmdText, Action<OracleCommand> cmdAction)
        {
            List<OracleSingleScript> listScript = SplitOracleScript(cmdText);
            if (listScript == null || listScript.Count <= 0 || listScript.Count == 1)
            {
                return SingleExecuteScalar(conn, tran, cmdText, cmdAction);
            }
            else
            {
                OracleBatchResult<object> result = ExecuteBatchScript<object>(conn, tran, listScript, false, cmdAction, null);
                return result.ExecuteScalar();
            }
        }
        public static DataSet ExecuteFillDataSet(OracleConnection conn, OracleTransaction tran, string cmdText, Action<OracleCommand> cmdAction)
        {
            List<OracleSingleScript> listScript = SplitOracleScript(cmdText);
            if (listScript == null || listScript.Count <= 0 || listScript.Count == 1)
            {
                DataTable dataTable = SingleExecuteFillDataTable(conn, tran, cmdText, cmdAction);
                DataSet dataSet = new DataSet();
                if (dataTable != null) dataSet.Tables.Add(dataTable);
                return dataSet;
            }
            else
            {
                OracleBatchResult<object> result = ExecuteBatchScript<object>(conn, tran, listScript, true, cmdAction, null);
                return result.ExecuteFillDataSet();
            }
        }



        private static OracleBatchResult<T> ExecuteBatchScript<T>(string connString, IEnumerable<OracleSingleScript> listScript, bool isSelectDataSet, Action<OracleCommand> cmdAction, Func<OracleDataReader, T> funcReader) where T : new()
        {
            using (OracleConnection conn = new OracleConnection(connString))
            {
                TryOpenSqlConnection(conn);
                using (OracleTransaction tran = conn.BeginTransaction())
                {
                    OracleBatchResult<T> result = ExecuteBatchScript(conn, tran, listScript, isSelectDataSet, cmdAction, funcReader);
                    return result;
                }
            }
        }
        private static OracleBatchResult<T> ExecuteBatchScript<T>(OracleConnection conn, OracleTransaction tran, IEnumerable<OracleSingleScript> listScript, bool isSelectDataSet, Action<OracleCommand> cmdAction, Func<OracleDataReader, T> funcReader) where T : new()
        {
            OracleBatchResult<T> result = new OracleBatchResult<T>();

            bool tranIsNull = tran == null;
            if (tranIsNull) tran = conn.BeginTransaction();
            try
            {
                foreach (OracleSingleScript script in listScript)
                {
                    #region  执行查询实体

                    if (script.IsSelect)
                    {
                        if (isSelectDataSet)
                        {
                            DataTable dataTable = SingleExecuteFillDataTable(conn, tran, script.SqlScript, cmdAction);
                            result.AddDataTable(dataTable);
                        }
                        else if (typeof(T) == typeof(object) && funcReader == null)
                        {
                            object scalar = SingleExecuteScalar(conn, tran, script.SqlScript, cmdAction);
                            result.AddScalar(scalar);
                        }
                        else
                        {
                            List<T> list = SingleExecuteReaderList<T>(conn, tran, script.SqlScript, cmdAction, funcReader);
                            result.AddList(list);
                        }
                    }

                    #endregion
                    #region  执行增加修改删除

                    if (script.IsInsert || script.IsUpdate || script.IsDelete)
                    {
                        int effect = SingleExecuteNonQuery(conn, tran, script.SqlScript, cmdAction);
                        result.AddEffect(effect);
                    }

                    #endregion
                }
                if (tranIsNull && tran != null) tran.Commit();
            }
            finally
            {
                if (tranIsNull && tran != null) tran.Dispose();
            }

            return result;
        }


        #region  执行单条脚本

        //private static List<T> SingleExecuteReaderList<T>(string connString, string cmdText, Action<OracleCommand> cmdAction, Func<OracleDataReader, T> funcReader) where T : new()
        //{
        //    using (OracleConnection conn = new OracleConnection(connString))
        //    {
        //        TryOpenSqlConnection(conn);
        //        return SingleExecuteReaderList(conn, null, cmdText, cmdAction, funcReader);
        //    }
        //}
        //private static T SingleExecuteReaderEntity<T>(string connString, string cmdText, Action<OracleCommand> cmdAction, Func<OracleDataReader, T> funcReader) where T : new()
        //{
        //    using (OracleConnection conn = new OracleConnection(connString))
        //    {
        //        TryOpenSqlConnection(conn);
        //        return SingleExecuteReaderEntity(conn, null, cmdText, cmdAction, funcReader);
        //    }
        //}
        //private static int SingleExecuteNonQuery(string connString, string cmdText, Action<OracleCommand> cmdAction)
        //{
        //    using (OracleConnection conn = new OracleConnection(connString))
        //    {
        //        TryOpenSqlConnection(conn);
        //        return SingleExecuteNonQuery(conn, null, cmdText, cmdAction);
        //    }
        //}
        //private static object SingleExecuteScalar(string connString, string cmdText, Action<OracleCommand> cmdAction)
        //{
        //    using (OracleConnection conn = new OracleConnection(connString))
        //    {
        //        TryOpenSqlConnection(conn);
        //        return SingleExecuteScalar(conn, null, cmdText, cmdAction);
        //    }
        //}
        //private static DataTable SingleExecuteFillDataTable(string connString, string cmdText, Action<OracleCommand> cmdAction)
        //{
        //    using (OracleConnection conn = new OracleConnection(connString))
        //    {
        //        TryOpenSqlConnection(conn);
        //        return SingleExecuteFillDataTable(conn, null, cmdText, cmdAction);
        //    }
        //}


        private static List<T> SingleExecuteReaderList<T>(OracleConnection conn, OracleTransaction tran, string cmdText, Action<OracleCommand> cmdAction, Func<OracleDataReader, T> funcReader) where T : new()
        {
            List<T> list = new List<T>();
            //需要查询的是否是 原生值类型
            bool isMetaValue = typeof(T).IsValueType && typeof(T).GetProperties().Length <= 0 && typeof(T).GetFields().Length <= 0;

            using (OracleCommand cmd = conn.CreateCommand())
            {
                cmd.CommandText = cmdText;
                cmd.CommandTimeout = int.MaxValue;
                //cmd.Transaction = tran;
                if (cmdAction != null) cmdAction(cmd);

                using (OracleDataReader reader = cmd.ExecuteReader())
                {
                    List<string> listFieldName = new List<string>();
                    while (reader.Read())
                    {
                        if (funcReader != null)
                        {
                            //通过指定的 函数 创建对象
                            T item = funcReader(reader);
                            if (!object.Equals(item, default(T))) list.Add(item);
                        }
                        else
                        {
                            if (listFieldName.Count <= 0)
                                for (int i = 0; i < reader.FieldCount; i++)
                                {
                                    string fieldName = reader.GetName(i).Trim();
                                    listFieldName.Add(fieldName);
                                }

                            //通过反射 创建对象
                            if (isMetaValue)
                            {
                                object value = reader[listFieldName[0]];
                                T item = (T)Tools.ConvertValue(value, typeof(T));
                                if (!object.Equals(item, default(T))) list.Add(item);
                            }
                            else
                            {
                                T item = new T();
                                foreach (string fieldName in listFieldName)
                                {
                                    object value = reader[fieldName];
                                    //if (value == null || value == DBNull.Value) value = ConstDefine.InvalidValueDoulbe;
                                    if (value != null && value != DBNull.Value)
                                        Tools.SetValue(item, fieldName, value);
                                }
                                if (!object.Equals(item, default(T))) list.Add(item);
                            }
                        }
                    }
                }
            }

            return list;
        }
        private static T SingleExecuteReaderEntity<T>(OracleConnection conn, OracleTransaction tran, string cmdText, Action<OracleCommand> cmdAction, Func<OracleDataReader, T> funcReader) where T : new()
        {
            T result = default(T);
            //需要查询的是否是 原生值类型
            bool isMetaValue = typeof(T).IsValueType && typeof(T).GetProperties().Length <= 0 && typeof(T).GetFields().Length <= 0;

            using (OracleCommand cmd = conn.CreateCommand())
            {
                cmd.CommandText = cmdText;
                cmd.CommandTimeout = int.MaxValue;
                if (cmdAction != null) cmdAction(cmd);

                using (OracleDataReader reader = cmd.ExecuteReader())
                {
                    List<string> listFieldName = new List<string>();
                    if (reader.Read())
                    {
                        if (funcReader != null)
                        {
                            //通过指定的 函数 创建对象
                            T item = funcReader(reader);
                            result = item;
                        }
                        else
                        {
                            if (listFieldName.Count <= 0)
                                for (int i = 0; i < reader.FieldCount; i++)
                                {
                                    string fieldName = reader.GetName(i).Trim();
                                    listFieldName.Add(fieldName);
                                }

                            //通过反射 创建对象
                            if (isMetaValue)
                            {
                                object value = reader[listFieldName[0]];
                                T item = (T)Tools.ConvertValue(value, typeof(T));
                                result = item;
                            }
                            else
                            {
                                T item = new T();
                                foreach (string fieldName in listFieldName)
                                    Tools.SetValue(item, fieldName, reader[fieldName]);
                                result = item;
                            }
                        }
                    }
                }
            }

            return result;
        }
        private static int SingleExecuteNonQuery(OracleConnection conn, OracleTransaction tran, string cmdText, Action<OracleCommand> cmdAction)
        {
            int result = 0;

            using (OracleCommand cmd = conn.CreateCommand())
            {
                cmd.CommandText = cmdText;
                cmd.CommandTimeout = int.MaxValue;
                if (cmdAction != null) cmdAction(cmd);

                result = cmd.ExecuteNonQuery();
            }

            return result;
        }
        private static object SingleExecuteScalar(OracleConnection conn, OracleTransaction tran, string cmdText, Action<OracleCommand> cmdAction)
        {
            object result = null;

            using (OracleCommand cmd = conn.CreateCommand())
            {
                cmd.CommandText = cmdText;
                cmd.CommandTimeout = int.MaxValue;
                if (cmdAction != null) cmdAction(cmd);

                result = cmd.ExecuteScalar();
            }

            return result;

        }
        private static DataTable SingleExecuteFillDataTable(OracleConnection conn, OracleTransaction tran, string cmdText, Action<OracleCommand> cmdAction)
        {
            DataTable dataTable = new DataTable();

            using (OracleCommand cmd = conn.CreateCommand())
            {
                cmd.CommandText = cmdText;
                cmd.CommandTimeout = int.MaxValue;
                if (cmdAction != null) cmdAction(cmd);

                using (OracleDataAdapter adapter = new OracleDataAdapter(cmd))
                {
                    adapter.Fill(dataTable);
                }
            }

            return dataTable;
        }

        #endregion



        /// <summary>
        /// 用指定的 脚本开启一个事务, 并且 打开数据库链接
        /// </summary>
        public static void DoTransaction(string connString, Action<OracleTransaction, OracleConnection> tranAction)
        {
            using (OracleConnection conn = new OracleConnection(connString))
            {
                TryOpenSqlConnection(conn);
                using (OracleTransaction tran = conn.BeginTransaction())
                {
                    if (tranAction == null) throw new Exception("OracleHelper.DoTransaction(string, Action) 必须提供 有效的 回调委托!");
                    if (tranAction != null) tranAction(tran, conn);
                }
            }
        }
        /// <summary>
        /// 尝试 打开 指定的 数据库链接
        /// </summary>
        public static void TryOpenSqlConnection(OracleConnection conn)
        {
            TryOpenSqlConnection(conn, 50);
        }
        /// <summary>
        /// 尝试 打开 指定的 数据库链接
        /// </summary>
        public static void TryOpenSqlConnection(OracleConnection conn, int reTryCount)
        {
            if (conn == null) return;
            int expCount = 0;
            do
            {
                try
                {
                    conn.Open();
                    break;
                }
                catch (Exception exp)
                {
                    if (exp is InvalidOperationException || exp is OracleException)
                    {
                        expCount++;
                        if (expCount >= reTryCount) throw;          //重试reTryCount次都失败, 则不再重试, 抛出异常
                        Thread.Sleep(5);                            //打开失败时, 休眠 5毫秒, 再重试打开 
                    }
                    else throw;
                }

            } while (true);
        }



        /// <summary>
        /// Oracle 不支持 多条脚本 同时执行, 这里按照 ; 拆分脚本, 分次执行
        /// </summary>
        private static List<OracleSingleScript> SplitOracleScript(string cmdText)
        {
            List<OracleSingleScript> listScript = new List<OracleSingleScript>();
            if (string.IsNullOrWhiteSpace(cmdText)) return listScript;
            if (!cmdText.Contains(";"))
            {
                OracleSingleScript singleScript = OracleSingleScript.Create(cmdText);
                if (singleScript != null) listScript.Add(singleScript);
                return listScript;
            }

            string cmdTextTrim = cmdText.Trim().Trim(';').Trim();
            if (cmdTextTrim.StartsWith("BEGIN", StringComparison.CurrentCultureIgnoreCase))
                cmdTextTrim = cmdTextTrim.Substring("BEGIN".Length);
            if (cmdTextTrim.EndsWith("END", StringComparison.CurrentCultureIgnoreCase))
                cmdTextTrim = cmdTextTrim.Substring(0, cmdTextTrim.Length - "END".Length);


            string[] splitTemp = cmdTextTrim.Split(new[] { ';' }, StringSplitOptions.None);

            List<string> listGroup = new List<string>();
            foreach (string temp in splitTemp)
            {
                string tempTrim = temp.Trim();

                //可以作为开头的 脚本
                if (tempTrim.StartsWith("SELECT", StringComparison.CurrentCultureIgnoreCase)
                    || tempTrim.StartsWith("INSERT", StringComparison.CurrentCultureIgnoreCase)
                    || tempTrim.StartsWith("UPDATE", StringComparison.CurrentCultureIgnoreCase)
                    || tempTrim.StartsWith("DELETE", StringComparison.CurrentCultureIgnoreCase)
                    )
                {
                    if (listGroup.Count > 0)
                    {
                        string script = string.Join(";", listGroup).Trim().Trim(';').Trim();
                        OracleSingleScript singleScript = OracleSingleScript.Create(script);
                        if (singleScript != null) listScript.Add(singleScript);
                        listGroup.Clear();
                    }
                }

                listGroup.Add(temp);
            }

            if (listGroup.Count > 0)
            {
                string lastScript = string.Join(";", listGroup).Trim().Trim(';').Trim();
                OracleSingleScript singleScript = OracleSingleScript.Create(lastScript);
                if (singleScript != null) listScript.Add(singleScript);
                listGroup.Clear();
            }

            return listScript;
        }





        [Serializable]
        internal class OracleSingleScript
        {
            private string sqlScript = string.Empty;

            public bool IsSelect { get; private set; }
            public bool IsInsert { get; private set; }
            public bool IsUpdate { get; private set; }
            public bool IsDelete { get; private set; }
            public string SqlScript
            {
                get { return sqlScript; }
                set { sqlScript = (value ?? string.Empty).Trim().Trim(';').Trim(); }
            }


            private OracleSingleScript() { }
            public override string ToString()
            {
                return SqlScript;
            }
            public static OracleSingleScript Create(string script)
            {
                script = (script ?? string.Empty).Trim().Trim(';').Trim();
                if (string.IsNullOrWhiteSpace(script)) return null;

                OracleSingleScript item = new OracleSingleScript();
                item.SqlScript = script;

                item.IsSelect = script.StartsWith("SELECT", StringComparison.CurrentCultureIgnoreCase);
                if (!item.IsSelect)
                {
                    item.IsInsert = script.StartsWith("INSERT", StringComparison.CurrentCultureIgnoreCase);
                    if (!item.IsInsert)
                    {
                        item.IsUpdate = script.StartsWith("UPDATE", StringComparison.CurrentCultureIgnoreCase);
                        if (!item.IsUpdate)
                            item.IsDelete = script.StartsWith("DELETE", StringComparison.CurrentCultureIgnoreCase);
                    }
                }

                return item;
            }
        }

        [Serializable]
        internal class OracleBatchResult<T> where T : new()
        {
            private readonly List<List<T>> list = new List<List<T>>();
            private readonly List<int> effect = new List<int>();
            private readonly List<object> scalar = new List<object>();
            private readonly List<DataTable> dataTable = new List<DataTable>();
            private readonly DataSet dataSet = new DataSet();

            public void AddList(List<T> temp) { list.Add(temp); }
            public void AddEffect(int temp) { effect.Add(temp); }
            public void AddScalar(object temp) { scalar.Add(temp); }
            public void AddDataTable(DataTable temp) { dataTable.Add(temp); }

            public List<T> ExecuteReaderList()
            {
                return list.Count >= 1 ? list[list.Count - 1] : null;
            }
            public T ExecuteReaderEntity()
            {
                List<T> listT = ExecuteReaderList();
                return (listT != null && listT.Count >= 1) ? listT[0] : default(T);
            }
            public int ExecuteNonQuery()
            {
                return effect.Count >= 1 ? effect[effect.Count - 1] : 0;
            }
            public object ExecuteScalar()
            {
                //如果 scalar 集合 存在数据, 直接返回 最后一个
                //如果 scalar 集合无效, 但是 list 集合有效, 尝试获取 第一个实体, 第一个属性的值
                //如果 scalar、list 集合有效, 但是 dataTable 集合有效, 尝试获取 第一个DataRow, 第一个DataCell的值

                if (scalar.Count >= 1)
                {
                    return scalar[scalar.Count - 1];
                }
                else if (list.Count >= 1)
                {
                    #region  尝试从 list 实体集合中 返回 单行单列 值

                    try
                    {
                        List<T> listT = list[list.Count - 1];
                        if ((listT != null && listT.Count >= 1))
                        {
                            T first = listT[0];
                            PropertyInfo[] firstProperties = typeof(T).GetProperties();
                            if (firstProperties.Length >= 1)
                                return firstProperties[0].GetValue(first, null);
                            else
                            {
                                FieldInfo[] firstFields = typeof(T).GetFields();
                                if (firstFields.Length >= 1)
                                    return firstFields[0].GetValue(first);
                            }
                        }
                    }
                    catch (Exception) { }

                    #endregion
                }
                else if (dataTable.Count >= 1)
                {
                    #region  尝试从 dataTable 集合中 返回 但行单列 值

                    try
                    {
                        DataTable table = dataTable[dataTable.Count - 1];
                        if ((table != null && table.Rows.Count >= 1))
                        {
                            DataRow first = table.Rows[0];
                            if (table.Columns.Count >= 1)
                                return first[0];
                        }
                    }
                    catch (Exception) { }

                    #endregion
                }
                return null;
            }
            public DataSet ExecuteFillDataSet()
            {
                dataSet.Clear();
                dataSet.Tables.AddRange(dataTable.ToArray());
                return dataSet;
            }
        }
    }
}
