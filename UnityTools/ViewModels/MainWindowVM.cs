using UnityControl.Events;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using Prism.Regions;
using System;
using System.Diagnostics;
using System.Reflection;
using System.Windows;
using UnityMethods.Extend;
using UnityTools.Views;

namespace UnityTools.ViewModels
{
    public class MainWindowVM : BindableBase
    {
        private readonly IRegionManager _regionManager;
        private IEventAggregator _receivedEvent;
        public string Title { get; set; } = "VCBlog 工具集合";

        public string VersionInfo { get; set; }

        public string ModulesName { get; set; } = "VCBlog 工具";

        public int ProgressValue { get; set; }

        public int ProgressTotal { get; set; }

        public DelegateCommand MyHomePageCommand { get; private set; }

        public MainWindowVM(IRegionManager regionManager, IEventAggregator receivedEvent)
        {
            _regionManager = regionManager;
            _receivedEvent = receivedEvent;

            _receivedEvent.GetEvent<ProgressSentEvent>().Subscribe(ProgressReceived);
            _receivedEvent.GetEvent<MessagesNotificationEvent>().Subscribe(MessageReceived);

            MyHomePageCommand = new DelegateCommand(GoMyHomePage);

            var versionInfo = FileVersionInfo.GetVersionInfo(Assembly.GetEntryAssembly().Location);

            VersionInfo = $"{Title} - v{versionInfo.ProductVersion}";
        }

        /// <summary>
        /// 关于页面
        /// </summary>
        public DelegateCommand CopyrightCommand => new DelegateCommand(() =>
        {
            CopyrightBox copyrightBox = new CopyrightBox();
            copyrightBox.ShowDialog();
        });

        private void GoMyHomePage()
        {
            System.Diagnostics.Process.Start("https://blog.csdn.net/MengHuanXinZuo");
        }

        private void MessageReceived(MessageInfo obj)
        {
            switch (obj.MsgType)
            {
                case MessageType.ShowOnStatusBar:
                    ModulesName = obj.Message;
                    break;
            }
        }

        private void ProgressReceived(ProgressInfo obj)
        {
            ProgressTotal = obj.TotalProgress;
            ProgressValue = obj.CurrentProgress;
        }

        /// <summary>
        /// 导航
        /// </summary>
        public DelegateCommand<string> NavigateCommand => new DelegateCommand<string>((navigatePath) =>
        {
            if (!navigatePath.IsEmpty())
            {
                _regionManager.RequestNavigate("ContentRegion", navigatePath);
            }
        });

        /// <summary>
        /// 主题切换
        /// </summary>
        public DelegateCommand<object> ThemeSwitchCommand => new DelegateCommand<object>((theme) =>
        {
            if (GlobalData.Config.Skin != (HandyControl.Data.SkinType)theme)
            {
                ((App)Application.Current).UpdateSkin((HandyControl.Data.SkinType)theme);
            }
        });

        public DelegateCommand<object> ExitCommand => new DelegateCommand<object>((theme) =>
        {
            ((App)Application.Current).Shutdown();
        });

    }
}