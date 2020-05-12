using SQLite;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DALDesktopService
{
    public interface IDBContext
    {
        Task Connect(string connectionString);
        SQLiteAsyncConnection Instance { get; }
    }
}
