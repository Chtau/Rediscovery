using System;
using System.Collections.Generic;
using System.Text;

namespace Rediscovery.Communication.Protocol.Internal.Network
{
    internal interface INetworkState
    {
        /// <summary>
        /// Add passwords for network Symmetric encryption.
        /// This will be used for Discovery and Handshake data encryption.
        /// If you want to set the password for the local network use <see cref="SetNetworkPassword(string)"/>.
        /// </summary>
        /// <param name="passwords">String passwords</param>
        void AddNetworkPasswords(params string[] passwords);
        /// <summary>
        /// Password used for the current networks Symmetric encryption.
        /// If you need to add a password for a remote network use <see cref="AddNetworkPasswords(string[])"/>.
        /// </summary>
        /// <param name="password">String password.</param>
        void SetNetworkPassword(string password);
        /// <summary>
        /// Executes a callback per password until the callback returns true or no passowrd matches
        /// </summary>
        /// <param name="passwordCallback"></param>
        void EnumerateDecryptPasswords(Func<string, bool> passwordCallback);
        /// <summary>
        /// Encrypt the raw outgoing traffic with the own network password
        /// </summary>
        /// <param name="raw">Raw traffic before it will be send</param>
        /// <returns></returns>
        byte[] Encrypt(byte[] raw);
    }
}
