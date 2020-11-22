using CommunicationBase.Models;
using CommunicationFeatureProvider;
using Microsoft.Extensions.Logging;
using PluginFeature.Models;
using SharedBase.Feature;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;

namespace Rediscovery.Client.App.Service.Features.DeviceFeature
{
    public class FeatureManager : IFeatureManager
    {
        private readonly ILogger<FeatureManager> _logger;
        private readonly IFeatureService _featureService;

        public event EventHandler<ExchangeEntity<FeatureData>> SendData;

        public FeatureManager(ILoggerFactory loggerFactory,
            IFeatureService featureService)
        {
            _logger = loggerFactory.CreateLogger<FeatureManager>();
            _featureService = featureService;
            _featureService.RespondToClient += _featureService_RespondToClient;
        }

        public ExchangeEntity<FeatureState> FeatureStateChange(ExchangeEntity<FeatureState> featureStateChange)
        {
            try
            {
                switch (featureStateChange.Entity.CurrentState)
                {
                    case FeatureState.State.Unknown:
                        _logger.LogWarning("Service received feature state changed request for [Unknown]");
                        break;
                    case FeatureState.State.Start:
                        _featureService.StartFeature(featureStateChange.Entity.FeatureId, featureStateChange.Sid);
                        break;
                    case FeatureState.State.Stop:
                        _featureService.StopFeature(featureStateChange.Entity.FeatureId, featureStateChange.Sid);
                        break;
                    case FeatureState.State.Error:
                        _logger.LogWarning("Service received feature state changed request for [Error]");
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Feature state changed (Requested State:{Enum.GetName(typeof(FeatureState.State), featureStateChange.Entity.CurrentState)})");
                featureStateChange.Entity.CurrentState = FeatureState.State.Error;
            }
            return featureStateChange;
        }

        public List<FeatureProfil> GetFeatureProfiles(Guid featureId)
        {
            return _featureService.GetFeatureProfiles(featureId);
        }

        public FeatureSetting GetFeatureSettings(Guid featureId)
        {
            return _featureService.GetFeatureSettings(featureId);
        }

        public byte[] GetFeatureUIArchive(Guid featureId)
        {
            string uiPath = null;
            try
            {
                uiPath = _featureService.GetFeatureUIArchivePath(featureId);
                if (!string.IsNullOrWhiteSpace(uiPath) && System.IO.File.Exists(uiPath))
                {
                    return System.IO.File.ReadAllBytes(uiPath);
                }
            } catch (Exception ex)
            {
                _logger.LogError(ex, $"Feature UI ZipArchive get byte[] (Path:{uiPath})");
            }
            return null;
        }

        public void ReceivedData(ExchangeEntity<FeatureData> deviceFeatureData)
        {
            _featureService.ReceiveData(deviceFeatureData.Entity.FeatureId, deviceFeatureData);
        }

        private void _featureService_RespondToClient(object sender, ExchangeEntity<FeatureData> e)
        {
            SendData?.Invoke(sender, e);
        }
    }
}
