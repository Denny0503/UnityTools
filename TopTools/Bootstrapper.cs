using Prism.Unity;
using Prism.Ioc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using TopTools.Views;
using Prism.Modularity;
using Prism.Regions;
using Prism.Mvvm;
using System.Reflection;
using System.Globalization;
namespace TopTools
{
    class Bootstrapper : PrismBootstrapper
    {
        protected override DependencyObject CreateShell()
        {
            return Container.Resolve<MainWindow>();
        }

        //protected override void ConfigureModuleCatalog(IModuleCatalog moduleCatalog)
        //{
        //    moduleCatalog.AddModule<WaterMarkToolsModule>();
        //    moduleCatalog.AddModule<TesseractOCRModule>();

        //}

        protected override IModuleCatalog CreateModuleCatalog()
        {
            return new DirectoryModuleCatalog() { ModulePath = @".\Modules" };
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterForNavigation<HomePage>();
            containerRegistry.RegisterForNavigation<Readme>();
        }

        protected override void ConfigureRegionAdapterMappings(RegionAdapterMappings regionAdapterMappings)
        {
            //regionAdapterMappings.RegisterMapping(typeof(TabablzControl), Container.Resolve<TabablzControlRegionAdapter>());
            base.ConfigureRegionAdapterMappings(regionAdapterMappings);
        }

        protected override void ConfigureDefaultRegionBehaviors(IRegionBehaviorFactory regionBehaviors)
        {
            base.ConfigureDefaultRegionBehaviors(regionBehaviors);

            ViewModelLocationProvider.SetDefaultViewTypeToViewModelTypeResolver((viewType) =>
            {
                var viewModelName = String.Format(CultureInfo.InvariantCulture, "{0}VM, {1}", viewType.FullName.Replace(".Views.", ".ViewModels."), viewType.GetTypeInfo().Assembly.FullName);
                return Type.GetType(viewModelName);
            });
        }
    }
}
