using System;
using System.Linq;
using HttpClientTools.Behaviors;
using HttpClientTools.Events;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using UnityControl.Events;
using UnityMethods.Extend;
using WebSocketSharp;
using WebSocketSharp.Server;
using UnityControl.Prism;

namespace HttpClientTools.ViewModels
{
    public class WebSocketControlViewModel : BaseViewModel
    {

        #region WebSocket服务器

        #region 属性

        private bool _IsServerStart;
        /// <summary>
        /// 是否启动服务
        /// </summary>
        public bool IsServerStart
        {
            get { return WebSocketServer?.IsListening ?? false; }
            set { _IsServerStart = value; }
        }

        #endregion

        /// <summary>
        /// 监听服务
        /// </summary>
        private WebSocketServer WebSocketServer { set; get; }

        public string WebSocketServerUrl { set; get; } = "0.0.0.0:8080";

        private string _showReceivedData;
        public string ShowReceivedData
        {
            get { return _showReceivedData; }
            set
            {
                value = $"{ShowReceivedData}\r\n{value}";

                SetProperty(ref _showReceivedData, value);
            }
        }

        public string WebSocketStatus { set; get; }

        /// <summary>
        /// 服务器发送内容
        /// </summary>
        public string ServerSendText { set; get; }

        public string BtnServerStatus { set; get; } = "启动服务";

        #endregion

        #region Websocket客户端

        private WebSocket WebSocketClient;

        private bool _IsClientConnect;
        public bool IsClientConnect
        {
            get { return WebSocketClient?.IsAlive ?? false; }
            set { _IsClientConnect = value; }
        }

        /// <summary>
        /// 客户端连接服务器地址
        /// </summary>
        public string ClientConnectUrl { set; get; } = "127.0.0.1:8080/Chat";

        /// <summary>
        /// 客户端连接状态
        /// </summary>
        public string ConnectedStatus { set; get; }

        public string BtnClientConnect { set; get; } = "连接";

        private string _clientShowReceivedData;
        /// <summary>
        /// 显示客户端接收数据
        /// </summary>
        public string ClientShowReceivedData
        {
            get { return _clientShowReceivedData; }
            set
            {
                value = $"{ClientShowReceivedData}\r\n{value}";
                SetProperty(ref _clientShowReceivedData, value);
            }
        }

        /// <summary>
        /// 客户端发送数据
        /// </summary>
        public string ClientSendText { set; get; }

        private void ClientConnectServer()
        {
            if (!IsClientConnect)
            {
                WebSocketClient = new WebSocket($"ws://{ClientConnectUrl}");

                WebSocketClient.OnMessage += WebSocketClient_OnMessage;
                WebSocketClient.OnOpen += WebSocketClient_OnOpen;
                WebSocketClient.OnClose += WebSocketClient_OnClose;
                WebSocketClient.ConnectAsync();
            }
        }

        /// <summary>
        /// 断开服务器连接
        /// </summary>
        private void ClientDisConnect()
        {
            if (IsClientConnect)
            {
                WebSocketClient.Close();
            }
        }

        private void WebSocketClient_OnClose(object sender, CloseEventArgs e)
        {
            ShowStatusInfo("断开连接服务器！");

            ConnectedStatus = "断开连接服务器！";

            BtnClientConnect = "连接";
            IsClientConnect = false;
        }

        private void WebSocketClient_OnOpen(object sender, System.EventArgs e)
        {
            ShowStatusInfo("连接服务器成功！");

            ConnectedStatus = "连接服务器成功！";

            BtnClientConnect = "断开";
            IsClientConnect = true;
        }

        private void WebSocketClient_OnMessage(object sender, MessageEventArgs e)
        {
            ClientShowReceivedData = e.Data;
        }

        #endregion

        public DelegateCommand<string> BtnFunctionCommand { get; private set; }

        public WebSocketControlViewModel(IEventAggregator receivedEvent) : base(receivedEvent)
        {
            BtnFunctionCommand = new DelegateCommand<string>(BtnFunction);
        }

        /// <summary>
        /// 显示提示信息
        /// </summary>
        /// <param name="status"></param>
        private void ShowStatusInfo(string status)
        {
            this.EventAggregator.GetEvent<MessagesNotificationEvent>().Publish(new MessageInfo() { MsgType = MessageType.ShowOnToast, Message = status });
        }

        private void BtnFunction(string parameter)
        {
            switch (parameter)
            {
                case "StartWebSocketServer":
                    StartWebSocketServer();
                    break;
                case "ConnectServer":
                    if (!IsClientConnect)
                    {
                        ClientConnectServer();
                    }
                    else
                    {
                        ClientDisConnect();
                    }
                    break;
                case "ServerSend":
                    if (!ServerSendText.IsEmpty())
                    {
                        WebSocketServer.WebSocketServices.Broadcast(ServerSendText);
                        ServerSendText = "";
                    }
                    else
                    {
                        ShowStatusInfo("请输入内容！");
                    }
                    break;
                case "ClientSend":
                    if (!ClientSendText.IsEmpty() && IsClientConnect)
                    {
                        WebSocketClient?.Send(ClientSendText);
                        ClientSendText = "";
                    }
                    else
                    {
                        ShowStatusInfo("未连接服务器或请输入内容！");
                    }
                    break;
            }
        }

        private void StartWebSocketServer()
        {
            if (!IsServerStart)
            {
                try
                {
                    WebSocketServer = new WebSocketServer($"ws://{WebSocketServerUrl}");

                    WebSocketServer.AddWebSocketService("/Chat", () =>
                    {
                        ServerChat serverChat = new ServerChat();

                        serverChat.OnMessageEvent += OnMessageEvent;

                        return serverChat;
                    });

                    WebSocketServer.Start();

                    WebSocketStatus = $@"服务器监听地址：ws://{WebSocketServer.Address}:{WebSocketServer.Port}{WebSocketServer.WebSocketServices.Paths.ToList<string>()[0]}";

                    BtnServerStatus = "关闭服务";

                    IsServerStart = true;
                }
                catch (Exception ex)
                {
                    ShowReceivedData = ex.Message;
                    ShowStatusInfo(ex.Message);
                }
            }
            else
            {
                WebSocketServer.Stop();
                BtnServerStatus = "启动服务";
            }
        }

        private void OnMessageEvent(object sender, ServerReceiveArgs args)
        {
            ShowReceivedData = $"ID：{args.ID}({args.Name}) ——>> {args.Message.Data}";
        }
    }
}
