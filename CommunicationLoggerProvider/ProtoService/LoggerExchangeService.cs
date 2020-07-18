using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Logger;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CommunicationLoggerProvider.ProtoService
{
    public class LoggerExchangeService : Logger.LoggerExchange.LoggerExchangeBase
    {
        private readonly IDirectLogger _directLogger;
        private readonly ILoggerHandler _loggerHandler;

        public LoggerExchangeService(IDirectLogger directLogger, ILoggerHandler loggerHandler)
        {
            _directLogger = directLogger;
            _loggerHandler = loggerHandler;
        }

        [Authorize(Policy = "DeviceAndConsumer")]
        public override async Task Add(IAsyncStreamReader<LogEntry> requestStream, IServerStreamWriter<LogState> responseStream, ServerCallContext context)
        {
            try
            {
                var readTask = Task.Run(async () =>
                {
                    await foreach (var message in requestStream.ReadAllAsync(cancellationToken: context.CancellationToken))
                    {
                        try
                        {
                            await responseStream.WriteAsync(new LogState
                            {
                                Ok = true
                            });

                            var user = context.GetHttpContext().User;
                            string sid = user.Claims.GetSid();

                            //await responseStream.WriteAsync(message);
                            //_heartbeatStatistic.NewBeat(new CommunicationHeartbeatProvider.HeartbeatResult(sid, true, new TimeSpan((long)message.LastRoundTripTicks), new DateTime((long)message.Ticks)));

                            SharedBase.Logging.LoggerEntry.LoggerType loggerType = SharedBase.Logging.LoggerEntry.LoggerType.Debug;
                            switch (message.LoggerType)
                            {
                                case LogEntry.Types.LoggerType.Trace:
                                    loggerType = SharedBase.Logging.LoggerEntry.LoggerType.Trace;
                                    break;
                                case LogEntry.Types.LoggerType.Debug:
                                    loggerType = SharedBase.Logging.LoggerEntry.LoggerType.Debug;
                                    break;
                                case LogEntry.Types.LoggerType.Information:
                                    loggerType = SharedBase.Logging.LoggerEntry.LoggerType.Information;
                                    break;
                                case LogEntry.Types.LoggerType.Warning:
                                    loggerType = SharedBase.Logging.LoggerEntry.LoggerType.Warning;
                                    break;
                                case LogEntry.Types.LoggerType.Error:
                                    loggerType = SharedBase.Logging.LoggerEntry.LoggerType.Error;
                                    break;
                                case LogEntry.Types.LoggerType.Critical:
                                    loggerType = SharedBase.Logging.LoggerEntry.LoggerType.Critical;
                                    break;
                                default:
                                    break;
                            }

                            _loggerHandler.NewEntry(new SharedBase.Logging.LoggerEntry
                            {
                                Id = message.Id,
                                Message = message.Message,
                                Module = message.Module,
                                LogLevel = loggerType,
                                Sid = sid,
                                Time = message.Time.TicksLongDatetimeNotNull(),
                            });
                        }
                        catch (Exception ex)
                        {
                            _directLogger.LogException(ex);
                        }
                    }
                });
                do
                {
                    await Task.Delay(100);
                } while (!context.CancellationToken.IsCancellationRequested);
            }
            catch (Exception ex)
            {
                _directLogger.LogException(ex);
            }
        }

        /*[Authorize(Policy = "DeviceAndConsumer")]
        public override async Task<Empty> Add(IAsyncStreamReader<LogEntry> requestStream, ServerCallContext context)
        {
            try
            {
                var readTask = Task.Run(async () =>
                {
                    await foreach (var message in requestStream.ReadAllAsync(cancellationToken: context.CancellationToken))
                    {
                        try
                        {
                            System.Diagnostics.Debug.Print("Logger new LogEntry received");
                            var user = context.GetHttpContext().User;
                            string sid = user.Claims.GetSid();

                            SharedBase.Logging.LoggerEntry.LoggerType loggerType = SharedBase.Logging.LoggerEntry.LoggerType.Debug;
                            switch (message.LoggerType)
                            {
                                case LogEntry.Types.LoggerType.Trace:
                                    loggerType = SharedBase.Logging.LoggerEntry.LoggerType.Trace;
                                    break;
                                case LogEntry.Types.LoggerType.Debug:
                                    loggerType = SharedBase.Logging.LoggerEntry.LoggerType.Debug;
                                    break;
                                case LogEntry.Types.LoggerType.Information:
                                    loggerType = SharedBase.Logging.LoggerEntry.LoggerType.Information;
                                    break;
                                case LogEntry.Types.LoggerType.Warning:
                                    loggerType = SharedBase.Logging.LoggerEntry.LoggerType.Warning;
                                    break;
                                case LogEntry.Types.LoggerType.Error:
                                    loggerType = SharedBase.Logging.LoggerEntry.LoggerType.Error;
                                    break;
                                case LogEntry.Types.LoggerType.Critical:
                                    loggerType = SharedBase.Logging.LoggerEntry.LoggerType.Critical;
                                    break;
                                default:
                                    break;
                            }

                            _loggerHandler.NewEntry(new SharedBase.Logging.LoggerEntry
                            {
                                Id = message.Id,
                                Message = message.Message,
                                Module = message.Module,
                                LogLevel = loggerType,
                                Sid = sid,
                                Time = message.Time.TicksLongDatetimeNotNull(),
                            });
                        }
                        catch (Exception ex)
                        {
                            _directLogger.LogException(ex);
                        }
                    }
                });
                do
                {
                    await Task.Delay(100);
                } while (!context.CancellationToken.IsCancellationRequested);
            }
            catch (Exception ex)
            {
                _directLogger.LogException(ex);
            }
            return new Empty();
        }*/
    }
}
