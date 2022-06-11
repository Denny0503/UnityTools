using System.Windows.Controls;

namespace WaterMarkTools.Views
{
    /// <summary>
    /// WaterMark.xaml 的交互逻辑
    /// </summary>
    public partial class WaterMark : UserControl
    {
        public WaterMark()
        {
            InitializeComponent();
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            InfoScrollViewer.ScrollToEnd();
        }
    }
}
