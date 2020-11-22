using System;
using System.Collections.Generic;
using System.Text;

namespace Rediscovery.Client.App.Manager.Console
{
    public class ActiveDevicesHandler : BaseDisplayDevice
    {
        private const string DisplayName = "activedevices";
        private readonly IManager _manager;

        public ActiveDevicesHandler(IManager manager)
        {
            _manager = manager;
            _manager.DeviceCollectionChanged += (obj, args) =>
            {
                if (string.Equals(SharedUI.CurrentDisplay, DisplayName))
                {
                    WaitForWriting();
                    if (currentNavigationIndex > _manager.ActiveDevices.Count)
                        currentNavigationIndex = 0;
                    DisplayTitle();
                }
            };
        }

        internal override string WriteTitle()
        {
            return "Active Devices: ";
        }

        internal override IList<SharedBase.Device.DeviceInfo> DeviceCollection()
        {
            return _manager.ActiveDevices;
        }
    }
}
