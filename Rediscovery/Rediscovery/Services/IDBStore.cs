using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace Rediscovery.Services
{
    [Obsolete("replace with JSON store")]
    public interface IDBStore
    {
        SQLiteAsyncConnection Store { get; }
    }
}
