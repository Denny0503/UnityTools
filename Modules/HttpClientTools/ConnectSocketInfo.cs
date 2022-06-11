using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HttpClientTools
{
    /// <summary>
    /// 连接的客户端信息
    /// </summary>
    public class ConnectSocketInfo
    {
        public int ID { set; get; }

        public IntPtr ConnectID { set; get; }

        public IntPtr ClientID { set; get; }

        /// <summary>
        /// 连接时间
        /// </summary>
        public DateTime ConnectTime { set; get; }
    }
}
