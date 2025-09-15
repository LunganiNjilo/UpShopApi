using Microsoft.Data.Sqlite;
using System.Reflection;
using UpShopApi.Application.Interfaces;
using UpShopApi.Infrastructure.Extensions;

namespace UpShopApi.Infrastructure.Repositories
{
    public class GenericRepository<T> : IRepository<T> where T : new()
    {
        private readonly string _connectionString;
        private readonly string _tableName;

        public GenericRepository(string connectionString, string tableName)
        {
            _connectionString = connectionString;
            _tableName = tableName;
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            var results = new List<T>();
            using var connection = new SqliteConnection(_connectionString);
            await connection.OpenAsync();

            var cmd = connection.CreateCommand();
            cmd.CommandText = $"SELECT * FROM {_tableName};";

            using var reader = await cmd.ExecuteReaderAsync();
            var props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);

            while (await reader.ReadAsync())
            {
                var obj = new T();
                foreach (var prop in props)
                {
                    if (!reader.HasColumn(prop.Name) || reader[prop.Name] is DBNull) continue;
                    prop.SetValue(obj, Convert.ChangeType(reader[prop.Name], prop.PropertyType));
                }
                results.Add(obj);
            }
            return results;
        }
    }
}
