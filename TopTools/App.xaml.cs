using HandyControl.Data;
using HandyControl.Tools;
using System;
using System.Windows;
using TopMethods.Logs;
//using TopMethods.Memory;

namespace TopTools
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
        //private MemoryClean memoryClean = new MemoryClean();

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            App.Current.DispatcherUnhandledException += App_DispatcherUnhandledException;

            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);

            GlobalData.Init();
            log4net.Config.XmlConfigurator.Configure();

            //memoryClean.StartMemoryTask();

            var bootstrapper = new Bootstrapper();
            bootstrapper.Run();

            if (GlobalData.Config.Skin != SkinType.Default)
            {
                UpdateSkin(GlobalData.Config.Skin);
            }
            ConfigHelper.Instance.SetLang(GlobalData.Config.Lang);
            ConfigHelper.Instance.SetWindowDefaultStyle();
            ConfigHelper.Instance.SetNavigationWindowDefaultStyle();
        }

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            HandleException((Exception)e.ExceptionObject);
        }

        private static void HandleException(Exception ex)
        {
            LogFactory.SystemLog("运行时捕捉异常：", LogLevelType.Error, ex);
        }

        private static void App_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            HandleException(e.Exception);
        }

        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);
            GlobalData.Save();
        }

        /// <summary>
        /// 切换皮肤主题
        /// </summary>
        /// <param name="skin"></param>
        internal void UpdateSkin(SkinType skin)
        {
            var skins0 = Resources.MergedDictionaries[0];
            skins0.MergedDictionaries.Clear();
            skins0.MergedDictionaries.Add(ResourceHelper.GetSkin(skin));
            skins0.MergedDictionaries.Add(new ResourceDictionary
            {
                Source = new Uri($"pack://application:,,,/PhoenixSkins;component/Themes/Skin{skin}.xaml")
            });

            var skins1 = Resources.MergedDictionaries[1];
            skins1.MergedDictionaries.Clear();
            skins1.MergedDictionaries.Add(new ResourceDictionary
            {
                Source = new Uri("pack://application:,,,/HandyControl;component/Themes/Theme.xaml")
            });
            skins1.MergedDictionaries.Add(new ResourceDictionary
            {
                Source = new Uri("pack://application:,,,/PhoenixSkins;component/Themes/Style.xaml")
            });

            Current.MainWindow?.OnApplyTemplate();
            GlobalData.Config.Skin = skin;

        }

    }
}
