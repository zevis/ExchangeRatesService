﻿using System;
using System.Threading.Tasks;

namespace ExchangeRates.Interfaces.BL
{
    public interface IRatesLogic
    {
        /// <summary>
        /// Получить курс валюты на дату.
        /// </summary>
        /// <param name="currency_code">Код валюты.</param>
        /// <param name="rate_date">Дата, на которую нужен курс.</param>
        /// <returns>Курс валюты на заданную дату.</returns>
        Task<double> GetRateOnDateAsync(string currency_code, DateTime? rate_date);
    }
}
