using Grpc.Core;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CommunicationAuthenticationConsumer
{
    public class AuthenticationConsumerService : IAuthenticationConsumerService
    {
        public event EventHandler<SharedCoreModels.WelcomeDeviceReply> ReceivedWelcomeReply;
        public event EventHandler<SharedCoreModels.Manifest> ReceivedManifestReply;

        private Authentication.AuthentionExchange.AuthentionExchangeClient authenticationClient;
        private Manifest.ManifestExchange.ManifestExchangeClient manifestClient;

        public void Connect(string ipAddress, int port, string certificatePEM)
        {
            var channelCredentials = new SslCredentials(certificatePEM);
            Channel channel = new Channel(ipAddress, port, channelCredentials);
            authenticationClient = new Authentication.AuthentionExchange.AuthentionExchangeClient(channel);
            manifestClient = new Manifest.ManifestExchange.ManifestExchangeClient(channel);
        }

        public void SendWelcome(SharedCoreModels.WelcomeDeviceMessage message)
        {
            Task.Run(() =>
            {
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
                using (var call = authenticationClient.Welcome(msg))
                {
                    var readTask = Task.Run(async () =>
                    {
                        await foreach (var message in call.ResponseStream.ReadAllAsync())
                        {
                            var replyMsg = new SharedCoreModels.WelcomeDeviceReply
                            {
                                State = (SharedCoreModels.Enums.ConnectionState)(int)message.ConnectionState,
                                Token = message.Token
                            };
                            ReceivedWelcomeReply?.Invoke(this, replyMsg);
                        }
                    });
                }
            });
        }

        public void RequestManifest()
        {
            Task.Run(() =>
            {
                using (var call = manifestClient.Device(new Google.Protobuf.WellKnownTypes.Empty()))
                {
                    var readTask = Task.Run(async () =>
                    {
                        await foreach (var message in call.ResponseStream.ReadAllAsync())
                        {
                            var replyMsg = new SharedCoreModels.Manifest
                            {
                                AppMinimumVersion = SharedBase.Core.Version.ConvertTo(message.AppMinimumVersion),
                                ClientName = message.ClientName,
                                ClientVersion = SharedBase.Core.Version.ConvertTo(message.ClientVersion),
                                SupportedFeatures = OnGetFeatures(message.SupportedFeatures)
                            };
                            ReceivedManifestReply?.Invoke(this, replyMsg);
                        }
                    });
                }
            });
        }

        private List<SharedBase.Device.FeatureDefinitionExtended> OnGetFeatures(IEnumerable<Manifest.FeatureDefinitionExtended> featureDefinitionExtendeds)
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
