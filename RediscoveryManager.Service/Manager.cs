using CommunicationAuthenticationConsumer;
using CommunicationResourceConsumer;
using SharedBase.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace RediscoveryManager.Service
{
    public class Manager : IManager
    {
        public Models.ManagerConnectionState ManagerConnectionState { get; private set; }
        public Models.CurrentConnection CurrentConnection { get; private set; }

        private readonly IAuthenticationConsumerService authenticationConsumer;
        private readonly IGreetingConsumerService greetingConsumer;
        private readonly IResourceConsumerService resourceConsumer;

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
                }
            };
            resourceConsumer.ReceiveActiveDevices += (obj, args) =>
            {

            };
            resourceConsumer.ReceiveDeleteDevicesResult += (obj, args) =>
            {

            };
            resourceConsumer.ReceiveDevices += (obj, args) =>
            {

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

            };
            resourceConsumer.ReceivePendingDevices += (obj, args) =>
            {

            };
            resourceConsumer.ReceiveResolvePendingDevicesResult += (obj, args) =>
            {

            };
            resourceConsumer.ReceiveUpdateDevices += (obj, args) =>
            {

            };
        }

        public void Connect(string ip, int port, string deviceIdentifier)
        {
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
        }
    }
}
