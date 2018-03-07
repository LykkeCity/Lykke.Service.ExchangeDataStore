using System.Collections.Generic;
using System.Threading.Tasks;

namespace Lykke.Service.ExchangeDataStore.Core.Domain.Exchange
{
    public interface IExchangeInstrumentsRepository
    {
        Task SaveIfNotExists(string exchangeName, string instrument);
        Task<IEnumerable<ExchangeInstruments>> GetExchangeInstrumentsAsync();
        Task<ExchangeInstruments> GetExchangeInstrumentsAsync(string exchangeName);

    }
}
