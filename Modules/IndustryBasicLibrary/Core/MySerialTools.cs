using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IndustryBasicLibrary.Commons;

namespace IndustryBasicLibrary.Core
{
    public class MySerialTools
    {
        /// <summary>
        /// 串口句柄
        /// </summary>
        public SerialPort GlobalSerialPort = null;

        public MySerialTools()
        {
            GlobalSerialPort = new SerialPort();
        }

        public bool InitSerialPort(SerialInfo serialInfo, out string error)
        {
            error = "";

            //如果当前串口已打开，则关闭
            if (null != GlobalSerialPort && GlobalSerialPort.IsOpen)
            {
                GlobalSerialPort.Close();
            }

            if (null == GlobalSerialPort || !GlobalSerialPort.IsOpen)
            {
                try
                {
                    GlobalSerialPort.PortName = serialInfo.SerialName;
                    GlobalSerialPort.BaudRate = serialInfo.iBaudRate;   /*波特率*/
                    GlobalSerialPort.DataBits = serialInfo.iDateBits;   /*数据位*/
                    GlobalSerialPort.StopBits = serialInfo.StopBits;    /*停止位*/
                    GlobalSerialPort.Parity = serialInfo.Parity;        /*校验位*/

                    GlobalSerialPort.ErrorReceived += GlobalSerialPort_ErrorReceived;
                    GlobalSerialPort.PinChanged += GlobalSerialPort_PinChanged;
                    GlobalSerialPort.DataReceived += new SerialDataReceivedEventHandler(GlobalSerialPort_DataReceived);
                    GlobalSerialPort.Open();     //打开串口
                    return true;
                }
                catch (Exception ex)
                {
                    error = ex.Message;
                    return false;
                }
            }

            return false;
        }

        #region 串口处理

        /// <summary>
        /// 非数据信号事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GlobalSerialPort_PinChanged(object sender, SerialPinChangedEventArgs e)
        {

        }

        /// <summary>
        /// 发生错误
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GlobalSerialPort_ErrorReceived(object sender, SerialErrorReceivedEventArgs e)
        {

        }

        /// <summary>
        /// 接收串口数据
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GlobalSerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            //创建接收字节数组
            Byte[] rData = new Byte[GlobalSerialPort.BytesToRead];
            //读取数据
            GlobalSerialPort.Read(rData, 0, rData.Length);
        }

        #endregion

        /// <summary>
        /// 发送数据
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public bool Send(byte[] data)
        {
            if (null != GlobalSerialPort && GlobalSerialPort.IsOpen)
            {
                try
                {
                    GlobalSerialPort.Write(data, 0, data.Length);
                    return true;
                }
                catch (Exception ex)
                {
                    return false;
                }
            }
            return false;
        }

    }
}
