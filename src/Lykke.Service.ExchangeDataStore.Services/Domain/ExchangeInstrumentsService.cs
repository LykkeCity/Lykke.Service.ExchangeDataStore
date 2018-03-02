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

        public Task SaveIfNotExistsAsync(string exchangeName, string instrument)
        {
            return _repo.SaveIfNotExists(exchangeName, instrument);
        }

        public Task<IEnumerable<ExchangeInstruments>> GetExchangeInstrumentsAsync()
        {
            return _repo.GetExchangeInstruments();
        }

        public Task<ExchangeInstruments> GetExchangeInstrumentsAsync(string exchangeName)
        {
            return _repo.GetExchangeInstruments(exchangeName);
        }
    }
}
