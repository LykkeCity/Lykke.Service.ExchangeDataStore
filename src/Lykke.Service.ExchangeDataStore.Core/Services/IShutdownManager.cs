using System.Threading.Tasks;
using Common;

namespace Lykke.Service.ExchangeDataStore.Core.Services
{
    public interface IShutdownManager
    {
        Task StopAsync();

        void Register(IStopable stopable);
    }
}
