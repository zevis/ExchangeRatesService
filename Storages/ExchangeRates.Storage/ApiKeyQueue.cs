using System.Threading;

namespace ExchangeRates.Storage
{
    public class ApiKeyQueue
    {
        private readonly IAckableQueue _ackableQueue;
        private string _currentApiKey { get; set; }

        public ApiKeyQueue(IAckableQueue ackable_queue)
        {
            _ackableQueue = ackable_queue;
        }

        public string GetCurrentApiKey()
        {
            return _currentApiKey;
        }

        public void KeyIsOver(string api_key)
        {
            _ackableQueue.Dequeue();
            string new_api_key = string.Empty;
        }
    }
}