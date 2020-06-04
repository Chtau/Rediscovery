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
        private readonly ILogger<ManifestExchangeService> _logger;
        private readonly IAuthenticationManager _authenticationManager;

        public ManifestExchangeService(ILoggerFactory loggerFactory, IAuthenticationManager authenticationManager)
        {
            _logger = loggerFactory.CreateLogger<ManifestExchangeService>();
            _authenticationManager = authenticationManager;
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
                        Author = item.Author.EmptyIfNull(),
                        ControlIntegrationPoint = (FeatureDefinitionExtended.Types.IntegrationPoint)(int)item.ControlIntegrationPoint,
                        DisplayName = item.DisplayName.EmptyIfNull(),
                        Documentation = item.Documentation.EmptyIfNull(),
                        FeatureIntegrationPoint = (FeatureDefinitionExtended.Types.IntegrationPoint)(int)item.FeatureIntegrationPoint,
                        HasProfiles = item.HasProfiles,
                        HasSettings = item.HasSettings,
                        Id = item.Id.ToString().EmptyIfNull(),
                        MinimalControlIntegrationPoint = item.MinimalControlIntegrationPoint.ToString().EmptyIfNull(),
                        MinimalFeatureIntegrationPoint = item.MinimalFeatureIntegrationPoint.ToString().EmptyIfNull(),
                        PluginDirectory = item.PluginDirectory.EmptyIfNull(),
                        ProfileUIElementName = item.ProfileUIElementName.EmptyIfNull(),
                        ProfileUIReadonly = item.ProfileUIReadonly,
                        SettingUIElementName = item.SettingUIElementName.EmptyIfNull(),
                        SettingUIReadonly = item.SettingUIReadonly,
                        Version = item.Version.ToString().EmptyIfNull(),
                        Website = item.Website.EmptyIfNull(),
                    });
                }
            }
            return list;
        }

        [Authorize]
        public override Task<ManifestReply> Device(Empty request, ServerCallContext context)
        {
            var manifest = new ManifestReply
            {
                AppMinimumVersion = "",
                ClientName = "",
                ClientVersion = ""
            };
            try
            {
                _logger.LogTrace("Received Manifest request");
                var e = _authenticationManager.GetManifest();
                manifest = new ManifestReply
                {
                    AppMinimumVersion = e.AppMinimumVersion.ToString().EmptyIfNull(),
                    ClientName = e.ClientName.EmptyIfNull(),
                    ClientVersion = e.ClientVersion.ToString().EmptyIfNull()
                };
                manifest.SupportedFeatures.Add(OnGetFeatures(e.SupportedFeatures));
                return Task.FromResult(manifest);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Device");
            }
            return Task.FromResult(manifest);
        }
    }
}
