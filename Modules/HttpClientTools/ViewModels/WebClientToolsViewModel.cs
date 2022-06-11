using Prism.Events;
using Prism.Mvvm;
using Prism.Regions;
using TopUIControl.Prism;

namespace HttpClientTools.ViewModels
{
    public class WebClientToolsViewModel : BaseViewModel
    {

        public WebClientToolsViewModel(RegionManager regionManager, IEventAggregator receivedEvent) : base(regionManager, receivedEvent)
        {

        }

    }
}
