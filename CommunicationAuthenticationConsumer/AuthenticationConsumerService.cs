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

        private Channel channel = null;
        private CancellationTokenSource ctsRequestManifest = null;
        private CancellationTokenSource ctsWelcome = null;

        public AuthenticationConsumerService(ILogger logger)
        {
            _logger = logger;
        }

        public bool Connect(string ipAddress, int port, string certificatePEM)
        {
            try
            {
                var channelCredentials = new SslCredentials(certificatePEM);
                channel = new Channel(ipAddress, port, channelCredentials);
                authenticationClient = new Authentication.AuthentionExchange.AuthentionExchangeClient(channel);
                manifestClient = new Manifest.ManifestExchange.ManifestExchangeClient(channel);
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
                    var msg = new Authentication.WelcomeDeviceMessage
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
                    System.Diagnostics.Debug.Print(ex.ToString());
                    replyMsg.State = Enums.ConnectionState.Error;
                } finally
                {
                    callback?.Invoke(replyMsg);
                    ReceivedWelcomeReply?.Invoke(this, replyMsg);
                }
            });
        }

        public void RequestManifest(string token, Action<SharedBase.Connection.Manifest> callback = null)
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
                    var replyMsg = new SharedBase.Connection.Manifest
                    {
                        AppMinimumVersion = SharedBase.Core.Version.ConvertTo(reply.AppMinimumVersion),
                        ClientName = reply.ClientName,
                        ClientVersion = SharedBase.Core.Version.ConvertTo(reply.ClientVersion),
                        SupportedFeatures = OnGetFeatures(reply.SupportedFeatures),
                    };
                    callback?.Invoke(replyMsg);
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
                        Id = item.Id.SafeGuid(),
                        MinimalControlIntegrationPoint = SharedBase.Core.Version.ConvertTo(item.MinimalControlIntegrationPoint),
                        MinimalFeatureIntegrationPoint = SharedBase.Core.Version.ConvertTo(item.MinimalFeatureIntegrationPoint),
                        PluginDirectory = item.PluginDirectory,
                        HasProfilConfiguration = item.HasProfilConfiguration,
                        HasSettingConfiguration = item.HasSettingConfiguration,
                        Version = SharedBase.Core.Version.ConvertTo(item.Version),
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
