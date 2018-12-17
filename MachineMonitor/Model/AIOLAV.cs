
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Models
{
   public class AIOLAV
    {
        /// <summary>
        /// 铭板号
        /// </summary>
        public string sn { get; set; }
        /// <summary>
        /// 压头号
        /// </summary>
        public string indenter { get; set; }
        /// <summary>
        /// 平均值
        /// </summary>
        public double agvValue { get; set; }
        /// <summary>
        /// 极值
        /// </summary>
        public double linmitVal { get; set; }
    }
}
