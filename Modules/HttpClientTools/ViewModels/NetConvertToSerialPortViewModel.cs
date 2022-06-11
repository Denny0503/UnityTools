using HPSocket;
using HPSocket.Tcp;
using HPSocket.Udp;
using Prism.Commands;
using Prism.Events;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TopMethods.Extend;
using TopUIControl;
using TopUIControl.Prism;

namespace HttpClientTools.ViewModels
{
    class NetConvertToSerialPortViewModel : BaseViewModel
    {
        IServer GlobalServer;

        IClient GlobalClient;

        private List<ConnectSocketInfo> ConnectClientList = new List<ConnectSocketInfo>();

        #region 属性

        /// <summary>
        /// 服务器类型
        /// </summary>
        public ObservableCollection<ComboBoxDataBing> ServerTypeList { set; get; } = new ObservableCollection<ComboBoxDataBing>();

        /// <summary>
        /// 本地IP地址列表
        /// </summary>
        public ObservableCollection<ComboBoxDataBing> LocalIPAddressList { set; get; } = new ObservableCollection<ComboBoxDataBing>();

        /// <summary>
        /// 客户端连接IP列表
        /// </summary>
        public ObservableCollection<ComboBoxDataBing> ConnectClientIPList { set; get; } = new ObservableCollection<ComboBoxDataBing>();

        /// <summary>
        /// 选择的IP地址
        /// </summary>
        public string SelectServerIP { set; get; }

        /// <summary>
        /// 选中的客户端ID
        /// </summary>
        public string SelectConnectID { set; get; }

        /// <summary>
        /// 端口号
        /// </summary>
        public ushort ServerPort { set; get; } = 5000;

        private string _SeverTypeSelectValue;
        /// <summary>
        /// 服务器类型
        /// </summary>
        public string SeverTypeSelectValue
        {
            set
            {
                SetProperty(ref _SeverTypeSelectValue, value);

                Enum.TryParse(value, out ServerType serverType);

                SelectedServerType = serverType;
            }
            get
            {
                return _SeverTypeSelectValue;
            }
        }

        /// <summary>
        /// 服务器类型
        /// </summary>
        public ServerType SelectedServerType { set; get; }

        /// <summary>
        /// 服务器开关
        /// </summary>
        public bool BtnServerChecked { set; get; }

        /// <summary>
        /// 是否以16进制显示
        /// </summary>
        public bool IsShowAsHEX { set; get; }

        /// <summary>
        /// 待发送数据
        /// </summary>
        public string SendMessageInfo { set; get; }

        private string _ShowMessageInfo;
        /// <summary>
        /// 数据显示
        /// </summary>
        public string ShowMessageInfo { set { _ShowMessageInfo += value + "\r\n"; } get { return _ShowMessageInfo; } }

        public DelegateCommand BtnServerCommand => new Lazy<DelegateCommand>(() => new DelegateCommand(BtnServerCommandFunc)).Value;

        public DelegateCommand BtnSendCmd => new Lazy<DelegateCommand>(() => new DelegateCommand(BtnSendCmdFunc)).Value;

        #endregion

        public NetConvertToSerialPortViewModel(RegionManager regionManager, IEventAggregator receivedEvent) : base(regionManager, receivedEvent)
        {
            ConnectClientIPList.Add(new ComboBoxDataBing() { SelectValue = "", Name = "所有IP" });

            ServerTypeList.Add(new ComboBoxDataBing() { SelectValue = ServerType.TCP_Server.ToString(), Name = "TCP服务器" });
            ServerTypeList.Add(new ComboBoxDataBing() { SelectValue = ServerType.TCP_Client.ToString(), Name = "TCP客户端" });
            ServerTypeList.Add(new ComboBoxDataBing() { SelectValue = ServerType.UDP_Server.ToString(), Name = "UDP服务器" });
            ServerTypeList.Add(new ComboBoxDataBing() { SelectValue = ServerType.UDP_Client.ToString(), Name = "UDP客户端" });

            /*本地IP地址列表*/
            List<string> ipList = Ext.GetLocalIPList();

            if (ipList.Count > 0)
            {
                int i = 0;
                ipList.ForEach(ch =>
                {
                    LocalIPAddressList.Add(new ComboBoxDataBing() { ID = ++i, Name = ch });
                });
            }
        }

        private void BtnSendCmdFunc()
        {
            switch (SelectedServerType)
            {
                case ServerType.TCP_Client:
                case ServerType.UDP_Client:
                    if (GlobalClient != null)
                    {
                        byte[] send = Encoding.UTF8.GetBytes(SendMessageInfo);
                        GlobalClient.Send(send, send.Length);
                    }
                    break;
                case ServerType.TCP_Server:
                case ServerType.UDP_Server:
                    if (GlobalServer != null)
                    {
                        byte[] send = Encoding.UTF8.GetBytes(SendMessageInfo);

                        ConnectClientList.ForEach(ch =>
                        {
                            GlobalServer.Send(ch.ConnectID, send, send.Length);
                        });

                    }
                    break;
            }

        }

        private void BtnServerCommandFunc()
        {
            if (BtnServerChecked)
            {
                switch (SelectedServerType)
                {
                    case ServerType.UDP_Server:
                        GlobalServer = new UdpServer();
                        break;
                    case ServerType.UDP_Client:
                        GlobalClient = new UdpClient();
                        break;
                    case ServerType.TCP_Client:
                        GlobalClient = new TcpClient();
                        break;
                    case ServerType.TCP_Server:
                        GlobalServer = new TcpServer();
                        break;
                }

                switch (SelectedServerType)
                {
                    case ServerType.TCP_Client:
                    case ServerType.UDP_Client:
                        if (GlobalClient != null)
                        {
                            GlobalClient.OnClose += GlobalClient_OnClose;
                            GlobalClient.OnConnect += GlobalClient_OnConnect;
                            GlobalClient.OnReceive += GlobalClient_OnReceive;
                            GlobalClient.OnSend += GlobalClient_OnSend;

                            GlobalClient.Address = SelectServerIP;
                            GlobalClient.Port = ServerPort;

                            GlobalClient.Connect();
                        }
                        break;
                    case ServerType.TCP_Server:
                    case ServerType.UDP_Server:
                        if (GlobalServer != null)
                        {
                            GlobalServer.Address = SelectServerIP;
                            GlobalServer.Port = ServerPort;

                            GlobalServer.OnAccept += GlobalServer_OnAccept;
                            GlobalServer.OnClose += GlobalServer_OnClose;
                            GlobalServer.OnShutdown += GlobalServer_OnShutdown;
                            GlobalServer.OnReceive += GlobalServer_OnReceive;
                            GlobalServer.OnSend += GlobalServer_OnSend;
                            GlobalServer.OnPrepareListen += GlobalServer_OnPrepareListen;

                            GlobalServer.Start();
                        }
                        break;
                }

            }
            else
            {
                switch (SelectedServerType)
                {
                    case ServerType.TCP_Client:
                    case ServerType.UDP_Client:
                        if (GlobalClient != null)
                        {
                            GlobalClient.Stop();

                            GlobalClient.OnClose -= GlobalClient_OnClose;
                            GlobalClient.OnConnect -= GlobalClient_OnConnect;
                            GlobalClient.OnReceive -= GlobalClient_OnReceive;
                            GlobalClient.OnSend -= GlobalClient_OnSend;

                        }
                        break;
                    case ServerType.TCP_Server:
                    case ServerType.UDP_Server:
                        if (GlobalServer != null)
                        {
                            GlobalServer.Stop();

                            GlobalServer.OnAccept -= GlobalServer_OnAccept;
                            GlobalServer.OnClose -= GlobalServer_OnClose;
                            GlobalServer.OnShutdown -= GlobalServer_OnShutdown;
                            GlobalServer.OnReceive -= GlobalServer_OnReceive;
                            GlobalServer.OnSend -= GlobalServer_OnSend;
                            GlobalServer.OnPrepareListen -= GlobalServer_OnPrepareListen;
                        }
                        break;
                }
            }
        }

        #region 客户端事件

        private HandleResult GlobalClient_OnSend(IClient sender, byte[] data)
        {
            string rece;
            if (IsShowAsHEX)
            {
                rece = Ext.ToHEX(data);
            }
            else
            {
                rece = Encoding.UTF8.GetString(data);
            }

            ShowMessageInfo = $"发送数据：{rece} \t {DateTime.Now:yyyy-MM-dd HH:mm:ss}";

            return HandleResult.Ok;
        }

        private HandleResult GlobalClient_OnReceive(IClient sender, byte[] data)
        {
            string rece;
            if (IsShowAsHEX)
            {
                rece = Ext.ToHEX(data);
            }
            else
            {
                rece = Encoding.UTF8.GetString(data);
            }

            ShowMessageInfo = $"接收数据：{rece} \t {DateTime.Now:yyyy-MM-dd HH:mm:ss}";

            return HandleResult.Ok;
        }

        private HandleResult GlobalClient_OnConnect(IClient sender)
        {
            ShowMessageInfo = $"成功连接服务器! \t {DateTime.Now:yyyy-MM-dd HH:mm:ss}";
            return HandleResult.Ok;
        }

        private HandleResult GlobalClient_OnClose(IClient sender, SocketOperation socketOperation, int errorCode)
        {
            ShowMessageInfo = $"客户端断开! \t {DateTime.Now:yyyy-MM-dd HH:mm:ss}";
            return HandleResult.Ok;
        }

        #endregion

        #region 服务器事件

        private HandleResult GlobalServer_OnPrepareListen(IServer sender, IntPtr listen)
        {
            ShowMessageInfo = $"开启监听! \t {DateTime.Now:yyyy-MM-dd HH:mm:ss}";
            return HandleResult.Ok;
        }

        private HandleResult GlobalServer_OnReceive(IServer sender, IntPtr connId, byte[] data)
        {
            string rece;
            if (IsShowAsHEX)
            {
                rece = Ext.ToHEX(data);
            }
            else
            {
                rece = Encoding.UTF8.GetString(data);
            }

            ShowMessageInfo = $"接收数据：{rece} \t {DateTime.Now:yyyy-MM-dd HH:mm:ss}";

            return HandleResult.Ok;
        }

        private HandleResult GlobalServer_OnSend(IServer sender, IntPtr connId, byte[] data)
        {
            string rece;
            if (IsShowAsHEX)
            {
                rece = Ext.ToHEX(data);
            }
            else
            {
                rece = Encoding.UTF8.GetString(data);
            }

            ShowMessageInfo = $"发送数据：{rece} \t {DateTime.Now:yyyy-MM-dd HH:mm:ss}";

            return HandleResult.Ok;
        }

        private HandleResult GlobalServer_OnShutdown(IServer sender)
        {
            ShowMessageInfo = $"服务器关闭! \t {DateTime.Now:yyyy-MM-dd HH:mm:ss}";
            return HandleResult.Ok;
        }

        private HandleResult GlobalServer_OnClose(IServer sender, IntPtr connId, SocketOperation socketOperation, int errorCode)
        {
            ShowMessageInfo = $"客户端断开! \t {DateTime.Now:yyyy-MM-dd HH:mm:ss}";

            for (int i = 0; i < ConnectClientList.Count; i++)
            {
                if (ConnectClientList[i].ConnectID == connId)
                {
                    ConnectClientList.RemoveAt(i);
                    break;
                }
            }

            return HandleResult.Ok;
        }

        private HandleResult GlobalServer_OnAccept(IServer sender, IntPtr connId, IntPtr client)
        {
            ConnectSocketInfo connect = new ConnectSocketInfo()
            {
                ID = ConnectClientList.Count + 1,
                ConnectID = connId,
                ClientID = client,
                ConnectTime = DateTime.Now
            };
            ConnectClientList.Add(connect);

            ShowMessageInfo = $"客户端，ID：{connect.ID} 连接! \t {connect.ConnectTime:yyyy-MM-dd HH:mm:ss}";

            //ConnectClientIPList.Add(new ComboBoxDataBing() { ID = connect.ID, Name = $"连接：{connect.ID}" });

            return HandleResult.Ok;
        }

        #endregion
    }
}
