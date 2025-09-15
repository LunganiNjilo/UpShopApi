using Microsoft.Data.Sqlite;

namespace UpShopApi.Infrastructure.Extensions
{
    public static class SqliteDataReaderExtensions
    {
        public static bool HasColumn(this SqliteDataReader reader, string columnName)
        {
            for (int i = 0; i < reader.FieldCount; i++)
            {
                if (reader.GetName(i).Equals(columnName, StringComparison.OrdinalIgnoreCase))
                    return true;
            }
            return false;
        }
    }

}
