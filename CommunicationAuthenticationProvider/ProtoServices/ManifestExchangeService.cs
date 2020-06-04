using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using CommunicationAuthenticationProvider.Services;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Manifest;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;

namespace CommunicationAuthenticationProvider.ProtoServices
{
    public class ManifestExchangeService : ManifestExchange.ManifestExchangeBase
    {
        private readonly IEventService _eventService;
        private readonly ILogger<ManifestExchangeService> _logger;
        private readonly IAuthenticationManager _authenticationManager;
        private ServerCallContext _context;

        public ManifestExchangeService(ILoggerFactory loggerFactory, IEventService eventService, IAuthenticationManager authenticationManager)
        {
            _logger = loggerFactory.CreateLogger<ManifestExchangeService>();
            _authenticationManager = authenticationManager;
            _eventService = eventService;
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

        [Authorize]
        public override async Task Device(Empty request, IServerStreamWriter<ManifestReply> responseStream, ServerCallContext context)
        {
            try
            {
                Console.WriteLine("Received Manifest request");
                _context = context;

                await Task.Run(async () =>
                {
                    try
                    {
                        var e = _authenticationManager.GetManifest();
                        var manifest = new ManifestReply
                        {
                            AppMinimumVersion = e.AppMinimumVersion.ToString(),
                            ClientName = e.ClientName,
                            ClientVersion = e.ClientVersion.ToString()
                        };
                        manifest.SupportedFeatures.Add(OnGetFeatures(e.SupportedFeatures));
                        await responseStream.WriteAsync(manifest);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Device send Manifest");
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Device");
            }
        }
    }
}
