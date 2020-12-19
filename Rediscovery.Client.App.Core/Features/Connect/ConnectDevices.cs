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

        public event EventHandler<object> ConnectionHeartbeat;
        public event EventHandler<object> ConnectionCreated;
        public event EventHandler<object> ConnectionLost;

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
            try
            {
                var reply = _greetingConsumerService.GreetHost(connectionConfiguration.Address, connectionConfiguration.Port, 
                    _monitorSettings.CurrentValue.GreetingDeviceMessage, _monitorSettings.CurrentValue.TimeoutSeconds);
                if (reply.CanConnect == Shared.Base.Connection.Enums.AllowConnect.OK)
                {
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
                        _authenticationConsumerService.SendWelcome(_monitorSettings.CurrentValue.WelcomeDeviceMessage, deviceReply =>
                        {
                            if (deviceReply.State == Shared.Base.Connection.Enums.ConnectionState.OK)
                            {
                                /*OnSetData(item.Id, new ConnectConfigurationData
                                {
                                    Token = deviceReply.Token,
                                    PEM = reply.PEM,
                                    SSLPort = reply.SSLPort,
                                    UseSSL = reply.UseSSL,
                                    Port = item.Port,
                                });*/
                                _authenticationConsumerService.RequestManifest(deviceReply.Token, manifest => OnManifestReceived(connectionConfiguration, manifest, deviceReply));
                                //OnConnectLogger(connectionConfiguration, deviceReply.Token);
                                //OnConnectHeartbeat(connectionConfiguration, deviceReply.Token, item.Id);
                                //resultCallback?.Invoke(item, deviceReply.Token, deviceReply.State);
                            }
                            else
                            {
                                /*nextIndex++;
                                if (desktopConfigurations.Count > nextIndex)
                                {
                                    OnTryConnect(desktopConfigurations, resultCallback, nextIndex);
                                }
                                else
                                {
                                    if (item.ConnectionState == SharedBase.Connection.Enums.ConnectionState.None)
                                        item.ConnectionState = deviceReply.State;
                                    resultCallback?.Invoke(item, null, item.ConnectionState);
                                }*/
                            }
                        });
                    }
                    else
                    {
                        //resultCallback?.Invoke(null, null, SharedBase.Connection.Enums.ConnectionState.Error);
                    }
                }
                else
                {
                    /*if (reply.Offline)
                        resultCallback?.Invoke(null, null, SharedBase.Connection.Enums.ConnectionState.Offline);
                    else
                        resultCallback?.Invoke(null, null, SharedBase.Connection.Enums.ConnectionState.Error);*/
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

            }
            catch (Exception ex)
            {
                _logger.LogError(ex);
            }
        }
    }
}
