using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace Rediscovery.Services
{
    public interface IDBStore
    {
        SQLiteAsyncConnection Store { get; }
    }
}
