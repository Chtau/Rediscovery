using Rediscovery.Communication.Protocol.Internal.Data;
using Rediscovery.Communication.Protocol.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rediscovery.Communication.Protocol.Internal.Diagnostic
{
    internal class DiagnosticPackage : IDiagnosticPackage
    {
        private readonly IProtocolLogger _logger;

        public Traffic Traffic { get; private set; } = new Traffic();
        public List<Timing> Timings { get; private set; } = new List<Timing>();

        public DiagnosticPackage(IProtocolLogger logger)
        {
            _logger = logger;
        }

        public void Add(PackagePartState package)
        {
            Task.Run(() =>
            {
                try
                {
                    Traffic.AddIncomingPackageParts();
                    var difTimestamp = package.SenderTimestamp - package.ReceivedTimestamp;
                    var index = Timings.FindIndex(x => x.DeviceIdentifer == package.SenderIdentifier);
                    if (index != -1)
                        Timings[index].Add(difTimestamp);
                    else
                        Timings.Add(new Timing(package.SenderIdentifier, difTimestamp));
                } catch (Exception ex)
                {
                    _logger.Error(ex);
                }
            });
        }

        public void PackageComplete(string checksum)
        {
            Task.Run(() =>
            {
                try
                {
                    Traffic.AddIncomingPackagesCompleted();
                }
                catch (Exception ex)
                {
                    _logger.Error(ex);
                }
            });
        }

        public void Send(PackagePartState package)
        {
            Task.Run(() =>
            {
                try
                {
                    Traffic.AddOutgoingPackageParts();
                }
                catch (Exception ex)
                {
                    _logger.Error(ex);
                }
            });
        }
    }
}
