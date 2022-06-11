using TopUIControl.Controls;

namespace HttpClientTools.Views
{
    /// <summary>
    /// WebClientTools.xaml 的交互逻辑
    /// </summary>
    public partial class WebClientTools : RegionViewControl
    {
        public WebClientTools()
        {
            InitializeComponent();

            this.ItemHeader = "WebClientTools";
            this.ViewDataContext = this;
        }
    }
}
