using CommunicationBase;
using Grpc.Core;
using SharedBase.Connection;
using SharedBase.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CommunicationAuthenticationConsumer
{
    public class AuthenticationConsumerService : IAuthenticationConsumerService
    {
        public event EventHandler<WelcomeDeviceReply> ReceivedWelcomeReply;
        public event EventHandler<SharedBase.Connection.Manifest> ReceivedManifestReply;

        private Authentication.AuthentionExchange.AuthentionExchangeClient authenticationClient;
        private Manifest.ManifestExchange.ManifestExchangeClient manifestClient;
        private readonly ILogger _logger;

        public AuthenticationConsumerService(ILogger logger)
        {
            _logger = logger;
        }

        public void Connect(string ipAddress, int port, string certificatePEM)
        {
            var channelCredentials = new SslCredentials(certificatePEM);
            Channel channel = new Channel(ipAddress, port, channelCredentials);
            authenticationClient = new Authentication.AuthentionExchange.AuthentionExchangeClient(channel);
            manifestClient = new Manifest.ManifestExchange.ManifestExchangeClient(channel);
        }

        public void SendWelcome(WelcomeDeviceMessage message)
        {
            Task.Run(async () =>
            {
                try
                {
                    var cts = new CancellationTokenSource();
                    var msg = new Authentication.WelcomeDeviceMessage
                    {
                        DeviceIdentifier = message.DeviceIdentifier,
                        DeviceName = message.DeviceName,
                        DeviceType = message.DeviceType,
                        Idiom = message.Idiom,
                        Manufacturer = message.Manufacturer,
                        Model = message.Model,
                        OSVersion = message.OSVersion,
                        Platform = message.Platform
                    };
                    _logger.LogTrace("Consumer Welcome send Welcome");
                    var reply = await authenticationClient.WelcomeAsync(msg, cancellationToken: cts.Token);
                    _logger.LogTrace("Consumer Welcome reply received");
                    var replyMsg = new WelcomeDeviceReply
                    {
                        State = (Enums.ConnectionState)(int)reply.ConnectionState,
                        Token = reply.Token
                    };
                    ReceivedWelcomeReply?.Invoke(this, replyMsg);
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.Print(ex.ToString());
                }
            });
        }

        public void RequestManifest(string token)
        {
            Task.Run(async () =>
            {
                try
                {
                    _logger.LogTrace("Consumer request Manifest");
                    var cts = new CancellationTokenSource();
                    var meta = new Metadata();
                    meta.AddAuthorizationHeader(token);
                    var reply = await manifestClient.DeviceAsync(new Google.Protobuf.WellKnownTypes.Empty(), headers: meta, cancellationToken: cts.Token);
                    var replyMsg = new SharedBase.Connection.Manifest
                    {
                        AppMinimumVersion = SharedBase.Core.Version.ConvertTo(reply.AppMinimumVersion),
                        ClientName = reply.ClientName,
                        ClientVersion = SharedBase.Core.Version.ConvertTo(reply.ClientVersion),
                        SupportedFeatures = OnGetFeatures(reply.SupportedFeatures)
                    };
                    ReceivedManifestReply?.Invoke(this, replyMsg);
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.Print(ex.ToString());
                }
            });
        }

        private List<SharedBase.Device.FeatureDefinitionExtended> OnGetFeatures(IEnumerable<FeatureDefinitionExtended> featureDefinitionExtendeds)
        {
            var list = new List<SharedBase.Device.FeatureDefinitionExtended>();
            if (featureDefinitionExtendeds != null)
            {
                foreach (var item in featureDefinitionExtendeds)
                {
                    list.Add(new SharedBase.Device.FeatureDefinitionExtended
                    {
                        Author = item.Author,
                        ControlIntegrationPoint = (SharedBase.Device.IntegrationPoint)(int)item.ControlIntegrationPoint,
                        DisplayName = item.DisplayName,
                        Documentation = item.Documentation,
                        FeatureIntegrationPoint = (SharedBase.Device.IntegrationPoint)(int)item.FeatureIntegrationPoint,
                        HasProfiles = item.HasProfiles,
                        HasSettings = item.HasSettings,
                        Id = item.Id.SafeGuid(),
                        MinimalControlIntegrationPoint = SharedBase.Core.Version.ConvertTo(item.MinimalControlIntegrationPoint),
                        MinimalFeatureIntegrationPoint = SharedBase.Core.Version.ConvertTo(item.MinimalFeatureIntegrationPoint),
                        PluginDirectory = item.PluginDirectory,
                        ProfileUIElementName = item.ProfileUIElementName,
                        ProfileUIReadonly = item.ProfileUIReadonly,
                        SettingUIElementName = item.SettingUIElementName,
                        SettingUIReadonly = item.SettingUIReadonly,
                        Version = SharedBase.Core.Version.ConvertTo(item.Version),
                        Website = item.Website,
                    });
                }
            }
            return list;
        }
    }
}
