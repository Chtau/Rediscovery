using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using Rediscovery.Communication.Protocol.Internal.Device;
using Rediscovery.Communication.Protocol.Internal.Diagnostic;
using Rediscovery.Communication.Protocol.Internal.Encryption;

namespace Rediscovery.Communication.Protocol.Internal.Data
{
    internal class PackagePipeline : IPackagePipeline
    {
        private readonly IProtocolLogger _logger;
        private readonly ISerializer _serializer;
        private readonly IEncryption _encryption;
        private readonly ICommunication _communication;
        private readonly ICommunication _communicationLarge;
        private readonly IDeviceManager _deviceManager;
        private readonly IDiagnosticPackage _diagnosticPackage;
        private readonly List<PackagePartState> outgoingPackages = new List<PackagePartState>();
        private readonly List<PackagePartState> outgoingLargePackages = new List<PackagePartState>();
        private readonly List<PackagePartState> incomingPackages = new List<PackagePartState>();
        private readonly List<PackagePartState> incomingLargePackages = new List<PackagePartState>();
        private readonly List<PackagePartState> incomingPackagesProxy = new List<PackagePartState>();
        private readonly List<PackagePartState> incomingLargePackagesProxy = new List<PackagePartState>();

        private string currentIdentifier;
        private Task outTask;
        private Task outLargeTask;
        private Action<byte[], string, string> incomingPackageCompleteCallback;

        public PackagePipeline(IProtocolLogger logger, 
            ISerializer serializer,
            IEncryption encryption,
            ICommunication communication,
            ICommunication communicationLarge,
            IDeviceManager deviceManager,
            IDiagnosticPackage diagnosticPackage)
        {
            _logger = logger;
            _serializer = serializer;
            _encryption = encryption;
            _deviceManager = deviceManager;
            _diagnosticPackage = diagnosticPackage;
            _communication = communication;
            _communication.Receive += Communication_Receive;
            _communicationLarge = communicationLarge;
            _communicationLarge.Receive += CommunicationLarge_Receive;
        }

        public void SetIdentifier(string identifier) => currentIdentifier = identifier;

        public void Incoming<T>(Action<T, string> instanceCallback)
        {
            try
            {
                incomingPackageCompleteCallback = (payload, identifer, callbackKey) =>
                {
                    instanceCallback.Invoke(_serializer.Deserialize<T>(payload), identifer);
                };
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }
        }

        public void IncomingRaw(Action<byte[], string, string> instanceCallback)
        {
            try
            {
                incomingPackageCompleteCallback = (payload, identifer, callbackKey) =>
                {
                    instanceCallback.Invoke(payload, identifer, callbackKey);
                };
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }
        }

        public bool Outgoing<T>(T instance, DeviceGreetingReceived deviceGreeting, string callbackKey)
        {
            return OnOutgoing(instance, deviceGreeting, callbackKey);
        }

        private bool OnOutgoing<T>(T instance, DeviceGreetingReceived deviceGreeting, string callbackKey, PackagePartState.PackageMessageType messageType = PackagePartState.PackageMessageType.Data)
        {
            try
            {
#if PIPELINE
                _logger.Trace($"{nameof(PackagePipeline)}.{nameof(OnOutgoing)} adding packages for instance of Type:\"{instance.GetType().FullName}\"");
#endif
                var rawPayload = _serializer.Serialize(instance).ToList();
                if (rawPayload.Count > (deviceGreeting.Device.Communication.DataLarge.PackageSize * 5))
                {
                    return OnCreatePackageParts(rawPayload,
                        deviceGreeting.Device.Communication.DataLarge.PackageSize,
                        deviceGreeting.Device.Identifier,
                        outgoingLargePackages,
                        () =>
                        {
                            if (outLargeTask == null)
                            {
#if PIPELINE
                                _logger.Trace($"{nameof(PackagePipeline)}.{nameof(OnOutgoing)} starting {nameof(OnOutgoingTaskRunner)} for Large data after adding packages.");
#endif
                                outLargeTask = Task.Run(() =>
                                {
                                    OnOutgoingTaskRunner(outgoingLargePackages, _communicationLarge.Send);
                                    outLargeTask = null;
                                });
                            }
                        }, messageType, callbackKey);
                }
                else
                {
                    return OnCreatePackageParts(rawPayload,
                        deviceGreeting.Device.Communication.Data.PackageSize,
                        deviceGreeting.Device.Identifier,
                        outgoingPackages,
                        () =>
                        {
                            if (outTask == null)
                            {
#if PIPELINE
                                _logger.Trace($"{nameof(PackagePipeline)}.{nameof(OnOutgoing)} starting {nameof(OnOutgoingTaskRunner)} after adding packages.");
#endif
                                outTask = Task.Run(() =>
                                {
                                    OnOutgoingTaskRunner(outgoingPackages, _communication.Send);
                                    outTask = null;
                                });
                            }
                        }, messageType, callbackKey);
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }
            return false;
        }

        private bool OnCreatePackageParts(List<byte> rawPayload, int packSize, string receiverIdentifier, List<PackagePartState> packages, Action taskRunner, PackagePartState.PackageMessageType messageType, string callbackKey)
        {
#if PIPELINE
            _logger.Trace($"{nameof(PackagePipeline)}.{nameof(OnCreatePackageParts)} create packages with from payload with size:{rawPayload.Count}");
            var beforeAdd = DateTime.UtcNow;
#endif
            var payloadSize = rawPayload.Count;
            var checksum = rawPayload.ToArray().GetHashString(HashExtensions.HashAlgorithmTypes.MD5).Substring(0, 16);

            var packs = new List<PackagePartState>();
            var index = 0;
            while (rawPayload.Count > 0)
            {
                var pack = new PackagePartState(packSize,
                    currentIdentifier,
                    receiverIdentifier,
                    checksum,
                    payloadSize,
                    index,
                    callbackKey,
                    messageType);
                // get payload based on preliminar header size
                var headerSize = pack.HeaderSizeOnly();
                var takePayload = packSize - headerSize;
                pack.SetPayload(rawPayload.Take(takePayload).ToArray());
                // remove used bytes from raw payload when added to packs
                if (takePayload > rawPayload.Count)
                    rawPayload.Clear();
                else
                    rawPayload.RemoveRange(0, takePayload);
                index++;
                packs.Add(pack);
            }

#if PIPELINE
            var timeDif = DateTime.UtcNow - beforeAdd;
            _logger.Trace($"{nameof(PackagePipeline)}.{nameof(OnCreatePackageParts)} added new packages. (Count:{packs.Count} Time:{timeDif:G})");
#endif

            packages.AddRange(packs);
            taskRunner.Invoke();

            return true;
        }

        private void OnOutgoingTaskRunner(List<PackagePartState> packages, Func<CommunicationPayload, bool> send)
        {
            try
            {
                while (packages.Count > 0)
                {
                    // invoke sender to clear the collection of created packages
#if PIPELINE
                    var totalSize = packages.Sum(x => x.PayloadPartSize);
                    _logger.Trace($"{nameof(PackagePipeline)}.{nameof(OnOutgoingTaskRunner)} total payload Bytes:{totalSize}");
                    var beforeSend = DateTime.UtcNow;
#endif
                    try
                    {
                        var item = packages.FirstOrDefault();
                        if (item != null)
                        {
                            packages.Remove(item);
                            send.Invoke(new CommunicationPayload(_encryption.EncryptAES(item.CreateSenderPackage(DateTime.UtcNow)), item.ReceiverIdentifier));
#if PIPELINE
                            _logger.Trace($"{nameof(PackagePipeline)}.{nameof(OnOutgoingTaskRunner)} Header:{item}");
#endif
                            _diagnosticPackage.Send(item);
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.Error(ex);
                    }
#if PIPELINE
                    var timeDif = DateTime.UtcNow - beforeSend;
                    var workedBytes = totalSize - packages.Sum(x => x.PayloadPartSize);
                    _logger.Trace($"{nameof(PackagePipeline)}.{nameof(OnOutgoingTaskRunner)} Transmitted Bytes:{workedBytes} Time:{timeDif:G}");
                    
#endif
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }
        }

        private void Communication_Receive(object sender, byte[] e)
        {
            try
            {
                OnReceivePackage(e, incomingPackages, incomingPackagesProxy);
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }
        }

        private void CommunicationLarge_Receive(object sender, byte[] e)
        {
            try
            {
                OnReceivePackage(e, incomingLargePackages, incomingLargePackagesProxy);
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }
        }

        private void OnReceivePackage(byte[] raw, List<PackagePartState> packages, List<PackagePartState> packagesProxy)
        {
            // create package part for this payload
            var pack = new PackagePartState(_encryption.DecryptAES(raw));
            if (pack.IsValid())
            {
#if PIPELINE
                _logger.Trace($"{nameof(PackagePipeline)}.{nameof(OnReceivePackage)} Header:{pack}");
#endif
                if (pack.PackageType == PackagePartState.PackageMessageType.Proxy)
                {
                    // handle in proxy incoming workflow
                    packagesProxy.Add(pack);
                    OnCheckCompletePackages(packagesProxy, (payload, identifer, callbackKey) =>
                    {
#if PIPELINE
                        _logger.Trace($"{nameof(PackagePipeline)}.{nameof(OnReceivePackage)} Proxy Package complete with Payload size:{payload.Length} from:{identifer}");
#endif
                        OnReceivePackage(_serializer.Deserialize<byte[]>(payload), packages, packagesProxy);
                    });
                }
                else if (string.Equals(currentIdentifier, pack.ReceiverIdentifier, StringComparison.OrdinalIgnoreCase))
                {
                    // handle in normal incoming workflow
                    packages.Add(pack);
                    OnCheckCompletePackages(packages, (payload, identifer, callbackKey) =>
                    {
                        incomingPackageCompleteCallback.Invoke(payload, identifer, callbackKey);
                    });
                }
                else
                {
                    // we are only on proxy duty
                    // we need to create new package parts with the payload
                    // because the package size for the next receiver could
                    // be different then the received package size
                    var device = _deviceManager.GetGreeting(pack.ReceiverIdentifier);
                    if (OnOutgoing(raw, device, pack.CallbackKey, PackagePartState.PackageMessageType.Proxy))
                    {
#if PIPELINE
                        _logger.Trace($"{nameof(PackagePipeline)}.{nameof(OnReceivePackage)} add new package part where we are proxy");
#endif
                    }
                }
                _diagnosticPackage.Add(pack);
            }
            else
            {
#if PIPELINE
                _logger.Warning($"{nameof(PackagePipeline)}.{nameof(OnReceivePackage)} package is not valid");
#endif
            }
        }

        private void OnCheckCompletePackages(List<PackagePartState> packages, Action<byte[], string, string> packageCompleteCallback)
        {
            if (packages.Count > 0)
            {
#if PIPELINE
                _logger.Trace($"{nameof(PackagePipeline)}.{nameof(OnCheckCompletePackages)} check if any packages are complete from the parts collection. (Parts:{packages.Count})");
#endif
                var removeChecksums = new List<string>();
                var groupItems = packages.GroupBy(x => x.Checksum);
                foreach (var item in groupItems)
                {
                    var firstHeader = item.First();
                    var payload = new List<byte>();
                    foreach (var part in item.OrderBy(x => x.Index))
                    {
                        payload.AddRange(part.PayloadPart);
                    }
                    if (payload.Count == firstHeader.PayloadSize)
                    {
                        // if the size from the aggregated payload and header size match the data should be complete
                        var checksum = payload.ToArray().GetHashString(HashExtensions.HashAlgorithmTypes.MD5).Substring(0, 16);
                        if (firstHeader.Checksum == checksum)
                        {
#if PIPELINE
                            _logger.Trace($"{nameof(PackagePipeline)}.{nameof(OnCheckCompletePackages)} Package complete with Checksum:\"{checksum}\" with payload Size:{payload.Count}");
#endif
                            packageCompleteCallback.Invoke(payload.ToArray(), firstHeader.SenderIdentifier, firstHeader.CallbackKey);
                            removeChecksums.Add(checksum);
                            _diagnosticPackage.PackageComplete(checksum);
                        } else
                        {
#if PIPELINE
                            _logger.Warning($"{nameof(PackagePipeline)}.{nameof(OnCheckCompletePackages)} aggregated payload size matches header provided size but Checksum match failed");
#endif
                        }
                    }
                }
                if (removeChecksums.Count > 0)
                {
                    foreach (var checksum in removeChecksums)
                    {
                        packages.RemoveAll(x => x.Checksum == checksum);
                    }
                }
            }
        }
    }
}
