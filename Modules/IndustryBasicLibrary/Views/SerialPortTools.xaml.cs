using System;
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
using TopUIControl.Controls;

namespace IndustryBasicLibrary.Views
{
    /// <summary>
    /// SerialPortTools.xaml 的交互逻辑
    /// </summary>
    public partial class SerialPortTools : RegionViewControl
    {
        public SerialPortTools()
        {
            InitializeComponent();

            this.ItemHeader = "串口";
            this.ViewDataContext = this;
        }
    }
}
