using System;
using System.Windows;
using System.Windows.Controls;

namespace UnityTools.Views
{
    /// <summary>
    /// Readme.xaml 的交互逻辑
    /// </summary>
    public partial class Readme : UserControl
    {

        public Readme()
        {
            InitializeComponent();
        }

        private async void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            await WebView2Browser.InitializeAsync();

            WebView2Browser.Address = $@"https://gitee.com/denny0503/UnityTools/wikis";
        }
    }
}
