using CommunicationBase.Models;
using CommunicationFeatureProvider;
using Microsoft.Extensions.Logging;
using PluginFeature.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;

namespace DesktopService.Features.DeviceFeature
{
    public class FeatureManager : IFeatureManager
    {
        private readonly ILogger<FeatureManager> _logger;
        private readonly IFeatureService _featureService;

        public event EventHandler<ExchangeEntity<DeviceFeatureData>> SendData;

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
                        _featureService.StartFeature(featureStateChange.Entity.FeatureId.SafeGuid(), featureStateChange.Sid);
                        break;
                    case FeatureState.State.Stop:
                        _featureService.StopFeature(featureStateChange.Entity.FeatureId.SafeGuid(), featureStateChange.Sid);
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
                _logger.LogError(ex, "Feature state changed");
                featureStateChange.Entity.CurrentState = FeatureState.State.Error;
            }
            return featureStateChange;
        }

        public List<DeviceFeatureProfil> GetFeatureProfiles(Guid featureId)
        {
            return _featureService.GetFeatureProfiles(featureId);
        }

        public DeviceFeatureSetting GetFeatureSettings(Guid featureId)
        {
            return _featureService.GetFeatureSettings(featureId);
        }

        public byte[] GetFeatureUIArchive(Guid featureId)
        {
            try
            {
                string uiPath = _featureService.GetFeatureUIArchivePath(featureId);
                using (FileStream fs = new FileStream(uiPath, FileMode.Open, FileAccess.Read))
                {
                    byte[] archiveData = new byte[fs.Length];
                    fs.Read(archiveData, 0, System.Convert.ToInt32(fs.Length));
                    fs.Close();
                    return archiveData;
                }
            } catch (Exception ex)
            {
                _logger.LogError(ex, "Feature UI ZipArchive get byte[]");
                return new byte[0];
            }
        }

        public void ReceivedData(ExchangeEntity<DeviceFeatureData> deviceFeatureData)
        {
            // TODO: SID required
            _featureService.ReceiveData(deviceFeatureData.Entity.FeatureId, deviceFeatureData.Entity);
        }

        private void _featureService_RespondToClient(object sender, DeviceFeatureData e)
        {
            // TODO: SID required
            SendData?.Invoke(sender, new ExchangeEntity<DeviceFeatureData>
            {
                Sid = "",
                Entity = e
            });
        }
    }
}
