using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Rediscovery.Features.Settings
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SettingPage : ContentPage
    {
        SettingViewModel viewModel;

        public SettingPage()
        {
            InitializeComponent();
            BindingContext = viewModel = new SettingViewModel();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            viewModel.LoadCommand.Execute(null);
        }

        private async void GenerateADI_Clicked(object sender, EventArgs e)
        {
            var result = await DisplayAlert("Generate new Identifier", "If you generate a new Application Device Identifier previous authentications from existing device will no longer work.", "Ok", "Cancel");
            if (result)
            {
                viewModel.GenerateNewApplicationDeviceIdentifierCommand.Execute(null);
            }
        }
    }
}