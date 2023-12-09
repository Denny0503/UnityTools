using Microsoft.Web.WebView2.Core;
using System;
using System.Collections.Generic;
using System.IO;
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
using UnityMethods.Extend;

namespace UnityControl.WebView2
{
    /// <summary>
    /// WebView2Browser.xaml 的交互逻辑
    /// </summary>
    public partial class WebView2Browser : UserControl, IDisposable
    {
        private bool disposedValue;

        /// <summary>
        /// js调用脚本
        /// </summary>
        public BridgeAddRemoteObject Bridge { private set; get; } = new BridgeAddRemoteObject();

        /// <summary>
        /// 是否已初始化
        /// </summary>
        private bool isInitialized;

        public string Address
        {
            set
            {
                if (WebView.IsInitialized && WebView.CoreWebView2 != null)
                {
                    WebView.CoreWebView2.Navigate(value);
                }
            }
            get
            {
                if (WebView.IsInitialized && WebView.CoreWebView2 != null)
                {
                    return WebView.CoreWebView2.Source;
                }
                else
                {
                    return "";
                }

            }
        }

        public WebView2Browser()
        {
            InitializeComponent();
        }

        public async Task InitializeAsync()
        {
            if (!isInitialized)
            {
                await WebView.EnsureCoreWebView2Async(null);

                //WebView.CoreWebView2.AddWebResourceRequestedFilter("*", CoreWebView2WebResourceContext.Font);
                //WebView.CoreWebView2.AddWebResourceRequestedFilter("*", CoreWebView2WebResourceContext.Image);
                //WebView.CoreWebView2.WebResourceRequested += CoreWebView2_WebResourceRequested;

#if !DEBUG

            WebView.CoreWebView2.Settings.AreBrowserAcceleratorKeysEnabled = false;
            WebView.CoreWebView2.Settings.AreDefaultContextMenusEnabled = false;
            WebView.CoreWebView2.Settings.AreDevToolsEnabled = false;

#endif
                WebView.CoreWebView2.Settings.IsWebMessageEnabled = true;
                WebView.CoreWebView2.Settings.IsScriptEnabled = true;
                WebView.CoreWebView2.Settings.IsZoomControlEnabled = false;
                WebView.CoreWebView2.NewWindowRequested += CoreWebView2_NewWindowRequested;
                WebView.CoreWebView2.WebMessageReceived += CoreWebView2_WebMessageReceived;
                //WebView.CoreWebView2.NavigationCompleted += CoreWebView2_NavigationCompleted;
                WebView.CoreWebView2.AddHostObjectToScript("Bridge", Bridge); /*JS 调用 chrome.webview.hostObjects.Bridge */

                isInitialized = true;
            }
        }

        private void CoreWebView2_WebMessageReceived(object? sender, CoreWebView2WebMessageReceivedEventArgs e)
        {
            FileDialogTools.OpenFolderBrowserDialog(e.WebMessageAsJson);
        }

        private async void CoreWebView2_NavigationCompleted(object sender, CoreWebView2NavigationCompletedEventArgs e)
        {
            /*禁用拖放功能*/
            await WebView.CoreWebView2?.ExecuteScriptAsync("window.addEventListener('dragover',function(e){e.preventDefault();},false);");
            await WebView.CoreWebView2?.ExecuteScriptAsync("window.addEventListener('drop',function(e){ e.preventDefault();}, false);");
            /*删除上下文菜单功能*/
            await WebView.CoreWebView2?.ExecuteScriptAsync("window.addEventListener('contextmenu', window => {window.preventDefault();});");
        }

        private void CoreWebView2_NewWindowRequested(object sender, CoreWebView2NewWindowRequestedEventArgs e)
        {
            CoreWebView2 view2 = sender as CoreWebView2;
            if (view2 != null)
            {
                var deferral = e.GetDeferral();
                e.NewWindow = view2;
                deferral.Complete();
            }
        }

        private void CoreWebView2_WebResourceRequested(object sender, CoreWebView2WebResourceRequestedEventArgs e)
        {
            string fileName = "";

            try
            {
                List<string> files = Directory.GetFiles($"{Directory.GetCurrentDirectory()}/Resource", "*", SearchOption.AllDirectories).ToList();

                if (files?.Any() ?? false)
                {
                    foreach (string item in files)
                    {
                        if (e.Request.Uri.Contains(System.IO.Path.GetFileNameWithoutExtension(item)) && e.Request.Uri.Contains(System.IO.Path.GetExtension(item)))
                        {
                            fileName = item;
                            break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                fileName = "";
            }

            if (!fileName.IsEmpty() && WebView.CoreWebView2 != null)
            {
                string contentType = "";
                string extension = System.IO.Path.GetExtension(fileName).Remove(0, 1);
                switch (e.ResourceContext)
                {
                    case CoreWebView2WebResourceContext.Image:
                        if (extension.Equals("svg"))
                        {
                            contentType = "image/svg+xml";
                        }
                        else
                        {
                            contentType = $"image/{extension}";
                        }
                        break;
                }

                e.Response = WebView.CoreWebView2.Environment.CreateWebResourceResponse(new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read), 200, "OK", "");

                if (!contentType.IsEmpty())
                {
                    e.Response.Headers.AppendHeader("Content-Type", contentType);
                }

            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    if (WebView != null)
                    {
                        WebView.CoreWebView2?.RemoveHostObjectFromScript("Bridge"); /*JS 调用 chrome.webview.hostObjects.Bridge */
                        // TODO: 释放托管状态(托管对象)
                        WebView.Dispose();
                    }
                }

                // TODO: 释放未托管的资源(未托管的对象)并重写终结器
                // TODO: 将大型字段设置为 null
                disposedValue = true;
            }
        }

        // // TODO: 仅当“Dispose(bool disposing)”拥有用于释放未托管资源的代码时才替代终结器
        // ~WebView2Control()
        // {
        //     // 不要更改此代码。请将清理代码放入“Dispose(bool disposing)”方法中
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // 不要更改此代码。请将清理代码放入“Dispose(bool disposing)”方法中
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {

        }

        /// <summary>
        /// 执行js脚本
        /// </summary>
        /// <param name="javaScript"></param>
        public void ExecuteJavaScriptAsync(string javaScript)
        {
            if (WebView != null && WebView.CoreWebView2 != null)
            {
                WebView.CoreWebView2.ExecuteScriptAsync(javaScript);
            }
        }
    }
}
