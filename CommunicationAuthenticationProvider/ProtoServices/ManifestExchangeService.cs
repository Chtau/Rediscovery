using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Rediscovery.Communication.Provider.Authentication.Services;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using Rediscovery.Shared.Base.Extensions;

namespace Rediscovery.Communication.Provider.Authentication.ProtoServices
{
    public class ManifestExchangeService : ProtoManifest.ManifestExchange.ManifestExchangeBase
    {
        private readonly ILogger<ManifestExchangeService> _logger;
        private readonly IAuthenticationManager _authenticationManager;

        public ManifestExchangeService(ILoggerFactory loggerFactory, IAuthenticationManager authenticationManager)
        {
            _logger = loggerFactory.CreateLogger<ManifestExchangeService>();
            _authenticationManager = authenticationManager;
        }

        private IEnumerable<FeatureDefinitionExtended> OnGetFeatures(List<Rediscovery.Shared.Base.Device.FeatureDefinitionExtended> featureDefinitionExtendeds)
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
                        Id = item.Id.ToString().EmptyIfNull(),
                        MinimalControlIntegrationPoint = item.MinimalControlIntegrationPoint.ToString().EmptyIfNull(),
                        MinimalFeatureIntegrationPoint = item.MinimalFeatureIntegrationPoint.ToString().EmptyIfNull(),
                        PluginDirectory = item.PluginDirectory.EmptyIfNull(),
                        HasProfilConfiguration = item.HasProfilConfiguration,
                        HasSettingConfiguration = item.HasSettingConfiguration,
                        Version = item.Version.ToString().EmptyIfNull(),
                        Website = item.Website.EmptyIfNull(),
                        ClientDescription = item.ClientDescription.EmptyIfNull(),
                        IsClientImplementation = item.IsClientImplementation,
                        NativeResources = item.NativeResources
                    });
                }
            }
            return list;
        }

        [Authorize(Policy = "DeviceAndConsumer")]
        public override Task<ProtoManifest.ManifestReply> Device(Empty request, ServerCallContext context)
        {
            var manifest = new ProtoManifest.ManifestReply
            {
                AppMinimumVersion = "",
                ClientName = "",
                ClientVersion = ""
            };
            try
            {
                _logger.LogTrace("Received Manifest request");
                var e = _authenticationManager.GetManifest();
                manifest = new ProtoManifest.ManifestReply
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
