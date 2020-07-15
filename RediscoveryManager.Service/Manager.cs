using CommunicationAuthenticationConsumer;
using CommunicationHeartbeatConsumer;
using CommunicationResourceConsumer;
using SharedBase.Logging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace RediscoveryManager.Service
{
    public class Manager : IManager
    {
        public Models.ManagerConnectionState ManagerConnectionState { get; private set; } = new Models.ManagerConnectionState();
        public Models.CurrentConnection CurrentConnection { get; set; } = new Models.CurrentConnection();
        public ObservableCollection<SharedBase.Device.DeviceInfo> ActiveDevices { get; set; } = new ObservableCollection<SharedBase.Device.DeviceInfo>();
        public ObservableCollection<SharedBase.Device.DeviceInfo> PendingDevices { get; set; } = new ObservableCollection<SharedBase.Device.DeviceInfo>();
        public ObservableCollection<SharedBase.Device.DeviceInfo> Devices { get; set; } = new ObservableCollection<SharedBase.Device.DeviceInfo>();
        public ObservableCollection<SharedBase.Device.FeatureDefinitionExtended> Features { get; set; } = new ObservableCollection<SharedBase.Device.FeatureDefinitionExtended>();
        public ObservableCollection<SharedBase.Statistics.HeartbeatStatisticItem> HeartbeatStatistics { get; set; } = new ObservableCollection<SharedBase.Statistics.HeartbeatStatisticItem>();
        public SharedBase.Connection.Manifest Manifest { get; private set; }

        public event EventHandler<SharedBase.Connection.Enums.ConnectionState> AfterConnecting;
        public event EventHandler<SharedBase.Connection.Enums.AllowConnect> GreetingsReply;
        public event EventHandler<RoundTripResult> RoundTripReceived;
        public event EventHandler DeviceCollectionChanged;
        public event EventHandler FeaturesCollectionChanged;
        public event EventHandler<Guid> PendingDeviceResolved;
        public event EventHandler<Guid> DeviceDeleted;
        public event EventHandler ManifestChanged;
        public event EventHandler HeartbeatStatisticsChanged;

        private readonly IAuthenticationConsumerService authenticationConsumer;
        private readonly IGreetingConsumerService greetingConsumer;
        private readonly IResourceConsumerService resourceConsumer;
        private readonly IHeartbeatConsumer heartbeatConsumer;

        private System.Threading.CancellationTokenSource tokenSource;

        public Manager(ILogger logger)
        {
            authenticationConsumer = new AuthenticationConsumerService(logger ?? SharedBase.Logging.DiagnosticsLoggerProvider.Instance);
            greetingConsumer = new GreetingConsumerService(logger ?? SharedBase.Logging.DiagnosticsLoggerProvider.Instance);
            resourceConsumer = new ResourceConsumerService(logger ?? SharedBase.Logging.DiagnosticsLoggerProvider.Instance);
            heartbeatConsumer = new HeartbeatConsumer(logger ?? SharedBase.Logging.DiagnosticsLoggerProvider.Instance);
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
                    authenticationConsumer.RequestManifest(CurrentConnection.Token);
                    resourceConsumer.Connect(CurrentConnection.IP, CurrentConnection.PortSSL, CurrentConnection.Pem);
                    resourceConsumer.ListenDevices(CurrentConnection.Token, tokenSource);
                    resourceConsumer.ListenActiveDevices(CurrentConnection.Token, tokenSource);
                    resourceConsumer.ListenPendingDevices(CurrentConnection.Token, tokenSource);
                    resourceConsumer.ListenFeatures(CurrentConnection.Token, tokenSource);
                    resourceConsumer.ListenHeartbeatStatistic(CurrentConnection.Token, tokenSource);
                    heartbeatConsumer.Connect(CurrentConnection.IP, CurrentConnection.PortSSL, CurrentConnection.Pem);
                    heartbeatConsumer.StartBeat(CurrentConnection.DeviceIdentifier, CurrentConnection.Token, tokenSource);
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
            resourceConsumer.ReceiveFeatureDetails += (obj, args) =>
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
            resourceConsumer.ReceiveHeartbeatStatistic += (obj, args) =>
            {
                HeartbeatStatistics.Clear();
                foreach (var item in args)
                {
                    HeartbeatStatistics.Add(item);
                }
                HeartbeatStatisticsChanged?.Invoke(this, EventArgs.Empty);
            };
            heartbeatConsumer.ReceivedBeatRoundtrip += (obj, args) =>
            {
                RoundTripReceived?.Invoke(this, args);
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
            GreetingsReply?.Invoke(this, ManagerConnectionState.CanConnect);
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
            Devices.Clear();
            ActiveDevices.Clear();
            PendingDevices.Clear();
            Features.Clear();
            HeartbeatStatistics.Clear();
            AfterConnecting?.Invoke(this, ManagerConnectionState.ConnectionState);
            GreetingsReply?.Invoke(this, ManagerConnectionState.CanConnect);
            DeviceCollectionChanged?.Invoke(this, EventArgs.Empty);
            FeaturesCollectionChanged?.Invoke(this, EventArgs.Empty);
            ManifestChanged?.Invoke(this, EventArgs.Empty);
            HeartbeatStatisticsChanged?.Invoke(this, EventArgs.Empty);
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
