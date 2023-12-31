﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using UnityControl.Controls;

namespace HttpClientTools.Views
{
    /// <summary>
    /// TcpUdpToolsControl.xaml 的交互逻辑
    /// </summary>
    public partial class TcpUdpToolsControl : RegionViewControl
    {
        public TcpUdpToolsControl()
        {
            InitializeComponent();

            this.ItemHeader = "Tcp/Udp工具";
            this.ViewDataContext = this;
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            (sender as TextBox).ScrollToEnd();
        }
    }
}
