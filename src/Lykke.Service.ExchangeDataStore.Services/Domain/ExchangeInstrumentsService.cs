using Lykke.Service.ExchangeDataStore.Core.Domain.Exchange;
using Lykke.Service.ExchangeDataStore.Core.Services.Exchange;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lykke.Service.ExchangeDataStore.Services.Domain
{
    // ReSharper disable once ClassNeverInstantiated.Global - Autofac managed
    public class ExchangeInstrumentsService : IExchangeInstrumentsService
    {
        private readonly IExchangeInstrumentsRepository _repo;

        public ExchangeInstrumentsService(IExchangeInstrumentsRepository repo)
        {
            _repo = repo;
        }

        public Task SaveIfNotExistsAsync(string exchangeName, string instrument)
        {
            return _repo.SaveIfNotExists(exchangeName, instrument);
        }

        public async Task<IEnumerable<ExchangeInstruments>> GetExchangeInstrumentsAsync(string exchangeName)
        {
            if (String.IsNullOrWhiteSpace(exchangeName))
                return await _repo.GetExchangeInstrumentsAsync();

            return new[] { await _repo.GetExchangeInstrumentsAsync(exchangeName) }.AsEnumerable();
        }
    }
}
