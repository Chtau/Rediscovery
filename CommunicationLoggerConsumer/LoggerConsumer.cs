using Rediscovery.Communication.Base;
using Grpc.Core;
using Rediscovery.Shared.Base.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Rediscovery.Shared.Base.Extensions;

namespace Rediscovery.Communication.Logger.Consumer
{
    public class LoggerConsumer : ILoggerConsumer
    {
        private readonly IDirectLogger _logger;

        private ProtoLogger.LoggerExchange.LoggerExchangeClient exchangeClient;
        private IClientStreamWriter<ProtoLogger.LogEntry> _requestStream;
        private ConcurrentQueue<LoggerEntry> concurrentQueue = new ConcurrentQueue<LoggerEntry>();

        private Channel channel = null;
        private CancellationTokenSource ctsLogger = null;

        public event EventHandler<Rediscovery.Shared.Base.Logging.LogCommandConfigResult> LoggerCommandExecuted;

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

        public bool Connect(ConsumerConnectionConfiguration connectionConfiguration)
        {
            try
            {
                channel = ChannelHelper.CreateChannel(connectionConfiguration);
                exchangeClient = new ProtoLogger.LoggerExchange.LoggerExchangeClient(channel);
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
            Task.Run(async () =>
            {
                do
                {
                    await Task.Delay(100);
                    if (!concurrentQueue.IsEmpty)
                    {
                        if (concurrentQueue.TryDequeue(out LoggerEntry entry))
                        {
                            await OnTryToSend(entry);
                        }
                    }
                } while (true);
            });
        }

        private async Task OnTryToSend(LoggerEntry loggerEntry)
        {
            try
            {
                var logLevel = ProtoLogger.LogEntry.Types.LoggerType.Information;
                switch (loggerEntry.LogLevel)
                {
                    case LoggerEntry.LoggerType.Trace:
                        logLevel = ProtoLogger.LogEntry.Types.LoggerType.Trace;
                        break;
                    case LoggerEntry.LoggerType.Debug:
                        logLevel = ProtoLogger.LogEntry.Types.LoggerType.Debug;
                        break;
                    case LoggerEntry.LoggerType.Information:
                        logLevel = ProtoLogger.LogEntry.Types.LoggerType.Information;
                        break;
                    case LoggerEntry.LoggerType.Warning:
                        logLevel = ProtoLogger.LogEntry.Types.LoggerType.Warning;
                        break;
                    case LoggerEntry.LoggerType.Error:
                        logLevel = ProtoLogger.LogEntry.Types.LoggerType.Error;
                        break;
                    case LoggerEntry.LoggerType.Critical:
                        logLevel = ProtoLogger.LogEntry.Types.LoggerType.Critical;
                        break;
                    default:
                        break;
                }
                await _requestStream.WriteAsync(new ProtoLogger.LogEntry
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
        }

        public void LogEntry(LoggerEntry loggerEntry)
        {
            concurrentQueue.Enqueue(loggerEntry);
        }

        public void LoggerCommand(string token, LogCommandConfig logCommandConfig)
        {
            Task.Run(async () =>
            {
                var cts = new CancellationTokenSource();
                try
                {
                    var meta = new Metadata();
                    meta.AddAuthorizationHeader(token);

                    ProtoLogger.LogCommandConfig.Types.Command commandType = ProtoLogger.LogCommandConfig.Types.Command.State;
                    switch (logCommandConfig.CommandType)
                    {
                        case LogCommandConfig.Command.Clear:
                            commandType = ProtoLogger.LogCommandConfig.Types.Command.Clear;
                            break;
                        case LogCommandConfig.Command.ChangeLogLevel:
                            commandType = ProtoLogger.LogCommandConfig.Types.Command.ChangeLogLevel;
                            break;
                        case LogCommandConfig.Command.State:
                            commandType = ProtoLogger.LogCommandConfig.Types.Command.State;
                            break;
                        default:
                            break;
                    }
                    ProtoLogger.LogCommandConfig logCommand = new ProtoLogger.LogCommandConfig
                    {
                        Id = logCommandConfig.Id.ToString(),
                        Data = logCommandConfig.Data.EmptyIfNull(),
                        LogCommand = commandType
                    };

                    var reply = await exchangeClient.LoggerCommandAsync(logCommand, cancellationToken: cts.Token, headers: meta);
                    LoggerCommandExecuted?.Invoke(this, new LogCommandConfigResult { Id = new Guid(reply.Id), Result = reply.Ok, Data = reply.Data });
                }
                catch (Exception ex)
                {
                    _logger.LogException(ex);
                }
                finally
                {
                    cts.Cancel();
                }
            });
        }
    }
}
