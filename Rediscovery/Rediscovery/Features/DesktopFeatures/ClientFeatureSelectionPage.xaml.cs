using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Rediscovery.Features.DesktopFeatures
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ClientFeatureSelectionPage : ContentPage
    {
        ClientFeatureSelectionViewModel viewModel;

        public ClientFeatureSelectionPage(ClientFeatureSelectionViewModel model)
        {
            InitializeComponent();

            BindingContext = viewModel = model;
        }
    }
}