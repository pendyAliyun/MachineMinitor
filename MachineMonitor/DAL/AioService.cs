using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Models;
using Common;
using System.Data.SqlClient;
using System.Data;
using System.Threading;

namespace DAL
{
    public delegate void GetDataFromDbEventHandler(int maxminum, int position);
    public class AioService
    {
        #region 构造初始化
        public event GetDataFromDbEventHandler ReadingDb;
        List<AIORAA> aIORAAs;

        public AioService()
        {

        }
        #endregion

        #region SQL语句获取CGL不良信息

        /// <summary>
        /// 获取CGL不良信息
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        public List<AIORAA> GetCGLNg(string startDate, string endDate)
        {
            StringBuilder sqlsb = new StringBuilder(@"select count(1) from shqt.FOOLPROOF_STATION_ALLINONE_NEW where STATION_NAME='All-In-One' and WORK_TIME between to_date('{0}','YYYY-MM-DD HH24:MI:SS') 
                   and to_date('{1}','YYYY-MM-DD HH24:MI:SS') and SENSOR_RESULT<>'OK'");
            sqlsb.Append(@"select sn,STATION_IP,SENSOR_RESULT,WORK_TIME,SPC_G_P1,SPC_F_P2,SPC_AS_P3,SPC_E_P4,SPC_D_P5,SPC_C_P6,SPC_B_P9,SPC_A_P7,SPC_H_P8 ");
            sqlsb.Append(@" from shqt.FOOLPROOF_STATION_ALLINONE_NEW where STATION_NAME='All-In-One' and WORK_TIME between to_date('{2}','YYYY-MM-DD HH24:MI:SS') 
                   and to_date('{3}','YYYY-MM-DD HH24:MI:SS') and SENSOR_RESULT<>'OK'");
            aIORAAs = new List<AIORAA>();
            string sql = string.Format(sqlsb.ToString(), startDate, endDate, startDate, endDate);
            OracleDataReader reader = null;
            try
            {
                using (reader = OraHelper.GetDataReader(string.Format(sqlsb.ToString(), startDate, endDate, startDate, endDate)))
                {
                    while (reader.Read())
                    {
                        //aIORAAs.Add(GetMachineInfo(new AIORAA()
                        //{
                        //    SN = reader["sn"].ToString(),
                        //    Work_Time = Convert.ToDateTime(reader["WORK_TIME"]),
                        //    SENSOR_RESULT = reader["SENSOR_RESULT"].ToString(),
                        //    STATION_IP = reader["STATION_IP"].ToString(),
                        //    SPC_G_P1 = Convert.ToDouble(reader["SPC_G_P1"]),
                        //    SPC_F_P2 = Convert.ToDouble(reader["SPC_F_P2"]),
                        //    SPC_AS_P3 = Convert.ToDouble(reader["SPC_AS_P3"]),
                        //    SPC_E_P4 = Convert.ToDouble(reader["SPC_E_P4"]),
                        //    SPC_D_P5 = Convert.ToDouble(reader["SPC_D_P5"]),
                        //    SPC_C_P6 = Convert.ToDouble(reader["SPC_C_P6"]),
                        //    SPC_B_P9 = Convert.ToDouble(reader["SPC_B_P9"]),
                        //    SPC_A_P7 = Convert.ToDouble(reader["SPC_A_P7"]),
                        //    SPC_H_P8 = Convert.ToDouble(reader["SPC_H_P8"])
                        //}));
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (!reader.IsClosed) reader.Close();
            }
            return aIORAAs;
        }
        #endregion

        #region 通过存储过程获取CGL不良信息及不良条数
        /// <summary>
        /// 通过存储过程获取CGL不良信息及不良条数
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        public List<AIORAA> GetCGLNgInfoByProc(DateTime startDate, DateTime endDate)
        {
            aIORAAs = new List<AIORAA>();
            int MaxNgCount = 0;
            sampleRaas = new List<AIORAA>();
            OracleParameter[] oracleParameters = new OracleParameter[]
            {
                    new OracleParameter("startDate",OracleDbType.Varchar2),
                    new OracleParameter("endDate",OracleDbType.Varchar2),
                    new OracleParameter("rs",OracleDbType.RefCursor,ParameterDirection.Output),
                    new OracleParameter("NGCOUNT",OracleDbType.Int32,ParameterDirection.Output)
            };
            oracleParameters[0].Value = startDate.GetDateTimeFormats()[99];
            oracleParameters[1].Value = endDate.GetDateTimeFormats()[99];
            DataSet dataSet = MemoryCacheUtil.GetOrAddCacheItem(startDate.ToShortDateString() + endDate.ToShortDateString(),
               () =>
               {
                   ReadingDb(100, 2);//进入数据库访问时初始化一个进度到窗口
                   return OraHelper.GetDataSet("pkg_aio.proc_GetCGL_NG_INFO", oracleParameters, CommandType.StoredProcedure);
               }, "cglNgInfo");
            aIORAAs = Util.ToList<AIORAA>(dataSet.Tables[0]);
            MaxNgCount = MemoryCacheUtil.GetOrAddCacheItem(startDate.ToShortDateString() + endDate.ToShortDateString() + "MaxNgCount",
              () => Convert.ToInt32(oracleParameters[3].Value.ToString()), "MaxNgCount");

            string snIn = "";
            //for (int i = 0; i < aIORAAs.Count; i++)
            //{
            //    snIn+= aIORAAs[i].SN.Substring(36, 17)+"'";
            //    aIORAAs[i] = GetMachineInfo(aIORAAs[i]);
            //    processCount++;
            //}
            int pageSize = 1000;
            int totalPage = MaxNgCount % pageSize == 0 ? MaxNgCount / pageSize : MaxNgCount / pageSize + 1;
            if (MemoryCacheUtil.GetCacheItem<object>(startDate.ToShortDateString() + endDate.ToShortDateString() + "completeCglNg") == null)
            {
                for (int i = 0; i < totalPage; i++)
                {
                    List<string> ss = (aIORAAs.Skip(i * pageSize).Take(pageSize)).Select(p => "'" + p.SN.Substring(36, 17) + "'").ToList();
                    snIn = string.Join(",", ss);
                    GetMachineInfo(snIn, aIORAAs);
                    ReadingDb(totalPage, i + 1);
                }
            }else 
            {
                ReadingDb(100, 100);//如果缓存中有数据直接将进度条置为 100%
            }
            sampleRaas = MemoryCacheUtil.GetOrAddCacheItem(startDate.ToShortDateString() + endDate.ToShortDateString() + "completeCglNg",
                () => { return sampleRaas; }, "completeCglNg");
            return sampleRaas;
        }

        List<AIORAA> sampleRaas = null;
        /// <summary>
        /// 获取不良对应的机台
        /// </summary>
        /// <param name="aIORAA"></param>
        /// <returns></returns>
        private void GetMachineInfo(string snIn, List<AIORAA> aIORAAs)
        {
            string sql = "SELECT LEFT(MAH003,17) AS MAH003,MAH002,MAH001 FROM dbo.CGLMAH(NOLOCK) WHERE LEFT(MAH003,17) IN ({0})";
            SqlParameter[] sqlParameters = new SqlParameter[]{
                new SqlParameter("@snIn",snIn)
            };
            sql = string.Format(sql, snIn);
            DataTable tableRaa = new DataTable();
            if (snIn.Length > 16) { tableRaa = SqlHelper.GetDataTable(String.Format(sql, snIn)); }
            //while (sqlDataReader.Read())
            //{
            //    aIORAA.machineNo = sqlDataReader["MAH002"].ToString();
            //    aIORAA.indenter = sqlDataReader["MAH001"].ToString();
            //}
            List<AIOMAH> aIOMAHs = Util.ToList<AIOMAH>(tableRaa);
            List<AIORAA> tempRaas = (from aioRaa in aIORAAs
                                     join mah in aIOMAHs on aioRaa.SN.Substring(36, 17) equals mah.MAH003
                                     select new AIORAA
                                     {
                                         SN = aioRaa.SN,
                                         machineNo = mah.MAH002,
                                         indenter = mah.MAH001,
                                         Work_Time = aioRaa.Work_Time,
                                         SENSOR_RESULT = aioRaa.SENSOR_RESULT,
                                         STATION_IP = aioRaa.STATION_IP,
                                         SPC_G_P1 = aioRaa.SPC_G_P1,
                                         SPC_F_P2 = aioRaa.SPC_F_P2,
                                         SPC_AS_P3 = aioRaa.SPC_AS_P3,
                                         SPC_E_P4 = aioRaa.SPC_E_P4,
                                         SPC_D_P5 = aioRaa.SPC_D_P5,
                                         SPC_C_P6 = aioRaa.SPC_C_P6,
                                         SPC_B_P9 = aioRaa.SPC_B_P9,
                                         SPC_A_P7 = aioRaa.SPC_A_P7,
                                         SPC_H_P8 = aioRaa.SPC_H_P8
                                     }).Where((p) => p.indenter != null).ToList();
            sampleRaas.AddRange(tempRaas);
        }
        /// <summary>
        /// 条件过滤设备
        /// </summary>
        /// <param name="machineNo"></param>
        /// <returns></returns>
        public DataTable GetMachineNo(string machineNo)
        {
            string sql = "SELECT AIO001,AIO002 FROM dbo.BASAIO";
            if (machineNo.Length > 0)
            {
                sql += " where AIO001 like '%" + machineNo + "%'";
            }
            DataTable dt = MemoryCacheUtil.GetOrAddCacheItem("machineNos",
                () => SqlHelper.GetDataTable(sql), "machineType.dt");
            return dt;
        }
        #endregion

        #region 获取设备倾向性数据
        /// <summary>
        /// 获取设备倾向性数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <param name="machineNo"></param>
        /// <returns></returns>
        public List<T> GetCGLQXX<T>(DateTime startDate, DateTime endDate, string machineNo, out List<AIOLAV> aIOLAVs)
        {
            SqlParameter[] sqlParameters = new SqlParameter[]
            {
                new SqlParameter("@startDate",startDate),
                new SqlParameter("@endDate",endDate),
                new SqlParameter("@machineNo",machineNo)
            };
            DataSet dataSet = MemoryCacheUtil.GetOrAddCacheItem((startDate.ToShortDateString() + endDate.ToShortDateString() + machineNo),
               () => SqlHelper.GetDataSetByProcedure("AIO_GetMachineTrend", sqlParameters), "QxxAndLimitAve");
            aIOLAVs = Util.ToList<AIOLAV>(dataSet.Tables[1]);
            return Util.ToList<T>(dataSet.Tables[0]);
        }
        #endregion
    }
}
