using System;
using System.Threading.Tasks;
using Npgsql;

namespace ExchangeRates.Storage
{
    /// <summary>
    /// Класс с типовыми операциями в ADO.net.
    /// </summary>
    public static class SqlAccess
    {
        /// <summary>
        /// Выполняет ExecuteNonQuery
        /// </summary>
        /// <param name="command_text">Текст запроса.</param>
        /// <param name="connection_string">Подключение к Sql.</param>
        /// <param name="parameters">Параметры в запросе.</param>
        public static async Task ExecuteCommandAsync(string command_text, string connection_string, NpgsqlParameter[] parameters = null)
        {
            using (NpgsqlConnection connection = new NpgsqlConnection(connection_string))
            {
                await connection.OpenAsync();
                using (NpgsqlCommand command = connection.CreateCommand())
                {
                    command.CommandText = command_text;
                    command.Parameters.AddRange(parameters ?? new NpgsqlParameter[0]);
                    await command.ExecuteNonQueryAsync();
                }
            }
        }

        /// <summary>
        /// Выполняет ExecuteScalar
        /// </summary>
        /// <param name="command_text">Текст запроса.</param>
        /// <param name="connection_string">Подключение к Sql.</param>
        /// <param name="parameters">Параметры в запросе.</param>
        /// <returns>Скалярное значение.</returns>
        public static async Task<object> ExecuteScalarCommandAsync(string command_text, string connection_string, NpgsqlParameter[] parameters = null)
        {
            using (NpgsqlConnection connection = new NpgsqlConnection(connection_string))
            {
                await connection.OpenAsync();
                using (NpgsqlCommand command = connection.CreateCommand())
                {
                    command.CommandText = command_text;
                    command.Parameters.AddRange(parameters ?? new NpgsqlParameter[0]);
                    return await command.ExecuteScalarAsync();
                }
            }
        }

        /// <summary>
        /// Выполняет ExecuteReader
        /// </summary>
        /// <param name="command_text">Текст запроса.</param>
        /// <param name="connection_string"></param>
        /// <param name="method"></param>
        /// <param name="parameters">Параметры в запросе.</param>
        /// <returns>Таблица.</returns>
        public static async void ExecuteReaderCommandAsync(string command_text, string connection_string, Action<NpgsqlDataReader> method, NpgsqlParameter[] parameters = null)
        {
            using (NpgsqlConnection connection = new NpgsqlConnection(connection_string))
            {
                await connection.OpenAsync();
                using (NpgsqlCommand command = connection.CreateCommand())
                {
                    command.CommandText = command_text;
                    command.Parameters.AddRange(parameters ?? new NpgsqlParameter[0]);
                    NpgsqlDataReader reader = command.ExecuteReader();
                    while (await reader.ReadAsync())
                    {
                        method(reader);
                    }
                    reader.Close();
                }
            }
        }
    }
}
