using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ExchangeRates.Interfaces.Models;
using ExchangeRates.Interfaces.Storages;
using ExchangeRates.Interfaces.Storages.Exceptions;
using NLog;

namespace ExchangeRates.Storage
{
    public class SimpleRatesCache : IRatesCache, IDisposable
    {
        private readonly Logger _logger;
        private readonly IRatesRepository _ratesStorage;
        private readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();

        public SimpleRatesCache(Logger logger, IRatesRepository rates_storage)
        {
            _logger = logger;
            _ratesStorage = rates_storage;
        }

        /// <inheritdoc />
        public void AddCurrencyRatesOnDate(List<CurrencyRateOnDate> currency_rates)
        {
            Task.Factory.StartNew(async () =>
            {
                foreach (var rate in currency_rates)
                {
                    if (_cancellationTokenSource.IsCancellationRequested)
                        return;
                    
                    await _ratesStorage.InsertCurrencyRateAsync(rate).ContinueWith(task =>
                    {
                        if (task.Status != TaskStatus.Faulted)
                            return;

                        if (task.Exception.InnerExceptions.Any(ex => ex.InnerException is DuplicateNameException))
                            return;

                        if (task.Exception.InnerExceptions.Any(ex => ex.InnerException is CustomFailException))
                            AddCurrencyRatesOnDate(new List<CurrencyRateOnDate> {rate});
                        else
                            _logger.Error(task.Exception);
                    });
                }
            }, _cancellationTokenSource.Token, TaskCreationOptions.LongRunning, TaskScheduler.Default);
        }

        public void Dispose()
        {
            _cancellationTokenSource?.Dispose();
        }
    }
}
