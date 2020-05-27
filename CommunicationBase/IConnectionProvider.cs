using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CommunicationBase
{
    public interface IConnectionProvider<T>
    {
        event EventHandler<(ConnectionConfiguration Config, bool IsConnected)> ConnectionChanged;
        event EventHandler ConnectionClosed;

        void Init(SharedBase.Logging.ILogger logger, string hubLink, Protocol protocol = Protocol.HTTP);
        Task<(T Connection, bool Result)> Connect(Action<bool, T> connectCallback, ConnectionConfiguration model, bool shouldUseToken = true);
        Task CloseConnection();
        T CurrentConnection { get; }
        bool IsConnected { get; }
        string BaseUrl { get; }
        string Token { get; }
    }
}
