using Common.Log;
using Lykke.Service.ExchangeDataStore.Core.Domain.Exchange;
using Lykke.Service.ExchangeDataStore.Core.Services.Exchange;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace Lykke.Service.ExchangeDataStore.Controllers.Api
{
    public class ExchangesController : BaseApiController
    {
        private readonly IExchangeInstrumentsService _exchangeInstrumentsService;
        
        public ExchangesController(ILog log, IExchangeInstrumentsService exchangeInstrumentsService) : base(log)
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
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> Get(string exchangeName)
        {
            try
            {
                return Ok(await _exchangeInstrumentsService.GetExchangeInstrumentsAsync(exchangeName));
            }
            catch (Exception ex)
            {
                return await LogAndReturnInternalServerError($"{exchangeName}", ControllerContext, ex);
            }
        }
    }
}
