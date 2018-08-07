using System;

namespace ExchangeRates.Interfaces.Models
{
    public class ValuteRateOnDate
    {
        /// <summary>
        /// Закодированное строковое обозначение валюты<para/>
        /// Например: USD, EUR, AUD и т.д.
        /// </summary>
        public string ValuteCode { get; }

        /// <summary>
        /// Дата курса валюты.
        /// </summary>
        public DateTime RateDate { get; }

        /// <summary>
        /// Обменный курс.
        /// </summary>
        public double Rate { get; }

        /// <param name="valute_code">Закодированное строковое обозначение валюты<para/> 
        /// Например: USD, EUR, AUD и т.д.</param>
        /// <param name="rate_date">Дата курса валюты.</param>
        /// <param name="rate">Обменный курс.</param>
        public ValuteRateOnDate(string valute_code, DateTime rate_date, double rate)
        {
            ValuteCode = valute_code;
            RateDate = rate_date;
            Rate = rate;
        }
    }
}
