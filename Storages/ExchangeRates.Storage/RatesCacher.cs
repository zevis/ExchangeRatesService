using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Threading;
using ExchangeRates.Interfaces.Models;
using ExchangeRates.Interfaces.Storages;
using ExchangeRates.Interfaces.Storages.Exceptions;
using NLog;

namespace ExchangeRates.Storage
{
    /// <summary>
    /// Консьюмер, отвечает за последовательную вставку курсов валют в бд, в отдельном потоке, чтобы не тормозить ответ пользователю.
    /// </summary>
    public class RatesCache : IRatesCache, IDisposable
    {
        private readonly Logger _logger;
        private readonly IRatesRepository _ratesStorage;
        private readonly Thread _threadConsumer;
        private readonly ConcurrentQueue<CurrencyRateOnDate> _tasksToConsume = new ConcurrentQueue<CurrencyRateOnDate>();
        private readonly AutoResetEvent _taskAddedEvent = new AutoResetEvent(false);
        private readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();

        public RatesCache(Logger logger, IRatesRepository rates_storage)
        {
            _logger = logger;
            _ratesStorage = rates_storage;
            _threadConsumer = new Thread(Consume) { IsBackground = true };
            _threadConsumer.Start();
        }

        /// <inheritdoc />
        public void AddCurrencyRatesOnDate(List<CurrencyRateOnDate> currency_rates)
        {
            foreach (var currency_rate_on_date in currency_rates)
                _tasksToConsume.Enqueue(currency_rate_on_date);

            _taskAddedEvent.Set();
        }

        /// <summary>
        /// Главный метод разгребателя очереди на запись в бд.
        /// </summary>
        private async void Consume()
        {
            while (!_cancellationTokenSource.IsCancellationRequested)
            {
                while (_tasksToConsume.TryDequeue(out CurrencyRateOnDate task))
                {
                    try
                    {
                        await _ratesStorage.InsertCurrencyRateAsync(task);
                    }
                    catch (CustomFailException)
                    {
                        _tasksToConsume.Enqueue(task);
                    }
                    catch (DuplicateNameException)
                    {
                    }
                    catch (Exception e)
                    {
                        _logger.Error(e.Message);
                    }
                }

                _taskAddedEvent.WaitOne(5000);
            }
        }

        public void Dispose()
        {
            _taskAddedEvent?.Set();
            _taskAddedEvent?.Dispose();
            _cancellationTokenSource?.Dispose();
            _threadConsumer.Join();
        }
    }
}
