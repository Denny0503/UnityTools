using Prism.Events;

namespace PhoenixControl.Events
{
    /// <summary>
    /// 消息类型
    /// </summary>
    public enum MessageType
    {
        /// <summary>
        /// 显示在状态栏
        /// </summary>
        ShowOnStatusBar,
        /// <summary>
        /// 面包板弹框显示
        /// </summary>
        ShowOnToast
    }

    /// <summary>
    /// 消息详细信息
    /// </summary>
    public struct MessageInfo
    {
        /// <summary>
        /// 消息类型
        /// </summary>
        public MessageType MsgType { set; get; }

        /// <summary>
        /// 消息内容
        /// </summary>
        public string Message { set; get; }
    }

    /// <summary>
    /// 消息通知事件
    /// </summary>
    public class MessagesNotificationEvent : PubSubEvent<MessageInfo>
    {

    }
}
