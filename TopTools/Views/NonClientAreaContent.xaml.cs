using System.Diagnostics;
using System.Windows;

namespace TopTools.Views
{
    /// <summary>
    /// NonClientAreaContent.xaml 的交互逻辑
    /// </summary>
    public partial class NonClientAreaContent
    {
        public NonClientAreaContent(Window main)
        {
            InitializeComponent();
        }

        private void ButtonConfig_OnClick(object sender, RoutedEventArgs e)
        {

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Process.Start(new ProcessStartInfo("cmd", $@"/c start https://gitee.com/denny0503/TopTools.git")
            {
                UseShellExecute = false,
                CreateNoWindow = true
            });
        }
    }
}
