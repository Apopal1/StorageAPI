using Microsoft.Data.Sqlite;
using Dapper;
using StorageManagement.API.Models;

namespace StorageManagement.API.Repositories
{
    public class StorageItemRepository : IStorageItemRepository
    {
        private readonly string _connectionString;

        public StorageItemRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<IEnumerable<StorageItem>> GetAllAsync()
        {
            using var connection = new SqliteConnection(_connectionString);
            return await connection.QueryAsync<StorageItem>("SELECT * FROM StorageItems ORDER BY Name");
        }

        public async Task<StorageItem?> GetByIdAsync(int id)
        {
            using var connection = new SqliteConnection(_connectionString);
            return await connection.QueryFirstOrDefaultAsync<StorageItem>(
                "SELECT * FROM StorageItems WHERE Id = @Id", new { Id = id });
        }

        public async Task<StorageItem> CreateAsync(StorageItem item)
        {
            using var connection = new SqliteConnection(_connectionString);
            var sql = @"INSERT INTO StorageItems (Name, Quantity, Location) 
                       VALUES (@Name, @Quantity, @Location);
                       SELECT last_insert_rowid();";
            
            var id = await connection.QuerySingleAsync<int>(sql, item);
            item.Id = id;
            return item;
        }

        public async Task<StorageItem> UpdateAsync(StorageItem item)
        {
            using var connection = new SqliteConnection(_connectionString);
            await connection.ExecuteAsync(@"
                UPDATE StorageItems 
                SET Name = @Name, Quantity = @Quantity, Location = @Location, 
                    UpdatedAt = CURRENT_TIMESTAMP 
                WHERE Id = @Id", item);
            return item;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            using var connection = new SqliteConnection(_connectionString);
            var rowsAffected = await connection.ExecuteAsync(
                "DELETE FROM StorageItems WHERE Id = @Id", new { Id = id });
            return rowsAffected > 0;
        }
    }
}
