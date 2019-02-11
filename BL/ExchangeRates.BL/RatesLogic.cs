using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ExchangeRates.BL.Properties;
using ExchangeRates.Interfaces.BL;
using ExchangeRates.Interfaces.BL.Exceptions;
using ExchangeRates.Interfaces.Models;
using ExchangeRates.Interfaces.Storages;
using NLog;

namespace ExchangeRates.BL
{
    /// <summary>
    /// Класс, который занимается организацией логики получения, кеширования курсов валют.
    /// </summary>
    public class RatesLogic : IRatesLogic
    {
        private readonly Logger _logger;
        private readonly IRatesRepository _ratesStorage;
        private readonly IRatesSource _ratesSource;
        private readonly IRatesCache _ratesCache;

        public RatesLogic(Logger logger, IRatesRepository rates_storage, IRatesSource rates_source,
            IRatesCache rates_cache)
        {
            _logger = logger;
            _ratesStorage = rates_storage;
            _ratesSource = rates_source;
            _ratesCache = rates_cache;
        }

        /// <inheritdoc />
        public async Task<double> GetRateOnDateAsync(string currency_code, DateTime? rate_date)
        {
            if (string.IsNullOrEmpty(currency_code))
                throw new BadRequestException(Resources.NotCurrencyCodeError);

            rate_date = rate_date ?? DateTime.Now;
            if (rate_date.Value.Date > DateTime.Now.Date)
                throw new BadRequestException(Resources.FutureDateError);

            // Исхожу из того, что курсы не изменяются, никакой инвалидации не делалось.
            try
            {
                double? rate = await _ratesStorage.SelectCurrencyRateAsync(currency_code, (DateTime) rate_date);
                if (rate != null)
                    return (double) rate;
            }
            catch (Exception e)
            {
                _logger.Debug(e.Message);
            }

            List<CurrencyRateOnDate> rates;
            try
            {
                rates = await _ratesSource.GetCurrencyRatesOnDatesAsync(currency_code, (DateTime) rate_date);
                _ratesCache.AddCurrencyRatesOnDate(rates);
            }
            catch (Exception e)
            {
                _logger.Error(e.Message);
                throw new Exception(Resources.CbLogicFail);
            }

            CurrencyRateOnDate currency_rate = rates.FirstOrDefault(x =>
                string.Equals(currency_code, x.CurrencyCode) && x.RateDate.Date.Equals(rate_date.Value.Date));
            if (currency_rate == null)
                throw new NotFoundException(string.Format(Resources.ExchangeRateNotFound, currency_code, rate_date));

            return currency_rate.Rate;
        }
    }
}
