using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Rediscovery.Features.DesktopConfiguration
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DiscoveryDevicesPage : ContentPage
    {
        DiscoveryDevicesViewModel viewModel;

        public DiscoveryDevicesPage()
        {
            InitializeComponent();
            BindingContext = viewModel = new DiscoveryDevicesViewModel();
        }
    }
}