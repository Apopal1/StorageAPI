using Microsoft.Data.Sqlite;
using Dapper;
using StorageManagement.API.Models;

namespace StorageManagement.API.Repositories
{
    public class SupplierRepository : ISupplierRepository
    {
        private readonly string _connectionString;

        public SupplierRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<IEnumerable<Supplier>> GetAllAsync()
        {
            using var connection = new SqliteConnection(_connectionString);
            return await connection.QueryAsync<Supplier>("SELECT * FROM Suppliers ORDER BY Name");
        }

        public async Task<Supplier?> GetByIdAsync(int id)
        {
            using var connection = new SqliteConnection(_connectionString);
            return await connection.QueryFirstOrDefaultAsync<Supplier>(
                "SELECT * FROM Suppliers WHERE Id = @Id", new { Id = id });
        }

        public async Task<Supplier> CreateAsync(Supplier supplier)
        {
            using var connection = new SqliteConnection(_connectionString);
            var sql = @"INSERT INTO Suppliers (Name, Afm, PhoneNumber, Address, Email) 
                       VALUES (@Name, @Afm, @PhoneNumber, @Address, @Email);
                       SELECT last_insert_rowid();";
            
            var id = await connection.QuerySingleAsync<int>(sql, supplier);
            supplier.Id = id;
            return supplier;
        }

        public async Task<Supplier> UpdateAsync(Supplier supplier)
        {
            using var connection = new SqliteConnection(_connectionString);
            
            // Get existing supplier
            var existingSupplier = await connection.QueryFirstOrDefaultAsync<Supplier>(
                "SELECT * FROM Suppliers WHERE Id = @Id", new { Id = supplier.Id });
            
            if (existingSupplier == null)
            {
                throw new InvalidOperationException($"Supplier with ID {supplier.Id} not found");
            }
            
            // Use existing values if new values are null or empty
            var updatedName = string.IsNullOrWhiteSpace(supplier.Name) ? existingSupplier.Name : supplier.Name;
            var updatedAfm = string.IsNullOrWhiteSpace(supplier.Afm) ? existingSupplier.Afm : supplier.Afm;
            var updatedPhone = string.IsNullOrWhiteSpace(supplier.PhoneNumber) ? existingSupplier.PhoneNumber : supplier.PhoneNumber;
            var updatedAddress = string.IsNullOrWhiteSpace(supplier.Address) ? existingSupplier.Address : supplier.Address;
            var updatedEmail = string.IsNullOrWhiteSpace(supplier.Email) ? existingSupplier.Email : supplier.Email;
            
            await connection.ExecuteAsync(@"
                UPDATE Suppliers 
                SET Name = @Name, Afm = @Afm, PhoneNumber = @PhoneNumber, 
                    Address = @Address, Email = @Email 
                WHERE Id = @Id", 
                new { 
                    Id = supplier.Id, 
                    Name = updatedName, 
                    Afm = updatedAfm, 
                    PhoneNumber = updatedPhone, 
                    Address = updatedAddress, 
                    Email = updatedEmail 
                });
            
            return await GetByIdAsync(supplier.Id);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            using var connection = new SqliteConnection(_connectionString);
            var rowsAffected = await connection.ExecuteAsync(
                "DELETE FROM Suppliers WHERE Id = @Id", new { Id = id });
            return rowsAffected > 0;
        }
    }
}
