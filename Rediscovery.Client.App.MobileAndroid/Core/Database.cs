using System;
using System.Collections.Generic;
using System.Linq.Expressions;

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

        private LiteDB.ILiteDatabase _database;
        private LiteDB.ILiteDatabase OnGetDatabase()
        {
            if (_database == null)
                _database = new LiteDB.LiteDatabase(OnGetConnectionString());
            return _database;
        }

        public bool Insert<T>(T instance)
        {
            try
            {
                var result = OnGetDatabase().GetCollection<T>().Insert(instance);
                return true;
            }
            catch (Exception ex)
            {
                Logger.Instance.Error(ex);
            }
            return false;
        }

        public bool Update<T>(T instance)
        {
            try
            {
                var result = OnGetDatabase().GetCollection<T>().Update(instance);
                return true;
            }
            catch (Exception ex)
            {
                Logger.Instance.Error(ex);
            }
            return false;
        }

        public bool Delete<T>(Expression<Func<T, bool>> predicate)
        {
            try
            {
                var result = OnGetDatabase().GetCollection<T>().DeleteMany(predicate);
                return true;
            }
            catch (Exception ex)
            {
                Logger.Instance.Error(ex);
            }
            return false;
        }

        public IEnumerable<T> Get<T>(Expression<Func<T, bool>> predicate)
        {
            try
            {
                return OnGetDatabase().GetCollection<T>().Find(predicate);
            }
            catch (Exception ex)
            {
                Logger.Instance.Error(ex);
            }
            return Array.Empty<T>();
        }

        public IEnumerable<T> GetAll<T>()
        {
            try
            {
                return OnGetDatabase().GetCollection<T>().FindAll();
            }
            catch (Exception ex)
            {
                Logger.Instance.Error(ex);
            }
            return Array.Empty<T>();
        }
    }
}