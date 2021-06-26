using Rediscovery.Communication.Protocol.Models;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace Rediscovery.Communication.Protocol.Internal.Device
{
    internal interface IDeviceManager
    {
        List<DeviceGreeting> Devices { get; }

        /// <summary>
        /// Will be triggered after a device is changed or added
        /// </summary>
        event EventHandler<string> DeviceChanged;
        /// <summary>
        /// Will be triggered every time a device reports from discovery
        /// </summary>
        event EventHandler<string> DeviceIncomingPing;
        /// <summary>
        /// Get the current device greeting information from the device identifier
        /// </summary>
        /// <param name="identifier"></param>
        /// <returns></returns>
        DeviceGreetingReceived GetGreeting(string identifier);
        /// <summary>
        /// Add or update device greeting information.
        /// Timeout check will be started if no timeout is running.
        /// </summary>
        /// <param name="deviceGreeting"></param>
        /// <param name="ipEndPoint"></param>
        /// <returns>True if this is a new device or something has changed. False when no changes happend.</returns>
        bool Change(DeviceGreeting deviceGreeting, IPEndPoint ipEndPoint);
        /// <summary>
        /// Set the identifier of the current device
        /// </summary>
        /// <param name="identifer"></param>
        void SetIdentifier(string identifer);
        /// <summary>
        /// Symmetric password for the device Identifer.
        /// If no password is found the Identifer will be returned.
        /// </summary>
        /// <param name="identifer">Device Identifer</param>
        /// <returns>Password or Identifer</returns>
        string DeviceSymmetricPassword(string identifer);
        /// <summary>
        /// Adds or updates the Symmetric password for a device
        /// </summary>
        /// <param name="identifer">Device identifer</param>
        /// <param name="password">Symmetric password</param>
        void AddOrUpdateDeviceSymmetric(string identifer, string password);
        /// <summary>
        /// Adds or updates the public key for a device
        /// </summary>
        /// <param name="identifer">Device identifer</param>
        /// <param name="publicKey">Public key</param>
        void AddOrUpdateDevicePublicKey(string identifer, string publicKey);
        /// <summary>
        /// Get the current IP address for the device identifer
        /// </summary>
        /// <param name="identifier">Known device identifer</param>
        /// <returns>IP or null</returns>
        string GetIP(string identifier);
        /// <summary>
        /// Public key for a device Identifer.
        /// If no public key is found <see cref="null"/> will be returned.
        /// </summary>
        /// <param name="identifer">Device Identifer</param>
        /// <returns>Public key or <see cref="null"/></returns>
        string DevicePublicKey(string identifer);
        /// <summary>
        /// Only true if all device data from handshakes are up to date for the identifer
        /// </summary>
        /// <param name="identifer">Device identifer</param>
        /// <returns></returns>
        bool HandshakeRequired(string identifer);
    }
}
