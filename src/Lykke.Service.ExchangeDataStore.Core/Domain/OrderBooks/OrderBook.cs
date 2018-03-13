using Lykke.Service.ExchangeDataStore.Core.Helpers;
using MessagePack;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Lykke.Service.ExchangeDataStore.Core.Domain.OrderBooks
{
    [MessagePackObject()]
    public sealed class OrderBook
    {
        public OrderBook(string source, string assetPairId, IReadOnlyCollection<VolumePrice> asks, IReadOnlyCollection<VolumePrice> bids, DateTime timestamp)
        {
            Source = source;
            AssetPairId = assetPairId;
            Asks = asks;
            Bids = bids;
            Timestamp = timestamp;
        }

        [JsonProperty("source")]
        [Key(0)]
        public string Source { get; }

        [JsonProperty("asset")]
        [Key(1)]
        public string AssetPairId { get; }

        [JsonProperty("timestamp")]
        [Key(2)]
        public DateTime Timestamp { get; }

        [JsonProperty("asks")]
        [Key(3)]
        public IReadOnlyCollection<VolumePrice> Asks { get; }

        [JsonProperty("bids")]
        [Key(4)]
        public IReadOnlyCollection<VolumePrice> Bids { get; }

        public string Info()
        {
            return $"{Source},{AssetPairId},{Timestamp.ToSnapshotTimestampFormat()}";
        }

    }

    
}
