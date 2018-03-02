using Common.Log;
using Lykke.Service.ExchangeDataStore.Core.Domain.OrderBooks;
using Lykke.Service.ExchangeDataStore.Core.Services.Exchange;
using Lykke.Service.ExchangeDataStore.Services.DataHarvesters;
using Lykke.Service.ExchangeDataStore.Services.Domain;
using System;
using System.Threading;
using System.Threading.Tasks;

// ReSharper disable ClassNeverInstantiated.Global

namespace Lykke.Service.ExchangeDataStore.Services.DataPersisters
{
    public class OrderbookDataPersister : IDisposable
    {
        private string Component = nameof(OrderbookDataPersister);
        private readonly IOrderBookSnapshotsRepository _orderBookSnapshotsRepository;
        private readonly IExchangeInstrumentsService _exchangeInstrumentsService;
        private CancellationTokenSource cancellationSource;
        private readonly ILog _log;

        public OrderbookDataPersister(IOrderBookSnapshotsRepository orderBookSnapshotsRepository, IExchangeInstrumentsService exchangeInstrumentsService, ILog log)
        {
            _orderBookSnapshotsRepository = orderBookSnapshotsRepository;
            _exchangeInstrumentsService = exchangeInstrumentsService;
            _log = log;
        }

        private async Task PersistData(object sender, OrderBook orderBook)
        {
            var orderBookSnapshot = new OrderBookSnapshot(orderBook);
            await _orderBookSnapshotsRepository.SaveAsync(orderBookSnapshot, cancellationSource.Token);
            await _exchangeInstrumentsService.SaveIfNotExistsAsync(orderBook.Source, orderBook.AssetPairId);
        }

        public void Start()
        {
            cancellationSource = new CancellationTokenSource();
            OrderbookDataHarvester.OrderBookReceived += PersistData;
            _log.WriteInfoAsync(Component, "Initializing", "", "Started");
        }

        public void Dispose()
        {
            cancellationSource?.Cancel();
            cancellationSource?.Dispose();
        }

        public void Stop()
        {
            cancellationSource?.Cancel();
            OrderbookDataHarvester.OrderBookReceived -= PersistData;
            _log.WriteInfoAsync(Component, "Initializing", "", "Stopped");
        }
    }
}
