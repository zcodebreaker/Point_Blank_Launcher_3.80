using Npgsql;
using System;

namespace Launcher.Services.Database
{
    public class DatabaseService
    {
        private readonly string _connectionString;

        public DatabaseService(string host, int port, string database, string user, string password)
        {
            _connectionString = string.Format(
                "Host={0};Port={1};Database={2};Username={3};Password={4};",
                host, port, database, user, password);
        }

        public bool ValidateLogin(string username, string passwordMd5)
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(_connectionString))
            {
                conn.Open();
                using (NpgsqlCommand cmd = new NpgsqlCommand(
                    "SELECT 1 FROM accounts WHERE username = @u AND password = @p LIMIT 1", conn))
                {
                    cmd.Parameters.AddWithValue("u", username);
                    cmd.Parameters.AddWithValue("p", passwordMd5);
                    using (NpgsqlDataReader reader = cmd.ExecuteReader())
                    {
                        return reader.HasRows;
                    }
                }
            }
        }
    }
}
