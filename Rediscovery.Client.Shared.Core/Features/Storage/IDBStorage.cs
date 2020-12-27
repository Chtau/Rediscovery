using Rediscovery.Client.Shared.Core.Dependency;
using Rediscovery.Client.Shared.Core.Features.Storage.Models;
using Rediscovery.Shared.Logging;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Rediscovery.Client.Shared.Core.Features.Storage
{
    public interface IDBStorage : IDisposable
    {
        bool Delete();
        bool FileSave<T>(FileInfo<T> fileInfo);
        FileInfo<T> FileLoad<T>(T id);
        bool FileDelete<T>(T id);

        bool EntityInsertBulk<T>(params T[] entities);
        bool EntityUpdate<T>(T entity);
        T EntityLoad<T>(Expression<Func<T, bool>> predicate);
        List<T> EntitiesLoad<T>(Expression<Func<T, bool>> predicate);
        bool EntityDelete<T>(Expression<Func<T, bool>> predicate);
    }
}
