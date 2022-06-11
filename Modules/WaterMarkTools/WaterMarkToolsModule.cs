
using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;
using WaterMarkTools.Views;

namespace WaterMarkTools
{
    public class WaterMarkToolsModule : IModule
    {
        private IRegionManager _regionManager;

        public WaterMarkToolsModule(RegionManager regionManager)
        {
            _regionManager = regionManager;
        }

        public void OnInitialized(IContainerProvider containerProvider)
        {
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterForNavigation<WaterMark>();
        }
    }
}
