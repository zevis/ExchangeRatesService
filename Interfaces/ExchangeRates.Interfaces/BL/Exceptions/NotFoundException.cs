using System;

namespace ExchangeRates.Interfaces.BL
{
    public class NotFoundException : Exception
    {
        public NotFoundException(string message) : base(message)
        {
        }
    }
}
