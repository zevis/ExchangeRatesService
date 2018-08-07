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
        /// <param name="valute_code">Код валюты.</param>
        /// <param name="rate_date">Дата, на которую нужен курс.</param>
        /// <returns>Курс валюты на заданную дату.</returns>
        Task<double?> SelectValuteRateAsync(string valute_code, DateTime rate_date);

        /// <summary>
        /// Добавить запись курса валюты на дату.
        /// </summary>
        /// <param name="valute_rate_on_date">Курс валюты на дату.</param>
        Task InsertValuteRateAsync(ValuteRateOnDate valute_rate_on_date);
    }
}
