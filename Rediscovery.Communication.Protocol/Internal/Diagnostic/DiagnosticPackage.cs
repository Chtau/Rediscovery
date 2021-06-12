using Rediscovery.Communication.Protocol.Internal.Data;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Rediscovery.Communication.Protocol.Internal.Diagnostic
{
    internal class DiagnosticPackage : IDiagnosticPackage
    {
        private readonly IProtocolLogger _logger;

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

                }
                catch (Exception ex)
                {
                    _logger.Error(ex);
                }
            });
        }
    }
}
