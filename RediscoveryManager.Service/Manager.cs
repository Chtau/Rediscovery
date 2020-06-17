using CommunicationAuthenticationConsumer;
using CommunicationResourceConsumer;
using SharedBase.Logging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace RediscoveryManager.Service
{
    public class Manager : IManager
    {
        public Models.ManagerConnectionState ManagerConnectionState { get; private set; }
        public Models.CurrentConnection CurrentConnection { get; private set; }
        public ObservableCollection<SharedBase.Device.DeviceInfo> ActiveDevices { get; set; } = new ObservableCollection<SharedBase.Device.DeviceInfo>();
        public ObservableCollection<SharedBase.Device.DeviceInfo> PendingDevices { get; set; } = new ObservableCollection<SharedBase.Device.DeviceInfo>();
        public ObservableCollection<SharedBase.Device.DeviceInfo> Devices { get; set; } = new ObservableCollection<SharedBase.Device.DeviceInfo>();
        public ObservableCollection<SharedBase.Device.FeatureDefinitionExtended> Features { get; set; } = new ObservableCollection<SharedBase.Device.FeatureDefinitionExtended>();

        public event EventHandler<SharedBase.Connection.Enums.ConnectionState> AfterConnecting;

        private readonly IAuthenticationConsumerService authenticationConsumer;
        private readonly IGreetingConsumerService greetingConsumer;
        private readonly IResourceConsumerService resourceConsumer;

        private System.Threading.CancellationTokenSource tokenSource;

        public Manager(ILogger logger)
        {
            authenticationConsumer = new AuthenticationConsumerService(logger ?? SharedBase.Logging.DiagnosticsLoggerProvider.Instance);
            greetingConsumer = new GreetingConsumerService(logger ?? SharedBase.Logging.DiagnosticsLoggerProvider.Instance);
            resourceConsumer = new ResourceConsumerService(logger ?? SharedBase.Logging.DiagnosticsLoggerProvider.Instance);

            authenticationConsumer.ReceivedManifestReply += (obj, args) =>
            {
                
            };
            authenticationConsumer.ReceivedWelcomeReply += (obj, args) =>
            {
                ManagerConnectionState.ConnectionState = args.State;
                if (ManagerConnectionState.ConnectionState == SharedBase.Connection.Enums.ConnectionState.OK)
                {
                    CurrentConnection.Token = args.Token;
                    resourceConsumer.Connect(CurrentConnection.IP, CurrentConnection.PortSSL, CurrentConnection.Pem);
                    resourceConsumer.ListenDevices(CurrentConnection.Token, tokenSource);
                    resourceConsumer.ListenActiveDevices(CurrentConnection.Token, tokenSource);
                    resourceConsumer.ListenPendingDevices(CurrentConnection.Token, tokenSource);
                    resourceConsumer.ListenFeatures(CurrentConnection.Token, tokenSource);
                }
                AfterConnecting?.Invoke(this, ManagerConnectionState.ConnectionState);
            };
            resourceConsumer.ReceiveActiveDevices += (obj, args) =>
            {
                ActiveDevices.Clear();
                foreach (var item in args)
                {
                    ActiveDevices.Add(item);
                }
            };
            resourceConsumer.ReceiveDeleteDevicesResult += (obj, args) =>
            {

            };
            resourceConsumer.ReceiveDevices += (obj, args) =>
            {
                Devices.Clear();
                foreach (var item in args)
                {
                    Devices.Add(item);
                }
            };
            resourceConsumer.ReceiveFeatureDetailProfileDeleteResult += (obj, args) =>
            {

            };
            resourceConsumer.ReceiveFeatureDetailProfileSave += (obj, args) =>
            {

            };
            resourceConsumer.ReceiveFeatureDetails += (obj, args) =>
            {

            };
            resourceConsumer.ReceiveFeatureDetailSettingSave += (obj, args) =>
            {

            };
            resourceConsumer.ReceiveFeatures += (obj, args) =>
            {
                Features.Clear();
                foreach (var item in args)
                {
                    Features.Add(item);
                }
            };
            resourceConsumer.ReceivePendingDevices += (obj, args) =>
            {
                PendingDevices.Clear();
                foreach (var item in args)
                {
                    PendingDevices.Add(item);
                }
            };
            resourceConsumer.ReceiveResolvePendingDevicesResult += (obj, args) =>
            {

            };
            resourceConsumer.ReceiveUpdateDevices += (obj, args) =>
            {

            };
        }

        public bool TryConnect(string ip, int port, string deviceIdentifier)
        {
            Disconnect();
            tokenSource = new System.Threading.CancellationTokenSource();
            CurrentConnection = new Models.CurrentConnection
            {
                IP = ip,
                Port = port,
                DeviceIdentifier = deviceIdentifier
            };
            ManagerConnectionState = new Models.ManagerConnectionState
            {
                CanConnect = SharedBase.Connection.Enums.AllowConnect.None,
                ConnectionState = SharedBase.Connection.Enums.ConnectionState.None
            };

            var result = greetingConsumer.GreetHost(CurrentConnection.IP, CurrentConnection.Port, new SharedBase.Connection.GreetingDeviceMessage
            {
                DeviceIdentifier = CurrentConnection.DeviceIdentifier,
                DeviceName = "",
                DeviceType = "",
                Idiom = "",
                Manufacturer = "",
                Model = "",
                OSVersion = "",
                Platform = ""
            });
            ManagerConnectionState.CanConnect = result.CanConnect;
            if (ManagerConnectionState.CanConnect == SharedBase.Connection.Enums.AllowConnect.OK)
            {
                CurrentConnection.Pem = result.PEM;
                CurrentConnection.PortSSL = result.SSLPort;
                authenticationConsumer.Connect(CurrentConnection.IP, CurrentConnection.PortSSL, CurrentConnection.Pem);
                authenticationConsumer.SendWelcome(new SharedBase.Connection.WelcomeDeviceMessage
                {
                    DeviceIdentifier = CurrentConnection.DeviceIdentifier,
                });
            }
            return ManagerConnectionState.CanConnect == SharedBase.Connection.Enums.AllowConnect.OK;
        }

        public void Disconnect()
        {
            if (tokenSource != null)
            {
                try
                {
                    tokenSource.Cancel();
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.Fail(ex.ToString());
                }
            }
        }
    }
}
