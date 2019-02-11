using System;

namespace ExchangeRates.Interfaces.Models
{
    public class CurrencyRateOnDate
    {
        /// <summary>
        /// Закодированное строковое обозначение валюты<para/>
        /// Например: USD, EUR, AUD и т.д.
        /// </summary>
        public string CurrencyCode { get; }

        /// <summary>
        /// Дата курса валюты.
        /// </summary>
        public DateTime RateDate { get; }

        /// <summary>
        /// Обменный курс.
        /// </summary>
        public double Rate { get; }

        /// <param name="currency_code">Закодированное строковое обозначение валюты<para/> 
        /// Например: USD, EUR, AUD и т.д.</param>
        /// <param name="rate_date">Дата курса валюты.</param>
        /// <param name="rate">Обменный курс.</param>
        public CurrencyRateOnDate(string currency_code, DateTime rate_date, double rate)
        {
            CurrencyCode = currency_code;
            RateDate = rate_date;
            Rate = rate;
        }
    }
}
