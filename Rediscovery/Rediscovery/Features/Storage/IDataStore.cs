using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Rediscovery.Features.Storage
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
        Tuple<bool, T> AddItem(T item);
        Tuple<bool, T> UpdateItem(T item);
        bool DeleteItem(Guid id);
        T GetItem(Guid id);
        IEnumerable<T> GetItems(bool forceRefresh = false);

        Task<Tuple<bool, T>> AddItemAsync(T item);
        Task<Tuple<bool, T>> UpdateItemAsync(T item);
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
        Task<IEnumerable<T>> GetItemsAsync();
    }
}
