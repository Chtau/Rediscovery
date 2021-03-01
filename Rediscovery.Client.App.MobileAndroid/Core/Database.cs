using System;

namespace Rediscovery.Client.App.MobileAndroid.Core
{
    public sealed class Database
    {
        private Database()
        {
        }

        private static readonly Lazy<Database> lazy = new Lazy<Database>(() => new Database());

        public static Database Instance
        {
            get
            {
                return lazy.Value;
            }
        }

        private const string dbName = "maindb.db";
        private const string dbPassword = "123321"; // TODO: add function to override the used password if a new DB is created

        private LiteDB.ConnectionString OnGetConnectionString()
        {
            return new LiteDB.ConnectionString
            {
                Filename = System.IO.Path.Combine(CoreIO.DefaultDirectory, dbName),
                Password = dbPassword
            };
        }
    }
}