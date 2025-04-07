using PainKiller.CommandPrompt.CoreLib.Modules.StorageModule.Services;
using PainKiller.ThirdEyeClient.Contracts;

namespace PainKiller.ThirdEyeClient.BaseClasses;

public class ObjectStorageBase<T, TItem>(string storagePath) : StorageBase where T : IDataObjects<TItem>, new() where TItem : class, new()
{
    private T _dataObject = StorageService<T>.Service.GetObject(Path.Combine(storagePath, $"{typeof(T).Name}.json"));

    public List<TItem> GetItems() => _dataObject.Items;

    public void SaveItems(List<TItem> items)
    {
        _dataObject.Items = items;
        _dataObject.LastUpdated = DateTime.Now;
        StorageService<T>.Service.StoreObject(_dataObject, Path.Combine(CorePath, $"{typeof(T).Name}.json"));
    }
    public bool Insert(TItem item, Func<TItem, bool> match)
    {
        var existing = _dataObject.Items.FirstOrDefault(match);
        if (existing != null)
        {
            _dataObject.Items.Remove(existing);
            _dataObject.Items.Add(item);
            _dataObject.LastUpdated = DateTime.Now;
            StorageService<T>.Service.StoreObject(_dataObject, Path.Combine(CorePath, $"{typeof(T).Name}.json"));
            return true;
        }
        _dataObject.LastUpdated = DateTime.Now;
        _dataObject.Items.Add(item);
        StorageService<T>.Service.StoreObject(_dataObject, Path.Combine(CorePath, $"{typeof(T).Name}.json"));
        return false;
    }
    public void InsertOrUpdate(TItem item, Func<TItem, bool> match)
    {
        var existing = _dataObject.Items.FirstOrDefault(match);
        if (existing != null)
        {
            _dataObject.Items.Remove(existing);
        }
        _dataObject.Items.Add(item);
        _dataObject.LastUpdated = DateTime.Now;
        StorageService<T>.Service.StoreObject(_dataObject, Path.Combine(CorePath, $"{typeof(T).Name}.json"));
    }
    public bool Remove(Func<TItem, bool> match)
    {
        var existing = _dataObject.Items.FirstOrDefault(match);
        if (existing == null) return false;

        _dataObject.Items.Remove(existing);
        _dataObject.LastUpdated = DateTime.Now;
        StorageService<T>.Service.StoreObject(_dataObject, Path.Combine(CorePath, $"{typeof(T).Name}.json"));
        return true;
    }
    public void ReLoad() => _dataObject = StorageService<T>.Service.GetObject(Path.Combine(CorePath, $"{typeof(T).Name}.json"));
}
