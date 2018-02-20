using Lykke.SettingsReader.Attributes;

namespace Lykke.Service.ExchangeDataStore.Settings.ServiceSettings
{
    public class DbSettings
    {
        [AzureTableCheck]
        public string LogsConnString { get; set; }
    }
}
