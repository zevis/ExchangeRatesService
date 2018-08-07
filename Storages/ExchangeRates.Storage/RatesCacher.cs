using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using ExchangeRates.Interfaces.Models;
using ExchangeRates.Interfaces.Storages;
using ExchangeRates.Interfaces.Storages.Exceptions;
using NLog;
using Npgsql;

namespace ExchangeRates.Storage
{
    /// <summary>
    /// Консьюмер, отвечает за последовательную вставку курсов валют в бд, в отдельном потоке, чтобы не тормозить ответ пользователю.
    /// </summary>
    public class RatesCacher : IRatesCacher, IDisposable
    {
        private readonly Logger _logger;
        private readonly IRatesRepository _ratesStorage;
        private readonly Thread _threadConsumer;
        private readonly ConcurrentQueue<ValuteRateOnDate> _tasksToConsume = new ConcurrentQueue<ValuteRateOnDate>();
        private readonly AutoResetEvent _taskAddedEvent = new AutoResetEvent(false);
        private readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();

        public RatesCacher(Logger logger, IRatesRepository rates_storage)
        {
            _logger = logger;
            _ratesStorage = rates_storage;
            _threadConsumer = new Thread(Consume) { IsBackground = true };
            _threadConsumer.Start();
        }

        /// <inheritdoc />
        public void AddValuteRatesOnDate(IEnumerable<ValuteRateOnDate> valute_rates)
        {
            foreach (var valute_rate_on_date in valute_rates)
                _tasksToConsume.Enqueue(valute_rate_on_date);

            _taskAddedEvent.Set();
        }

        /// <summary>
        /// Главный метод разгребателя очереди на запись в бд.
        /// </summary>
        private async void Consume()
        {
            while (!_cancellationTokenSource.IsCancellationRequested)
            {
                while (_tasksToConsume.TryDequeue(out ValuteRateOnDate task))
                {
                    try
                    {
                        await _ratesStorage.InsertValuteRateAsync(task);
                    }
                    catch (CustomFailException fail_exception)
                    {
                        _tasksToConsume.Enqueue(task);
                    }
                    catch (PostgresException e)
                    {
                        // Код ошибки дубликата ключа, значит уже закешировано.
                        if (string.Equals(e.Code, "23505"))
                            continue;

                        _logger.Error(e.Message);
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
