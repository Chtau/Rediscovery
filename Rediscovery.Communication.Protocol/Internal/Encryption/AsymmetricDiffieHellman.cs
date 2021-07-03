using Org.BouncyCastle.Asn1.Nist;
using Org.BouncyCastle.Asn1.Sec;
using Org.BouncyCastle.Asn1.X9;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Digests;
using Org.BouncyCastle.Crypto.Generators;
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
            //X9ECParameters x9EC = NistNamedCurves.GetByName("P-521");
            //ECDomainParameters ecDomain = new ECDomainParameters(x9EC.Curve, x9EC.G, x9EC.N, x9EC.H, x9EC.GetSeed());
            var keyPair = instance.OnGenerateKeyPair(seed);// GenerateKeyPair(ecDomain);
            var privateKey = (ECPrivateKeyParameters)keyPair.Private;
            var publicKey = (ECPublicKeyParameters)keyPair.Public;

            /*ECKeyPairGenerator g = (ECKeyPairGenerator)GeneratorUtilities.GetKeyPairGenerator("ECDH");
            g.Init(new ECKeyGenerationParameters(ecDomain, new SecureRandom()));

            AsymmetricCipherKeyPair aliceKeyPair = g.GenerateKeyPair();
            */
            //var p1 = new ECPrivateKeyParameters(privateKey.AlgorithmName, privateKey.D, privateKey.Parameters);
            //var p2 = new ECPrivateKeyParameters(privateKey.AlgorithmName, privateKey.D, ecDomain);
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
            return sharedSecret.ToByteArray();
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

        /*public static byte[] KeyExchange(Uri url)
        {
            X9ECParameters x9EC = NistNamedCurves.GetByName("P-521");
            ECDomainParameters ecDomain = new ECDomainParameters(x9EC.Curve, x9EC.G, x9EC.N, x9EC.H, x9EC.GetSeed());
            AsymmetricCipherKeyPair aliceKeyPair = GenerateKeyPair(ecDomain);

            ECPublicKeyParameters alicePublicKey = (ECPublicKeyParameters)aliceKeyPair.Public;
            ECPublicKeyParameters bobPublicKey = GetBobPublicKey(url, x9EC, alicePublicKey);

            byte[] AESKey = GenerateAESKey(bobPublicKey, aliceKeyPair.Private);

            return AESKey;
        }

        private static AsymmetricCipherKeyPair GenerateKeyPair(ECDomainParameters ecDomain)
        {
            ECKeyPairGenerator g = (ECKeyPairGenerator)GeneratorUtilities.GetKeyPairGenerator("ECDH");
            g.Init(new ECKeyGenerationParameters(ecDomain, new SecureRandom()));

            AsymmetricCipherKeyPair aliceKeyPair = g.GenerateKeyPair();
            return aliceKeyPair;
        }

        private static ECPublicKeyParameters GetBobPublicKey(Uri url,
                                                    X9ECParameters x9EC,
                                                    ECPublicKeyParameters alicePublicKey)
        {
            var bobCoords = GetBobCoords(url, alicePublicKey);
            var point = x9EC.Curve.CreatePoint(new BigInteger(bobCoords.X), new BigInteger(bobCoords.Y));
            return new ECPublicKeyParameters("ECDH", point, SecObjectIdentifiers.SecP521r1);
        }

        private static KeyCoords GetBobCoords(Uri url, ECPublicKeyParameters publicKey)
        {
            string xml = GetXmlString(publicKey);

            string responseXml = null;// Encoding.UTF8.GetString(Http.Post(url, Encoding.UTF8.GetBytes(xml)));

            XmlDocument doc = new XmlDocument();
            doc.LoadXml(responseXml);
            XmlElement root = doc.DocumentElement;
            XmlNodeList elemList = doc.DocumentElement.GetElementsByTagName("PublicKey");

            return new KeyCoords(new BigInteger(elemList[0].FirstChild.Attributes["Value"].Value).ToByteArray(),
                new BigInteger(elemList[0].LastChild.Attributes["Value"].Value).ToByteArray());
        }

        private static string GetXmlString(ECPublicKeyParameters publicKeyParameters)
        {
            string publicKeyXmlTemplate = @"<ECDHKeyValue xmlns=""http://www.w3.org/2001/04/xmldsig-more#""> <DomainParameters> <NamedCurve URN=""urn:oid:1.3.132.0.35"" /> </DomainParameters> <PublicKey> <X Value=""X_VALUE"" xsi:type=""PrimeFieldElemType"" xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" /> <Y Value=""Y_VALUE"" xsi:type=""PrimeFieldElemType"" xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" /> </PublicKey> </ECDHKeyValue>";
            string xml = publicKeyXmlTemplate;
            xml = xml.Replace("X_VALUE", publicKeyParameters.Q.AffineXCoord.ToBigInteger().ToString());
            xml = xml.Replace("Y_VALUE", publicKeyParameters.Q.AffineYCoord.ToBigInteger().ToString());
            return xml;
        }

        private static byte[] GenerateAESKey(ECPublicKeyParameters bobPublicKey,
                                AsymmetricKeyParameter alicePrivateKey)
        {
            IBasicAgreement aKeyAgree = AgreementUtilities.GetBasicAgreement("ECDH");
            aKeyAgree.Init(alicePrivateKey);
            BigInteger sharedSecret = aKeyAgree.CalculateAgreement(bobPublicKey);
            byte[] sharedSecretBytes = sharedSecret.ToByteArray();
            // TODO: should use HMACSHA256
            IDigest digest = new Sha256Digest();
            byte[] symmetricKey = new byte[digest.GetDigestSize()];
            digest.BlockUpdate(sharedSecretBytes, 0, sharedSecretBytes.Length);
            digest.DoFinal(symmetricKey, 0);

            return symmetricKey;
        }*/
    }
}
