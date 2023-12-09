using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityControl;
using Prism.Commands;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;
using TesseractOCR.Views;

namespace TesseractOCR
{
    public class TesseractOCRModule : IModule
    {
        private IRegionManager _regionManager;


        public TesseractOCRModule(RegionManager regionManager)
        {
            _regionManager = regionManager;
        }

        public void OnInitialized(IContainerProvider containerProvider)
        {
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterForNavigation<TesseractImage>();
            containerRegistry.RegisterForNavigation<TesseractVideo>();
        }



    }
}
