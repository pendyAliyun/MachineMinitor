using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    /// <summary>
    /// CGL取样模型
    /// </summary>
    public class AIOCGL:AIOBLU
    {
        /// <summary>
        /// 压头
        /// </summary>
        public string indenter { get; set; }
        public double SPC_G_P1 { get; set; }
        public double SPC_F_P2 { get; set; }
        public double SPC_AS_P3 { get; set; }
        public double SPC_E_P4 { get; set; }
        public double SPC_D_P5 { get; set; }
        public double SPC_C_P6 { get; set; }
        public double SPC_B_P9 { get; set; }
        public double SPC_A_P7 { get; set; }
        public double SPC_H_P8 { get; set; }

    }

    /// <summary>
    /// 设备分类汇总
    /// </summary>
    public class AIOTOA
    {
        public string Work_Time { get; set; }
        public string MachineNo{ get; set; }
        public string intender { get; set; }
        public int Quty { get; set; }
    }
}
