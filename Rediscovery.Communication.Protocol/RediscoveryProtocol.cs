using Rediscovery.Communication.Protocol.Internal;
using Rediscovery.Communication.Protocol.Internal.Listener;
using Rediscovery.Communication.Protocol.Internal.Sender;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Rediscovery.Communication.Protocol
{
    // TODO: https://docs.microsoft.com/en-us/dotnet/framework/network-programming/asynchronous-server-socket-example
    // TODO: https://www.c-sharpcorner.com/article/building-a-blockchain-in-net-core-p2p-network/

    /* Bash
     * Listen via Netcat: nc -l -p 11000
     * Write via Netcat: echo 'test<EOF>' | sudo  netcat 192.168.1.102 11000
     */

    public class RediscoveryProtocol : IRediscoveryProtocol
    {
        private readonly IProtocolLogger _logger;
        private readonly IListener _discoveryListener;
        private readonly IListener _dataListener;
        private readonly IListener _lowDataListener;
        private readonly ISender _discoverySender;
        private readonly ISender _dataSender;
        private readonly ISender _lowDataSender;
        private Setting setting;
        

        public RediscoveryProtocol(IProtocolLogger protocolLogger = null)
        {
            _logger = protocolLogger ?? new Internal.ProtocolLogger();
            _discoveryListener = new DiscoveryListener(_logger);
            _dataListener = new DataListener(_logger);
            _lowDataListener = new LowDataListener(_logger);
            _discoverySender = new DiscoverySender(_logger);
            _dataSender = new DataSender(_logger);
            _lowDataSender = new LowDataSender(_logger);
        }

        public ConnectionState Connect(Connection connection)
        {
            try
            {

            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }
            return ConnectionState.Unkown;
        }

        public bool Disconnect()
        {
            throw new NotImplementedException();
        }

        public object GetConnectionInfo()
        {
            throw new NotImplementedException();
        }

        public object GetDiagnosticData()
        {
            throw new NotImplementedException();
        }

        public void Listen(Action<Transfer> receivedCallback)
        {
            try
            {
                _dataListener.StateCompleteListener((array) =>
                {
                    receivedCallback?.Invoke(new Transfer
                    {
                        Content = array
                    });
                });
                return;

                byte[] bytes = new Byte[setting.ListenPackageBytesData];
                // Establish the local endpoint for the socket.  
                // The DNS name of the computer  
                // running the listener is "host.contoso.com".  
                
                IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
                IPAddress ipAddress = ipHostInfo.AddressList[0];
                IPEndPoint localEndPoint = new IPEndPoint(ipAddress, 11000);
                Socket listener = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                
                //Socket listener = Network.CreateSocket(setting.ListenPortData);
                listener.Bind(localEndPoint);//listener.LocalEndPoint);// 
                listener.Listen(10);
                string data = null;
                // Start listening for connections.  
                while (true)
                {
                    Console.WriteLine("Waiting for a connection...");
                    // Program is suspended while waiting for an incoming connection.  
                    Socket handler = listener.Accept();
                    data = null;

                    // An incoming connection needs to be processed.  
                    while (true)
                    {
                        int bytesRec = handler.Receive(bytes);
                        data += Encoding.ASCII.GetString(bytes, 0, bytesRec);
                        if (data.IndexOf("<EOF>") > -1)
                        {
                            receivedCallback?.Invoke(new Transfer
                            {
                                Content = Encoding.ASCII.GetBytes(data)
                            });
                            break;
                        }
                    }

                    // Show the data on the console.  
                    Console.WriteLine("Text received : {0}", data);

                    // Echo the data back to the client.  
                    byte[] msg = Encoding.ASCII.GetBytes(data);

                    handler.Send(msg);
                    handler.Shutdown(SocketShutdown.Both);
                    handler.Close();
                }

            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }
        }

        public void LowLatencyListen(Action<Transfer> receivedCallback)
        {
            try
            {
                _lowDataListener.StateCompleteListener((array) =>
                {
                    receivedCallback?.Invoke(new Transfer
                    {
                        Content = array
                    });
                });
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }
        }

        public TransportState LowLatencySend(Transfer transfer)
        {
            throw new NotImplementedException();
        }

        public TransportState LowLatencyStream(Action<object> streamData)
        {
            throw new NotImplementedException();
        }

        public TransportState Send(Transfer transfer)
        {
            try
            {
                _dataSender.Send(transfer.Content, 11000);// setting.ListenPortData);
                return TransportState.Ok;
                byte[] bytes = new Byte[setting.SendPackageBytesData];
                // Establish the remote endpoint for the socket.  
                // This example uses port 11000 on the local computer.  
                
                IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
                IPAddress ipAddress = ipHostInfo.AddressList[0];
                IPEndPoint remoteEP = new IPEndPoint(ipAddress, 11000);// 13571);// 11000);
                
                // Create a TCP/IP  socket.  
                Socket sender =  new Socket(ipAddress.AddressFamily,
                    SocketType.Stream, ProtocolType.Tcp);
                    //Network.CreateSocket(setting.ListenPortData);/**/
                // Connect the socket to the remote endpoint. Catch any errors.  
                try
                {
                    sender.Connect(remoteEP);// sender.RemoteEndPoint);

                    Console.WriteLine("Socket connected to {0}",
                        sender.RemoteEndPoint.ToString());

                    // Encode the data string into a byte array.  
                    byte[] msg = Encoding.ASCII.GetBytes("This is a test<EOF>");
                    var data = msg.ToList();
                    data.AddRange(Network.EOFBytes);
                    // Send the data through the socket.  
                    int bytesSent = sender.Send(data.ToArray());

                    // Receive the response from the remote device.  
                    int bytesRec = sender.Receive(bytes);
                    Console.WriteLine("Echoed test = {0}",
                        Encoding.ASCII.GetString(bytes, 0, bytesRec));

                    // Release the socket.  
                    sender.Shutdown(SocketShutdown.Both);
                    sender.Close();

                }
                catch (ArgumentNullException ane)
                {
                    Console.WriteLine("ArgumentNullException : {0}", ane.ToString());
                }
                catch (SocketException se)
                {
                    Console.WriteLine("SocketException : {0}", se.ToString());
                }
                catch (Exception e)
                {
                    Console.WriteLine("Unexpected exception : {0}", e.ToString());
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }
            return TransportState.Unkown;
        }

        public TransportState Send(string ip)
        {
            try
            {
                byte[] bytes = new Byte[1024];
                // Establish the remote endpoint for the socket.  
                // This example uses port 11000 on the local computer.  
                IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
                IPAddress ipAddress = ipHostInfo.AddressList[0];
                IPEndPoint remoteEP = new IPEndPoint(IPAddress.Parse(ip), 11000);

                // Create a TCP/IP  socket.  
                Socket sender = new Socket(ipAddress.AddressFamily,
                    SocketType.Stream, ProtocolType.Tcp);

                // Connect the socket to the remote endpoint. Catch any errors.  
                try
                {
                    sender.Connect(remoteEP);

                    Console.WriteLine("Socket connected to {0}",
                        sender.RemoteEndPoint.ToString());

                    // Encode the data string into a byte array.  
                    byte[] msg = Encoding.ASCII.GetBytes("This is a test via IP<EOF>");

                    // Send the data through the socket.  
                    int bytesSent = sender.Send(msg);

                    // Receive the response from the remote device.  
                    int bytesRec = sender.Receive(bytes);
                    Console.WriteLine("Echoed test = {0}",
                        Encoding.ASCII.GetString(bytes, 0, bytesRec));

                    // Release the socket.  
                    sender.Shutdown(SocketShutdown.Both);
                    sender.Close();

                }
                catch (ArgumentNullException ane)
                {
                    Console.WriteLine("ArgumentNullException : {0}", ane.ToString());
                }
                catch (SocketException se)
                {
                    Console.WriteLine("SocketException : {0}", se.ToString());
                }
                catch (Exception e)
                {
                    Console.WriteLine("Unexpected exception : {0}", e.ToString());
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }
            return TransportState.Unkown;
        }

        public void Start(Setting setting)
        {
            try
            {
                this.setting = setting ?? new Setting();
                _discoverySender.Initialize(this.setting);
                _dataSender.Initialize(this.setting);
                _lowDataSender.Initialize(this.setting);
                _discoveryListener.Initialize(this.setting);
                _dataListener.Initialize(this.setting);
                _lowDataListener.Initialize(this.setting);
                
                // start listen for portocol data and discovery requests
                //OnListenDiscovery();
                OnListenData();
                //OnListenLowData();
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }
        }

        public TransportState Stream(Action<object> streamData)
        {
            throw new NotImplementedException();
        }

        private void OnListenDiscovery()
        {
            try
            {
                _discoveryListener.Start();
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }
        }

        private void OnListenData()
        {
            try
            {
                _dataListener.Start();
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }
        }

        private void OnListenLowData()
        {
            try
            {
                _lowDataListener.Start();
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }
        }
    }
}
