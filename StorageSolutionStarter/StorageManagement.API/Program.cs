using Microsoft.Data.Sqlite;
using Dapper;
using StorageManagement.API.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configure CORS for MAUI app
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowMauiApp", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// Register repositories
var connectionString = "Data Source=storage.db";
builder.Services.AddScoped<IUserRepository>(provider => 
    new UserRepository(connectionString));
builder.Services.AddScoped<IStorageItemRepository>(provider => 
    new StorageItemRepository(connectionString));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowMauiApp");
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

// Initialize database
InitializeDatabase(connectionString);

app.Run();

void InitializeDatabase(string connectionString)
{
    using var connection = new SqliteConnection(connectionString);
    connection.Open();
    
    // Create Users table
    connection.Execute(@"
        CREATE TABLE IF NOT EXISTS Users (
            Id INTEGER PRIMARY KEY AUTOINCREMENT,
            FirstName TEXT NOT NULL,
            LastName TEXT NOT NULL,
            Email TEXT UNIQUE NOT NULL,
            Username TEXT UNIQUE NOT NULL,
            Password TEXT NOT NULL,
            CreatedAt DATETIME DEFAULT CURRENT_TIMESTAMP
        )");
    
    // Create StorageItems table
    connection.Execute(@"
        CREATE TABLE IF NOT EXISTS StorageItems (
            Id INTEGER PRIMARY KEY AUTOINCREMENT,
            Name TEXT NOT NULL,
            Quantity INTEGER NOT NULL DEFAULT 0,
            Location TEXT NOT NULL,
            CreatedAt DATETIME DEFAULT CURRENT_TIMESTAMP,
            UpdatedAt DATETIME DEFAULT CURRENT_TIMESTAMP
        )");
}
