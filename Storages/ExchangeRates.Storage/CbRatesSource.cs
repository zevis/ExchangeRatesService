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
        public class ValCurs
        {
            [XmlElement("Valute")]
            public ValCursValute[] ValuteList;
        }

        public class ValCursValute
        {

            [XmlElement("CharCode")]
            public string ValuteCode;

            [XmlElement("Value")]
            public string Rate;
        }

        /// <inheritdoc />
        public async Task<List<ValuteRateOnDate>> GetValuteRatesOnDatesAsync(string valute_code, DateTime rate_date)
        {
            XmlSerializer xs = new XmlSerializer(typeof(ValCurs));
            var uri = $@"http://www.cbr.ru/scripts/XML_daily.asp?date_req={rate_date:dd/MM/yyyy}";
            HttpResponseMessage xml = await HttpClientFactory.Create().GetAsync(uri);

            XmlReader xr = new XmlTextReader(await xml.Content.ReadAsStreamAsync());
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            return ((ValCurs) xs.Deserialize(xr)).ValuteList.Select(valute =>
                new ValuteRateOnDate(valute.ValuteCode, rate_date, Convert.ToDouble(valute.Rate))).ToList();
        }
    }
}
