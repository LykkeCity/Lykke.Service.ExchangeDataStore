using Lykke.Service.ExchangeDataStore.Infrastructure.Logging;
using Microsoft.AspNetCore.Mvc;

namespace Lykke.Service.ExchangeDataStore.Controllers.Api
{
    [Route("api/v1/[controller]")]
    [Produces("application/json")]
    [LoggingAspNetFilter]
    public class BaseApiController : Controller
    {
       
    }
}
