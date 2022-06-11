using AudioVideoControlLibrary.Views;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AudioVideoControlLibrary
{
    public class AudioVideoControlLibraryModule : IModule
    {
        private IRegionManager _regionManager;

        public AudioVideoControlLibraryModule(RegionManager regionManager)
        {
            _regionManager = regionManager;
        }

        public void OnInitialized(IContainerProvider containerProvider)
        {
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterForNavigation<VlcVideoSample>();
        }
    }
}
