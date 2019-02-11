using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ExchangeRates.Interfaces.Models;

namespace ExchangeRates.Interfaces.Storages
{
    public interface IRatesSource
    {
        /// <summary>
        /// Получить список курсов валют на дату.
        /// </summary>
        /// <param name="currency_code">Код валюты.</param>
        /// <param name="rate_date"></param>
        /// <returns>Список курсов валют на дату.</returns>
        Task<List<CurrencyRateOnDate>> GetCurrencyRatesOnDatesAsync(string currency_code, DateTime rate_date);
    }
}
