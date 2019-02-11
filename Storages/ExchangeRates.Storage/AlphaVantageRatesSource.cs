using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using ExchangeRates.Interfaces.Models;
using ExchangeRates.Interfaces.Storages;
using Newtonsoft.Json;

namespace ExchangeRates.Storage
{
    public class AlphaVantageRatesSource : IRatesSource
    {
#pragma warning disable 649
        private class CurrencyRates
        {
            [JsonProperty("Time Series FX (Daily)")]
            public Dictionary<string, CurrencyRate> Rates;
        }

        private class CurrencyRate
        {
            [JsonProperty("1. open")]
            public double Value;
        }
#pragma warning restore 649

        /// <inheritdoc />
        public async Task<List<CurrencyRateOnDate>> GetCurrencyRatesOnDatesAsync(string currency_code, DateTime rate_date)
        {
            var uri =
                $@"https://www.alphavantage.co/query?function=FX_DAILY&from_symbol={currency_code}&to_symbol=RUB&outputsize=full&apikey=5P6UFL2J0TAQ4U4S";
            HttpResponseMessage response = await HttpClientFactory.Create().GetAsync(uri);

            return JsonConvert.DeserializeObject<CurrencyRates>(await response.Content.ReadAsStringAsync()).Rates
                .Select(currency =>
                    new CurrencyRateOnDate(currency_code,
                        DateTime.ParseExact(currency.Key, new[] {"yyyy-MM-dd"}, CultureInfo.InvariantCulture,
                            DateTimeStyles.None), Convert.ToDouble(currency.Value.Value))).ToList();
        }
    }
}
