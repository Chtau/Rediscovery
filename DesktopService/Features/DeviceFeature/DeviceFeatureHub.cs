using DesktopService.Features.RemoteResources;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using PluginFeature.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DesktopService.Features.DeviceFeature
{
    // TODO: refactor to Communication library

    [Authorize]
    public class DeviceFeatureHub : Hub
    {
        public override Task OnConnectedAsync()
        {
            _remoteResourcesSenderService.AddActiveDevice(Context.UserIdentifier);
            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            _remoteResourcesSenderService.RemoveActiveDevice(Context.UserIdentifier);
            return base.OnDisconnectedAsync(exception);
        }

        private readonly ILogger<DeviceFeatureHub> _logger;
        private readonly IFeatureService _featureService;
        private readonly CommunicationResourceProvider.IRemoteResourcesSenderService _remoteResourcesSenderService;

        public DeviceFeatureHub(ILoggerFactory loggerFactory, IFeatureService featureService,
            CommunicationResourceProvider.IRemoteResourcesSenderService remoteResourcesSenderService)
        {
            _logger = loggerFactory.CreateLogger<DeviceFeatureHub>();
            _featureService = featureService;
            _remoteResourcesSenderService = remoteResourcesSenderService;
        }

        public void ClientMessage(Guid featureId, string profileId, object data)
        {
            _logger.LogTrace($"Feature (id: {featureId}) Message on Service received");
            var feature = _featureService.GetFeature(featureId);
            if (feature != null)
            {
                var val = new DeviceFeatureData(Context.UserIdentifier, featureId, profileId, data);
                feature.ReceiveData(val);
            }
        }

        public void ClientFeatureStart(Guid featureId)
        {
            _logger.LogTrace($"Feature (id: {featureId}) START on Service received");
            var feature = _featureService.GetFeature(featureId);
            if (feature != null)
            {
                feature.Register(Context.UserIdentifier);
            }
        }

        public void ClientFeatureStop(Guid featureId)
        {
            _logger.LogTrace($"Feature (id: {featureId}) STOP on Service received");
            var feature = _featureService.GetFeature(featureId);
            if (feature != null)
            {
                feature.Unregister(Context.UserIdentifier);
            }
        }

        [AllowAnonymous]
        public void LogEntry(SharedBase.Logging.LoggerEntry loggerEntry)
        {
            // TODO: refactor create Log from SharedCoreModels.LoggerEntryModel
            try
            {
                if (loggerEntry != null)
                {
                    switch (loggerEntry.LogLevel)
                    {
                        case SharedBase.Logging.LoggerEntry.LoggerType.Trace:
                            _logger.LogTrace(loggerEntry.Message);
                            break;
                        case SharedBase.Logging.LoggerEntry.LoggerType.Debug:
                            _logger.LogDebug(loggerEntry.Message);
                            break;
                        case SharedBase.Logging.LoggerEntry.LoggerType.Information:
                            _logger.LogInformation(loggerEntry.Message);
                            break;
                        case SharedBase.Logging.LoggerEntry.LoggerType.Warning:
                            _logger.LogWarning(loggerEntry.Message);
                            break;
                        case SharedBase.Logging.LoggerEntry.LoggerType.Error:
                            _logger.LogError(loggerEntry.Message);
                            break;
                        case SharedBase.Logging.LoggerEntry.LoggerType.Critical:
                            _logger.LogCritical(loggerEntry.Message);
                            break;
                        default:
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
            }
        }
    }
}
