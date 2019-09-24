using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Rediscovery.Features.Authentication
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AuthenticationKey : ContentPage
    {
        Models.AuthenticationKeyViewModel viewModel;

        public AuthenticationKey(Guid connectionId)
        {
            InitializeComponent();

            BindingContext = viewModel = new Models.AuthenticationKeyViewModel(connectionId);
            viewModel.ShouldClose += ViewModel_ShouldClose;
        }

        private void ViewModel_ShouldClose(object sender, EventArgs e)
        {
            Navigation.PopModalAsync();
        }

        async void Close_Clicked(object sender, EventArgs e)
        {
            await Navigation.PopModalAsync();
        }
    }
}