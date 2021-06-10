using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using Rediscovery.Communication.Protocol.Internal.Device;

namespace Rediscovery.Communication.Protocol.Internal.Data
{
    internal class PackagePipeline : IPackagePipeline
    {
        private readonly IProtocolLogger _logger;
        private readonly ISerializer _serializer;
        private readonly ICommunication _communication;
        private readonly ICommunication _communicationLarge;
        private readonly IDeviceManager _deviceManager;
        private readonly List<PackagePartState> outgoingPackages = new List<PackagePartState>();
        private readonly List<PackagePartState> outgoingLargePackages = new List<PackagePartState>();
        private readonly List<PackagePartState> incomingPackages = new List<PackagePartState>();
        private readonly List<PackagePartState> incomingLargePackages = new List<PackagePartState>();

        private string currentIdentifier;
        private Task outTask;
        private Task outLargeTask;
        private Action<byte[], string> incomingPackageCompleteCallback;

        public PackagePipeline(IProtocolLogger logger, 
            ISerializer serializer, 
            ICommunication communication,
            ICommunication communicationLarge,
            IDeviceManager deviceManager)
        {
            _logger = logger;
            _serializer = serializer;
            _deviceManager = deviceManager;
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
                incomingPackageCompleteCallback = (payload, identifer) =>
                {
                    instanceCallback.Invoke(_serializer.Deserialize<T>(payload), identifer);
                };
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }
        }

        public bool Outgoing<T>(T instance, DeviceGreetingReceived deviceGreeting)
        {
            try
            {
#if PIPELINE
                _logger.Trace($"{nameof(PackagePipeline)}.{nameof(Outgoing)} adding packages for instance of Type:\"{instance.GetType().FullName}\"");
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
                                _logger.Trace($"{nameof(PackagePipeline)}.{nameof(OnCreatePackageParts)} starting {nameof(OnOutgoingTaskRunner)} for Large data after adding packages.");
#endif
                                outLargeTask = Task.Run(() =>
                                {
                                    OnOutgoingTaskRunner(outgoingLargePackages, _communicationLarge.Send);
                                    outLargeTask = null;
                                });
                            }
                        });
                } else
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
                                _logger.Trace($"{nameof(PackagePipeline)}.{nameof(OnCreatePackageParts)} starting {nameof(OnOutgoingTaskRunner)} after adding packages.");
#endif
                                outTask = Task.Run(() =>
                                {
                                    OnOutgoingTaskRunner(outgoingPackages, _communication.Send);
                                    outTask = null;
                                });
                            }
                        });
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }
            return false;
        }

        private bool OnCreatePackageParts(List<byte> rawPayload, int packSize, string receiverIdentifier, List<PackagePartState> packages, Action taskRunner)
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
                    index);
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
                            send.Invoke(new CommunicationPayload(item.CreateSenderPackage(DateTime.UtcNow), item.ReceiverIdentifier));
#if PIPELINE
                            _logger.Trace($"{nameof(PackagePipeline)}.{nameof(OnOutgoingTaskRunner)} Header:{item.DumpHeader()}");
#endif
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
                OnReceivePackage(e, incomingPackages);
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
                OnReceivePackage(e, incomingLargePackages);
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }
        }

        private void OnReceivePackage(byte[] raw, List<PackagePartState> packages)
        {
            // create package part for this payload
            var pack = new PackagePartState(raw);
            if (pack.IsValid())
            {
#if PIPELINE
                _logger.Trace($"{nameof(PackagePipeline)}.{nameof(Communication_Receive)} Header:{pack.DumpHeader()}");
#endif
                if (string.Equals(currentIdentifier, pack.ReceiverIdentifier, StringComparison.OrdinalIgnoreCase))
                {
                    // handle in normal incoming workflow
                    packages.Add(pack);
                    OnCheckCompletePackages(packages);
                }
                else
                {
                    // we are only on proxy duty
                    // we need to create new package parts with the payload
                    // because the package size for the next receiver could
                    // be different then the received package size

                    // TODO: we need to create packages which are compatible with the original Index & Checksum
                    var payload = pack.PayloadPart;
                    var device = _deviceManager.GetGreeting(pack.ReceiverIdentifier);
                    /*
                    if (OnCreatePackageParts(payload.ToList(), device.Device.Communication.Data.PackageSize, device.Device.Identifier, outgoingPackages))
                    {
#if PIPELINE
                        _logger.Trace($"{nameof(PackagePipeline)}.{nameof(Communication_Receive)} add new package part where we are proxy");
#endif
                    }
                    else
                    {
#if PIPELINE
                        _logger.Warning($"{nameof(PackagePipeline)}.{nameof(Communication_Receive)} proxy package failed to add");
#endif
                    }
                    */
                }
            }
            else
            {
#if PIPELINE
                _logger.Warning($"{nameof(PackagePipeline)}.{nameof(Communication_Receive)} package is not valid");
#endif
            }
        }

        private void OnCheckCompletePackages(List<PackagePartState> packages)
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
                            incomingPackageCompleteCallback.Invoke(payload.ToArray(), firstHeader.SenderIdentifier);
                            removeChecksums.Add(checksum);
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
