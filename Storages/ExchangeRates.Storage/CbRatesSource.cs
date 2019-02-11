using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using ExchangeRates.Interfaces.Models;
using ExchangeRates.Interfaces.Storages;

namespace ExchangeRates.Storage
{
    public class CbRatesSource : IRatesSource
    {
#pragma warning disable 649
        private class CurrencyRates
        {
            [XmlElement("Currency")]
            public CurrencyRate[] CurrencyList;
        }

        private class CurrencyRate
        {

            [XmlElement("CharCode")]
            public string CurrencyCode;

            [XmlElement("Value")]
            public string Rate;
        }
#pragma warning restore 649

        /// <inheritdoc />
        public async Task<List<CurrencyRateOnDate>> GetCurrencyRatesOnDatesAsync(string currency_code, DateTime rate_date)
        {
            XmlSerializer xs = new XmlSerializer(typeof(CurrencyRates));
            var uri = $@"http://www.cbr.ru/scripts/XML_daily.asp?date_req={rate_date:dd/MM/yyyy}";
            HttpResponseMessage xml = await HttpClientFactory.Create().GetAsync(uri);

            XmlReader xr = new XmlTextReader(await xml.Content.ReadAsStreamAsync());
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            return ((CurrencyRates) xs.Deserialize(xr)).CurrencyList.Select(currency =>
                new CurrencyRateOnDate(currency.CurrencyCode, rate_date, Convert.ToDouble(currency.Rate))).ToList();
        }
    }
}
