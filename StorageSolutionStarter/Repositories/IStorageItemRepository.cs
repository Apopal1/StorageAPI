using StorageManagement.API.Models;

namespace StorageManagement.API.Repositories
{
    public interface IStorageItemRepository
    {
        Task<IEnumerable<StorageItem>> GetAllAsync();
        Task<StorageItem?> GetByIdAsync(int id);
        Task<StorageItem> CreateAsync(StorageItem item);
        Task<StorageItem> UpdateAsync(StorageItem item);
        Task<bool> DeleteAsync(int id);
    }
}
