using System;
using System.Windows.Input;

using Xamarin.Forms;

namespace Rediscovery.ViewModels
{
    public class AboutViewModel : BaseViewModel
    {
        public Services.INetworkDiscovery netService => DependencyService.Get<Services.INetworkDiscovery>() ?? new Services.NetworkDiscovery();

        public AboutViewModel()
        {
            Title = "About";

            OpenWebCommand = new Command(() => Device.OpenUri(new Uri("https://xamarin.com/platform")));
            TestCommand = new Command(() =>
            {
                netService.Send();
            });
        }

        public ICommand OpenWebCommand { get; }
        public ICommand TestCommand { get; }
    }
}