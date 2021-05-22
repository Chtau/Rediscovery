using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Linq;

namespace Rediscovery.Communication.Protocol.Internal.Listener
{
    internal abstract class BaseListener : IListener
    {
        private System.Threading.Thread listenThread;
        private readonly IProtocolLogger _logger;
        private readonly string threadName = $"Thread";

        internal Setting setting;
        private bool working = false;
        private static readonly ManualResetEvent allDone = new ManualResetEvent(false);
        private Action<byte[]> stateCompleteCallback;

        public virtual int BufferSize => setting.ListenPackageBytesData;
        public virtual int Port => setting.ListenPortData;

        public BaseListener(IProtocolLogger protocolLogger = null, string threadName = null)
        {
            _logger = protocolLogger ?? new ProtocolLogger();
            if (!string.IsNullOrWhiteSpace(threadName))
                this.threadName = threadName;
            OnInitThread();
        }

        public virtual void Initialize(Setting setting)
        {
            this.setting = setting;
        }

        public virtual void StateCompleteListener(Action<byte[]> callback)
        {
            stateCompleteCallback = callback;
        }

        public virtual bool Start()
        {
            try
            {
                working = true;
                listenThread.Start();
                return true;
            }
            catch (System.Threading.ThreadStateException tsEx)
            {
                _logger.Warning(tsEx);
                OnInitThread();
                try
                {
                    working = true;
                    listenThread.Start();
                    return true;
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
            return false;
        }

        public virtual bool Stop()
        {
            try
            {
                working = false;
                listenThread?.Abort();
                return true;
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }
            return false;
        }

        internal virtual void OnBeforeDoWork()
        {

        }

        internal virtual void OnBeforeRestartWorker()
        {

        }

        private void OnInitThread()
        {
            try
            {
                listenThread = new System.Threading.Thread(() =>
                {
                    try
                    {
                        OnBeforeDoWork();
                        Socket listener = Network.CreateSocket(11000);// Port);
                        listener.Bind(Network.LocalEndPoint(11000));// Port));// listener.LocalEndPoint) ;
                        listener.Listen(10);

                        while (working)
                        {
                            // Set the event to nonsignaled state.  
                            allDone.Reset();

                            // Start an asynchronous socket to listen for connections.  
                            Console.WriteLine("Waiting for a connection...");
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
                        OnBeforeRestartWorker();
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
            // Signal the main thread to continue.  
            allDone.Set();

            // Get the socket that handles the client request.  
            Socket listener = (Socket)ar.AsyncState;
            Socket handler = listener.EndAccept(ar);

            // Create the state object.  
            var state = new StateObjectListener
            {
                WorkSocket = handler,
                Buffer = new byte[BufferSize]
            };
            handler.BeginReceive(state.Buffer, 0, BufferSize, 0, new AsyncCallback(ReadCallback), state);
            
        }

        private void ReadCallback(IAsyncResult ar)
        {
            String content = String.Empty;

            // Retrieve the state object and the handler socket  
            // from the asynchronous state object.  
            var state = (StateObjectListener)ar.AsyncState;
            Socket handler = state.WorkSocket;

            // Read data from the client socket.
            int bytesRead = handler.EndReceive(ar);

            if (bytesRead > 0)
            {
                int eofIndex = 0;
                int bufferEnd = BufferSize;
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
                if (bufferEnd < BufferSize)
                {
                    state.Data.AddRange(state.Buffer.Take(bufferEnd));
                    // All the data has been read from the client.
                    var rawData = state.Data.ToArray();
                    OnStateObjectComplete(rawData);
                    stateCompleteCallback?.Invoke(rawData);
                }
                else
                {
                    state.Data.AddRange(state.Buffer);
                    // Not all data received. Get more.  
                    handler.BeginReceive(state.Buffer, 0, BufferSize, 0, new AsyncCallback(ReadCallback), state);
                }
            }
        }

        internal virtual void OnStateObjectComplete(byte[] data)
        {
            
        }
    }
}
