using Rediscovery.Client.App.Core.Dependency;
using Rediscovery.Client.App.Core.Features.Connect.Models;
using Rediscovery.Communication.Consumer.Authentication;
using Rediscovery.Shared.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace Rediscovery.Client.App.Core.Features.Connect
{
    public class ConnectDevices : IConnectDevices
    {
        private readonly ILogger _logger;
        private readonly ISettingValue<ConnectSetting> _monitorSettings;
        private readonly IGreetingConsumerService _greetingConsumerService;
        private readonly IAuthenticationConsumerService _authenticationConsumerService;

        public ConnectDevices(ILogger logger, ISettingValue<ConnectSetting> settingValue,
            IGreetingConsumerService greetingConsumerService, IAuthenticationConsumerService authenticationConsumerService)
        {
            _logger = logger;
            _greetingConsumerService = greetingConsumerService;
            _authenticationConsumerService = authenticationConsumerService;
            _monitorSettings = settingValue;
        }

        public event EventHandler<DeviceConnectionState> ConnectionStateChanged;

        public void Autoconnect()
        {
            try
            {

            }
            catch (Exception ex)
            {
                _logger.LogError(ex);
            }
        }

        public void Connect(ConnectionConfiguration connectionConfiguration)
        {
            try
            {
                OnConnect(connectionConfiguration);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex);
            }
        }

        public void Disconnect(Guid connectionConfigurationId)
        {
            try
            {

            }
            catch (Exception ex)
            {
                _logger.LogError(ex);
            }
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
                                _authenticationConsumerService.RequestManifest(deviceReply.Token, manifest => OnManifestReceived(connectionConfiguration, manifest, deviceReply));
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
            }
        }

        private void OnManifestReceived(ConnectionConfiguration connectionConfiguration, Rediscovery.Shared.Base.Connection.Manifest manifest, Shared.Base.Connection.WelcomeDeviceReply welcomeDeviceReply)
        {
            try
            {
                ConnectionStateChanged?.Invoke(this, new DeviceConnectionState
                {
                    Change = DeviceConnectionState.StateChange.ManifestReceived,
                    Configuration = connectionConfiguration,
                    CurrentState = welcomeDeviceReply.State,
                    DeviceManifest = manifest,
                    Token = welcomeDeviceReply.Token
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex);
            }
        }
    }
}
