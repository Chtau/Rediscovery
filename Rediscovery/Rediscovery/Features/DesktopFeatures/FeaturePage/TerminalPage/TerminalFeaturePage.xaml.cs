using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Rediscovery.Features.DesktopFeatures.FeaturePage.TerminalPage
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TerminalFeaturePage : ContentPage
    {
        TerminalFeatureViewModel viewModel;

        public TerminalFeaturePage(TerminalFeatureViewModel model)
        {
            InitializeComponent();

            BindingContext = viewModel = model;
            viewModel.ReceivedData += ViewModel_ReceivedData;
            terminal.AddLines("Rediscovery Terminal Version " + model.FeatureVersion);
            terminal.SendCommand += Terminal_SendCommand;
        }

        private void ViewModel_ReceivedData(object sender, object e)
        {
            terminal.AddLines(e?.ToString());
        }

        private async void Back_Clicked(object sender, EventArgs e)
        {
            await Navigation.PopModalAsync();
        }

        private void Terminal_SendCommand(object sender, string e)
        {
            viewModel.Send(e);
        }
    }
}