using HttpClientTools.Views;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;

namespace HttpClientTools
{
    public class HttpClientToolsModule : IModule
    {
        private IRegionManager _regionManager;

        public HttpClientToolsModule(RegionManager regionManager)
        {
            _regionManager = regionManager;
        }

        public void OnInitialized(IContainerProvider containerProvider)
        {
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterForNavigation<WebSocketControl>();
            containerRegistry.RegisterForNavigation<TcpUdpToolsControl>();
            containerRegistry.RegisterForNavigation<WebClientTools>();
            containerRegistry.RegisterForNavigation<NetConvertToSerialPort>();
        }
    }
}
