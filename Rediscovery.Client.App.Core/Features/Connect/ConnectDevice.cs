using Rediscovery.Client.App.Core.Dependency;
using Rediscovery.Client.App.Core.Features.Connect.Models;
using Rediscovery.Communication.Consumer.Authentication;
using Rediscovery.Communication.Consumer.Feature;
using Rediscovery.Communication.Consumer.Heartbeat;
using Rediscovery.Communication.Consumer.Logger;
using Rediscovery.Shared.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace Rediscovery.Client.App.Core.Features.Connect
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

        public ConnectionConfiguration ConnectionConfiguration { get; private set; }

        public ConnectDevice(ILogger logger, ISettingValue<ConnectSetting> settingValue)
        {
            _logger = logger;
            _monitorSettings = settingValue;
            _greetingConsumerService = Resolver.Scope<IGreetingConsumerService>();
            _authenticationConsumerService = Resolver.Scope<IAuthenticationConsumerService>();
            _heartbeatConsumer = Resolver.Scope<IHeartbeatConsumer>();
            _loggerConsumer = Resolver.Scope<ILoggerConsumer>();
            _featureConsumerService = Resolver.Scope<IFeatureConsumerService>();
        }

        public event EventHandler<DeviceConnectionState> ConnectionStateChanged;

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
                ConnectionStateChanged?.Invoke(this, deviceConnectionState);
                var reply = _greetingConsumerService.GreetHost(ConnectionConfiguration.Address, ConnectionConfiguration.Port,
                    _monitorSettings.CurrentValue.GreetingDeviceMessage, _monitorSettings.CurrentValue.TimeoutSeconds);
                deviceConnectionState.Change = DeviceConnectionState.StateChange.ProbeReply;
                deviceConnectionState.Allowed = reply.CanConnect;
                ConnectionStateChanged?.Invoke(this, deviceConnectionState);
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
            DeviceConnectionState deviceConnectionState = new DeviceConnectionState
            {
                Change = DeviceConnectionState.StateChange.GreetHostReply,
                Configuration = connectionConfiguration
            };
            try
            {
                ConnectionStateChanged?.Invoke(this, deviceConnectionState);

                var reply = _greetingConsumerService.GreetHost(connectionConfiguration.Address, connectionConfiguration.Port, 
                    _monitorSettings.CurrentValue.GreetingDeviceMessage, _monitorSettings.CurrentValue.TimeoutSeconds);

                deviceConnectionState.Change = DeviceConnectionState.StateChange.GreetHostReply;
                deviceConnectionState.Allowed = reply.CanConnect;
                ConnectionStateChanged?.Invoke(this, deviceConnectionState);

                if (reply.CanConnect == Shared.Base.Connection.Enums.AllowConnect.OK)
                {
                    deviceConnectionState.Change = DeviceConnectionState.StateChange.Connect;
                    ConnectionStateChanged?.Invoke(this, deviceConnectionState);
                    Communication.Base.ConsumerConnectionConfiguration consumerConfig = new Communication.Base.ConsumerConnectionConfiguration
                    {
                        UseSSL = reply.UseSSL,
                        CertificatePEM = reply.PEM,
                        IPAddress = connectionConfiguration.Address,
                        Port = connectionConfiguration.Port,
                        SSLPort = reply.SSLPort
                    };
                    if (_authenticationConsumerService.Connect(consumerConfig))
                    {
                        deviceConnectionState.Change = DeviceConnectionState.StateChange.ConnectReply;
                        deviceConnectionState.CurrentStateConnectReply = DeviceConnectionState.StateConnectReply.Ok;
                        ConnectionStateChanged?.Invoke(this, deviceConnectionState);

                        deviceConnectionState.Change = DeviceConnectionState.StateChange.Welcome;
                        ConnectionStateChanged?.Invoke(this, deviceConnectionState);
                        _authenticationConsumerService.SendWelcome(_monitorSettings.CurrentValue.WelcomeDeviceMessage, deviceReply =>
                        {
                            deviceConnectionState.Change = DeviceConnectionState.StateChange.WelcomeReply;
                            deviceConnectionState.CurrentState = deviceReply.State;
                            deviceConnectionState.Token = deviceReply.Token;
                            ConnectionStateChanged?.Invoke(this, deviceConnectionState);

                            if (deviceReply.State == Shared.Base.Connection.Enums.ConnectionState.OK)
                            {
                                _authenticationConsumerService.RequestManifest(deviceReply.Token, manifest => OnManifestReceived(deviceConnectionState, connectionConfiguration, manifest, deviceReply));
                            }
                        });
                    }
                    else
                    {
                        deviceConnectionState.Change = DeviceConnectionState.StateChange.ConnectReply;
                        deviceConnectionState.CurrentStateConnectReply = DeviceConnectionState.StateConnectReply.Failed;
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
                ConnectionStateChanged?.Invoke(this, deviceConnectionState);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex);
            }
        }
    }
}
