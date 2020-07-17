using CommunicationBase;
using Grpc.Core;
using SharedBase.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CommunicationLoggerConsumer
{
    public class LoggerConsumer : ILoggerConsumer
    {
        private readonly IDirectLogger _logger;

        private Logger.LoggerExchange.LoggerExchangeClient exchangeClient;

        private Channel channel = null;
        private string authorizationToken = null;

        public LoggerConsumer()
        {
            _logger = new DirectLogger();
        }

        public LoggerConsumer(IDirectLogger logger)
        {
            _logger = logger;
        }

        public bool Connect(string ipAddress, int port, string certificatePEM, string authorizationToken)
        {
            try
            {
                this.authorizationToken = authorizationToken;
                var channelCredentials = new SslCredentials(certificatePEM);
                channel = new Channel(ipAddress, port, channelCredentials);
                exchangeClient = new Logger.LoggerExchange.LoggerExchangeClient(channel);
                return exchangeClient != null;
            }
            catch (Exception ex)
            {
                _logger.LogException(ex);
                return false;
            }
        }

        public bool Disconnect()
        {
            try
            {
                channel?.ShutdownAsync().GetAwaiter();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogException(ex);
                return false;
            }
        }

        public void LogEntry(LoggerEntry loggerEntry)
        {
            Task.Run(async () =>
            {
                try
                {
                    var meta = new Metadata();
                    meta.AddAuthorizationHeader(authorizationToken);
                    using (var call = exchangeClient.Add(headers: meta))
                    {
                        var requestStream = call.RequestStream;

                        var logLevel = Logger.LogEntry.Types.LoggerType.Information;
                        switch (loggerEntry.LogLevel)
                        {
                            case LoggerEntry.LoggerType.Trace:
                                logLevel = Logger.LogEntry.Types.LoggerType.Trace;
                                break;
                            case LoggerEntry.LoggerType.Debug:
                                logLevel = Logger.LogEntry.Types.LoggerType.Debug;
                                break;
                            case LoggerEntry.LoggerType.Information:
                                logLevel = Logger.LogEntry.Types.LoggerType.Information;
                                break;
                            case LoggerEntry.LoggerType.Warning:
                                logLevel = Logger.LogEntry.Types.LoggerType.Warning;
                                break;
                            case LoggerEntry.LoggerType.Error:
                                logLevel = Logger.LogEntry.Types.LoggerType.Error;
                                break;
                            case LoggerEntry.LoggerType.Critical:
                                logLevel = Logger.LogEntry.Types.LoggerType.Critical;
                                break;
                            default:
                                break;
                        }

                        await requestStream.WriteAsync(new Logger.LogEntry
                        {
                            Id = loggerEntry.Id,
                            LoggerType = logLevel,
                            Message = loggerEntry.Message,
                            Module = loggerEntry.Module,
                            Time = loggerEntry.Time.DatetimeTicksLong(),
                        });
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogException(ex);
                }
            });
        }
    }
}
