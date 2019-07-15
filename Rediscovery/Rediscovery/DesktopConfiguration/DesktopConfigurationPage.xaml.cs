using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Rediscovery.DesktopConfiguration
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DesktopConfigurationPage : ContentPage
    {
        DesktopConfigurationViewModel viewModel;

        public DesktopConfigurationPage()
        {
            InitializeComponent();
            BindingContext = viewModel = new DesktopConfigurationViewModel();
        }


        async void OnItemSelected(object sender, SelectedItemChangedEventArgs args)
        {
            DesktopConfigurationModel item = args.SelectedItem as DesktopConfigurationModel;
            if (item == null)
                return;

            await Navigation.PushModalAsync(new NavigationPage(new DesktopConfigurationEditPage(item)));

            ItemsListView.SelectedItem = null;
        }

        async void AddItem_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushModalAsync(new NavigationPage(new DesktopConfigurationEditPage()));
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            if (viewModel.Items.Count == 0)
                viewModel.LoadItemsCommand.Execute(null);
        }
    }
}