using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace DesktopHub.Info
{
    public class ServiceInfoViewModel : BaseViewModel
    {
        public const string ServiceInfoArgStart = "--serviceinfo";

        string ipAddr = string.Empty;
        public string IpAddr
        {
            get { return ipAddr; }
            set { SetProperty(ref ipAddr, value); }
        }

        public ServiceInfoViewModel()
        {
            OnGetIpAddr();
        }

        private void OnGetIpAddr()
        {
            string localIP;
            using (Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, 0))
            {
                socket.Connect("8.8.8.8", 65530);
                IPEndPoint endPoint = socket.LocalEndPoint as IPEndPoint;
                localIP = endPoint.Address.ToString();
            }
            IpAddr = localIP;
        }
    }
}
