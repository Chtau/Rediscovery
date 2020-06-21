using RediscoveryManager.Service;
using System;
using System.Collections.Generic;
using System.Text;

namespace RediscoveryManager
{
    public class PendingDevicesHandler : BaseDisplayDevice
    {
        private const string DisplayName = "pendingdevices";
        private readonly IManager _manager;

        public PendingDevicesHandler(IManager manager)
        {
            _manager = manager;
            _manager.DeviceCollectionChanged += (obj, args) =>
            {
                if (string.Equals(SharedUI.CurrentDisplay, DisplayName))
                {
                    WaitForWriting();
                    if (currentNavigationIndex > _manager.PendingDevices.Count)
                        currentNavigationIndex = 0;
                    DisplayTitle();
                }
            };
        }

        internal override void WriteMenu()
        {
            Console.WriteLine($"{Commands.Accept.PutifyStringArray()} = Accept access request");
            Console.WriteLine($"{Commands.Deny.PutifyStringArray()} = Deny access request");
        }

        internal override string WriteTitle()
        {
            return "Pending Devices: ";
        }

        internal override IList<SharedBase.Device.DeviceInfo> DeviceCollection()
        {
            return _manager.PendingDevices;
        }

        internal override bool HandleSubMenu(string[] args, string lastInput)
        {
            if (_manager.PendingDevices?.Count > 0 && Commands.MatchInput(lastInput, Commands.Accept))
            {
                var item = _manager.PendingDevices[currentNavigationIndex];
                _manager.TryResolvePendingDevice(item.Id, true);
                return true;
            }
            else if (_manager.PendingDevices?.Count > 0 && Commands.MatchInput(lastInput, Commands.Deny))
            {
                var item = _manager.PendingDevices[currentNavigationIndex];
                _manager.TryResolvePendingDevice(item.Id, false);
                return true;
            }
            return false;
        }
    }
}
