using Rediscovery.Communication.Consumer.Authentication;
using Rediscovery.Communication.Consumer.Heartbeat;
using Rediscovery.Communication.Consumer.Logger;
using Rediscovery.Communication.Consumer.Resource;
using Rediscovery.Shared.Logging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace Rediscovery.Client.App.Manager
{
    public class Manager : IManager
    {
        public Models.ManagerConnectionState ManagerConnectionState { get; private set; } = new Models.ManagerConnectionState();
        public Models.CurrentConnection CurrentConnection { get; set; } = new Models.CurrentConnection();
        public ObservableCollection<Shared.Base.Device.DeviceInfo> ActiveDevices { get; set; } = new ObservableCollection<Shared.Base.Device.DeviceInfo>();
        public ObservableCollection<Shared.Base.Device.DeviceInfo> PendingDevices { get; set; } = new ObservableCollection<Shared.Base.Device.DeviceInfo>();
        public ObservableCollection<Shared.Base.Device.DeviceInfo> Devices { get; set; } = new ObservableCollection<Shared.Base.Device.DeviceInfo>();
        public ObservableCollection<Shared.Base.Device.FeatureDefinitionExtended> Features { get; set; } = new ObservableCollection<Shared.Base.Device.FeatureDefinitionExtended>();
        public ObservableCollection<Shared.Base.Statistics.HeartbeatStatisticItem> HeartbeatStatistics { get; set; } = new ObservableCollection<Shared.Base.Statistics.HeartbeatStatisticItem>();
        public Shared.Base.Connection.Manifest Manifest { get; private set; }
        public ObservableCollection<Shared.Logging.Models.LoggerEntry> LoggerEntires { get; set; } = new ObservableCollection<Shared.Logging.Models.LoggerEntry>();

        public event EventHandler<Shared.Base.Connection.Enums.ConnectionState> AfterConnecting;
        public event EventHandler<Shared.Base.Connection.Enums.AllowConnect> GreetingsReply;
        public event EventHandler<RoundTripResult> RoundTripReceived;
        public event EventHandler DeviceCollectionChanged;
        public event EventHandler FeaturesCollectionChanged;
        public event EventHandler<Guid> PendingDeviceResolved;
        public event EventHandler<Guid> DeviceDeleted;
        public event EventHandler ManifestChanged;
        public event EventHandler HeartbeatStatisticsChanged;
        public event EventHandler LoggerEntiresChanged;
        public event EventHandler<Shared.Logging.Commands.LogCommandConfigResult> LoggerCommandExecuted;

        private readonly IAuthenticationConsumerService _authenticationConsumer;
        private readonly IGreetingConsumerService _greetingConsumer;
        private readonly IResourceConsumerService _resourceConsumer;
        private readonly IHeartbeatConsumer _heartbeatConsumer;
        private readonly ILoggerConsumer _loggerConsumer;

        private System.Threading.CancellationTokenSource tokenSource;

        public Manager()
        {
            Shared.Logging.EventLoggerProvider.Instance.LogNewEntry += Instance_LogNewEntry;
            _authenticationConsumer = new AuthenticationConsumerService(Shared.Logging.EventLoggerProvider.Instance);
            _greetingConsumer = new GreetingConsumerService(Shared.Logging.EventLoggerProvider.Instance);
            _resourceConsumer = new ResourceConsumerService(Shared.Logging.EventLoggerProvider.Instance);
            _heartbeatConsumer = new HeartbeatConsumer(Shared.Logging.EventLoggerProvider.Instance);
            _loggerConsumer = new LoggerConsumer();
            _authenticationConsumer.ReceivedManifestReply += (obj, args) =>
            {
                Manifest = args;
                ManifestChanged?.Invoke(this, EventArgs.Empty);
            };
            _authenticationConsumer.ReceivedWelcomeReply += (obj, args) =>
            {
                ManagerConnectionState.ConnectionState = args.State;
                if (ManagerConnectionState.ConnectionState == Shared.Base.Connection.Enums.ConnectionState.OK)
                {
                    CurrentConnection.Token = args.Token;
                    _authenticationConsumer.RequestManifest(CurrentConnection.Token);
                    _resourceConsumer.Connect(CurrentConnection.ConnectionConfiguration);
                    _resourceConsumer.ListenDevices(CurrentConnection.Token, tokenSource);
                    _resourceConsumer.ListenActiveDevices(CurrentConnection.Token, tokenSource);
                    _resourceConsumer.ListenPendingDevices(CurrentConnection.Token, tokenSource);
                    _resourceConsumer.ListenFeatures(CurrentConnection.Token, tokenSource);
                    _resourceConsumer.ListenHeartbeatStatistic(CurrentConnection.Token, tokenSource);
                    _resourceConsumer.ListenLoggerEntires(CurrentConnection.Token, tokenSource);
                    _heartbeatConsumer.Connect(CurrentConnection.ConnectionConfiguration);
                    _heartbeatConsumer.StartBeat(CurrentConnection.DeviceIdentifier, CurrentConnection.Token, tokenSource);
                    _loggerConsumer.Connect(CurrentConnection.ConnectionConfiguration);
                    _loggerConsumer.StartLogger(CurrentConnection.Token, tokenSource);
                }
                AfterConnecting?.Invoke(this, ManagerConnectionState.ConnectionState);
            };
            _resourceConsumer.ReceiveActiveDevices += (obj, args) =>
            {
                ActiveDevices.Clear();
                foreach (var item in args)
                {
                    ActiveDevices.Add(item);
                }
                DeviceCollectionChanged?.Invoke(this, EventArgs.Empty);
            };
            _resourceConsumer.ReceiveDeleteDevicesResult += (obj, args) =>
            {
                if (args.result)
                    DeviceDeleted?.Invoke(this, args.deviceId);
            };
            _resourceConsumer.ReceiveDevices += (obj, args) =>
            {
                Devices.Clear();
                foreach (var item in args)
                {
                    Devices.Add(item);
                }
                DeviceCollectionChanged?.Invoke(this, EventArgs.Empty);
            };
            /*_resourceConsumer.ReceiveFeatureDetails += (obj, args) =>
            {

            };*/
            _resourceConsumer.ReceiveFeatures += (obj, args) =>
            {
                Features.Clear();
                foreach (var item in args)
                {
                    Features.Add(item);
                }
                FeaturesCollectionChanged?.Invoke(this, EventArgs.Empty);
            };
            _resourceConsumer.ReceivePendingDevices += (obj, args) =>
            {
                PendingDevices.Clear();
                foreach (var item in args)
                {
                    PendingDevices.Add(item);
                }
                DeviceCollectionChanged?.Invoke(this, EventArgs.Empty);
            };
            _resourceConsumer.ReceiveResolvePendingDevicesResult += (obj, args) => PendingDeviceResolved?.Invoke(this, args.deviceId);
            /*_resourceConsumer.ReceiveUpdateDevices += (obj, args) =>
            {

            };*/
            _resourceConsumer.ReceiveHeartbeatStatistic += (obj, args) =>
            {
                HeartbeatStatistics.Clear();
                foreach (var item in args?.OrderByDescending(x => x.ResultReceived))
                {
                    HeartbeatStatistics.Add(item);
                }
                HeartbeatStatisticsChanged?.Invoke(this, EventArgs.Empty);
            };
            _resourceConsumer.ReceiveLoggerEntires += (obj, args) =>
            {
                LoggerEntires.Clear();
                foreach (var item in args?.OrderByDescending(x => x.Time))
                {
                    LoggerEntires.Add(item);
                }
                LoggerEntiresChanged?.Invoke(this, EventArgs.Empty);
            };
            _heartbeatConsumer.ReceivedBeatRoundtrip += (obj, args) => RoundTripReceived?.Invoke(this, args);
            _loggerConsumer.LoggerCommandExecuted += (obj, args) => LoggerCommandExecuted?.Invoke(this, args);
        }

        private void Instance_LogNewEntry(object sender, Shared.Logging.Models.LoggerEntry e)
        {
            RemoteLogEntry(e);
        }

        public bool TryConnect()
        {
            Disconnect();
            tokenSource = new System.Threading.CancellationTokenSource();

            var result = _greetingConsumer.GreetHost(CurrentConnection.IP, CurrentConnection.Port, new Shared.Base.Connection.GreetingDeviceMessage
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
            if (ManagerConnectionState.CanConnect == Shared.Base.Connection.Enums.AllowConnect.OK)
            {
                CurrentConnection.Pem = result.PEM;
                CurrentConnection.PortSSL = result.SSLPort;
                CurrentConnection.UseSSL = result.UseSSL;
                _authenticationConsumer.Connect(CurrentConnection.ConnectionConfiguration);
                _authenticationConsumer.SendWelcome(new Shared.Base.Connection.WelcomeDeviceMessage
                {
                    DeviceIdentifier = CurrentConnection.DeviceIdentifier,
                });
            }
            GreetingsReply?.Invoke(this, ManagerConnectionState.CanConnect);
            return ManagerConnectionState.CanConnect == Shared.Base.Connection.Enums.AllowConnect.OK;
        }

        public void TryResolvePendingDevice(Guid deviceId, bool resolve)
        {
            _resourceConsumer.ResolvePendingDevice(CurrentConnection?.Token, deviceId, resolve);
        }

        public void TryDeleteDevice(Guid deviceId)
        {
            _resourceConsumer.DeleteDevice(CurrentConnection?.Token, deviceId);
        }

        public void Disconnect()
        {
            ManagerConnectionState = new Models.ManagerConnectionState
            {
                CanConnect = Shared.Base.Connection.Enums.AllowConnect.None,
                ConnectionState = Shared.Base.Connection.Enums.ConnectionState.None
            };
            Manifest = null;
            Devices.Clear();
            ActiveDevices.Clear();
            PendingDevices.Clear();
            Features.Clear();
            HeartbeatStatistics.Clear();
            LoggerEntires.Clear();
            AfterConnecting?.Invoke(this, ManagerConnectionState.ConnectionState);
            GreetingsReply?.Invoke(this, ManagerConnectionState.CanConnect);
            DeviceCollectionChanged?.Invoke(this, EventArgs.Empty);
            FeaturesCollectionChanged?.Invoke(this, EventArgs.Empty);
            ManifestChanged?.Invoke(this, EventArgs.Empty);
            HeartbeatStatisticsChanged?.Invoke(this, EventArgs.Empty);
            LoggerEntiresChanged?.Invoke(this, EventArgs.Empty);
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

        public void RemoteLogEntry(Shared.Logging.Models.LoggerEntry loggerEntry)
        {
            if (_loggerConsumer?.IsConnect == true)
                _loggerConsumer.LogEntry(loggerEntry);
        }

        public bool RemoteLogExecuteCommand(Shared.Logging.Commands.LogCommandConfig logCommandConfig)
        {
            if (_loggerConsumer?.IsConnect == true)
            {
                _loggerConsumer.LoggerCommand(CurrentConnection.Token, logCommandConfig);
            }
            return false;
        }
    }
}
