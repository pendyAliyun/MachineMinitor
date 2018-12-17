using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Configuration;
using Oracle.ManagedDataAccess.Client;

namespace Common
{
    public class OraHelper
    {
        static OracleConnection conn = null;
        public static OracleDataReader GetDataReader(string sql)
        {            
            try
            {
                conn= OpenConn() ;
                var cmd = conn.CreateCommand();
                cmd.CommandText = sql;
                cmd.CommandType = CommandType.Text;
                return cmd.ExecuteReader(CommandBehavior.CloseConnection);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        /// <summary>
        /// 使用存储过程访问oracle数据库
        /// </summary>
        /// <param name="procName"></param>
        /// <param name="oracleParameters"></param>
        /// <param name="commandType"></param>
        /// <returns></returns>
        public static DataSet GetDataSet(string procName,OracleParameter[] oracleParameters,CommandType commandType)
        {
            try
            {
                using (conn = OpenConn())
                {
                    DataSet dataSet=new DataSet();
                    var cmd = conn.CreateCommand();
                    cmd.CommandText = procName;
                    cmd.CommandType = commandType;
                    cmd.Parameters.AddRange(oracleParameters);
                    OracleDataAdapter dataAdapter = new OracleDataAdapter(cmd);
                    dataAdapter.Fill(dataSet);
                    return dataSet;
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        /// <summary>
        /// 不带参数存储过程
        /// </summary>
        /// <param name="procName"></param>
        /// <param name="commandType"></param>
        /// <returns></returns>
        public static OracleDataReader GetDataReader(string procName,CommandType commandType)
        {
            try
            {
                using (conn = OpenConn())
                {
                    var cmd = conn.CreateCommand();
                    cmd.CommandText = procName;
                    cmd.CommandType = commandType;
                    return cmd.ExecuteReader(CommandBehavior.CloseConnection);
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        static OracleConnection OpenConn()
        {
            OracleConnection conn = new OracleConnection(StringSecurity.DESDecrypt(ConfigurationManager.ConnectionStrings["bobcatConn"].ToString()));
            //conn.ConnectionString = "Data Source=(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST=10.1.0.33)(PORT=1521))(CONNECT_DATA=(SERVICE_NAME=orcl)));Persist Security Info=True;User ID=aio;Password=aio123456;Pooling=True;Max Pool Size=10;Min Pool Size=1";
            conn.Open();
            return conn;
        }

        static void CloseConn(OracleConnection conn)
        {
            if (conn == null) { return; }
            try
            {
                if (conn.State != ConnectionState.Closed)
                {
                    conn.Close();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            finally
            {
                conn.Dispose();
            }
        }
    }
}
