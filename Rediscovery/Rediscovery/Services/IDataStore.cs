using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Rediscovery.Services
{
    public interface IDataStore<T>
    {
        Task<bool> AddItemAsync(T item);
        Task<bool> UpdateItemAsync(T item);
        Task<bool> DeleteItemAsync(string id);
        Task<T> GetItemAsync(string id);
        Task<IEnumerable<T>> GetItemsAsync(bool forceRefresh = false);
    }

    public interface IDataStoreGuid<T>
    {
        Task<bool> AddItemAsync(T item);
        Task<bool> UpdateItemAsync(T item);
        Task<bool> DeleteItemAsync(Guid id);
        Task<T> GetItemAsync(Guid id);
        Task<IEnumerable<T>> GetItemsAsync(bool forceRefresh = false);
    }

    public interface IDataStoreConnectionGuid<T>
    {
        Task<bool> AddItemAsync(T item);
        Task<bool> UpdateItemAsync(T item);
        Task<bool> DeleteAllAsync(Guid connectionId);
        Task<bool> DeleteItemAsync(Guid connectionId, Guid id);
        Task<T> GetItemAsync(Guid connectionId, Guid id);
        Task<IEnumerable<T>> GetItemsAsync(Guid connectionId);
    }
}
