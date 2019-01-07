using System.Collections.Generic;
using ExchangeRates.Interfaces.Models;

namespace ExchangeRates.Interfaces.Storages
{
    public interface IRatesCacher
    {
        /// <summary>
        /// Добавить данные о курсах валют.
        /// </summary>
        /// <param name="rates">Данные о курсах валют.</param>
        void AddValuteRatesOnDate(List<ValuteRateOnDate> rates);
    }
}
