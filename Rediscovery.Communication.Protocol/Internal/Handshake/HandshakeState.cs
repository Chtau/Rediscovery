using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rediscovery.Communication.Protocol.Internal.Handshake
{
    internal class HandshakeState
    {
        public const string ValueDelimiter = "+";
        private readonly byte valueDelimiter = Encoding.UTF8.GetBytes(ValueDelimiter).First();

        public enum MessageValueType
        {
            Undefined = 0,
            PublicKey = 1,
            [Obsolete("No longer needed because we removed the usage of RSA for Diffie Hellman")]
            SymmetricPasswordCypher = 2
        }

        public enum ExpectedResponseType
        {
            None = 0,
            PublicKey = 1,
            [Obsolete("No longer needed because we removed the usage of RSA for Diffie Hellman")]
            SymmetricPasswordCypher = 2
        }

        public string SenderIdentifier { get; private set; }
        public string ReceiverIdentifier { get; private set; }
        public string Checksum { get; private set; }
        public byte[] Value { get; private set; }
        public MessageValueType ValueType { get; private set; }
        public ExpectedResponseType ResponseType { get; private set; }

        public HandshakeState(string senderIdentifier,
            string receiverIdentifier,
            string checksum,
            byte[] value,
            MessageValueType valueType,
            ExpectedResponseType responseType)
        {
            SenderIdentifier = senderIdentifier.ExactLength(16);
            ReceiverIdentifier = receiverIdentifier.ExactLength(16);
            Checksum = checksum.ExactLength(16);
            Value = value;
            ValueType = valueType;
            ResponseType = responseType;
        }

        public HandshakeState(byte[] raw)
        {
            if (raw?.Length > 0)
            {
                var rawList = raw.ToList();
                SenderIdentifier = Convert.ToBase64String(rawList.Take(12).ToArray());
                rawList.RemoveRange(0, 12);
                ReceiverIdentifier = Convert.ToBase64String(rawList.Take(12).ToArray());
                rawList.RemoveRange(0, 12);
                Checksum = Convert.ToBase64String(rawList.Take(12).ToArray());
                rawList.RemoveRange(0, 12);
                var msgValueType = Encoding.UTF8.GetString(rawList.Take(1).ToArray());
                ValueType = (MessageValueType)int.Parse(msgValueType);
                rawList.RemoveRange(0, 1);
                var msgResponseType = Encoding.UTF8.GetString(rawList.Take(1).ToArray());
                ResponseType = (ExpectedResponseType)int.Parse(msgResponseType);
                rawList.RemoveRange(0, 1);

                rawList.RemoveRange(0, 1); // remove delimiter
                var sizeEndIndex = rawList.IndexOf(valueDelimiter);
                var size = Encoding.UTF8.GetString(rawList.Take(sizeEndIndex).ToArray());
                var payloadsize = int.Parse(size);
                rawList.RemoveRange(0, sizeEndIndex + 1);

                Value = rawList.Take(payloadsize).ToArray();
            }
        }

        public List<byte> CreateRaw()
        {
            var raw = new List<byte>();
            raw.AddRange(Convert.FromBase64String(SenderIdentifier));
            raw.AddRange(Convert.FromBase64String(ReceiverIdentifier));
            raw.AddRange(Convert.FromBase64String(Checksum));
            raw.AddRange(Encoding.UTF8.GetBytes(((int)ValueType).ToString()));
            raw.AddRange(Encoding.UTF8.GetBytes(((int)ResponseType).ToString()));
            raw.AddRange(Encoding.UTF8.GetBytes($"{ValueDelimiter}{Value.Length}{ValueDelimiter}"));
            raw.AddRange(Value);
            return raw;
        }

        public bool IsValid()
        {
            return !string.IsNullOrWhiteSpace(SenderIdentifier)
                && !string.IsNullOrWhiteSpace(ReceiverIdentifier)
                && !string.IsNullOrWhiteSpace(Checksum)
                && Value?.Length > 0;
        }

        public override string ToString()
        {
            return $"{nameof(SenderIdentifier)}:{SenderIdentifier};{nameof(ReceiverIdentifier)}:{ReceiverIdentifier};{nameof(Checksum)}:{Checksum};{nameof(Value)}:{Value};{nameof(ValueType)}:{Enum.GetName(typeof(MessageValueType), ValueType)};{nameof(ResponseType)}:{Enum.GetName(typeof(ExpectedResponseType), ResponseType)};{nameof(Value)}:{Value?.Length}";
        }
    }
}
