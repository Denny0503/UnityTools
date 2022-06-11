using System;
using System.Collections.Generic;
using System.Linq;
using HttpClientTools.Events;
using WebSocketSharp;
using WebSocketSharp.Server;

namespace HttpClientTools.Behaviors
{
    public class ServerChat : WebSocketBehavior
    {
        public EventHandler<ServerReceiveArgs> OnMessageEvent;

        /// <summary>
        /// 连接用户名
        /// </summary>
        public string ConnectName { private set; get; }

        /// <summary>
        /// 连接ID
        /// </summary>
        public string ConnectID { private set; get; }

        public ServerChat() { }

        private string GetName()
        {
            return Context.QueryString["name"];
        }

        protected override void OnOpen()
        {
            ConnectName = GetName();

            List<string> ids = Sessions.IDs.ToList<string>();

            ConnectID = this.ID;
        }

        protected override void OnClose(CloseEventArgs e)
        {
            base.OnClose(e);
        }

        protected override void OnError(ErrorEventArgs e)
        {
            base.OnError(e);
        }

        protected override void OnMessage(MessageEventArgs e)
        {
            //Sessions.Broadcast(String.Format("{0}: {1}", _name, e.Data));

            OnMessageEvent?.Invoke(this, new ServerReceiveArgs()
            {
                ID = ConnectID,
                Name = ConnectName,
                Message = e
            });
        }
    }
}
