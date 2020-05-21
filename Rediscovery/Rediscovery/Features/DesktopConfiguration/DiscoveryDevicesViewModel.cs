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
        private ILogger logger => DependencyService.Get<ILogger>() ?? new Logger();
        private Services.IDiscoveryService discoveryService => DependencyService.Get<Services.IDiscoveryService>() ?? new Services.DiscoveryService();
        public ObservableCollection<SharedCoreModels.DiscoveryServiceInfo> FoundDevices { get; set; } = new ObservableCollection<SharedCoreModels.DiscoveryServiceInfo>();

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
                discoveryService.Boardcast((answer) =>
                {
                    Console.WriteLine("Answer Received from IPAddress:{0}", answer);
                    var item = FoundDevices.FirstOrDefault(x => x.Name == answer.Name);
                    if (item != null)
                    {
                        item.IPAddress = answer.IPAddress;
                        item.Metadata = answer.Metadata;
                        item.Port = answer.Port;
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
                IsDiscoveryRunning = false;
                shouldStop = true;
            }, () =>
            {
                return true;
            });
        }
    }
}
