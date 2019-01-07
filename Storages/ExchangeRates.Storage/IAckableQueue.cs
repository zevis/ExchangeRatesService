namespace ExchangeRates.Storage
{
    public interface IAckableQueue
    {
        void Dequeue();

        void Enqueue();

        void Ack();
    }
}