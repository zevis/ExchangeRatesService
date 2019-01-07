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
        public class Rate
        {
            [JsonProperty("1. open")]
            public double Value;
        }

        public class ValCursValute
        {
            [JsonProperty("Time Series FX (Daily)")]
            public Dictionary<string, Rate> Rates;
        }

        /// <inheritdoc />
        public async Task<List<ValuteRateOnDate>> GetValuteRatesOnDatesAsync(string valute_code, DateTime rate_date)
        {
            var uri =
                $@"https://www.alphavantage.co/query?function=FX_DAILY&from_symbol={valute_code}&to_symbol=RUB&outputsize=full&apikey=5P6UFL2J0TAQ4U4S";
            HttpResponseMessage response = await HttpClientFactory.Create().GetAsync(uri);

            return JsonConvert.DeserializeObject<ValCursValute>(await response.Content.ReadAsStringAsync()).Rates
                .Select(valute =>
                    new ValuteRateOnDate(valute_code,
                        DateTime.ParseExact(valute.Key, new[] {"yyyy-MM-dd"}, CultureInfo.InvariantCulture,
                            DateTimeStyles.None), Convert.ToDouble(valute.Value.Value))).ToList();
        }
    }
}
