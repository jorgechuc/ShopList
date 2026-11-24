using Microsoft.Data.Sqlite;
using ShopList.Maui.Models;

namespace ShopList.Maui.Data
{
    public class AppSQLiteDatabase
    {
        private readonly string _databasePath;

        public AppSQLiteDatabase(string databasePath)
        {
            _databasePath = databasePath;
            InitializeDatabase();
        }

        private async Task InitializeDatabase()
        {
            const string CreateDbSqlText =
                @"CREATE TABLE IF NOT EXISTS ShopListItems
                    (Id INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL,
                    Nombre VARCHAR NOT NULL,
                    Cantidad INTEGER NOT NULL DEFAULT 1,
                    Comprado INTEGER NOT NULL DEFAULT 0)";

            using var connection = new SqliteConnection(
                $"Data Source={_databasePath}");
            connection.Open();

            // Crea la base de datos
            using var command = new SqliteCommand(CreateDbSqlText, connection);
            await command.ExecuteNonQueryAsync();
        }

        public async Task AddShopListItemAsync(ShopListItem item)
        {
            const string InsertShopListItemSqlText =
                @"INSERT INTO ShopListItems
                    (Nombre, Cantidad, Comprado)
                    VALUES ($nombre, $cantidad, $comprado)
                ";

            using var connection = new SqliteConnection(
                $"Data Source={_databasePath}");
            connection.Open();

            using var command = connection.CreateCommand();
            command.CommandText = InsertShopListItemSqlText;
            command.Parameters.AddWithValue("$nombre", item.Nombre);
            command.Parameters.AddWithValue("$cantidad", item.Cantidad);
            command.Parameters.AddWithValue("$comprado", item.Comprado);
            await command.ExecuteNonQueryAsync();
        }

        public async Task<IEnumerable<ShopListItem>> GetAllShopListItemAsync()
        {
            const string SelectShopListItemsSqlText =
                @"SELECT * FROM ShopListItems";

            using var connection = new SqliteConnection(
                $"Data Source={_databasePath}");
            connection.Open();

            using var command = connection.CreateCommand();
            command.CommandText= SelectShopListItemsSqlText;
            using var reader = await command.ExecuteReaderAsync();
            var items = new List<ShopListItem>();
            while (reader.Read())
            {
                //items.Add(new ShopListItem
                //{
                //    //Id = reader.GetInt32(0),
                //    Nombre = reader.GetString(0),
                //    Cantidad = reader.GetInt32(1),
                //    Comprado = reader.GetInt32(2) == 0 ? false : true
                //});
                items.Add(new ShopListItem
                {
                    //Id = reader[,
                    Nombre = reader["Nombre"].ToString(),
                    Cantidad = (int) reader["Cantidad"],
                    Comprado = ((int) reader["Comprado"]) == 0 ? false : true
                });
            }
            return items;
        }
    }
}
