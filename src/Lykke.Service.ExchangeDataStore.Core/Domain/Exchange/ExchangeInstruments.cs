using System.Collections.Generic;

namespace Lykke.Service.ExchangeDataStore.Core.Domain.Exchange
{
    public class ExchangeInstruments
    {
        public ExchangeInstruments(string exchangeName, IReadOnlyCollection<string> instruments)
        {
            ExchangeName = exchangeName;
            Instruments = instruments;
        }

        public string ExchangeName { get; set; }
        public IReadOnlyCollection<string> Instruments { get; }
    }
}

