using Lykke.Service.ExchangeDataStore.Settings.ServiceSettings;
using Lykke.Service.ExchangeDataStore.Settings.SlackNotifications;

namespace Lykke.Service.ExchangeDataStore.Settings
{
    public class AppSettings
    {
        public ExchangeDataStoreSettings ExchangeDataStoreService { get; set; }
        public SlackNotificationsSettings SlackNotifications { get; set; }
    }
}
