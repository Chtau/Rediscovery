using Rediscovery.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Rediscovery.Views.Sidebar
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SidebarDrawer : ContentPage
    {
        internal Services.ILoggerEvent _logger => DependencyService.Get<Services.ILoggerEvent>() ?? new Services.Logger();
        MainPage RootPage { get => Application.Current.MainPage as MainPage; }
        List<SidebarMenuItem> menuItems;

        public SidebarDrawer()
        {
            InitializeComponent();

            versionText.Text = "Rediscovery Mobile Client Version " + App.ClientVersion.ToString();

            menuItems = new List<SidebarMenuItem>
            {
                new SidebarMenuItem {Id = SidebarItemType.Home, Title="Home", Icon = "home_fill_primary.png", IconNotActive = "home_fill_lightgray.png" },
                new SidebarMenuItem {Id = SidebarItemType.Feature, Title="Feature", Icon = "apps_line_primary.png", IconNotActive = "apps_line_lightgray.png" },
                new SidebarMenuItem {Id = SidebarItemType.DesktopConfiguration, Title="Desktop Configuration", Icon = "computer_fill_primary.png", IconNotActive = "computer_fill_lightgray.png" },
                new SidebarMenuItem {Id = SidebarItemType.Setting, Title="Setting", Icon = "settings_fill_primary.png", IconNotActive = "settings_fill_lightgray.png" },
            };
            ListViewMenu.ItemsSource = menuItems;

            ListViewMenu.ItemSelected += async (sender, e) =>
            {
                if (e.SelectedItem == null)
                    return;

                var id = (int)((SidebarMenuItem)e.SelectedItem).Id;
                ListViewMenu.SelectedItem = null;
                await RootPage.NavigateFromMenu(id);
            };
        }

        public void SetMenuActive(int id)
        {
            try
            {
                if (menuItems?.Count > 0)
                {
                    foreach (var item in menuItems)
                    {
                        if ((int)item.Id == id)
                            item.IsActive = true;
                        else
                            item.IsActive = false;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex);
            }
        }
    }
}