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
    public class SimpleRatesCacher : IRatesCacher, IDisposable
    {
        private readonly Logger _logger;
        private readonly IRatesRepository _ratesStorage;
        private readonly Thread _threadConsumer;
        private readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();

        public SimpleRatesCacher(Logger logger, IRatesRepository rates_storage)
        {
            _logger = logger;
            _ratesStorage = rates_storage;
        }

        /// <inheritdoc />
        public void AddValuteRatesOnDate(List<ValuteRateOnDate> valute_rates)
        {
            Task.Factory.StartNew(async () =>
            {
                foreach (var rate in valute_rates)
                {
                    if (_cancellationTokenSource.IsCancellationRequested)
                        return;
                    
                    await _ratesStorage.InsertValuteRateAsync(rate).ContinueWith(task =>
                    {
                        if (task.Status != TaskStatus.Faulted)
                            return;

                        if (task.Exception.InnerExceptions.Any(ex => ex.InnerException is DuplicateNameException))
                            return;

                        if (task.Exception.InnerExceptions.Any(ex => ex.InnerException is CustomFailException))
                        {
                            AddValuteRatesOnDate(new List<ValuteRateOnDate> {rate});
                        }
                        else
                        {
                            _logger.Error(task.Exception);
                        }
                    });
                }
            }, _cancellationTokenSource.Token, TaskCreationOptions.LongRunning, TaskScheduler.Default);
        }

        public void Dispose()
        {
            _cancellationTokenSource?.Dispose();
            _threadConsumer.Join();
        }
    }
}
