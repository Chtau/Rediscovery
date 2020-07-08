using Rediscovery.Services;
using Rediscovery.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;

namespace Rediscovery.Features.DesktopConfiguration
{
    public class DiscoveryDevicesViewModel : BaseViewModel
    {
        private Services.IDiscoveryService discoveryService => DependencyService.Get<Services.IDiscoveryService>() ?? new Services.DiscoveryService();
        public ObservableCollection<SharedBase.Discovery.DiscoveryServiceInfo> FoundDevices { get; set; } = new ObservableCollection<SharedBase.Discovery.DiscoveryServiceInfo>();

        public ICommand DiscoveryCommand { get; set; }
        public ICommand StopDiscoveryCommand { get; set; }

        bool isDiscoveryRunning = false;
        public bool IsDiscoveryRunning
        {
            get { return isDiscoveryRunning; }
            set { SetProperty(ref isDiscoveryRunning, value); }
        }

        private bool shouldStop = false;

        public DiscoveryDevicesViewModel()
        {
            DiscoveryCommand = new Command(() =>
            {
                IsDiscoveryRunning = true;
                _userNotification.ShowToast("Start discover devices");
                discoveryService.Boardcast((answer) =>
                {
                    var item = FoundDevices.FirstOrDefault(x => x.DesktopName == answer.DesktopName);
                    if (item != null)
                    {
                        item.IPAddress = answer.IPAddress;
                        item.Metadata = answer.Metadata;
                        item.Port = answer.Port;
                        item.DesktopOS = answer.DesktopOS;
                        item.DesktopName = answer.DesktopName;
                    } else
                    {
                        FoundDevices.Add(answer);
                    }
                }, () =>
                {
                    return !shouldStop;
                });
            }, () =>
            {
                return true;
            });
            StopDiscoveryCommand = new Command(() =>
            {
                if (IsDiscoveryRunning)
                {
                    _userNotification.ShowToast("Stopping device discovery");
                }
                IsDiscoveryRunning = false;
                shouldStop = true;
            }, () =>
            {
                return true;
            });
        }
    }
}
