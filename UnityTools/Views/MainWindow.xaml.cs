using HandyControl.Controls;
using UnityControl.Events;
using Prism.Events;
using Prism.Regions;
using System;
using System.Windows.Threading;

namespace UnityTools.Views
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : HandyControl.Controls.Window
    {
        ///// <summary>
        ///// 弹窗信息显示
        ///// </summary>
        //public static Snackbar MainStatusInfoSnackbar;

        private IRegionManager regionManager;

        public MainWindow(IRegionManager regionManager, IEventAggregator receivedEvent)
        {
            InitializeComponent();
            this.regionManager = regionManager;
            //MainStatusInfoSnackbar = StatusInfoSnackbar;
            NonClientAreaContent = new NonClientAreaContent(this);

            receivedEvent.GetEvent<MessagesNotificationEvent>().Subscribe(MessageReceived);
        }

        private void MessageReceived(MessageInfo obj)
        {
            Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
            {
                switch (obj.MsgType)
                {
                    case MessageType.ShowOnToast:
                        //MainWindow.MainStatusInfoSnackbar.MessageQueue.Enqueue(obj.Message);
                        break;
                }
            }));
        }

        private void MetroWindow_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            //regionManager.RequestNavigate("ContentRegion", "HomePage");
        }

        private void MetroWindow_Unloaded(object sender, System.Windows.RoutedEventArgs e)
        {
        }


    }
}
