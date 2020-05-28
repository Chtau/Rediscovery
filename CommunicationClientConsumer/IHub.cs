using CommunicationBase;
using PluginFeature.Models;
using SharedCoreModels;
using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Text;
using System.Threading.Tasks;

namespace CommunicationClientConsumer
{
    public interface IHub
    {
        event EventHandler<Models.ResponseReceived> FeatureResponseReceived;
        void Init(SharedBase.Logging.ILogger logger, string authHubLink, string exchangeHubLink, Protocol protocol = Protocol.HTTP);
        void Authenticate(WelcomeDeviceMessage welcomeDeviceMessage, ConnectionConfiguration configuration, Action<ConnectionConfiguration, bool> callback, Action<Manifest> manifestCallback);
        void Connect(ConnectionConfiguration configuration, Action<bool, ConnectionState> resultCallback);
        Task<bool> Disconnect();
        void Send(Guid featureId, string profileId, object data);
        void Start(Guid featureId);
        void Stop(Guid featureId);
        Task<ZipArchive> GetUIArchive(Guid featureId);
        Task<List<DeviceFeatureProfil>> GetDeviceFeatureProfils(Guid featureId);
        Task<DeviceFeatureSetting> GetDeviceFeatureSetting(Guid featureId);
        void LogEntry(SharedCoreModels.LoggerEntryModel loggerEntry);
    }
}
