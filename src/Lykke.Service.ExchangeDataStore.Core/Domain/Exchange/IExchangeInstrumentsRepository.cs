using System.Collections.Generic;
using System.Threading.Tasks;

namespace Lykke.Service.ExchangeDataStore.Core.Domain.Exchange
{
    public interface IExchangeInstrumentsRepository
    {
        Task SaveIfNotExists(string exchangeName, string instrument);
        Task<IEnumerable<ExchangeInstruments>> GetExchangeInstruments();
        Task<ExchangeInstruments> GetExchangeInstruments(string exchangeName);

    }
}
