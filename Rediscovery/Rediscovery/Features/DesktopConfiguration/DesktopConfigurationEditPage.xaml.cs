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
    public partial class DesktopConfigurationEditPage : ContentPage
    {
        DesktopConfigurationEditViewModel viewModel;

        public DesktopConfigurationEditPage(DesktopConfigurationModel item = null)
        {
            InitializeComponent();
            BindingContext = viewModel = new DesktopConfigurationEditViewModel(item);
        }

        async void Save_Clicked(object sender, EventArgs e)
        {
            await viewModel.Save();
        }

        async void Remove_Clicked(object sender, EventArgs e)
        {
            await viewModel.Remove();
            await Navigation.PopModalAsync();
        }

        async void Back_Clicked(object sender, EventArgs e)
        {
            await Navigation.PopModalAsync();
        }
    }
}