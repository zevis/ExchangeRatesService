﻿namespace ExchangeRates.Storage
{
    public class LocalQueue : IAckableQueue
    {
        public void Dequeue()
        {
            throw new System.NotImplementedException();
        }

        public void Enqueue()
        {
            throw new System.NotImplementedException();
        }

        public void Ack()
        {
            throw new System.NotImplementedException();
        }
    }
}