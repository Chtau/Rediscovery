using Rediscovery.Communication.Protocol.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Rediscovery.Communication.Protocol
{
    public interface IRediscoveryProtocol
    {
        /// <summary>
        /// Device Identifer
        /// This should be a unique key in the Network
        /// </summary>
        string Identifer { get; }
        /// <summary>
        /// Actvie devices in the Network
        /// </summary>
        List<DeviceGreeting> Devices { get; }
        Traffic Traffic { get; }
        /// <summary>
        /// Triggered if a device greeting has changed from the discovery.
        /// Device Identifier will be provided as argument.
        /// </summary>
        event EventHandler<string> DevicesChanged;
        /// <summary>
        /// Start the auto discovery and listener for incoming data
        /// </summary>
        /// <param name="configuration">Protocol configuration</param>
        void Start(Configuration configuration);
        /// <summary>
        /// Stop the discovery and incoming data listener
        /// </summary>
        void Stop();
        /// <summary>
        /// Send a object to another device
        /// </summary>
        /// <typeparam name="T">Type of the data</typeparam>
        /// <param name="transfer">Transfer object which contains the receiver identifer</param>
        void Send<T>(Transfer<T> transfer);
        void Send<T>(string callbackKey, Transfer<T> transfer);
        /// <summary>
        /// Callback to get incoming data
        /// </summary>
        /// <typeparam name="T">Type of the data</typeparam>
        /// <param name="receivedCallback">Callback action when data is ready</param>
        void Listen<T>(Action<Transfer<T>> receivedCallback);
        void Listen<T>(string key, Action<Transfer<T>> receivedCallback);
        /// <summary>
        /// Updates the metadata for the current device
        /// </summary>
        /// <param name="identifer">Network device identifer</param>
        /// <param name="friendlyName">Friendly name</param>
        /// <param name="idiomType">Device type</param>
        void SetMetadata(string identifer, string friendlyName, DeviceMetadata.IdiomType idiomType);
        /// <summary>
        /// Create a new Identifer for the device
        /// </summary>
        /// <returns></returns>
        string NewIdentifier();
    }
}
