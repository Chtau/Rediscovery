using Rediscovery.Communication.Protocol.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Rediscovery.Communication.Protocol
{
    public interface IRediscoveryProtocol
    {
        List<DeviceGreeting> Devices { get; }
        /// <summary>
        /// Triggered if a device greeting has changed from the discovery.
        /// Device Identifier will be provided as argument.
        /// </summary>
        event EventHandler<string> DevicesChanged;

        void Start(Models.Configuration configuration);
        void Stop();
        void Send<T>(Transfer<T> transfer, Action<TransportState> successCallback = null);
        void Listen<T>(Action<Transfer<T>> receivedCallback);
        void SetMetadata(string identifer, string friendlyName, DeviceMetadata.IdiomType idiomType);
        string NewIdentifier();
    }
}
