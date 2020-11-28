using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Rediscovery.Shared.Base.Extensions;
using Rediscovery.Communication.Base;
using Rediscovery.Shared.Logging;
using Rediscovery.Shared.Logging.Models;
using Rediscovery.Shared.Logging.Commands;

namespace Rediscovery.Communication.Provider.Logger.ProtoService
{
    public class LoggerExchangeService : ProtoLogger.LoggerExchange.LoggerExchangeBase
    {
        private readonly IDirectLogger _directLogger;
        private readonly ILoggerHandler _loggerHandler;

        public LoggerExchangeService(IDirectLogger directLogger, ILoggerHandler loggerHandler)
        {
            _directLogger = directLogger;
            _loggerHandler = loggerHandler;
        }

        [Authorize(Policy = "DeviceAndConsumer")]
        public override async Task Add(IAsyncStreamReader<ProtoLogger.LogEntry> requestStream, IServerStreamWriter<ProtoLogger.LogState> responseStream, ServerCallContext context)
        {
            try
            {
                var readTask = Task.Run(async () =>
                {
                    await foreach (var message in requestStream.ReadAllAsync(cancellationToken: context.CancellationToken))
                    {
                        try
                        {
                            await responseStream.WriteAsync(new ProtoLogger.LogState
                            {
                                Ok = true
                            });

                            var user = context.GetHttpContext().User;
                            string sid = user.Claims.GetSid();

                            LoggerType loggerType = LoggerType.Debug;
                            switch (message.LoggerType)
                            {
                                case ProtoLogger.LogEntry.Types.LoggerType.Trace:
                                    loggerType = LoggerType.Trace;
                                    break;
                                case ProtoLogger.LogEntry.Types.LoggerType.Debug:
                                    loggerType = LoggerType.Debug;
                                    break;
                                case ProtoLogger.LogEntry.Types.LoggerType.Information:
                                    loggerType = LoggerType.Information;
                                    break;
                                case ProtoLogger.LogEntry.Types.LoggerType.Warning:
                                    loggerType = LoggerType.Warning;
                                    break;
                                case ProtoLogger.LogEntry.Types.LoggerType.Error:
                                    loggerType = LoggerType.Error;
                                    break;
                                case ProtoLogger.LogEntry.Types.LoggerType.Critical:
                                    loggerType = LoggerType.Critical;
                                    break;
                                default:
                                    break;
                            }

                            _loggerHandler.NewEntry(new LoggerEntry
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

        [Authorize(Policy = "ResourceConsumer")]
        public override Task<ProtoLogger.LogCommandState> LoggerCommand(ProtoLogger.LogCommandConfig request, ServerCallContext context)
        {
            try
            {
                Command cmdType = Command.State;
                switch (request.LogCommand)
                {
                    case ProtoLogger.LogCommandConfig.Types.Command.Clear:
                        cmdType = Command.Clear;
                        break;
                    case ProtoLogger.LogCommandConfig.Types.Command.ChangeLogLevel:
                        cmdType = Command.ChangeLogLevel;
                        break;
                    case ProtoLogger.LogCommandConfig.Types.Command.State:
                        cmdType = Command.State;
                        break;
                    default:
                        break;
                }
                var cmdId = new Guid(request.Id);

                var logCommand = new LogCommandConfig
                {
                    Id = cmdId,
                    Data = request.Data,
                    CommandType = cmdType
                };
                var result = _loggerHandler.ExecuteCommand(logCommand);
                return Task.FromResult(new ProtoLogger.LogCommandState { Id = result.Id.ToString(), Ok = result.Result, Data = result.Data.EmptyIfNull(), });
            }
            catch (Exception ex)
            {
                _directLogger.LogException(ex);
            }
            return Task.FromResult(new ProtoLogger.LogCommandState { Id = request.Id, Ok = false, Data = "" });
        }
    }
}
