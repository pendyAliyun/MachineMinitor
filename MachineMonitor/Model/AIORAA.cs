using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    /// <summary>
    /// AIO检测结果模型
    /// </summary>
   public class AIORAA:AIOCGL
    {
        public string SN { get; set; }
        public DateTime Work_Time { get; set; }
        public string SENSOR_RESULT { get; set; }
        public string STATION_IP { get; set; }
        public string STATION_NAME { get; set; }

    }

}
