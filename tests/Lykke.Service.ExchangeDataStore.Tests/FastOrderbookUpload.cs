using Lykke.Service.ExchangeDataStore.Core.Domain.OrderBooks;
using Lykke.Service.ExchangeDataStore.Services.Domain;
using MessagePack;
using Microsoft.WindowsAzure.Storage;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Xunit;

namespace TradingBot.Tests
{
    public class OrderBooksToBlobSaveTest
    {
        [Fact]
        public async Task TestFastWriteMultipleSnapshots()
        {
            const int noRepeats = 50000;
            //  var set = new LocalSettingsReloadingManager<Settings>("appsettings.Develop.json");
            // var ac = CloudStorageAccount.Parse(set.CurrentValue.Db);
            var ac = CloudStorageAccount.DevelopmentStorageAccount;
            var blobClient = ac.CreateCloudBlobClient();
            var cr = blobClient.GetContainerReference("mycont");
            var blob = cr.GetBlockBlobReference("mykey");

            using (var str = await blob.OpenWriteAsync())
            {
                for (int i = 0; i < noRepeats; i++)
                {
                    var sn = GenerateSnapshot();
                    await MessagePackSerializer.SerializeAsync(str, sn);

                }
                await str.CommitAsync();
            }
        }




        [Fact]
        public async Task TestWriteInIntervals()
        {
            const int saveIntervalSeconds = 1;
            //  var set = new LocalSettingsReloadingManager<Settings>("appsettings.Develop.json");
            // var ac = CloudStorageAccount.Parse(set.CurrentValue.Db);
            var ac = CloudStorageAccount.DevelopmentStorageAccount;
            var blobClient = ac.CreateCloudBlobClient();
            var cr = blobClient.GetContainerReference("mycont");
            
            var orderBook = GenerateSnapshot();

            var nextRound = DateTime.UtcNow.AddSeconds(saveIntervalSeconds);

            while (true)
            {
                //orderBook.Asks.Add(new OrderBookItem {IsBuy = false, Price = (decimal)(10000 + 100d * DateTime.Now.Ticks) });
                //orderBook.Bids.Add(new OrderBookItem { IsBuy = true, Price = (decimal)(10000 + 100d * DateTime.Now.Ticks) });

                if (DateTime.UtcNow >= nextRound)
                {
                    //this_round = next_round;
                    nextRound = DateTime.UtcNow.AddSeconds(saveIntervalSeconds);
                    var blob = cr.GetBlockBlobReference(DateTime.UtcNow.ToString("yyyyMMddTHHmmss"));

                    using (var str = await blob.OpenWriteAsync())
                    {
                        await MessagePackSerializer.SerializeAsync(str, orderBook);
                        await str.CommitAsync();
                    }

                    orderBook = GenerateSnapshot();
                }
            }
        }

        [Fact]
        public async Task TestReadBlobWithManyBlocks()
        {
            //  var set = new LocalSettingsReloadingManager<Settings>("appsettings.Develop.json");
            // var ac = CloudStorageAccount.Parse(set.CurrentValue.Db);
            var ac = CloudStorageAccount.DevelopmentStorageAccount;
            var blobClient = ac.CreateCloudBlobClient();
            var cr = blobClient.GetContainerReference("mycont");
            var blob = cr.GetBlockBlobReference("mykey");

            using (var str = await blob.OpenReadAsync())
            {
                try
                {
                    while (true)
                    {
                        var db = MessagePackSerializer.Deserialize<OrderBookSnapshot>(str, true);
                    }
                }
                catch (InvalidOperationException ex)
                {
                    //All messages are readed
                }
            }
        }

        [Fact]
        public async Task TestRead()
        {
            //  var set = new LocalSettingsReloadingManager<Settings>("appsettings.Develop.json");
            // var ac = CloudStorageAccount.Parse(set.CurrentValue.Db);
            var ac = CloudStorageAccount.DevelopmentStorageAccount;
            var blobClient = ac.CreateCloudBlobClient();
            var cr = blobClient.GetContainerReference("mycont");
            var blob = cr.GetBlockBlobReference("mykey"); //or yyyyMMddTHHmmss

            using (var strr = new MemoryStream()) // var str = await blob.OpenReadAsync();
            {
                await blob.DownloadToStreamAsync(strr);

                try
                {
                    strr.Position = 0;
                    var db = MessagePackSerializer.Deserialize<OrderBookSnapshot>(strr);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }
        }

        private OrderBookSnapshot GenerateSnapshot()
        {
            const int depth = 50;
            var asks = new List<VolumePrice>();
            var bids = new List<VolumePrice>();
            var rnd = new Random((int)DateTime.Now.Ticks);
            for (int i = 0; i < depth; i++)
            {
                asks.Add(new VolumePrice((decimal)(10000 + 100d * rnd.NextDouble()), (decimal)(100d * rnd.NextDouble())));
                bids.Add(new VolumePrice((decimal)(5000 + 100d * rnd.NextDouble()), (decimal)(100d * rnd.NextDouble())));
            }

            var orderBook = new OrderBook("ICM", "EURUSD", asks, bids, DateTime.UtcNow);
            var result = new OrderBookSnapshot(orderBook);
            return result;
        }
    }
}
