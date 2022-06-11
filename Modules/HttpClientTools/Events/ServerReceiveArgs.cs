using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebSocketSharp;

namespace HttpClientTools.Events
{
    public class ServerReceiveArgs : EventArgs
    {
        /// <summary>
        /// 连接ID
        /// </summary>
        public string ID { set; get; }

        /// <summary>
        /// 连接用户名
        /// </summary>
        public string Name { set; get; }

        public MessageEventArgs Message { set; get; }
    }
}
