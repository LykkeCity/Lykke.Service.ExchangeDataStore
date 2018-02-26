using AzureStorage;
using Common.Log;
using Lykke.Service.ExchangeDataStore.Core.Domain.Exchange;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace Lykke.Service.ExchangeDataStore.AzureRepositories.ExchangeInstruments
{
    // ReSharper disable once ClassNeverInstantiated.Global - Autofac injected
    public class ExchangeInstrumentsRepository : IExchangeInstrumentsRepository
    {
        private readonly ILog _log;
        private static readonly string _className = nameof(ExchangeInstrumentsRepository);
        private readonly INoSQLTableStorage<ExchangeInstrumentEntity> _tableStorage;
        private readonly ConcurrentDictionary<string, IList<string>> _knownExchangeInstruments; //local cache, dont check in Azure multiple times for exchanges/instruments we already have data for
        
        public ExchangeInstrumentsRepository(INoSQLTableStorage<ExchangeInstrumentEntity> tableStorage, ILog log)
        {
            _tableStorage = tableStorage;
            _log = log;
            _knownExchangeInstruments = new ConcurrentDictionary<string, IList<string>>();
        }

        public async Task SaveIfNotExists(string exchangeName, string instrument)
        {
            if (!_knownExchangeInstruments.ContainsKey(exchangeName) || (_knownExchangeInstruments.TryGetValue(exchangeName, out var instruments) && !instruments.Contains(instrument)) )
            {
                var added = await _tableStorage.CreateIfNotExistsAsync(new ExchangeInstrumentEntity(exchangeName, instrument));
                if (added)
                {
                    await _log.WriteInfoAsync(_className, nameof(SaveIfNotExists), $"{exchangeName} & {instrument} added to ExchangeInstruments table.");
                    _knownExchangeInstruments.AddOrUpdate(exchangeName, new List<string> { instrument }, (key, oldValue) =>
                    {
                        oldValue.Add(instrument);
                        return oldValue;
                    });
                }
            }
        }

        public async Task<IEnumerable<Core.Domain.Exchange.ExchangeInstruments>> GetExchangeInstruments()
        {
            try
            {
                var allExchangeInstruments = (await _tableStorage.GetDataAsync()).GroupBy(k => k.ExchangeName).ToDictionary(g => g.Key, g => g.Select(i => i.Instrument));
                return allExchangeInstruments.Select(e => new Core.Domain.Exchange.ExchangeInstruments(e.Key, new ReadOnlyCollection<string>(e.Value.ToList())));
            }
            catch (Exception ex)
            {
                await _log.WriteErrorAsync(_className, nameof(GetExchangeInstruments), ex);
                throw;
            }
        }

        public async Task<Core.Domain.Exchange.ExchangeInstruments> GetExchangeInstruments(string exchangeName)
        {
            try
            {
                var exchangeInstruments = await _tableStorage.GetDataAsync(exchangeName);
                return new Core.Domain.Exchange.ExchangeInstruments(exchangeName, new ReadOnlyCollection<string>(exchangeInstruments.Select(s => s.Instrument).ToList()));
            }
            catch (Exception ex)
            {
                await _log.WriteErrorAsync(_className, $"{nameof(GetExchangeInstruments)}, {exchangeName}", ex);
                throw;
            }
        }
    }
}
