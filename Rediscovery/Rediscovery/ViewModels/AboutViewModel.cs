using System;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Windows.Input;

using Xamarin.Forms;

namespace Rediscovery.ViewModels
{
    public class AboutViewModel : BaseViewModel
    {
        public Services.INetworkDiscovery netService => DependencyService.Get<Services.INetworkDiscovery>() ?? new Services.NetworkDiscovery();

        public AboutViewModel()
        {
            Title = "About";

            OpenWebCommand = new Command(() => Device.OpenUri(new Uri("https://xamarin.com/platform")));
            TestCommand = new Command(() =>
            {
                using (var client = new UdpClient())
                {
                    client.EnableBroadcast = true;
                    //var endpoint = new IPEndPoint(IPAddress.Broadcast, 15000);
                    var endpoint = new IPEndPoint(IPAddress.Parse("192.168.1.160"), 15000); // works with correct ip
                    //var endpoint = new IPEndPoint(IPAddress.Parse("255.255.255.0"), 15000);
                    //var endpoint = new IPEndPoint(IPAddress.Broadcast, 15000);
                    var message = Encoding.ASCII.GetBytes("Hello World - " + DateTime.Now.ToString());
                    client.Send(message, message.Length, endpoint);
                    client.Close();
                }

                OnBrodcast();


                /*try
                {
                    IPEndPoint targetEndPoint = new IPEndPoint(IPAddress.Broadcast, 15000);
                    MyUdpClient sendUdpClient = new MyUdpClient();//localEndPoint
                    var message = Encoding.ASCII.GetBytes("Hello World - " + DateTime.Now.ToString());
                    sendUdpClient.Send(message, message.Length, targetEndPoint);
                } catch (Exception ex)
                {
                    System.Diagnostics.Debug.Print(ex.ToString());
                }*/
                //IPEndPoint localEndPoint = new IPEndPoint(IPAddress.Parse(LocalIP), 0);
                
                //int numBytesSent = sendUdpClient.Send(CombineHeaderBody, CombineHeaderBody.Length, targetEndPoint);

                /*using (var client = new UdpClient())
                {
                    client.EnableBroadcast = true;
                    //var endpoint = new IPEndPoint(IPAddress.Broadcast, 15000);
                    //var endpoint = new IPEndPoint(IPAddress.Parse("192.168.1.160"), 15000); // works with correct ip
                    //var endpoint = new IPEndPoint(IPAddress.Parse("255.255.255.0"), 15000);
                    var endpoint = new IPEndPoint(IPAddress.Broadcast, 15000);
                    var message = Encoding.ASCII.GetBytes("Hello World - " + DateTime.Now.ToString());
                    client.Send(message, message.Length, endpoint);
                    client.Close();
                }*/
                //netService.Send();
            });
        }

        public ICommand OpenWebCommand { get; }
        public ICommand TestCommand { get; }

        private void OnBrodcast()
        {
            //DevicesList = new List<MyDevice>();
            byte[] data = Encoding.ASCII.GetBytes("Hello World - " + DateTime.Now.ToString()); //new byte[2]; //broadcast data
            //data[0] = 0x0A;
            //data[1] = 0x60;

            IPEndPoint ip = new IPEndPoint(IPAddress.Broadcast, 45000); //braodcast IP address, and corresponding port

            var nics = System.Net.NetworkInformation.NetworkInterface.GetAllNetworkInterfaces(); //get all network interfaces of the computer

            foreach (var adapter in nics)
            {
                // Only select interfaces that are Ethernet type and support IPv4 (important to minimize waiting time)
                //if (adapter.NetworkInterfaceType != NetworkInterfaceType.Ethernet) { continue; }
                //if (adapter.Supports(NetworkInterfaceComponent.IPv4) == false) { continue; }
                try
                {
                    IPInterfaceProperties adapterProperties = adapter.GetIPProperties();
                    foreach (var ua in adapterProperties.UnicastAddresses)
                    {
                        if (ua.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                        {
                            var localEndpoint = new IPEndPoint(ua.Address, 15000);
                            using (var client = new UdpClient(localEndpoint))
                            {
                                client.EnableBroadcast = true;
                                //var endpoint = new IPEndPoint(IPAddress.Broadcast, 15000);
                                //var endpoint = new IPEndPoint(IPAddress.Parse("192.168.1.160"), 15000); // works with correct ip
                                var endpoint = new IPEndPoint(IPAddress.Parse("255.255.255.255"), 15000);
                                
                                var message = Encoding.ASCII.GetBytes("1 => Hello World - " + DateTime.Now.ToString());
                                //client.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Broadcast, 1);
                                //client.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.DontRoute, 1);
                                client.Send(message, message.Length, endpoint);
                                client.Close();
                            }

                            /*
                            //SEND BROADCAST IN THE ADAPTER
                            //1) Set the socket as UDP Client
                            Socket bcSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp); //broadcast socket
                                                                                                                          //2) Set socker options
                            bcSocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Broadcast, 1);
                            bcSocket.ReceiveTimeout = 200; //receive timout 200ms
                                                           //3) Bind to the current selected adapter
                            IPEndPoint myLocalEndPoint = new IPEndPoint(ua.Address, 15000);
                            bcSocket.Bind(myLocalEndPoint);
                            //4) Send the broadcast data
                            bcSocket.SendTo(data, ip);
                            */
                            //RECEIVE BROADCAST IN THE ADAPTER
                            /*int BUFFER_SIZE_ANSWER = 1024;
                            byte[] bufferAnswer = new byte[BUFFER_SIZE_ANSWER];
                            do
                            {
                                try
                                {
                                    bcSocket.Receive(bufferAnswer);
                                    //DevicesList.Add(GetMyDevice(bufferAnswer)); //Corresponding functions to get the devices information. Depends on the application.
                                }
                                catch { break; }

                            } while (bcSocket.ReceiveTimeout != 0); //fixed receive timeout for each adapter that supports our broadcast
                            */
                            //bcSocket.Close();
                        }
                    }
                }
                catch { }
            }
        }
    }

    public class MyUdpClient : UdpClient
    {
        public MyUdpClient() : base()
        {
            //Calls the protected Client property belonging to the UdpClient base class.
            Socket s = this.Client;
            //Uses the Socket returned by Client to set an option that is not available using UdpClient.
            s.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Broadcast, 1);
            s.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.DontRoute, 1);
        }

        public MyUdpClient(IPEndPoint ipLocalEndPoint) : base(ipLocalEndPoint)
        {
            //Calls the protected Client property belonging to the UdpClient base class.
            Socket s = this.Client;
            //Uses the Socket returned by Client to set an option that is not available using UdpClient.
            s.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Broadcast, 1);
            s.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.DontRoute, 1);
        }

    }
}