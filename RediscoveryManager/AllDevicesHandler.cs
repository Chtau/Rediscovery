using System;
using System.Collections.Generic;
using System.Text;

namespace Rediscovery.Client.App.Manager.Console
{
    public class AllDevicesHandler : BaseDisplayDevice
    {
        private const string DisplayName = "alldevices";
        private readonly IManager _manager;

        public AllDevicesHandler(IManager manager)
        {
            _manager = manager;
            _manager.DeviceCollectionChanged += (obj, args) =>
            {
                if (string.Equals(SharedUI.CurrentDisplay, DisplayName))
                {
                    WaitForWriting();
                    if (currentNavigationIndex > _manager.Devices.Count)
                        currentNavigationIndex = 0;
                    DisplayTitle();
                }
            };
        }

        internal override string WriteTitle()
        {
            return "Devices: ";
        }

        internal override IList<SharedBase.Device.DeviceInfo> DeviceCollection()
        {
            return _manager.Devices;
        }
    }
}
