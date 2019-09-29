using Rediscovery.Services;
using System;
using System.ComponentModel;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Rediscovery.Views
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class MainPage : TabbedPage
    {
        private MainPageViewModel viewModel;

        private Features.Authentication.IConnect auth => DependencyService.Get<Features.Authentication.IConnect>() ?? new Features.Authentication.Connect();
        
        public MainPage()
        {
            InitializeComponent();

            BindingContext = viewModel = new MainPageViewModel();
            viewModel.Items.CollectionChanged += Items_CollectionChanged;
            viewModel.Load();
            auth.HelloReceived += Auth_HelloReceived;
        }

        private void Items_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (var item in e.NewItems)
                {
                    var model = (Features.Authentication.Models.Connection)item;
                    var navigationPage = new NavigationPage(new Desktops.DesktopPage());
                    navigationPage.IconImageSource = "tab_feed.png";
                    navigationPage.Title = model.DisplayName;

                    Children.Add(navigationPage);
                }
            }
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