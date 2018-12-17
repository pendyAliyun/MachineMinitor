using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Data;
using System.IO;
using System.Configuration;

namespace Common
{
    [Serializable]
    /// <summary>
    /// 数据访问类
    /// </summary>
    public static class SqlHelper
    {
        public static string connstring = StringSecurity.DESDecrypt(ConfigurationManager.ConnectionStrings["slm_erpConn"].ToString());
        public static string connSYS;

        //static string connstring = @"server=10.0.0.9;database=EB_shintech;UID=sc;PWD=DZP@STE11";
        //static string connstring = ConfigurationManager.ConnectionStrings["MySqlConnection"].ToString();
        //static string connstring = @"server=10.0.0.7;;database=SRM;UID=sa;PWD=it5002";
        //static string connstring = StringSecurity.DESDecrypt("skl1kKgFLhOkGmGIkIT5AX40+Jt1yicULLaHCobuA6D2HXAbNMCet6VyM3ExtY5UlaMWChElIjIR2w3O4rPsmA==");


        #region 加解密字符串测试
        /// <summary>
        /// 加解密字符串设定
        /// </summary>
        /// <returns></returns>
        public static string GetStringSecurity()
        {
            //connstring = StringSecurity.DESEncrypt(connstring);
            connstring = StringSecurity.DESDecrypt(connstring);
            return connstring;
        }
        #endregion

        #region 标准SQL查询语句
        /// <summary>
        /// 返回一行一列结果查询
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public static object GetSingleResult(string sql)
        {
            SqlConnection conn = new SqlConnection(connstring);
            SqlCommand cmd = new SqlCommand(sql, conn);
            return ExecuteScalar(conn, cmd);
        }
        /// <summary>
        /// 返回单一结果集的方法
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public static SqlDataReader GetDataReader(string sql)
        {
            SqlConnection conn = new SqlConnection(connstring);
            SqlCommand cmd = new SqlCommand(sql, conn);
            try
            {
                conn.Open();
                return cmd.ExecuteReader(CommandBehavior.CloseConnection);
            }
            catch (Exception ex)
            {
                WritterError("执行UpdateBySql(string sql)出现异常:", ex.Message);
                conn.Close();
                throw ex;
            }
        }

        /// <summary>
        /// 增删改方法
        /// </summary>
        /// <param name="sql">sql语句</param>
        /// <returns></returns>
        public static int Update(string sql)
        {
            SqlConnection conn = new SqlConnection(connstring);
            SqlCommand cmd = new SqlCommand(sql, conn);
            try
            {
                conn.Open();
                return cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                WritterError("执行UpdateBySql(string sql)出现异常:", ex.Message);
                throw ex;
            }
            finally
            {
                conn.Close();
            }
        }
        #endregion

        #region 带参数SQL语句方法
        public static object GetSingleResult(string sql, SqlParameter[] param)
        {
            SqlConnection conn = new SqlConnection(connstring);
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conn;
            cmd.CommandText = sql;
            try
            {
                conn.Open();
                cmd.Parameters.AddRange(param);
                return cmd.ExecuteScalar();
            }
            catch (Exception ex)
            {
                WritterError("执行 GetSingleResult(string sql,SqlParameter[] param)出现异常:", ex.Message);
                throw;
            }
            finally
            {
                conn.Close();
            }
        }

        public static List<String> GetSingleResult(string sql, List<SqlParameter[]> paramlist)
        {
            List<String> repeatList = new List<string>();
            SqlConnection conn = new SqlConnection(connstring);
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conn;
            cmd.CommandText = sql;
            try
            {
                conn.Open();
                for (int i = 0; i < paramlist.Count; i++)
                {
                    cmd.Parameters.AddRange(paramlist[i]);
                    if (cmd.ExecuteScalar() != null)
                    {
                        repeatList.Add(Convert.ToString(cmd.ExecuteScalar()));
                    };
                    cmd.Parameters.Clear();
                }
                return repeatList;
            }
            catch (Exception ex)
            {
                WritterError("执行 GetSingleResult(string sql,List<SqlParameter[]> paramlist)出现异常:", ex.Message);
                throw;
            }
            finally
            {
                if (conn != null) conn.Close();
            }
        }

        public static SqlDataReader GetDataReader(string sql, SqlParameter[] param)
        {
            SqlConnection conn = new SqlConnection(connstring);
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conn;
            cmd.CommandText = sql;
            try
            {
                conn.Open();
                cmd.Parameters.AddRange(param);
                return cmd.ExecuteReader(CommandBehavior.CloseConnection);
            }
            catch (Exception ex)
            {
                WritterError("执行 GetDataReader(string sql,SqlParameter[] param)出现异常:", ex.Message);
                conn.Close();
                throw ex;
            }
        }

        public static int Update(string sql, SqlParameter[] param)
        {
            SqlConnection conn = new SqlConnection(connstring);
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conn;
            cmd.CommandText = sql;
            try
            {
                conn.Open();
                cmd.Parameters.AddRange(param);
                return cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                WritterError("Update(string sql,SqlParameter[] param)出现异常:", ex.Message);
                throw ex;
            }
            finally
            {
                conn.Close();
            }
        }
        /// <summary>
        /// 多条SQL语句事务处理方法
        /// </summary>
        /// <param name="listSql">SQL语句集合</param>
        /// <param name="param"></param>
        /// <returns></returns>
        public static bool UpdateByTrans(List<string> listSql, SqlParameter[] param)
        {
            SqlConnection conn = new SqlConnection(connstring);
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conn;
            try
            {
                conn.Open();
                if (param != null) cmd.Parameters.AddRange(param);
                cmd.Transaction = conn.BeginTransaction();
                foreach (string sql in listSql)
                {
                    cmd.CommandText = sql;
                    cmd.ExecuteNonQuery();
                }
                cmd.Transaction.Commit();
                return true;
            }
            catch (Exception ex)
            {
                if (cmd.Transaction != null)
                {
                    cmd.Transaction.Rollback();
                    cmd.Transaction = null;
                }
                WritterError("UpdateByTrans(List<string> listSql,SqlParameter[] param)出现异常:", ex.Message);
                return false;
                throw ex;
            }
            finally
            {
                if (cmd.Transaction != null)
                    cmd.Transaction = null;
                conn.Close();
            }
        }

        #endregion

        #region 调用存储过程带参数方法
        public static object UpdateByProcedure(string procedureName, SqlParameter[] param)
        {
            SqlConnection conn = new SqlConnection(connstring);
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conn;
            try
            {
                conn.Open();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = procedureName;
                cmd.Parameters.AddRange(param);
                return cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                WritterError("执行UpdateByProcedure(string procedureName,SqlParameter[] param)出现异常:", ex.Message);
                throw ex;
            }
            finally
            {
                if (conn != null)
                    conn.Close();
            }
        }
        /// <summary>
        /// 通过存储过程返回结果集方法
        /// </summary>
        /// <param name="procedureName">存储过程名称</param>
        /// <param name="param">参数组</param>
        /// <returns>返回SqlDataReader数据结果集</returns>
        public static SqlDataReader GetReaderByProcedure(string procedureName, SqlParameter[] param)
        {
            SqlConnection conn = new SqlConnection(connstring);
            SqlCommand cmd = new SqlCommand();
            try
            {
                conn.Open();
                cmd.Connection = conn;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = procedureName;
                cmd.Parameters.AddRange(param);
                return cmd.ExecuteReader(CommandBehavior.CloseConnection);
            }
            catch (Exception ex)
            {
                WritterError("执行 GetReaderByProcedure(string procedureName,SqlParameter[] param)出现异常:", ex.Message);
                if (conn != null)
                    conn.Close();
                throw;
            }
        }
        #endregion

        #region 返回DataSet和DataTable查询方法
        public static DataSet GetDataSet(string sql, SqlParameter[] param)
        {
            SqlConnection conn = new SqlConnection(connstring);
            SqlCommand cmd = new SqlCommand(sql, conn);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            try
            {
                conn.Open();
                da.Fill(ds);
                return ds;
            }
            catch (Exception ex)
            {
                WritterError("执行 GetDataSet(string sql,SqlParameter[] param)出现异常:", ex.Message);
                throw ex;
            }
            finally
            {
                if (conn != null)
                    conn.Close();
            }

        }
        public static DataTable GetDataTable(string sql)
        {
            SqlConnection conn = new SqlConnection(connstring);            
            SqlCommand cmd = new SqlCommand(sql, conn);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            try
            {
                cmd.CommandTimeout = 60;
                da.Fill(ds);
                DataTable dt = ds.Tables[0];
                return dt;
            }
            catch (Exception ex)
            {
                WritterError("执行GetDataTable(string sql,SqlParameter[] param)出现异常:", ex.Message);
                throw ex;
            }
            finally
            {
                if (conn != null)
                    conn.Close();
            }
        }
        public static DataTable GetDataTable(string sql, SqlParameter[] param)
        {
            SqlConnection conn = new SqlConnection(connstring);
            SqlCommand cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddRange(param);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            try
            {

                da.Fill(ds);
                DataTable dt = ds.Tables[0];
                return dt;
            }
            catch (Exception ex)
            {
                WritterError("执行GetDataTable(string sql,SqlParameter[] param)出现异常:", ex.Message);
                throw ex;
            }
            finally
            {
                if (conn != null)
                    conn.Close();
            }
        }
        /// <summary>
        /// 通过存储过程返回一个DataSet数据集
        /// </summary>
        /// <param name="procedureName">存储过程名称</param>
        /// <param name="param">存储过程所需参数</param>
        /// <returns>DataSet数据集</returns>
        public static DataSet GetDataSetByProcedure(string procedureName, SqlParameter[] param)
        {
            SqlConnection conn = new SqlConnection(connstring);
            SqlCommand cmd = new SqlCommand(procedureName, conn);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            try
            {
                conn.Open();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddRange(param);
                DataSet ds = new DataSet();
                da.Fill(ds);
                return ds;

            }
            catch (Exception ex)
            {
                WritterError("执行GetTableByProcedure(string procedureName,SqlParameter[] param)出现异常:", ex.Message);
                throw ex;
            }
            finally
            {
                if (conn != null)
                    conn.Close();
            }
        }
        public static SqlDataReader GetObjReader(string procedureName, SqlParameter[] param)
        {
            SqlConnection conn = new SqlConnection(connstring);
            SqlCommand cmd = new SqlCommand(procedureName, conn);
            try
            {
                conn.Open();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddRange(param);
                return cmd.ExecuteReader(CommandBehavior.CloseConnection);
            }
            catch (Exception ex)
            {
                conn.Close();
                throw ex;
            }
        }

        #endregion

        #region 错误信息写入日志
        public static void WritterError(string errorObjName, string exMessage)
        {
            FileStream fs = new FileStream("libraryError.log", FileMode.Append);
            StreamWriter sw = new StreamWriter(fs);
            sw.WriteLine(string.Format("{0}{1}", errorObjName, exMessage + GetServerTime()));
            sw.Close();
            fs.Close();
        }
        #endregion

        #region 获取服务器时间
        public static DateTime GetServerTime()
        {
            return Convert.ToDateTime(GetSingleResult("select getdate()"));
        }

        #endregion

        public static object GetSingleResult(string sql,string configDbName)
        {
            connSYS = StringSecurity.DESDecrypt(ConfigurationManager.ConnectionStrings[configDbName].ToString());
            SqlConnection conn = new SqlConnection(connSYS);
            return ExecuteScalar(conn, new SqlCommand(sql, conn));
        }

        private static object ExecuteScalar(SqlConnection conn,SqlCommand cmd)
        {
            try
            {
                conn.Open();
                return cmd.ExecuteScalar();
            }
            catch (Exception ex)
            {
                WritterError("执行GetSingleResult(string sql)时出现异常:", ex.Message);
                throw ex;
            }
            finally
            {
                if (conn != null)
                    conn.Close();
            }
        }
    }
}
