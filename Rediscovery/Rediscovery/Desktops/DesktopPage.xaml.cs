using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Rediscovery.Desktops
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DesktopPage : ContentPage
    {
        private DesktopViewModel viewModel;

        public DesktopPage()
        {
            InitializeComponent();
            BindingContext = viewModel = new DesktopViewModel();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
        }
    }
}