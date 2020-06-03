using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using CommunicationAuthenticationProvider.Services;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Manifest;

namespace CommunicationAuthenticationProvider.ProtoServices
{
    public class ManifestExchangeService : ManifestExchange.ManifestExchangeBase
    {
        private readonly IEventService _eventService;
        private IServerStreamWriter<ManifestReply> _responseStream;
        private ServerCallContext _context;

        public ManifestExchangeService(IEventService eventService)
        {
            _eventService = eventService;
            _eventService.SendManifest += _eventService_SendManifest;
        }

        private void _eventService_SendManifest(object sender, SharedCoreModels.Manifest e)
        {
            if (_responseStream != null)
            {
                Task.Run(async () =>
                {
                    var manifest = new ManifestReply
                    {
                        AppMinimumVersion = e.AppMinimumVersion.ToString(),
                        ClientName = e.ClientName,
                        ClientVersion = e.ClientVersion.ToString()
                    };
                    manifest.SupportedFeatures.Add(OnGetFeatures(e.SupportedFeatures));
                    await _responseStream.WriteAsync(manifest);
                });
            }
        }

        private IEnumerable<FeatureDefinitionExtended> OnGetFeatures(List<SharedBase.Device.FeatureDefinitionExtended> featureDefinitionExtendeds)
        {
            var list = new List<FeatureDefinitionExtended>();
            if (featureDefinitionExtendeds != null)
            {
                foreach (var item in featureDefinitionExtendeds)
                {
                    list.Add(new FeatureDefinitionExtended
                    {
                        Author = item.Author,
                        ControlIntegrationPoint = (FeatureDefinitionExtended.Types.IntegrationPoint)(int)item.ControlIntegrationPoint,
                        DisplayName = item.DisplayName,
                        Documentation = item.Documentation,
                        FeatureIntegrationPoint = (FeatureDefinitionExtended.Types.IntegrationPoint)(int)item.FeatureIntegrationPoint,
                        HasProfiles = item.HasProfiles,
                        HasSettings = item.HasSettings,
                        Id = item.Id.ToString(),
                        MinimalControlIntegrationPoint = item.MinimalControlIntegrationPoint.ToString(),
                        MinimalFeatureIntegrationPoint = item.MinimalFeatureIntegrationPoint.ToString(),
                        PluginDirectory = item.PluginDirectory,
                        ProfileUIElementName = item.ProfileUIElementName,
                        ProfileUIReadonly = item.ProfileUIReadonly,
                        SettingUIElementName = item.SettingUIElementName,
                        SettingUIReadonly = item.SettingUIReadonly,
                        Version = item.Version.ToString(),
                        Website = item.Website,
                    });
                }
            }
            return list;
        }

        public override async Task Device(Empty request, IServerStreamWriter<ManifestReply> responseStream, ServerCallContext context)
        {
            _context = context;
            _responseStream = responseStream;
        }
    }
}
