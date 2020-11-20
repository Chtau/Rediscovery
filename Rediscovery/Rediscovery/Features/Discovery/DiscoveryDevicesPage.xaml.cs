using Rediscovery.Features.DesktopConfiguration;
using Rediscovery.Features.Storage;
using Rediscovery.Services;
using SharedBase.Connection;
using SharedBase.Discovery;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Rediscovery.Features.Discovery
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DiscoveryDevicesPage : ContentPage
    {
        private IDataStoreGuid<DesktopConfiguration.DesktopConfigurationModel> desktopStore => DependencyService.Get<IDataStoreGuid<DesktopConfiguration.DesktopConfigurationModel>>() ?? new DesktopConfiguration.DesktopConfigurationStore();

        DiscoveryDevicesViewModel viewModel;

        public DiscoveryDevicesPage()
        {
            InitializeComponent();
            BindingContext = viewModel = new DiscoveryDevicesViewModel();
        }

        private async void DevicesFoundControl_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            DiscoveryServiceInfo item = e.SelectedItem as DiscoveryServiceInfo;
            if (item != null)
            {
                string answer = await DisplayPromptAsync("Save Desktop", "Save new Desktop with the following Name", "Yes", "Cancel", placeholder:"Desktop Name", initialValue:item.DesktopName);
                if (!string.IsNullOrWhiteSpace(answer))
                {
                    viewModel.StopDiscoveryCommand.Execute(null);
                    var newDevice = new DesktopConfigurationModel
                    {
                        Id = Guid.NewGuid(),
                        DisplayName = answer,
                        Address = item.IPAddress,
                        Port = item.Port,
                        AutoConnect = false,
                        ConnectionState = Enums.ConnectionState.None,
                        LastConnection = null
                    };
                    var result = await desktopStore.AddItemAsync(newDevice);
                    if (result.Item1)
                    {
                        MessagingCenter.Send(this, "refresh_desktop_configuration", newDevice);
                    }
                    await Navigation.PopModalAsync();
                }
                DevicesFoundControl.SelectedItem = null;
            }
        }

        private async void Back_Clicked(object sender, EventArgs e)
        {
            viewModel.StopDiscoveryCommand.Execute(null);
            await Navigation.PopModalAsync();
        }
    }
}