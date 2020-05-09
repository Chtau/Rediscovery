using Newtonsoft.Json;
using Rediscovery.Services;
using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Rediscovery.Features.DesktopFeatures.FeaturePage.FeatureView
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class FeatureView : ContentPage
    {
        private FeatureViewViewModel viewModel;

        private Features.Connection.IConnect connect => DependencyService.Get<Features.Connection.IConnect>() ?? new Features.Connection.Connect();
        private IFeatureUIService featureUIService => DependencyService.Get<IFeatureUIService>() ?? new FeatureUIService();
        private ILogger logger => DependencyService.Get<ILogger>() ?? new Logger();

        public FeatureView(Guid desktopConfigId, Features.Connection.Models.ConnectionManifestFeature connectionManifestFeature)
        {
            InitializeComponent();

            BindingContext = viewModel = new FeatureViewViewModel(desktopConfigId, connectionManifestFeature);
            viewModel.Load.IsLoading = true;
            viewModel.UIDataReady += ViewModel_UIDataReady;
            viewModel.UIDataNoArchive += ViewModel_UIDataNoArchive;
            viewModel.ReceivedFeatureData += ViewModel_ReceivedData;
            viewModel.ProfilChanged += ViewModel_ProfilChanged;

            hybridWebView.RegisterErrorCallback((error) =>
            {
                logger.Error(new Exception(error));
            });
        }

        private void ViewModel_ProfilChanged(object sender, object e)
        {
            hybridWebView.SetProfileChanged(e?.ToString());
        }

        private void ViewModel_ReceivedData(object sender, object e)
        {
            hybridWebView.SetModel(e?.ToString());
        }

        private void ViewModel_UIDataNoArchive(object sender, Tuple<Guid, Guid> e)
        {
            Dispatcher.BeginInvokeOnMainThread(() =>
            {
                viewModel.Load.IsLoading = false;
            });
            hybridWebView.SetDefaultHtml();
        }

        private void ViewModel_UIDataReady(object sender, Tuple<Guid, string> e)
        {
            hybridWebView.RegisterDOMReady(() =>
            {
                Dispatcher.BeginInvokeOnMainThread(() =>
                {
                    hybridWebView.SetProfileChanged(viewModel.SelectedProfile?.ProfileData?.ToString());
                });
            });
            hybridWebView.RegisterLogger((data) =>
            {
                logger.Message(data);
            });
            hybridWebView.SourceFolderSet += (obj, args) =>
            {
                Dispatcher.BeginInvokeOnMainThread(() =>
                {
                    viewModel.Load.IsLoading = false;
                });
            };
            hybridWebView.SetFolderSource(e.Item2);
            hybridWebView.RegisterAction((data) =>
            {
                viewModel.Send(data);
            });
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            viewModel.Start();
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            viewModel.Stop();
        }

        private async void Back_Clicked(object sender, EventArgs e)
        {
            await Navigation.PopModalAsync();
        }

        private void Setting_Clicked(object sender, EventArgs e)
        {
            var model = new FeatureSettingPopupViewModel
            {
                SelectedProfile = viewModel.SelectedProfile,
                Profiles = viewModel.Profiles
            };
            model.ProfileChanged += (obj, args) =>
            {
                viewModel.SelectedProfile = args;
            };
            Rg.Plugins.Popup.Services.PopupNavigation.Instance.PushAsync(new FeatureSettingPopup(model));
        }
    }
}