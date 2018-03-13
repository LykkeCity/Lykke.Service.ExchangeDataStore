// ReSharper disable UnusedAutoPropertyAccessor.Global
using MessagePack;
namespace Lykke.Service.ExchangeDataStore.Core.Domain.OrderBooks
{
    [MessagePackObject()]
    public class OrderBookItem 
    {
        [Key(0)]
        public decimal Price { get; set; }
        [Key(1)]
        public decimal Size { get; set; }
        [Key(2)]
        public string Symbol { get; set; }
        [Key(3)]
        public bool IsBuy { get; set; }
    }
}
