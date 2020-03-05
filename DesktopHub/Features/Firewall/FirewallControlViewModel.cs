using Avalonia;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace DesktopHub.Features.Firewall
{
    public class FirewallControlViewModel : BaseViewModel
    {
        bool serviceFWExists = false;
        public bool ServiceFWExists
        {
            get { return serviceFWExists; }
            set { SetProperty(ref serviceFWExists, value); }
        }

        bool discoveryFWExists = false;
        public bool DiscoveryFWExists
        {
            get { return discoveryFWExists; }
            set { SetProperty(ref discoveryFWExists, value); }
        }

        public FirewallControlViewModel()
        {

        }

        public void CreateServiceFW()
        {

        }

        public void CreateDiscoveryFW()
        {

        }

        private static void Elevate(string filePath, string parameters)
        {
            var SelfProc = new ProcessStartInfo
            {
                UseShellExecute = true,
                //WorkingDirectory = Environment.CurrentDirectory,
                FileName = filePath,
                Arguments = parameters,
                Verb = "runas"
            };
            try
            {
                Process.Start(SelfProc);
            }
            catch
            {
                System.Diagnostics.Debug.Print("Unable to elevate!" + Environment.NewLine);
            }
        }
    }
}
