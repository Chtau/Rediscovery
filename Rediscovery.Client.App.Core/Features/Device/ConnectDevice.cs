using Rediscovery.Client.App.Core.Dependency;
using Rediscovery.Client.App.Core.Features.Device.Models;
using Rediscovery.Communication.Consumer.Authentication;
using Rediscovery.Communication.Consumer.Feature;
using Rediscovery.Communication.Consumer.Heartbeat;
using Rediscovery.Communication.Consumer.Logger;
using Rediscovery.Shared.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Rediscovery.Client.App.Core.Features.Device
{
    public class ConnectDevice : IConnectDevice
    {
        private readonly ILogger _logger;
        private readonly ISettingValue<ConnectSetting> _monitorSettings;

        private readonly IGreetingConsumerService _greetingConsumerService;
        private readonly IAuthenticationConsumerService _authenticationConsumerService;
        private readonly IFeatureConsumerService _featureConsumerService;
        private readonly IHeartbeatConsumer _heartbeatConsumer;
        private readonly ILoggerConsumer _loggerConsumer;
        private bool disposedValue;
        private Communication.Base.ConsumerConnectionConfiguration consumerConfig;
        private string authenticationToken;
        private CancellationTokenSource cancelationTokenSource;

        public ConnectionConfiguration ConnectionConfiguration { get; private set; }

        public ConnectDevice(ILogger logger, ISettingValue<ConnectSetting> settingValue)
        {
            _logger = logger;
            _monitorSettings = settingValue;
            _greetingConsumerService = Resolver.Scope<IGreetingConsumerService>();
            _authenticationConsumerService = Resolver.Scope<IAuthenticationConsumerService>();
            _heartbeatConsumer = Resolver.Scope<IHeartbeatConsumer>();
            _heartbeatConsumer.ReceivedBeatRoundtrip += _heartbeatConsumer_ReceivedBeatRoundtrip;
            _loggerConsumer = Resolver.Scope<ILoggerConsumer>();
            _featureConsumerService = Resolver.Scope<IFeatureConsumerService>();
        }

        public event EventHandler<DeviceConnectionState> ConnectionStateChanged;
        public event EventHandler<HeartbeatResult> HeartbeatReceived;

        public void SetConfiguration(ConnectionConfiguration connectionConfiguration)
        {
            try
            {
                ConnectionConfiguration = connectionConfiguration;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex);
            }
        }

        public bool Probe()
        {
            DeviceConnectionState deviceConnectionState = new DeviceConnectionState
            {
                Change = DeviceConnectionState.StateChange.Probe,
                Configuration = ConnectionConfiguration
            };
            try
            {
                _logger.LogTrace($"[Probe] Start probe host. Config:{ConnectionConfiguration} \r\nDeviceMessage:\r\n{Newtonsoft.Json.JsonConvert.SerializeObject(_monitorSettings.CurrentValue.GreetingDeviceMessage)}\r\n");
                ConnectionStateChanged?.Invoke(this, deviceConnectionState);
                var reply = _greetingConsumerService.GreetHost(ConnectionConfiguration.Address, ConnectionConfiguration.Port,
                    _monitorSettings.CurrentValue.GreetingDeviceMessage, _monitorSettings.CurrentValue.TimeoutSeconds);
                deviceConnectionState.Change = DeviceConnectionState.StateChange.ProbeReply;
                deviceConnectionState.Allowed = reply.CanConnect;
                ConnectionStateChanged?.Invoke(this, deviceConnectionState);
                _logger.LogTrace($"[Probe] Probe host reply. Config:{ConnectionConfiguration} \r\nReply:\r\n{Newtonsoft.Json.JsonConvert.SerializeObject(reply)}\r\n");
                if (reply.CanConnect == Shared.Base.Connection.Enums.AllowConnect.OK)
                    return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex);
                deviceConnectionState.Change = DeviceConnectionState.StateChange.ProbeReply;
                deviceConnectionState.Allowed = Shared.Base.Connection.Enums.AllowConnect.Error;
                ConnectionStateChanged?.Invoke(this, deviceConnectionState);
            }
            return false;
        }

        public void Connect()
        {
            try
            {
                OnConnect(ConnectionConfiguration);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex);
            }
        }

        public bool Disconnect()
        {
            try
            {
                return OnDisconnect();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex);
            }
            return false;
        }

        private bool OnDisconnect()
        {
            var retVal = true;
            try
            {
                try
                {
                    authenticationToken = null;
                    cancelationTokenSource?.Cancel();
                } catch (Exception ex)
                {
                    _logger.LogError(ex);
                }
                if (!_greetingConsumerService.Disconnect())
                    retVal = false;
                if (!_authenticationConsumerService.Disconnect())
                    retVal = false;
                if (!_heartbeatConsumer.Disconnect())
                    retVal = false;
                if (!_featureConsumerService.Disconnect())
                    retVal = false;
                if (!_loggerConsumer.Disconnect())
                    retVal = false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex);
            }
            return retVal;
        }

        private void OnConnect(ConnectionConfiguration connectionConfiguration)
        {
            authenticationToken = null;
            DeviceConnectionState deviceConnectionState = new DeviceConnectionState
            {
                Change = DeviceConnectionState.StateChange.GreetHostReply,
                Configuration = connectionConfiguration
            };
            try
            {
                _logger.LogTrace($"[Greeting] Start greet host. Config:{ConnectionConfiguration} \r\nDeviceMessage:\r\n{Newtonsoft.Json.JsonConvert.SerializeObject(_monitorSettings.CurrentValue.GreetingDeviceMessage)}\r\n");
                ConnectionStateChanged?.Invoke(this, deviceConnectionState);

                var reply = _greetingConsumerService.GreetHost(connectionConfiguration.Address, connectionConfiguration.Port, 
                    _monitorSettings.CurrentValue.GreetingDeviceMessage, _monitorSettings.CurrentValue.TimeoutSeconds);

                deviceConnectionState.Change = DeviceConnectionState.StateChange.GreetHostReply;
                deviceConnectionState.Allowed = reply.CanConnect;
                _logger.LogTrace($"[Greeting] Greet host reply received. Config:{ConnectionConfiguration} \r\nReply:\r\n{Newtonsoft.Json.JsonConvert.SerializeObject(reply)}\r\n");
                ConnectionStateChanged?.Invoke(this, deviceConnectionState);

                if (reply.CanConnect == Shared.Base.Connection.Enums.AllowConnect.OK)
                {
                    deviceConnectionState.Change = DeviceConnectionState.StateChange.Connect;
                    ConnectionStateChanged?.Invoke(this, deviceConnectionState);
                    consumerConfig = new Communication.Base.ConsumerConnectionConfiguration
                    {
                        UseSSL = reply.UseSSL,
                        CertificatePEM = reply.PEM,
                        IPAddress = connectionConfiguration.Address,
                        Port = connectionConfiguration.Port,
                        SSLPort = reply.SSLPort
                    };
                    _logger.LogTrace($"[Authentication] Connect to remote Address. Config:{ConnectionConfiguration} \r\nConsumerConfig:\r\n{Newtonsoft.Json.JsonConvert.SerializeObject(consumerConfig)}\r\n");
                    if (_authenticationConsumerService.Connect(consumerConfig))
                    {
                        deviceConnectionState.Change = DeviceConnectionState.StateChange.ConnectReply;
                        deviceConnectionState.CurrentStateConnectReply = DeviceConnectionState.StateConnectReply.Ok;
                        _logger.LogTrace($"[Authentication] Reply received. Config:{ConnectionConfiguration} \r\nReply:\r\n{Newtonsoft.Json.JsonConvert.SerializeObject(deviceConnectionState)}\r\n");
                        ConnectionStateChanged?.Invoke(this, deviceConnectionState);

                        deviceConnectionState.Change = DeviceConnectionState.StateChange.Welcome;
                        _logger.LogTrace($"[Welcome] Send welcome request message. Config:{ConnectionConfiguration} \r\nWelcomeMessage:\r\n{Newtonsoft.Json.JsonConvert.SerializeObject(_monitorSettings.CurrentValue.WelcomeDeviceMessage)}\r\n");
                        ConnectionStateChanged?.Invoke(this, deviceConnectionState);
                        _authenticationConsumerService.SendWelcome(_monitorSettings.CurrentValue.WelcomeDeviceMessage, deviceReply =>
                        {
                            authenticationToken = deviceReply.Token;
                            deviceConnectionState.Change = DeviceConnectionState.StateChange.WelcomeReply;
                            deviceConnectionState.CurrentState = deviceReply.State;
                            deviceConnectionState.Token = authenticationToken;
                            _logger.LogTrace($"[Welcome] Reply received. Config:{ConnectionConfiguration} \r\nReply:\r\n{Newtonsoft.Json.JsonConvert.SerializeObject(deviceReply)}\r\n");
                            ConnectionStateChanged?.Invoke(this, deviceConnectionState);

                            if (deviceReply.State == Shared.Base.Connection.Enums.ConnectionState.OK)
                            {
                                _authenticationConsumerService.RequestManifest(authenticationToken, manifest => OnManifestReceived(deviceConnectionState, connectionConfiguration, manifest, deviceReply));
                            }
                        });
                    }
                    else
                    {
                        deviceConnectionState.Change = DeviceConnectionState.StateChange.ConnectReply;
                        deviceConnectionState.CurrentStateConnectReply = DeviceConnectionState.StateConnectReply.Failed;
                        _logger.LogTrace($"[Authentication] Reply failed. Config:{ConnectionConfiguration} \r\nReply:\r\n{Newtonsoft.Json.JsonConvert.SerializeObject(deviceConnectionState)}\r\n");
                        ConnectionStateChanged?.Invoke(this, deviceConnectionState);
                    }
                }
            } catch (Exception ex)
            {
                _logger.LogError(ex);
                deviceConnectionState.Allowed = Shared.Base.Connection.Enums.AllowConnect.Error;
                ConnectionStateChanged?.Invoke(this, deviceConnectionState);
            }
        }

        private void OnManifestReceived(DeviceConnectionState deviceConnectionState, ConnectionConfiguration connectionConfiguration, Rediscovery.Shared.Base.Connection.Manifest manifest, Shared.Base.Connection.WelcomeDeviceReply welcomeDeviceReply)
        {
            try
            {
                deviceConnectionState.Change = DeviceConnectionState.StateChange.ManifestReceived;
                deviceConnectionState.DeviceManifest = manifest;
                _logger.LogTrace($"[Manifest] Received data. Config:{connectionConfiguration} \r\nManifest:\r\n{Newtonsoft.Json.JsonConvert.SerializeObject(manifest)}\r\n");
                ConnectionStateChanged?.Invoke(this, deviceConnectionState);

                OnInitServicesAfterConnect();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex);
            }
        }

        private void OnInitServicesAfterConnect()
        {
            try
            {
                // TODO: hook to all other services (consumer)
                cancelationTokenSource = new CancellationTokenSource();

                try
                {
                    if (_heartbeatConsumer.Connect(consumerConfig))
                        _heartbeatConsumer.StartBeat(OnGetDeviceIdentifier(), authenticationToken, cancelationTokenSource);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex);
                }     
            }
            catch (Exception ex)
            {
                _logger.LogError(ex);
            }
        }

        private void _heartbeatConsumer_ReceivedBeatRoundtrip(object sender, RoundTripResult e)
        {
            try
            {
                HeartbeatReceived?.Invoke(this, new HeartbeatResult(ConnectionConfiguration, e.OK, e.PingPongTime, e.PingStartDatetimeUTC));
                if (e.OK)
                    _logger.LogTrace($"[Heartbeat] round trip received. ({e.PingPongTime?.TotalMilliseconds} ms Config:{ConnectionConfiguration})");
                else
                    _logger.LogTrace($"[Heartbeat] round trip not OK received. Config:{ConnectionConfiguration}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex);
            }
        }

        private string OnGetDeviceIdentifier()
        {
            return ConnectionConfiguration?.Id.ToString();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    if (_heartbeatConsumer != null)
                        _heartbeatConsumer.ReceivedBeatRoundtrip -= _heartbeatConsumer_ReceivedBeatRoundtrip;
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                disposedValue = true;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~ConnectDevice()
        // {
        //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
