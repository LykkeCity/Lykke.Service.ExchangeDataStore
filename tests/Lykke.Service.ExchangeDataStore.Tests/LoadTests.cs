using AzureStorage;
using AzureStorage.Blob;
using AzureStorage.Tables;
using Common.Log;
using Lykke.RabbitMqBroker;
using Lykke.RabbitMqBroker.Publisher;
using Lykke.RabbitMqBroker.Subscriber;
using Lykke.Service.ExchangeDataStore.AzureRepositories.ExchangeInstruments;
using Lykke.Service.ExchangeDataStore.AzureRepositories.OrderBooks;
using Lykke.Service.ExchangeDataStore.Core.Domain.OrderBooks;
using Lykke.Service.ExchangeDataStore.Core.Settings;
using Lykke.Service.ExchangeDataStore.Core.Settings.ServiceSettings;
using Lykke.Service.ExchangeDataStore.Services.DataHarvesters;
using Lykke.Service.ExchangeDataStore.Services.DataPersisters;
using Lykke.Service.ExchangeDataStore.Services.Domain;
using Lykke.Service.ExchangeDataStore.Services.Helpers;
using Lykke.SettingsReader;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Lykke.Service.ExchangeDataStore.Tests
{
    public class LoadTests
    {
        private readonly ILog _log;
        private static int OrderBooksToPublish = 5000;
        private int _messagesProcessed = OrderBooksToPublish;
        private int _orderBookSize = 200;
        private readonly RabbitMqSubscriptionSettings rabbitSettings;
        private readonly IReloadingManager<AppSettings> settings;

        public LoadTests()
        {
            _log = new LogToConsole();
            settings = SettingsManager.GetSettingsMenager();
            rabbitSettings = new RabbitMqSubscriptionSettings
            {
                ConnectionString = settings.CurrentValue.ExchangeDataStoreService.RabbitMq.OrderBooks.ConnectionString,
                ExchangeName = settings.CurrentValue.ExchangeDataStoreService.RabbitMq.OrderBooks.Exchange,
                IsDurable = true,
                QueueName = settings.CurrentValue.ExchangeDataStoreService.RabbitMq.OrderBooks.Queue
            };
        }

        private RabbitMqPublisher<OrderBook> GetLocalPublisher()
        {
            var rabbitPublisher = new RabbitMqPublisher<OrderBook>(rabbitSettings)
                .DisableInMemoryQueuePersistence()
                .SetSerializer(new GenericRabbitModelConverter<OrderBook>())
                .SetPublishStrategy(new DefaultFanoutPublishStrategy(rabbitSettings))
                .SetLogger(_log)
                .PublishSynchronously();

            return rabbitPublisher;
        }

        private RabbitMqSubscriber<OrderBook> GetLocalSubscriber()
        {
            var errorStrategy = new DefaultErrorHandlingStrategy(_log, rabbitSettings);
            var subscriber = new RabbitMqSubscriber<OrderBook>(rabbitSettings, errorStrategy)
                .SetMessageDeserializer(new GenericRabbitModelConverter<OrderBook>())
                .SetMessageReadStrategy(new MessageReadQueueStrategy())
                .SetLogger(_log)
                .Subscribe(ProcessMessageWithNoDelay);

            return subscriber;
        }

        private Task ProcessMessageWithNoDelay(OrderBook ob)
        {
            _log.WriteInfo("", "", $"OrderBook received: {ob.Info()}");
            _messagesProcessed = _messagesProcessed - 1;
            return Task.CompletedTask;
        }

        private IBlobStorage GetRealBlobStorage()
        {
            var azureBlobStorage = AzureBlobStorage.Create(settings.ConnectionString(i => i.ExchangeDataStoreService.AzureStorage.EntitiesConnString));
            return azureBlobStorage;
        }

        private INoSQLTableStorage<OrderBookSnapshotEntity> GetRealTableOrderBookSnapshotRepo()
        {
            var orderBookSnapshotStorage = AzureTableStorage<OrderBookSnapshotEntity>.Create(settings.ConnectionString(i => i.ExchangeDataStoreService.AzureStorage.EntitiesConnString), settings.CurrentValue.ExchangeDataStoreService.AzureStorage.EntitiesTableName, _log);
            return orderBookSnapshotStorage;
        }

        private INoSQLTableStorage<ExchangeInstrumentEntity> GetRealTableExchangeInstrumentEntityRepo()
        {
            var orderBookSnapshotStorage = AzureTableStorage<ExchangeInstrumentEntity>.Create(settings.ConnectionString(i => i.ExchangeDataStoreService.AzureStorage.EntitiesConnString), settings.CurrentValue.ExchangeDataStoreService.AzureStorage.EntitiesTableName, _log);
            return orderBookSnapshotStorage;
        }

        private OrderbookDataPersister GetOrderbookDataPersister()
        {
            var persister = new OrderbookDataPersister(new OrderBookSnapshotsRepository(GetRealTableOrderBookSnapshotRepo(), GetRealBlobStorage(), _log, settings.CurrentValue.ExchangeDataStoreService.AzureStorage),
                            new ExchangeInstrumentsService(new ExchangeInstrumentsRepository(GetRealTableExchangeInstrumentEntityRepo(), _log)), _log);
            return persister;
        }

        private OrderbookDataHarvester GetOrderbookDataHarvester()
        {
            OrderbookDataHarvester harverster = new OrderbookDataHarvester(_log, new RabbitMqExchangeConfiguration()
            {
                ConnectionString = rabbitSettings.ConnectionString,
                Enabled = true,
                Exchange = rabbitSettings.ExchangeName,
                Queue = rabbitSettings.QueueName
            });

            return harverster;
        }

        [Fact]
        public async void TestHarvesterAndPersister()
        {
            var persister = GetOrderbookDataPersister();
            var harvester = GetOrderbookDataHarvester();

            persister.Start();
            harvester.Start();

            while (_messagesProcessed > 0)
            {
                Thread.SpinWait(1);
            }
        }

        [Fact]
        public async void TestLocalPublisherSubscriber()
        {
            var volumePrices = new List<VolumePrice>();
            for (int i = 1; i <= _orderBookSize; i++)
            {
                volumePrices.Add(new VolumePrice(i / i + 2, i + i * i));
            }

            List<OrderBook> orderbooks = new List<OrderBook>();
            for (int i = 1; i <= OrderBooksToPublish; i++)
            {
                orderbooks.Add(new OrderBook("GDAX", "BTCUSD", volumePrices, volumePrices, DateTime.UtcNow));
            }

            var subscriber = GetLocalSubscriber().Start();
            var publisher = GetLocalPublisher().Start();

            foreach (var ob in orderbooks)
            {
                publisher.ProduceAsync(ob);
            }

            while (_messagesProcessed > 0)
            {
               Thread.SpinWait(1); 
            }
        }
    }

}
