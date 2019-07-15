using System;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Windows.Input;

using Xamarin.Forms;

namespace Rediscovery.ViewModels
{
    public class AboutViewModel : BaseViewModel
    {

        public AboutViewModel()
        {
            Title = "About";

            OpenWebCommand = new Command(() => Device.OpenUri(new Uri("https://xamarin.com/platform")));
            TestCommand = new Command(() =>
            {
                
            });
        }

        public ICommand OpenWebCommand { get; }
        public ICommand TestCommand { get; }

    }
}