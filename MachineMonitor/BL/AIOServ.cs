using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Models;
using DAL;
using System.Data;

namespace BLL
{
    public class AIOServ
    {
        public AioService aioServ = new AioService();
        private static readonly object thislock = new object();
        public List<object> GetCGLNg(DateTime startDate, DateTime endDate, string machineNo)
        {
            lock (thislock)
            {
                List<object> list = new List<object>();
                AIORAA aIORAAa = new AIORAA();
                List<AIOTOA> aIOTOAs = new List<AIOTOA>();
                //List<AIORAA> aIORAAs = aioServ.GetCGLNg(startDate, endDate);
                List<AIORAA> aIORAAs = aioServ.GetCGLNgInfoByProc(startDate, endDate);

                //根据设备编号筛选结果集
                aIORAAs = (from n in aIORAAs
                           where n.machineNo != null && n.machineNo.Contains(machineNo)
                           select n).ToList();
                list.Add(aIORAAs);
                var objRaa1 = aIORAAs.GroupBy(p => new
                {
                    p.machineNo,
                    p.indenter
                }).ToList();
                foreach (var item in objRaa1)
                {
                    aIOTOAs.Add(new AIOTOA()
                    {
                        MachineNo = item.Key.machineNo,
                        intender = item.Key.indenter,
                        Quty = item.Count()
                    });
                }
                list.Add(aIOTOAs);
                return list;
            }            
        }

        /// <summary>
        /// 获取设备类型
        /// </summary>
        /// <returns></returns>
        public DataRow[] GetMachineNo(string machineNo,string machineType)
        {
            DataTable dt = aioServ.GetMachineNo(machineNo);
            DataRow[] dataRows = dt.Select();
            string filterStr = "1=1 ";
            if (machineNo.Length > 0)
            {
                filterStr += "AND AIO001 like '%" + machineNo + "%'";
            }
            if (machineType.Length > 0)
            {
                filterStr += " AND AIO002='"+machineType+"'";
            }
            dataRows = dt.Select(filterStr);
            return dataRows;
        }
        /// <summary>
        /// 获取CGL倾向性
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <param name="machineNo"></param>
        /// <returns></returns>
        public List<T> GetCGLQXX<T>(DateTime startDate,DateTime endDate,string machineNo,out List<AIOLAV> aIOLAVs)
        {
            return aioServ.GetCGLQXX<T>(startDate, endDate, machineNo,out aIOLAVs);
        }
        
    }
}
