using Prism;
using Prism.Events;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TopUIControl.Prism;

namespace AudioVideoControlLibrary.ViewModels
{
    public class VlcVideoSampleViewModel : BaseViewModel, IActiveAware
    {
        public VlcVideoSampleViewModel(RegionManager regionManager, IEventAggregator receivedEvent) : base(regionManager, receivedEvent)
        {

        }

        private bool _isActive;
        public bool IsActive
        {
            get { return _isActive; }
            set
            {
                _isActive = value;
                OnIsActiveChanged();
            }
        }
        private void OnIsActiveChanged()
        {
            IsActiveChanged?.Invoke(this, new EventArgs());
        }

        public event EventHandler IsActiveChanged;

    }
}
