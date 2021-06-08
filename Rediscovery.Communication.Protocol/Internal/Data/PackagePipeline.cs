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
        private readonly List<PackagePartState> outgoingPackages = new List<PackagePartState>();

        private string currentIdentifier;
        private Task outTask;

        public PackagePipeline(IProtocolLogger logger, ISerializer serializer, ICommunication communication)
        {
            _logger = logger;
            _serializer = serializer;
            _communication = communication;
            _communication.Receive += Communication_Receive;
        }

        public void SetIdentifier(string identifier) => currentIdentifier = identifier;

        public T Incoming<T>(byte[] raw)
        {
            try
            {
                return _serializer.Deserialize<T>(raw);
            } catch (Exception ex)
            {
                _logger.Error(ex);
            }
            return default;
        }

        public bool Outgoing<T>(T instance, DeviceGreetingReceived deviceGreeting)
        {
            try
            {
#if PIPELINE
                _logger.Trace($"{nameof(PackagePipeline)}.{nameof(Outgoing)} start adding packages for instance of Type:\"{instance.GetType().FullName}\"");
                var beforeAdd = DateTime.UtcNow;
#endif
                var rawPayload = _serializer.Serialize(instance).ToList();
                var payloadSize = rawPayload.Count;
                var checksum = rawPayload.ToArray().GetHashString(HashExtensions.HashAlgorithmTypes.MD5).Substring(0, 16);
                
                var packSize = deviceGreeting.Device.Communication.Data.PackageSize;

                var packs = new List<PackagePartState>();
                var index = 0;
                while (rawPayload.Count > 0)
                {
                    var pack = new PackagePartState(packSize,
                        currentIdentifier,
                        deviceGreeting.Device.Identifier,
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

                lock (outgoingPackages)
                {
                    outgoingPackages.AddRange(packs);
                }
#if PIPELINE
                var timeDif = DateTime.UtcNow - beforeAdd;
                _logger.Trace($"{nameof(PackagePipeline)}.{nameof(Outgoing)} added new packages. (Count:{packs.Count} Time:{timeDif:G})");
#endif
                if (outTask == null)
                {
#if PIPELINE
                    _logger.Trace($"{nameof(PackagePipeline)}.{nameof(Outgoing)} starting {nameof(OnOutgoingTaskRunner)} after adding packages.");
#endif
                    outTask = Task.Run(OnOutgoingTaskRunner);
                }

                return true;
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }
            return false;
        }

        private void OnOutgoingTaskRunner()
        {
            try
            {
                while (outgoingPackages.Count > 0)
                {
                    // invoke sender to clear the collection of created packages
#if PIPELINE
                    var totalSize = outgoingPackages.Sum(x => x.PayloadSize);
                    _logger.Trace($"{nameof(PackagePipeline)}.{nameof(OnOutgoingTaskRunner)} Total Bytes:{totalSize}");
                    var beforeSend = DateTime.UtcNow;
#endif
                    try
                    {
                        var item = outgoingPackages.FirstOrDefault();
                        if (item != null)
                        {
                            outgoingPackages.Remove(item);
                            _communication.Send(new CommunicationPayload(item.CreateSenderPackage(DateTime.UtcNow), item.ReceiverIdentifier));
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.Error(ex);
                    }
#if PIPELINE
                    var timeDif = DateTime.UtcNow - beforeSend;
                    var workedBytes = totalSize - outgoingPackages.Sum(x => x.PayloadSize);
                    _logger.Trace($"{nameof(PackagePipeline)}.{nameof(OnOutgoingTaskRunner)} Transmitted Bytes:{workedBytes} Time:{timeDif:G}");
#endif
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            } finally
            {
                outTask = null;
            }
        }

        private void Communication_Receive(object sender, CommunicationPayload e)
        {
            try
            {
                // TODO: create package part for this payload
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }
        }
    }
}
