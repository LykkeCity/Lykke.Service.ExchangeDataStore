using Lykke.Service.ExchangeDataStore.Core.Settings;
using Lykke.SettingsReader;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;


namespace Lykke.Service.ExchangeDataStore.Tests
{
    public static class SettingsManager
    {
        private static string _configFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "testsettings.json");

        public static AppSettings GetConfiguration()
        {
            return GetSettingsMenager().CurrentValue;
        }

        public static IReloadingManager<AppSettings> GetSettingsMenager()
        {
            var configBuilder = new ConfigurationBuilder();
            configBuilder.AddJsonFile(_configFilePath, optional: false, reloadOnChange: true);
            var configuration = configBuilder.Build();

            configuration["SettingsUrl"] = _configFilePath;
            var settingsManager = configuration.LoadSettings<AppSettings>("SettingsUrl");

            return settingsManager;
        }
    }
}
