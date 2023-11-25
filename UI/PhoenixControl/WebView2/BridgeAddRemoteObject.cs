using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace PhoenixControl.WebView2
{
    [ClassInterface(ClassInterfaceType.AutoDual)]
    [ComVisible(true)]
    public class BridgeAddRemoteObject
    {

        public BridgeAddRemoteObject() { }
        public string Message { get; set; } = "Hello {0} from .NET. Time is: {1}";

        public string SayHello(string name)
        {
            string msg = string.Format(Message, name, DateTime.Now.ToString("t"));

            MessageBox.Show(msg, "WPF Message from JavaScript",
                        MessageBoxButton.OK, MessageBoxImage.Information);
            return msg;

        }
    }
}
