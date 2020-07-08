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
        MainPage RootPage { get => Application.Current.MainPage as MainPage; }
        List<SidebarMenuItem> menuItems;

        public SidebarDrawer()
        {
            InitializeComponent();

            menuItems = new List<SidebarMenuItem>
            {
                new SidebarMenuItem {Id = SidebarItemType.Home, Title="Home" },
                new SidebarMenuItem {Id = SidebarItemType.Feature, Title="Feature" },
                new SidebarMenuItem {Id = SidebarItemType.DesktopConfiguration, Title="Desktop Configuration" },
                new SidebarMenuItem {Id = SidebarItemType.Setting, Title="Setting" },
            };

            ListViewMenu.ItemsSource = menuItems;

            ListViewMenu.SelectedItem = menuItems[0];
            ListViewMenu.ItemSelected += async (sender, e) =>
            {
                if (e.SelectedItem == null)
                    return;

                var id = (int)((SidebarMenuItem)e.SelectedItem).Id;
                await RootPage.NavigateFromMenu(id);
            };
        }
    }
}