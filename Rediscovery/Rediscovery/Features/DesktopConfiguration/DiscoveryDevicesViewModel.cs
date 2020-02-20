using Rediscovery.Services;
using Rediscovery.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;

namespace Rediscovery.Features.DesktopConfiguration
{
    public class DiscoveryDevicesViewModel : BaseViewModel
    {
        private ILogger logger => DependencyService.Get<ILogger>() ?? new Logger();
        private Services.IDiscoveryService discoveryService => DependencyService.Get<Services.IDiscoveryService>() ?? new Services.DiscoveryService();

        public ICommand DiscoveryCommand { get; set; }

        public DiscoveryDevicesViewModel()
        {
            DiscoveryCommand = new Command(() =>
            {
                discoveryService.Boardcast((answer) =>
                {
                    Console.WriteLine("Anwser Recived from IPAddress:{0}", answer);
                });
            }, () =>
            {
                return true;
            });
        }
    }
}
