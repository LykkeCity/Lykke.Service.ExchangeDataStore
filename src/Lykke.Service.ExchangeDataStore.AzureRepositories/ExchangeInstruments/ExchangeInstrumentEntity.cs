using Lykke.AzureStorage.Tables;

namespace Lykke.Service.ExchangeDataStore.AzureRepositories.ExchangeInstruments
{
    public class ExchangeInstrumentEntity : AzureTableEntity
    {
        public string ExchangeName => PartitionKey;
        public string Instrument => RowKey;

        public ExchangeInstrumentEntity(string exchangeName, string instrument)
        {
            PartitionKey = exchangeName;
            RowKey = instrument;
        }

        public ExchangeInstrumentEntity() //required by INoSQLTableStorage
        {
        }
    }
}





