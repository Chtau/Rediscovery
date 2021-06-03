using Rediscovery.Communication.Protocol.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace Rediscovery.Communication.Protocol.Internal.Listener
{
    internal class DataListener : IListener
    {
        private readonly IProtocolLogger _logger;
        private readonly IPackagePipeline _packagePipeline;
        private readonly string threadName = $"Thread_{nameof(DataListener)}";

        private Thread listenThread;

        internal DataConfiguration configuration;
        private bool working = false;
        private static readonly ManualResetEvent allDone = new ManualResetEvent(false);
        private Action<StateComplete> stateCompleteCallback;

        public DataListener(IProtocolLogger protocolLogger, IPackagePipeline packagePipeline)
        {
            _logger = protocolLogger;
            _packagePipeline = packagePipeline;
            OnInitThread();
        }

        public void Initialize(BaseConfiguration configuration)
        {
            this.configuration = (DataConfiguration)configuration;
        }

        public void Start()
        {
            try
            {
                working = true;
                listenThread.Start();
            }
            catch (System.Threading.ThreadStateException tsEx)
            {
                _logger.Warning(tsEx);
                OnInitThread();
                try
                {
                    working = true;
                    listenThread.Start();
                }
                catch (Exception ex)
                {
                    _logger.Error(ex);
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }
        }

        public void StateCompleteListener(Action<StateComplete> callback)
        {
            stateCompleteCallback = callback;
        }

        public void Stop()
        {
            try
            {
                working = false;
                listenThread?.Abort();
            }
            catch (PlatformNotSupportedException) { }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }
        }

        private void OnInitThread()
        {
            try
            {
                listenThread = new System.Threading.Thread(() =>
                {
                    try
                    {
                        Socket listener = new Socket(IPAddress.Any.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                        listener.Bind(new IPEndPoint(IPAddress.Any, configuration.Connection.ListenPort));
                        listener.Listen(10);

                        while (working)
                        {
                            // Set the event to nonsignaled state.  
                            allDone.Reset();

                            // Start an asynchronous socket to listen for connections.  
                            listener.BeginAccept(
                                new AsyncCallback(AcceptCallback),
                                listener);

                            // Wait until a connection is made before continuing.  
                            allDone.WaitOne();
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.Error(ex);
                        // if we reach this point we need to restart
                        Start();
                    }
                })
                {
                    Name = $"{threadName}_{DateTime.Today.Ticks}"
                };
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }
        }

        private void AcceptCallback(IAsyncResult ar)
        {
            try
            {
                // Signal the main thread to continue.  
                allDone.Set();

                // Get the socket that handles the client request.  
                Socket listener = (Socket)ar.AsyncState;
                Socket handler = listener.EndAccept(ar);

                // Create the state object.  
                var state = new StateObjectListener
                {
                    WorkSocket = handler,
                    Buffer = new byte[configuration.Connection.PackageSize]
                };
                handler.BeginReceive(state.Buffer, 0, configuration.Connection.PackageSize, 0, new AsyncCallback(ReadCallback), state);
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }
        }

        private void ReadCallback(IAsyncResult ar)
        {
            try
            {
                // Retrieve the state object and the handler socket  
                // from the asynchronous state object.  
                var state = (StateObjectListener)ar.AsyncState;
                Socket handler = state.WorkSocket;
                //handler.RemoteEndPoint
                // Read data from the client socket.
                int bytesRead = handler.EndReceive(ar);

                if (bytesRead > 0)
                {
                    int eofIndex = 0;
                    int bufferEnd = configuration.Connection.PackageSize;
                    for (int i = 0; i < state.Buffer.Length; i++)
                    {
                        if (state.Buffer[i] == Network.EOFBytes[eofIndex])
                        {
                            eofIndex++;
                            if (eofIndex == Network.EOFBytes.Length)
                            {
                                bufferEnd = (i + 1) - Network.EOFBytes.Length;
                                break;
                            }
                        }
                        else
                        {
                            eofIndex = 0;
                        }
                    }

                    // Check for end-of-file tag. If it is not there, read
                    // more data.  
                    if (bufferEnd < configuration.Connection.PackageSize)
                    {
                        state.Data.AddRange(state.Buffer.Take(bufferEnd));
                        // All the data has been read from the client.
                        var rawData = state.Data.ToArray();
                        var remoteEP = handler.RemoteEndPoint as IPEndPoint;
                        _logger.Trace($"{nameof(DataListener)} received From:{remoteEP.Address}:{remoteEP.Port} Bytes Count:{rawData.Length}");
                        stateCompleteCallback?.Invoke(new StateComplete(rawData, remoteEP?.Address?.ToString()));
                    }
                    else
                    {
                        state.Data.AddRange(state.Buffer);
                        // Not all data received. Get more.  
                        handler.BeginReceive(state.Buffer, 0, configuration.Connection.PackageSize, 0, new AsyncCallback(ReadCallback), state);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }
        }
    }
}
