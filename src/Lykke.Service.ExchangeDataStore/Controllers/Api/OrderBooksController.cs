using Lykke.Service.ExchangeDataStore.Core.Domain.OrderBooks;
using Lykke.Service.ExchangeDataStore.Core.Services.OrderBooks;
using Lykke.Service.ExchangeDataStore.Models.Requests;
using Lykke.Service.ExchangeDataStore.Models.ValidationModels;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace Lykke.Service.ExchangeDataStore.Controllers.Api
{
    [ValidateModel]
    public class OrderBooksController : BaseApiController
    {
        private readonly IOrderBookService _orderBookService;
        public OrderBooksController(IOrderBookService orderBookService)
        {
            _orderBookService = orderBookService;
        }

        /// <summary>
        /// Get list of order books
        /// <param name="request">The name of the exchange and instrument symbol</param>
        /// <param name="dateTimeFrom">Period from</param>
        /// <param name="dateTimeTo">Period to</param>
        /// </summary>
        [SwaggerOperation("GetOrderBooks")]
        [HttpGet("{exchangeName}/{instrument}")]
        [ProducesResponseType(typeof(IEnumerable<OrderBook>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> Get(OrderBookRequest request, [FromQuery]DateTime dateTimeFrom, [FromQuery]DateTime? dateTimeTo = null)
        {
            return Ok(await _orderBookService.GetAsync(request.ExchangeName, request.Instrument, dateTimeFrom, dateTimeTo ?? DateTime.UtcNow));
        }
    }
}
