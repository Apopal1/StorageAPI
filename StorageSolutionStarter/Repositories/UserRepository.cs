using Microsoft.Data.Sqlite;
using Dapper;
using StorageManagement.API.Models;

namespace StorageManagement.API.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly string _connectionString;

        public UserRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<User?> GetByUsernameAsync(string username)
        {
            using var connection = new SqliteConnection(_connectionString);
            return await connection.QueryFirstOrDefaultAsync<User>(
                "SELECT * FROM Users WHERE Username = @Username", 
                new { Username = username });
        }

        public async Task<User> CreateAsync(User user)
        {
            using var connection = new SqliteConnection(_connectionString);
            var sql = @"INSERT INTO Users (FirstName, LastName, Email, Username, Password) 
                       VALUES (@FirstName, @LastName, @Email, @Username, @Password);
                       SELECT last_insert_rowid();";
            
            var id = await connection.QuerySingleAsync<int>(sql, user);
            user.Id = id;
            return user;
        }

        public async Task<bool> UsernameExistsAsync(string username)
        {
            using var connection = new SqliteConnection(_connectionString);
            var count = await connection.QuerySingleAsync<int>(
                "SELECT COUNT(*) FROM Users WHERE Username = @Username", 
                new { Username = username });
            return count > 0;
        }

        public async Task<bool> EmailExistsAsync(string email)
        {
            using var connection = new SqliteConnection(_connectionString);
            var count = await connection.QuerySingleAsync<int>(
                "SELECT COUNT(*) FROM Users WHERE Email = @Email", 
                new { Email = email });
            return count > 0;
        }
    }
}
