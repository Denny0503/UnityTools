using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using TopMethods.Extend;
using TopMethods.Logs;

namespace CommonMethods.NetSockets
{
    public enum SocketEventType
    {
        /// <summary>
        /// 发生错误
        /// </summary>
        OnError,
        /// <summary>
        /// 启动服务器成功
        /// </summary>
        OnOpen,
        /// <summary>
        /// 关闭连接
        /// </summary>
        OnClose,
        /// <summary>
        /// 接收到消息
        /// </summary>
        OnMessage,
        /// <summary>
        /// 客户端连接
        /// </summary>
        OnConnect,
        /// <summary>
        /// 客户端断开
        /// </summary>
        OnDisConnect,
    }


    public class NetSocketMsgEvargs : EventArgs
    {
        public string UUID { set; get; }

        public string Message { set; get; }

        public SocketEventType EventType { set; get; }

        /// <summary>
        /// 远端IP地址
        /// </summary>
        public string RemoteIPAddress { set; get; }
    }

    public class SocketHelper
    {
        public class StateObject
        {
            public string UUID { set; get; }
            /// <summary>
            /// socket
            /// </summary>
            public Socket WorkSocket { set; get; }
            /// <summary>
            /// 接收数据缓冲大小
            /// </summary>
            public const int BufferSize = 4096;
            /// <summary>
            ///接收数据缓冲区
            /// </summary>
            public byte[] Buffer = new byte[BufferSize];
        }

        private Dictionary<string, StateObject> SocketClientDict = new Dictionary<string, StateObject>();

        public event EventHandler<NetSocketMsgEvargs> NetSocketMsgEvent;

        private void ShowEvent(NetSocketMsgEvargs msgEvargs)
        {
            NetSocketMsgEvent?.Invoke(this, msgEvargs);
        }

        private void AddClient(string uuid, StateObject state)
        {
            if (!SocketClientDict.ContainsKey(uuid))
            {
                SocketClientDict.Add(uuid, state);
            }
        }

        private bool IsExistSocket(string uuid)
        {
            return uuid.IsEmpty() && SocketClientDict.ContainsKey(uuid);
        }

        public bool StartServer_TCP(string ip, int port)
        {
            if (IPAddress.TryParse(ip, out IPAddress iPAddress))
            {
                IPEndPoint endPoint = new IPEndPoint(iPAddress, port);

                try
                {
                    // 创建负责监听的套接字，注意其中的参数；  
                    Socket listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                    listener.Bind(endPoint);

                    // 设置监听队列的长度；  
                    listener.Listen(10);

                    StateObject stateObject = new StateObject
                    {
                        WorkSocket = listener,
                        UUID = Ext.GetTimeStampMilliseconds().ToMD5_32()
                    };
                    AddClient(stateObject.UUID, stateObject);

                    ShowEvent(new NetSocketMsgEvargs()
                    {
                        EventType = SocketEventType.OnOpen,
                        UUID = stateObject.UUID,
                    });

                    listener.BeginAccept(new AsyncCallback(AcceptCallback), stateObject);

                }
                catch (Exception ex)
                {
                    return false;
                }

                return true;
            }
            else
            {
                return false;
            }

        }

        #region UDP

        public bool Start_UDP(string ip, int port, out string uuid)
        {
            uuid = "";

            if (IsPortInUse(port, ProtocolType.Udp))
            {
                return false;
            }

            IPEndPoint endPoint = new IPEndPoint(IPAddress.Parse(ip), port);

            try
            {
                // 创建负责监听的套接字，注意其中的参数；  
                Socket listener = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

                StateObject stateObject = new StateObject
                {
                    WorkSocket = listener,
                    UUID = Ext.GetTimeStampMilliseconds().ToMD5_32(),
                };
                AddClient(stateObject.UUID, stateObject);

                uuid = stateObject.UUID;

                stateObject.WorkSocket.Bind(endPoint);

                ShowEvent(new NetSocketMsgEvargs()
                {
                    EventType = SocketEventType.OnOpen,
                    UUID = stateObject.UUID,
                });

                EndPoint remoteEndPoint = new IPEndPoint(0, 0);
                stateObject.WorkSocket.BeginReceiveFrom(stateObject.Buffer, 0, stateObject.Buffer.Length, 0, ref remoteEndPoint, new AsyncCallback(ReceiveFromCallback), stateObject);

            }
            catch (Exception ex)
            {
                LogFactory.SystemLog($"{new System.Diagnostics.StackTrace().GetFrame(0).GetMethod().Name}，Exception：{ex.Message}", LogLevelType.Exception, ex);
                return false;
            }

            return false;
        }

        private void ReceiveFromCallback(IAsyncResult ar)
        {
            try
            {
                StateObject state = (StateObject)ar.AsyncState;
                if (null != state.WorkSocket && state.WorkSocket.IsBound)
                {
                    EndPoint remoteEndPoint = new IPEndPoint(0, 0);
                    int bytesRead = state.WorkSocket.EndReceiveFrom(ar, ref remoteEndPoint);

                    if (bytesRead > 0)
                    {
                        ShowEvent(new NetSocketMsgEvargs()
                        {
                            EventType = SocketEventType.OnMessage,
                            UUID = state.UUID,
                            Message = Encoding.UTF8.GetString(state.Buffer).Trim(),
                            RemoteIPAddress = remoteEndPoint.ToString()
                        });
                    }

                    Array.Clear(state.Buffer, 0, state.Buffer.Length);
                    state.WorkSocket.BeginReceiveFrom(state.Buffer, 0, state.Buffer.Length, 0, ref remoteEndPoint, new AsyncCallback(ReceiveFromCallback), state);
                }
            }
            catch (Exception ex)
            {
                LogFactory.SystemLog($"{new System.Diagnostics.StackTrace().GetFrame(0).GetMethod().Name}，Exception：{ex.Message}", LogLevelType.Exception, ex);
            }

        }

        public bool UdpSend(string uuid, string message, string ip, int port)
        {
            if (!uuid.IsEmpty() && SocketClientDict.ContainsKey(uuid))
            {
                try
                {
                    EndPoint point = new IPEndPoint(IPAddress.Parse(ip), port);

                    SocketClientDict[uuid].Buffer = Encoding.UTF8.GetBytes(message);

                    SocketClientDict[uuid].WorkSocket.SendTo(SocketClientDict[uuid].Buffer, 0, SocketClientDict[uuid].Buffer.Count(), 0, point);
                }
                catch (Exception ex)
                {
                    LogFactory.SystemLog($"{new System.Diagnostics.StackTrace().GetFrame(0).GetMethod().Name}，Exception：{ex.Message}", LogLevelType.Exception, ex);
                    return false;
                }

                return true;
            }

            return false;
        }

        public bool CloseUdp(string uuid)
        {
            if (!uuid.IsEmpty() && SocketClientDict.ContainsKey(uuid))
            {
                SocketClientDict[uuid].WorkSocket.Close();

                SocketClientDict.Remove(uuid);

                ShowEvent(new NetSocketMsgEvargs()
                {
                    EventType = SocketEventType.OnClose,
                    UUID = uuid,
                });
            }

            return IsExistSocket(uuid);
        }

        #endregion

        /// <summary>
        /// 等待客户端连接
        /// </summary>
        /// <param name="ar"></param>
        private void AcceptCallback(IAsyncResult ar)
        {
            try
            {
                StateObject state = (StateObject)ar.AsyncState;
                Socket client = state.WorkSocket.EndAccept(ar);

                IPEndPoint remoteEndPoint;
                remoteEndPoint = (IPEndPoint)client.RemoteEndPoint;

                StateObject clientState = new StateObject
                {
                    WorkSocket = client,
                    UUID = Ext.GetTimeStampMilliseconds().ToMD5_32()
                };
                AddClient(clientState.UUID, clientState);

                ShowEvent(new NetSocketMsgEvargs()
                {
                    EventType = SocketEventType.OnConnect,
                    UUID = clientState.UUID,
                });

                client.BeginReceive(clientState.Buffer, 0, clientState.Buffer.Length, 0, new AsyncCallback(ReadCallback), clientState);

                state.WorkSocket.BeginAccept(new AsyncCallback(AcceptCallback), state);
            }
            catch (Exception ex)
            {
                LogFactory.SystemLog($"{new System.Diagnostics.StackTrace().GetFrame(0).GetMethod().Name}，Exception：{ex.Message}", LogLevelType.Exception, ex);
            }
        }

        /// <summary>
        /// 读取客户端发送数据
        /// </summary>
        /// <param name="ar"></param>
        private void ReadCallback(IAsyncResult ar)
        {
            try
            {
                StateObject state = (StateObject)ar.AsyncState;

                int bytesRead = state.WorkSocket.EndReceive(ar);
                if (bytesRead > 0)
                {
                    ShowEvent(new NetSocketMsgEvargs()
                    {
                        EventType = SocketEventType.OnMessage,
                        UUID = state.UUID,
                        Message = Encoding.UTF8.GetString(state.Buffer).Trim()
                    });

                    Array.Clear(state.Buffer, 0, state.Buffer.Length);

                    state.WorkSocket.BeginReceive(state.Buffer, 0, state.Buffer.Length, 0, new AsyncCallback(ReadCallback), state);
                }

            }
            catch (Exception ex)
            {
                LogFactory.SystemLog($"{new System.Diagnostics.StackTrace().GetFrame(0).GetMethod().Name}，Exception：{ex.Message}", LogLevelType.Exception, ex);
            }
        }

        private void SendCallback(IAsyncResult ar)
        {
            try
            {
                Socket handler = (Socket)ar.AsyncState;
                int bytesSent = handler.EndSend(ar);
            }
            catch (Exception ex)
            {
                LogFactory.SystemLog($"{new System.Diagnostics.StackTrace().GetFrame(0).GetMethod().Name}，Exception：{ex.Message}", LogLevelType.Exception, ex);
            }
        }


        public bool Send(string uuid, string message)
        {
            if (SocketClientDict.ContainsKey(uuid))
            {
                SocketClientDict[uuid].Buffer = Encoding.UTF8.GetBytes(message);

                SocketClientDict[uuid].WorkSocket.BeginSend(SocketClientDict[uuid].Buffer, 0, SocketClientDict[uuid].Buffer.Count(), 0, new AsyncCallback(SendCallback), SocketClientDict[uuid].WorkSocket);

                return true;
            }
            else
            {
                return false;
            }
        }

        #region 指定类型的端口是否已经被使用

        /// <summary>
        /// 指定类型的端口是否已经被使用了
        /// </summary>
        /// <param name="port">端口号</param>
        /// <param name="type">端口类型</param>
        /// <returns></returns>
        private bool IsPortInUse(int port, ProtocolType type)
        {
            bool flag = false;

            IPGlobalProperties properties = IPGlobalProperties.GetIPGlobalProperties();
            IPEndPoint[] ipendpoints;

            if (type == ProtocolType.Tcp)
            {
                ipendpoints = properties.GetActiveTcpListeners();
            }
            else
            {
                ipendpoints = properties.GetActiveUdpListeners();
            }

            if (null != ipendpoints && ipendpoints.Count() > 0)
            {
                foreach (IPEndPoint ipendpoint in ipendpoints)
                {
                    if (ipendpoint.Port == port)
                    {
                        flag = true;
                        break;
                    }
                }
            }

            return flag;
        }

        #endregion
    }
}
