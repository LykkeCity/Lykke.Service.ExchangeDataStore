using Lykke.Service.ExchangeDataStore.Core.Domain.OrderBooks;
using Lykke.Service.ExchangeDataStore.Core.Helpers;
using MessagePack;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Lykke.Service.ExchangeDataStore.Services.Domain
{
    [MessagePackObject()]
    public sealed class OrderBookSnapshot : IOrderBookSnapshot
    {
        [Key(0)]
        public string Source { get; set; }

        [Key(1)]
        public string AssetPair { get; set; }

        [Key(2)]
        public DateTime Timestamp { get; set; }

        [Key(3)]
        public IReadOnlyCollection<OrderBookItem> Asks { get; set; }

        [Key(4)]
        public IReadOnlyCollection<OrderBookItem> Bids { get; set; }

        [IgnoreMember]
        public string GeneratedId { get; set; }

        public OrderBookSnapshot()
        {
                
        }

        public OrderBookSnapshot(OrderBook orderBook)
        {
            Timestamp = orderBook.Timestamp;
            Source = orderBook.Source;
            AssetPair = orderBook.AssetPairId;
            Asks = new ReadOnlyCollection<OrderBookItem>(orderBook.Asks.Select(order => order.ToAskOrderBookItem(orderBook.AssetPairId)).ToList());
            Bids = new ReadOnlyCollection<OrderBookItem>(orderBook.Bids.Select(order => order.ToBidOrderBookItem(orderBook.AssetPairId)).ToList());
        }
    }
}
