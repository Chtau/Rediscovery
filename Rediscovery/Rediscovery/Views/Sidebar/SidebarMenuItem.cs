using Rediscovery.Models;
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

    public class SidebarMenuItem : BaseModel
    {
        private SidebarItemType id;
        public SidebarItemType Id
        {
            get { return id; }
            set { SetProperty(ref id, value); }
        }

        private string title;
        public string Title
        {
            get { return title; }
            set { SetProperty(ref title, value); }
        }

        private string icon;
        public string Icon
        {
            get { return icon; }
            set { SetProperty(ref icon, value); }
        }

        private string iconNotActive;
        public string IconNotActive
        {
            get { return iconNotActive; }
            set { SetProperty(ref iconNotActive, value); }
        }

        private bool isActive;
        public bool IsActive
        {
            get { return isActive; }
            set { SetProperty(ref isActive, value); }
        }
    }
}
