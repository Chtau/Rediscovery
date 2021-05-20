using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Linq;

namespace Rediscovery.Communication.Protocol.Internal
{
    internal abstract class BaseListener
    {
        private const string EOF = "!#~^%$|";

        private System.Threading.Thread listenThread;
        private readonly IProtocolLogger _logger;
        private readonly string threadName = $"Thread";
        private readonly byte[] EOFBytes = Encoding.UTF8.GetBytes(EOF);

        internal Setting setting;
        private bool working = false;
        private static ManualResetEvent allDone = new ManualResetEvent(false);

        public virtual int ListenerBufferSize => setting.ListenPackageBytesData;
        public virtual int ListenerPort => setting.ListenPortData;

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

        public virtual void OnBeforeDoWork()
        {

        }

        public virtual void OnDoWork()
        {

        }

        public virtual void OnBeforeRestartWorker()
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
                        Socket listener = Network.CreateSocket(ListenerPort);
                        listener.Bind(listener.LocalEndPoint);
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
                            OnDoWork();
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
                Buffer = new byte[ListenerBufferSize]
            };
            handler.BeginReceive(state.Buffer, 0, ListenerBufferSize, 0, new AsyncCallback(ReadCallback), state);
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
                int bufferEnd = ListenerBufferSize;
                for (int i = 0; i < state.Buffer.Length; i++)
                {
                    if (state.Buffer[i] == EOFBytes[eofIndex])
                    {
                        eofIndex++;
                        if (eofIndex == EOFBytes.Length)
                        {
                            bufferEnd = i - EOFBytes.Length;
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
                if (bufferEnd < ListenerBufferSize)
                {
                    state.Data.AddRange(state.Buffer.Take(bufferEnd));
                    // All the data has been read from the client.
                }
                else
                {
                    state.Data.AddRange(state.Buffer);
                    // Not all data received. Get more.  
                    handler.BeginReceive(state.Buffer, 0, ListenerBufferSize, 0, new AsyncCallback(ReadCallback), state);
                }
            }
        }
    }
}
