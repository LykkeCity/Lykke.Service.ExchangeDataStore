using Lykke.Service.ExchangeDataStore.Core.Domain.Exchange;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Lykke.Service.ExchangeDataStore.Core.Services.Exchange
{
    public interface IExchangeInstrumentsService
    {
        Task SaveIfNotExistsAsync(string exchangeName, string instrument);
        Task<IEnumerable<ExchangeInstruments>> GetExchangeInstrumentsAsync(); 
        Task<ExchangeInstruments> GetExchangeInstrumentsAsync(string exchangeName);
    }
}
