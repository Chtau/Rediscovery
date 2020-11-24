using Rediscovery.Communication.Base;
using Grpc.Core;
using Rediscovery.Shared.Base.Connection;
using Rediscovery.Shared.Base.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Rediscovery.Shared.Base.Extensions;

namespace CommunicationAuthenticationConsumer
{
    public class AuthenticationConsumerService : IAuthenticationConsumerService
    {
        public event EventHandler<WelcomeDeviceReply> ReceivedWelcomeReply;
        public event EventHandler<Rediscovery.Shared.Base.Connection.Manifest> ReceivedManifestReply;

        private ProtoAuthentication.AuthentionExchange.AuthentionExchangeClient authenticationClient;
        private ProtoManifest.ManifestExchange.ManifestExchangeClient manifestClient;
        private readonly ILogger _logger;

        private Channel channel = null;
        private CancellationTokenSource ctsRequestManifest = null;
        private CancellationTokenSource ctsWelcome = null;

        public AuthenticationConsumerService(ILogger logger)
        {
            _logger = logger;
        }

        public bool Connect(ConsumerConnectionConfiguration connectionConfiguration)
        {
            try
            {
                channel = ChannelHelper.CreateChannel(connectionConfiguration);
                authenticationClient = new ProtoAuthentication.AuthentionExchange.AuthentionExchangeClient(channel);
                manifestClient = new ProtoManifest.ManifestExchange.ManifestExchangeClient(channel);
                return authenticationClient != null;
            } catch (Exception ex)
            {
                _logger.LogError(ex);
                return false;
            }
        }

        public bool Disconnect()
        {
            try
            {
                ctsWelcome?.Cancel();
                ctsRequestManifest?.Cancel();
                channel?.ShutdownAsync().GetAwaiter();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex);
                return false;
            }
        }

        public void SendWelcome(WelcomeDeviceMessage message, Action<WelcomeDeviceReply> callback = null)
        {
            Task.Run(async () =>
            {
                var replyMsg = new WelcomeDeviceReply
                {
                    State = Enums.ConnectionState.None,
                    Token = ""
                };
                try
                {
                    ctsWelcome = new CancellationTokenSource();
                    var msg = new ProtoAuthentication.WelcomeDeviceMessage
                    {
                        DeviceIdentifier = message.DeviceIdentifier,
                    };
                    _logger.LogTrace("Consumer Welcome send Welcome");
                    var reply = await authenticationClient.WelcomeAsync(msg, cancellationToken: ctsWelcome.Token);
                    _logger.LogTrace("Consumer Welcome reply received");
                    replyMsg = new WelcomeDeviceReply
                    {
                        State = (Enums.ConnectionState)(int)reply.ConnectionState,
                        Token = reply.Token
                    };
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex);
                    replyMsg.State = Enums.ConnectionState.Error;
                } finally
                {
                    callback?.Invoke(replyMsg);
                    ReceivedWelcomeReply?.Invoke(this, replyMsg);
                }
            });
        }

        public void RequestManifest(string token, Action<Rediscovery.Shared.Base.Connection.Manifest> callback = null)
        {
            Task.Run(async () =>
            {
                try
                {
                    _logger.LogTrace("Consumer request Manifest");
                    ctsRequestManifest = new CancellationTokenSource();
                    var meta = new Metadata();
                    meta.AddAuthorizationHeader(token);
                    var reply = await manifestClient.DeviceAsync(new Google.Protobuf.WellKnownTypes.Empty(), headers: meta, cancellationToken: ctsRequestManifest.Token);
                    var replyMsg = new Rediscovery.Shared.Base.Connection.Manifest
                    {
                        AppMinimumVersion = Rediscovery.Shared.Base.Core.Version.ConvertTo(reply.AppMinimumVersion),
                        ClientName = reply.ClientName,
                        ClientVersion = Rediscovery.Shared.Base.Core.Version.ConvertTo(reply.ClientVersion),
                        SupportedFeatures = OnGetFeatures(reply.SupportedFeatures),
                    };
                    callback?.Invoke(replyMsg);
                    ReceivedManifestReply?.Invoke(this, replyMsg);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex);
                }
            });
        }

        private List<Rediscovery.Shared.Base.Device.FeatureDefinitionExtended> OnGetFeatures(IEnumerable<FeatureDefinitionExtended> featureDefinitionExtendeds)
        {
            var list = new List<Rediscovery.Shared.Base.Device.FeatureDefinitionExtended>();
            if (featureDefinitionExtendeds != null)
            {
                foreach (var item in featureDefinitionExtendeds)
                {
                    list.Add(new Rediscovery.Shared.Base.Device.FeatureDefinitionExtended
                    {
                        Author = item.Author,
                        ControlIntegrationPoint = (Rediscovery.Shared.Base.Device.IntegrationPoint)(int)item.ControlIntegrationPoint,
                        DisplayName = item.DisplayName,
                        Documentation = item.Documentation,
                        FeatureIntegrationPoint = (Rediscovery.Shared.Base.Device.IntegrationPoint)(int)item.FeatureIntegrationPoint,
                        Id = item.Id.SafeGuid(),
                        MinimalControlIntegrationPoint = Rediscovery.Shared.Base.Core.Version.ConvertTo(item.MinimalControlIntegrationPoint),
                        MinimalFeatureIntegrationPoint = Rediscovery.Shared.Base.Core.Version.ConvertTo(item.MinimalFeatureIntegrationPoint),
                        PluginDirectory = item.PluginDirectory,
                        HasProfilConfiguration = item.HasProfilConfiguration,
                        HasSettingConfiguration = item.HasSettingConfiguration,
                        Version = Rediscovery.Shared.Base.Core.Version.ConvertTo(item.Version),
                        Website = item.Website,
                        IsClientImplementation = item.IsClientImplementation,
                        NativeResources = item.NativeResources,
                        ClientDescription = item.ClientDescription,
                    });
                }
            }
            return list;
        }
    }
}
