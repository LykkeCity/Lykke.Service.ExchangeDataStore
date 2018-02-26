using Lykke.Service.ExchangeDataStore.Core.Domain.Exchange;
using Lykke.Service.ExchangeDataStore.Core.Services.Exchange;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Lykke.Service.ExchangeDataStore.Services.Domain
{
    public class ExchangeInstrumentsService : IExchangeInstrumentsService
    {
        private IExchangeInstrumentsRepository _repo;


        public ExchangeInstrumentsService(IExchangeInstrumentsRepository repo)
        {
            _repo = repo;
        }

        public async Task SaveIfNotExistsAsync(string exchangeName, string instrument)
        {
            await _repo.SaveIfNotExists(exchangeName, instrument);
        }

        public async Task<IEnumerable<ExchangeInstruments>> GetExchangeInstrumentsAsync()
        {
            return await _repo.GetExchangeInstruments();
        }

        public async Task<ExchangeInstruments> GetExchangeInstrumentsAsync(string exchangeName)
        {
            return await _repo.GetExchangeInstruments(exchangeName);
        }
    }
}
