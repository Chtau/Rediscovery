using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace DesktopHub.Info
{
    public class ServiceInfoViewModel : BaseViewModel
    {
        public const string ServiceInfoArgStart = "--serviceinfo:";

        string ipAddr = string.Empty;
        public string IpAddr
        {
            get { return ipAddr; }
            set { SetProperty(ref ipAddr, value); }
        }

        public ServiceInfoViewModel(string[] args)
        {
            if (args != null)
            {
                if (args.Any(x => x.StartsWith(ServiceInfoArgStart, StringComparison.OrdinalIgnoreCase)))
                {
                    var codeArg = args.First(x => x.StartsWith(ServiceInfoArgStart, StringComparison.OrdinalIgnoreCase));
                    var vals = codeArg.Split(':');
                    IpAddr = vals[1].Trim();
                }
            }
        }
    }
}
