using Microsoft.AspNetCore.SignalR.Client;
using PluginFeature.Models;
using SharedCoreModels;
using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Rediscovery.Features.Connection
{
    public interface IConnect
    {
        Task AutoConnect();
        Task TryConnect(Guid connectionId);
        Task<HubConnection> GetConnectionAuth();
        Task<HubConnection> GetConnectionFeature();
        Task CloseConnections();
        Task ValidateKey(Guid connectionId, string key);
        bool IsConnected(DesktopConfiguration.DesktopConfigurationModel model, Connect.HubTypes hubType);
        Task<DesktopConfiguration.DesktopConfigurationModel> GetModel();
        event EventHandler<DesktopConfiguration.DesktopConfigurationModel> HelloReceived;
        event EventHandler<Tuple<DesktopConfiguration.DesktopConfigurationModel, List<Models.ConnectionManifestFeature>>> ManifestReceived;
        event EventHandler<DesktopConfiguration.DesktopConfigurationModel> ConnectionChanged;
        Task<ZipArchive> GetUIArchive(Guid featureId);
        Task<List<DeviceFeatureProfil>> GetDeviceFeatureProfils(Guid featureId);
        Task<DeviceFeatureSetting> GetDeviceFeatureSetting(Guid featureId);
    }
}
