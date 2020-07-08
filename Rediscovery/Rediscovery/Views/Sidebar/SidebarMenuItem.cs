using System;
using System.Collections.Generic;
using System.Text;

namespace Rediscovery.Views.Sidebar
{
    public enum SidebarItemType
    {
        Home,
        Feature,
        DesktopConfiguration,
        Setting
    }

    public class SidebarMenuItem
    {
        public SidebarItemType Id { get; set; }

        public string Title { get; set; }
    }
}
