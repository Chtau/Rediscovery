using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Rediscovery.Features.DesktopFeatures.FeaturePage.MediaPlayer
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MediaPlayerFeaturePage : ContentPage
    {
        MediaPlayerFeatureViewModel viewModel;

        public MediaPlayerFeaturePage(MediaPlayerFeatureViewModel model)
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