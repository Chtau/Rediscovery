using CommunicationBase;
using Grpc.Core;
using SharedBase.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CommunicationLoggerConsumer
{
    public class LoggerConsumer : ILoggerConsumer
    {
        private readonly IDirectLogger _logger;

        private Logger.LoggerExchange.LoggerExchangeClient exchangeClient;
        private IClientStreamWriter<Logger.LogEntry> _requestStream;

        private Channel channel = null;
        private CancellationTokenSource ctsLogger = null;

        public bool IsConnect
        {
            get
            {
                return channel != null && channel.State != ChannelState.Shutdown && channel.State != ChannelState.TransientFailure;
            }
        }

        public LoggerConsumer()
        {
            _logger = new DirectLogger();
        }

        public LoggerConsumer(IDirectLogger logger)
        {
            _logger = logger;
        }

        public bool Connect(string ipAddress, int port, string certificatePEM)
        {
            try
            {
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

        public void StartLogger(string token, CancellationTokenSource cts = null)
        {
            Task.Run(async () =>
            {
                if (cts == null)
                    ctsLogger = new CancellationTokenSource();
                else
                    ctsLogger = cts;
                try
                {
                    var meta = new Metadata();
                    meta.AddAuthorizationHeader(token);
                    using (var call = exchangeClient.Add(headers: meta, cancellationToken: ctsLogger.Token))
                    {
                        _requestStream = call.RequestStream;
                        do
                        {
                            await Task.Delay(100);
                        } while (!ctsLogger.IsCancellationRequested);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogException(ex);
                }
                finally
                {
                    _requestStream = null;
                    ctsLogger.Cancel();
                }
            });
        }

        public void LogEntry(LoggerEntry loggerEntry)
        {
            Task.Run(async () =>
            {
                try
                {
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

                    await _requestStream.WriteAsync(new Logger.LogEntry
                    {
                        Id = loggerEntry.Id,
                        LoggerType = logLevel,
                        Message = loggerEntry.Message,
                        Module = loggerEntry.Module,
                        Time = loggerEntry.Time.DatetimeTicksLong(),
                    });
                }
                catch (Exception ex)
                {
                    _logger.LogException(ex);
                }
            });
        }
    }
}
