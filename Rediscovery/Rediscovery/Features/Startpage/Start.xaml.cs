using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Rediscovery.Features.Startpage
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Start : ContentPage
    {
        StartViewModel viewModel;

        public Start()
        {
            InitializeComponent();
            BindingContext = viewModel = new StartViewModel();
        }

        private async void ClientFeatureQueue_Clicked(object sender, EventArgs e)
        {
            bool answer = await DisplayAlert("Client feature", "Request are waiting to be send to a remote device, try to send again?", "Send", "Cancel");
            Debug.WriteLine("Answer: " + answer);
        }
    }
}