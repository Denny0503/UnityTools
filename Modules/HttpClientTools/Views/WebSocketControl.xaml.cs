using HandyControl.Tools;
using System.Windows.Controls;
using UnityControl.Controls;

namespace HttpClientTools.Views
{
    /// <summary>
    /// WebSocketControl.xaml 的交互逻辑
    /// </summary>
    public partial class WebSocketControl : RegionViewControl
    {
        public WebSocketControl()
        {
            InitializeComponent();

            this.ItemHeader = "WebSocket";
            this.ViewDataContext = this;
        }

        private void TextBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            ReceivedDataScrollViewer?.ScrollToBottom();
        }
    }
}
