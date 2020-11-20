using Rediscovery.Features.DesktopFeatures;
using Rediscovery.Services;
using Rediscovery.Views;
using Rediscovery.Views.Sidebar;
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
        private const string QueuedData = "Queued data:";

        MainPage RootPage { get => Application.Current.MainPage as MainPage; }

        internal SharedBase.Logging.ILogger _logger => DependencyService.Get<SharedBase.Logging.ILogger>() ?? new Services.Logger();
        private Features.DesktopFeatures.IClientFeatureService clientFeatureService => DependencyService.Get<Features.DesktopFeatures.IClientFeatureService>() ?? new Features.DesktopFeatures.ClientFeatureService();
        StartViewModel viewModel;

        public Start()
        {
            InitializeComponent();

            versionText.Text = "Rediscovery Mobile Client Version " + App.ClientVersion.ToString();

            BindingContext = viewModel = new StartViewModel();
            viewModel.QuickFeatureSelected += ViewModel_QuickFeatureSelected;
            clientFeatureService.ClientQueueDisplay += ClientFeatureService_ClientQueueDisplay;
            ClientFeatureQueue.IsVisible = clientFeatureService.HasQueueItem;
            ClientFeatureQueue.Text = QueuedData + " " + clientFeatureService.CurrentQueueItem?.QueueInfoText;
            viewModel.UpdateGetQuickConnectItem();

            AddConfiguration.Clicked += AddConfiguration_Clicked;
        }

        private async void ViewModel_QuickFeatureSelected(object sender, Connection.Models.ConnectionManifestFeature e)
        {
            await Navigation.PushModalAsync(new NavigationPage(new DesktopFeatures.FeaturePage.FeatureView.FeatureView(e.ConfigurationId, e)));
        }

        private async void AddConfiguration_Clicked(object sender, EventArgs e)
        {
            await RootPage.NavigateFromMenu((int)SidebarItemType.DesktopConfiguration);
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            viewModel.UpdateGetQuickConnectItem();
        }

        private void ClientFeatureService_ClientQueueDisplay(object sender, ClientFeatureService.QueueItem e)
        {
            ClientFeatureQueue.IsVisible = clientFeatureService.HasQueueItem;
            if (clientFeatureService.HasQueueItem)
                ClientFeatureQueue.Text = QueuedData + " " + e.QueueInfoText;
        }

        private async void ClientFeatureQueue_Clicked(object sender, EventArgs e)
        {
            string message = "Request are waiting to be send to a remote device, try to send again?";
            message += Environment.NewLine + Environment.NewLine + clientFeatureService.CurrentQueueItem?.QueueInfoText;
            bool answer = await DisplayAlert("Client feature", message, "Send", "Cancel");
            if (answer)
            {
                clientFeatureService.InvokeCurrentQueue();
            }
        }
    }
}