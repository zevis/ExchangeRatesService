using System;
using System.Threading.Tasks;
using ExchangeRates.Interfaces.Models;

namespace ExchangeRates.Interfaces.Storages
{
    public interface IRatesRepository
    {
        /// <summary>
        /// Получить курс валюты на дату.
        /// </summary>
        /// <param name="currency_code">Код валюты.</param>
        /// <param name="rate_date">Дата, на которую нужен курс.</param>
        /// <returns>Курс валюты на заданную дату.</returns>
        Task<double?> SelectCurrencyRateAsync(string currency_code, DateTime rate_date);

        /// <summary>
        /// Добавить запись курса валюты на дату.
        /// </summary>
        /// <param name="currency_rate_on_date">Курс валюты на дату.</param>
        Task InsertCurrencyRateAsync(CurrencyRateOnDate currency_rate_on_date);
    }
}
