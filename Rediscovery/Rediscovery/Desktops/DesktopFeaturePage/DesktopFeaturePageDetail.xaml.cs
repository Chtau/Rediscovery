using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Rediscovery.Desktops.DesktopFeaturePage
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DesktopFeaturePageDetail : ContentPage
    {
        DesktopFeaturePageDetailViewModel viewModel;

        public DesktopFeaturePageDetail(DesktopFeaturePageDetailViewModel model)
        {
            InitializeComponent();

            BindingContext = viewModel = model;
        }

        private async void Back_Clicked(object sender, EventArgs e)
        {
            await Navigation.PopModalAsync();
        }
    }
}