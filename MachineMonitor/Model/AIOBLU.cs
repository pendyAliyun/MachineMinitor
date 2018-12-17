using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    /// <summary>
    /// BLU取样模型
    /// </summary>
    public class AIOBLU
    {
        /// <summary>
        /// CGL:28位铭板号MAH003前17位与116位第37全开始后17位匹配
        /// BLU:36位KBA002与116位第55-90位36位匹配
        /// </summary>
        public string SN { get; set; }
        /// <summary>
        /// 生产日期
        /// </summary>
        public string work_date { get; set; }
        /// <summary>
        /// 设备编号
        /// </summary>
        public string machineNo { get; set; }
        /// <summary>
        /// 生产时段
        /// </summary>
        public int timeSlot { get; set; }
        /// <summary>
        /// 是否取到AIO的检测记录标记（0代表未取到，1代表已经取到）
        /// </summary>
        public int flag { get; set; }
    }
}
