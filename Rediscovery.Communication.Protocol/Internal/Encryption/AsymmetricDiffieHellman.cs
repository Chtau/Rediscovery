using Org.BouncyCastle.Asn1.Nist;
using Org.BouncyCastle.Asn1.Sec;
using Org.BouncyCastle.Asn1.X9;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Digests;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Macs;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Security;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace Rediscovery.Communication.Protocol.Internal.Encryption
{
    internal class AsymmetricDiffieHellman
    {
        internal class KeyCoords
        {
            public byte[] X { get; }
            public byte[] Y { get; }

            public KeyCoords(byte[] x, byte[] y)
            {
                X = x;
                Y = y;
            }
        }

        public static Keys<byte[], KeyCoords> GetNewKeyPair(byte[] seed = null)
        {
            var instance = new AsymmetricDiffieHellman();
            var keyPair = instance.OnGenerateKeyPair(seed);
            var privateKey = (ECPrivateKeyParameters)keyPair.Private;
            var publicKey = (ECPublicKeyParameters)keyPair.Public;
            return new Keys<byte[], KeyCoords>(privateKey.D.ToByteArray(), new KeyCoords(publicKey.Q.AffineXCoord.GetEncoded(), publicKey.Q.AffineYCoord.GetEncoded()));
        }

        private const string CurveName = "P-521";
        private const string KeyAlgorithm = "ECDH";

        private readonly ECDomainParameters _ecDomain;
        private readonly X9ECParameters _x9EC;

        private ECPrivateKeyParameters localPrivateKey;
        private ECPublicKeyParameters localPublicKey;
        private ECPublicKeyParameters remotePublicKey;

        public KeyCoords LocalPublicKey => new KeyCoords(localPublicKey?.Q.AffineXCoord.GetEncoded(), localPublicKey?.Q.AffineYCoord.GetEncoded());

        public AsymmetricDiffieHellman(byte[] seed = null)
        {
            _x9EC = NistNamedCurves.GetByName(CurveName);
            _ecDomain = new ECDomainParameters(_x9EC.Curve, _x9EC.G, _x9EC.N, _x9EC.H, seed ?? _x9EC.GetSeed());
        }

        public void ImportKeyPair(Keys<byte[], KeyCoords> keys)
        {
            localPrivateKey = new ECPrivateKeyParameters(KeyAlgorithm, new BigInteger(keys.Private), _ecDomain);
            var point = _x9EC.Curve.CreatePoint(new BigInteger(keys.Public.X), new BigInteger(keys.Public.Y));
            localPublicKey = new ECPublicKeyParameters(KeyAlgorithm, point, SecObjectIdentifiers.SecP521r1);
        }

        public void SetPublicKey(KeyCoords remoteKeyCoords)
        {
            var point = _x9EC.Curve.CreatePoint(new BigInteger(remoteKeyCoords.X), new BigInteger(remoteKeyCoords.Y));
            remotePublicKey = new ECPublicKeyParameters(KeyAlgorithm, point, SecObjectIdentifiers.SecP521r1);
        }

        public byte[] GetSharedSecret()
        {
            IBasicAgreement aKeyAgree = AgreementUtilities.GetBasicAgreement(KeyAlgorithm);
            aKeyAgree.Init(localPrivateKey);
            BigInteger sharedSecret = aKeyAgree.CalculateAgreement(remotePublicKey);
            var secret = sharedSecret.ToByteArray();
            var hmac = new HMac(new Sha512Digest());
            hmac.Init(new KeyParameter(secret));
            byte[] result = new byte[hmac.GetMacSize()];
            hmac.BlockUpdate(secret, 0, secret.Length);
            hmac.DoFinal(result, 0);

            return result;
        }

        public void CreateKeyPair(byte[] seed = null)
        {
            var keyPair = OnGenerateKeyPair(seed);
            localPrivateKey = (ECPrivateKeyParameters)keyPair.Private;
            localPublicKey = (ECPublicKeyParameters)keyPair.Public;
        }

        internal AsymmetricCipherKeyPair OnGenerateKeyPair(byte[] seed = null)
        {
            ECKeyPairGenerator g = (ECKeyPairGenerator)GeneratorUtilities.GetKeyPairGenerator(KeyAlgorithm);
            var sec = new SecureRandom();
            if (seed != null)
                sec.SetSeed(seed);
            g.Init(new ECKeyGenerationParameters(_ecDomain, sec));
            return g.GenerateKeyPair();
        }
    }
}
