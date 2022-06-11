using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prism.Events;
using Prism.Mvvm;
using Prism.Regions;
using TopUIControl.Prism;

namespace IndustryBasicLibrary.ViewModels
{
    public class SerialPortToolsViewModel : BaseViewModel
    {

        public SerialPortToolsViewModel(RegionManager regionManager, IEventAggregator receivedEvent) : base(regionManager, receivedEvent)
        {

        }
    }
}
