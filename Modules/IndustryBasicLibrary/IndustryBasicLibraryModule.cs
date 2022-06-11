using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IndustryBasicLibrary.Views;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;

namespace IndustryBasicLibrary
{
    public class IndustryBasicLibraryModule : IModule
    {
        private IRegionManager _regionManager;

        public IndustryBasicLibraryModule(RegionManager regionManager)
        {
            _regionManager = regionManager;
        }

        public void OnInitialized(IContainerProvider containerProvider)
        {
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterForNavigation<SerialPortTools>();
        }
    }
}
