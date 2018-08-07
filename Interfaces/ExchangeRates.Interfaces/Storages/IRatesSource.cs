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
        /// <param name="rate_date"></param>
        /// <returns>Список курсов валют на дату.</returns>
        Task<List<ValuteRateOnDate>> GetValuteRatesOnDatesAsync(DateTime rate_date);
    }
}
