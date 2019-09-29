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

        public DesktopPage(DesktopViewModel model)
        {
            InitializeComponent();
            BindingContext = viewModel = model;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            viewModel.Load();
        }
    }
}