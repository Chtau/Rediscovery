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
        public Models.CurrentConnection CurrentConnection { get; set; }
        public ObservableCollection<SharedBase.Device.DeviceInfo> ActiveDevices { get; set; } = new ObservableCollection<SharedBase.Device.DeviceInfo>();
        public ObservableCollection<SharedBase.Device.DeviceInfo> PendingDevices { get; set; } = new ObservableCollection<SharedBase.Device.DeviceInfo>();
        public ObservableCollection<SharedBase.Device.DeviceInfo> Devices { get; set; } = new ObservableCollection<SharedBase.Device.DeviceInfo>();
        public ObservableCollection<SharedBase.Device.FeatureDefinitionExtended> Features { get; set; } = new ObservableCollection<SharedBase.Device.FeatureDefinitionExtended>();
        public SharedBase.Connection.Manifest Manifest { get; private set; }

        public event EventHandler<SharedBase.Connection.Enums.ConnectionState> AfterConnecting;
        public event EventHandler DeviceCollectionChanged;
        public event EventHandler FeaturesCollectionChanged;
        public event EventHandler<Guid> PendingDeviceResolved;
        public event EventHandler<Guid> DeviceDeleted;
        public event EventHandler ManifestChanged;

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
                Manifest = args;
                ManifestChanged?.Invoke(this, EventArgs.Empty);
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
                DeviceCollectionChanged?.Invoke(this, EventArgs.Empty);
            };
            resourceConsumer.ReceiveDeleteDevicesResult += (obj, args) =>
            {
                if (args.result)
                    DeviceDeleted?.Invoke(this, args.deviceId);
            };
            resourceConsumer.ReceiveDevices += (obj, args) =>
            {
                Devices.Clear();
                foreach (var item in args)
                {
                    Devices.Add(item);
                }
                DeviceCollectionChanged?.Invoke(this, EventArgs.Empty);
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
                FeaturesCollectionChanged?.Invoke(this, EventArgs.Empty);
            };
            resourceConsumer.ReceivePendingDevices += (obj, args) =>
            {
                PendingDevices.Clear();
                foreach (var item in args)
                {
                    PendingDevices.Add(item);
                }
                DeviceCollectionChanged?.Invoke(this, EventArgs.Empty);
            };
            resourceConsumer.ReceiveResolvePendingDevicesResult += (obj, args) =>
            {
                PendingDeviceResolved?.Invoke(this, args.deviceId);
            };
            resourceConsumer.ReceiveUpdateDevices += (obj, args) =>
            {

            };
        }

        public bool TryConnect()
        {
            Disconnect();
            tokenSource = new System.Threading.CancellationTokenSource();

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

        public void TryResolvePendingDevice(Guid deviceId, bool resolve)
        {
            resourceConsumer.ResolvePendingDevice(CurrentConnection?.Token, deviceId, resolve);
        }

        public void TryDeleteDevice(Guid deviceId)
        {
            resourceConsumer.DeleteDevice(CurrentConnection?.Token, deviceId);
        }

        public void Disconnect()
        {
            ManagerConnectionState = new Models.ManagerConnectionState
            {
                CanConnect = SharedBase.Connection.Enums.AllowConnect.None,
                ConnectionState = SharedBase.Connection.Enums.ConnectionState.None
            };
            Manifest = null;
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

        public void SetConnectionValues(string ip, int port, string deviceIdentifier)
        {
            CurrentConnection = new Models.CurrentConnection
            {
                IP = ip,
                Port = port,
                DeviceIdentifier = deviceIdentifier
            };
        }
    }
}
