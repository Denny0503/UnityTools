using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IndustryBasicLibrary.Commons
{
    class SerialPortSetting
    {
    }

    /// <summary>
    /// 串口信息
    /// </summary>
    public struct SerialInfo
    {
        /// <summary>
        /// 串口名称
        /// </summary>
        public string SerialName;
        /// <summary>
        /// 波特率
        /// </summary>
        public Int32 iBaudRate;
        /// <summary>
        /// 数据位
        /// </summary>
        public Int32 iDateBits;
        /// <summary>
        /// 停止位
        /// </summary>
        public StopBits StopBits;
        /// <summary>
        /// 校验位
        /// </summary>
        public Parity Parity;
    }
}
