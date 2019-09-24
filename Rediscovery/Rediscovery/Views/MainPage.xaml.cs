using System;
using System.ComponentModel;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Rediscovery.Views
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class MainPage : TabbedPage
    {
        private Features.Authentication.IConnect auth => DependencyService.Get<Features.Authentication.IConnect>() ?? new Features.Authentication.Connect();

        public MainPage()
        {
            InitializeComponent();

            auth.HelloReceived += Auth_HelloReceived;
        }

        private void Auth_HelloReceived(object sender, Features.Authentication.Models.Connection e)
        {
            if (e.ConnectionState == SharedCoreModels.Enums.ConnectionState.WaitForApprovel)
            {
                Navigation.PushModalAsync(new Features.Authentication.AuthenticationKey(e.Id));
            }
        }
    }
}