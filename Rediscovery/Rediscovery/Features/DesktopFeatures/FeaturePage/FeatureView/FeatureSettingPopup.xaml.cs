using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Rediscovery.Features.DesktopFeatures.FeaturePage.FeatureView
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class FeatureSettingPopup : Rg.Plugins.Popup.Pages.PopupPage
    {
        FeatureSettingPopupViewModel viewModel;

        public FeatureSettingPopup(FeatureSettingPopupViewModel model)
        {
            InitializeComponent();
            BindingContext = viewModel = model;
        }

        // Invoked when a hardware back button is pressed
        protected override bool OnBackButtonPressed()
        {
            // Return true if you don't want to close this popup page when a back button is pressed
            return base.OnBackButtonPressed();
        }

        // Invoked when background is clicked
        protected override bool OnBackgroundClicked()
        {
            // Return false if you don't want to close this popup page when a background of the popup page is clicked
            return base.OnBackgroundClicked();
        }

        private async void Close_Clicked(object sender, EventArgs e)
        {
            await Rg.Plugins.Popup.Services.PopupNavigation.Instance.PopAsync();
        }
    }
}