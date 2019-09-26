using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace DesktopHub.Devices.Models
{
    public class DevicesControlViewModel : BaseViewModel
    {
        public ObservableCollection<SharedCoreModels.DeviceInfo> Items { get; set; }
    }
}
