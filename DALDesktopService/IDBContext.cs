using SQLite;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Rediscovery.Service.DAL
{
    public interface IDBContext
    {
        Task Connect();
        SQLiteAsyncConnection Instance { get; }
    }
}
