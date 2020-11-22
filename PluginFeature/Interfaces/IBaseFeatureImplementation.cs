using Rediscovery.Feature.Plugin.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Rediscovery.Feature.Plugin.Interfaces
{
    public interface IBaseFeatureImplementation<TEntity, TDefinition>
    {
        string PluginDirectory { get; }
        void Init(string pluginDirectory, IPluginLogger pluginLogger);
        void Dispose();
        event EventHandler<PluginExchangeEntity<TEntity>> SendData;
        void ReceiveData(PluginExchangeEntity<TEntity> data);
        TDefinition GetDeviceFeatureInfo();

        /// <summary>
        /// Register a device id to send/receive data from the feature
        /// </summary>
        /// <param name="deviceId">unique user connection id</param>
        void Register(string deviceId);

        /// <summary>
        /// Unregister from sending/receiving data from the feature
        /// </summary>
        /// <param name="deviceId">unique user connection id</param>
        void Unregister(string deviceId);
    }
}
