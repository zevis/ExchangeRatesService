using System;
using System.Data;
using System.Security.Cryptography;
using System.Threading.Tasks;
using ExchangeRates.Interfaces.Models;
using ExchangeRates.Interfaces.Storages;
using ExchangeRates.Interfaces.Storages.Exceptions;
using Microsoft.Extensions.Configuration;
using Npgsql;

namespace ExchangeRates.Storage
{
    /// <summary>
    /// Класс отвечает за вставку и чтение курсов валют, работает с постгресом.
    /// </summary>
    public class PgsqlRatesRepository : IRatesRepository
    {
        /// <summary>
        /// Запрос вставки курса валюты на дату.
        /// </summary>
        private const string INSERT_RATE = "INSERT INTO public.\"Currency\"(\"Name\", \"Date\", \"Rate\") VALUES (@currency_code, @rate_date, @rate);";

        /// <summary>
        /// Запрос выбора курса валюты на заданную дату.
        /// </summary>
        private const string SELECT_RATE = "SELECT \"Rate\" FROM public.\"Currency\" WHERE \"Name\" = @currency_code AND \"Date\" = @rate_date";

        /// <summary>
        /// Строка подключения к бд.
        /// </summary>
        private readonly string _connectionString;

        public PgsqlRatesRepository(IConfigurationRoot configuration)
        {
            _connectionString = configuration["ConnectionString"];
        }

        /// <inheritdoc />
        public async Task<double?> SelectCurrencyRateAsync(string currency_code, DateTime rate_date)
        {
            object rate_result = await SqlAccess.ExecuteScalarCommandAsync(SELECT_RATE, _connectionString,
                new[]
                {
                    new NpgsqlParameter("@currency_code", DbType.String) {Value = currency_code},
                    new NpgsqlParameter("@rate_date", DbType.Date) {Value = rate_date}
                });
            return rate_result as double?;
        }

        /// <inheritdoc />
        public async Task InsertCurrencyRateAsync(CurrencyRateOnDate currency_rate_on_date)
        {
            TryMakeFail(0.6);
            try
            {
                await SqlAccess.ExecuteCommandAsync(INSERT_RATE, _connectionString,
                    new[]
                    {
                        new NpgsqlParameter("@currency_code", DbType.String) {Value = currency_rate_on_date.CurrencyCode},
                        new NpgsqlParameter("@rate_date", DbType.Date) {Value = currency_rate_on_date.RateDate},
                        new NpgsqlParameter("@rate", DbType.Double) {Value = currency_rate_on_date.Rate}
                    });
            }
            catch (PostgresException e)
            {
                // Код ошибки дубликата ключа, значит уже закешировано.
                if (string.Equals(e.SqlState, "23505"))
                    throw new DuplicateNameException();
            }
        }

        /// <summary>
        /// Метод для имитации неуспешных записей в бд.
        /// </summary>
        /// <param name="success_probability">Вероятность, с которой исключения не будет.</param>
        private static void TryMakeFail(double success_probability)
        {
            if (success_probability <= 0)
                throw new ArgumentOutOfRangeException(nameof(success_probability));

            byte[] bytes = new byte[8];
            new RNGCryptoServiceProvider().GetBytes(bytes);
            double value = Math.Abs(BitConverter.ToDouble(bytes, 0)) / double.MaxValue;
            if (value > success_probability)
                throw new CustomFailException();
        }
    }
}
