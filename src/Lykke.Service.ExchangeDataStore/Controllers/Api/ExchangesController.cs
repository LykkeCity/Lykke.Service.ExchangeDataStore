using Lykke.Service.ExchangeDataStore.Core.Domain.Exchange;
using Lykke.Service.ExchangeDataStore.Core.Services.Exchange;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace Lykke.Service.ExchangeDataStore.Controllers.Api
{
    public class ExchangesController : BaseApiController
    {
        private readonly IExchangeInstrumentsService _exchangeInstrumentsService;
        
        public ExchangesController(IExchangeInstrumentsService exchangeInstrumentsService)
        {
            _exchangeInstrumentsService = exchangeInstrumentsService;
        }

        /// <summary>
        /// Get availbale exchanges and instruments 
        /// <param name="exchangeName">Optional exchange name </param>
        /// </summary>
        [SwaggerOperation("GetExchangeInstrumentsAsync")]
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<ExchangeInstruments>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> Get(string exchangeName)
        {
            return Ok(await _exchangeInstrumentsService.GetExchangeInstrumentsAsync(exchangeName));
        }
    }
}
