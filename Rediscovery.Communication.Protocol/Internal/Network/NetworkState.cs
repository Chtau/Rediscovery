using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Rediscovery.Communication.Protocol.Internal.Encryption;

namespace Rediscovery.Communication.Protocol.Internal.Network
{
    internal class NetworkState : INetworkState
    {
        private const string DefaultPassword = "6ywr5eh9K83vuNxoGx1p3FPzCqjvfF5W";

        private readonly IProtocolLogger _logger;
        private readonly IEncryption _encryption;
        private readonly List<string> _networkPasswords = new List<string>();
        private readonly Random _random = new Random();

        private string currentNetworkPassword = DefaultPassword;

        public NetworkState(IProtocolLogger logger,
            IEncryption encryption)
        {
            _logger = logger;
            _encryption = encryption;
        }

        public void AddNetworkPasswords(params string[] passwords)
        {
            passwords?.Distinct().ToList().ForEach(password =>
            {
                if (!_networkPasswords.Contains(password))
                    _networkPasswords.Add(password);
            });
        }
        
        public void EnumerateDecryptPasswords(Func<string, bool> passwordCallback)
        {
            try
            {
                if (!passwordCallback.Invoke(currentNetworkPassword))
                {
                    foreach (var password in _networkPasswords)
                    {
                        if (passwordCallback.Invoke(password))
                            break;
                    }
                }
            } catch (Exception ex)
            {
                _logger.Error(ex);
            }
        }

        public byte[] Encrypt(byte[] raw)
        {
            return _encryption.EncryptSymmetric(currentNetworkPassword, raw);
        }

        public void SetNetworkPassword(string password) => currentNetworkPassword = password;

        public byte[] NormalizePackageSize(byte[] raw, int targetSize)
        {
            var fullRaw = new List<byte>(targetSize);
            fullRaw.AddRange(raw);
            if (raw.Length < targetSize)
            {
                byte[] b = new byte[targetSize - raw.Length];
                _random.NextBytes(b);
                fullRaw.AddRange(b);
            }
            return fullRaw.ToArray();
        }
    }
}
