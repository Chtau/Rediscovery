using System;
using System.Collections.Generic;
using System.Text;

namespace RediscoveryManager.GUI.ViewModels
{
    public class ConnectionConfigurationViewModel : ViewModelBase
    {
        public event EventHandler<bool> Close;

        public string IPAddress { get; set; }
        public int? Port { get; set; }
        public string DeviceIdentifier { get; set; }

        public void Ok()
        {
            Close?.Invoke(this, true);
        }

        public void Cancel()
        {
            Close?.Invoke(this, false);
        }
    }
}
