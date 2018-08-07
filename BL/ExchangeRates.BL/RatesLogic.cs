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
        private readonly IRatesCacher _ratesCacher;

        public RatesLogic(Logger logger, IRatesRepository rates_storage, IRatesSource rates_source,
            IRatesCacher rates_cacher)
        {
            _logger = logger;
            _ratesStorage = rates_storage;
            _ratesSource = rates_source;
            _ratesCacher = rates_cacher;
        }

        /// <inheritdoc />
        public async Task<double> GetRateOnDateAsync(string valute_code, DateTime? rate_date)
        {
            if (string.IsNullOrEmpty(valute_code))
                throw new BadRequestException(Resources.NotValuteCodeError);

            rate_date = rate_date ?? DateTime.Now;
            if (rate_date.Value.Date > DateTime.Now.Date)
                throw new BadRequestException(Resources.FutureDateError);

            // Исхожу из того, что курсы не изменяется, никакой инвалидации не делалось.
            try
            {
                double? rate = await _ratesStorage.SelectValuteRateAsync(valute_code, (DateTime) rate_date);
                if (rate != null)
                    return (double) rate;
            }
            catch (Exception e)
            {
                _logger.Debug(e.Message);
            }

            List<ValuteRateOnDate> rates;
            try
            {
                rates = await _ratesSource.GetValuteRatesOnDatesAsync((DateTime) rate_date);
                if (rate_date.Value.Date != DateTime.Now.Date)
                    _ratesCacher.AddValuteRatesOnDate(rates);

            }
            catch (Exception e)
            {
                _logger.Error(e.Message);
                throw new Exception(Resources.CbLogicFail);
            }

            ValuteRateOnDate valute_rate = rates.FirstOrDefault(x => string.Equals(valute_code, x.ValuteCode));
            if (valute_rate == null)
                throw new NotFoundException(string.Format(Resources.ExchangeRateNotFound, valute_code, rate_date));

            return valute_rate.Rate;
        }
    }
}
