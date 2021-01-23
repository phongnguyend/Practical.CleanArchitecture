using ClassifiedAds.Application;
using ClassifiedAds.Services.Product.Commands;
using ClassifiedAds.Services.Product.DTOs;
using ClassifiedAds.Services.Product.Models;
using ClassifiedAds.Services.Product.Queries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ClassifiedAds.Services.Product.Controllers
{
    [Authorize]
    [Produces("application/json")]
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly Dispatcher _dispatcher;
        private readonly ILogger _logger;

        public ProductsController(Dispatcher dispatcher, ILogger<ProductsController> logger)
        {
            _dispatcher = dispatcher;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Entities.Product>>> Get()
        {
            _logger.LogInformation("Getting all products");
            var products = await _dispatcher.DispatchAsync(new GetProductsQuery());
            var model = products.ToModels();
            return Ok(model);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<Entities.Product>> Get(Guid id)
        {
            var product = await _dispatcher.DispatchAsync(new GetProductQuery { Id = id, ThrowNotFoundIfNull = true });
            var model = product.ToModel();
            return Ok(model);
        }

        [HttpPost]
        [Consumes("application/json")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<ActionResult<Entities.Product>> Post([FromBody] ProductModel model)
        {
            var product = model.ToEntity();
            await _dispatcher.DispatchAsync(new AddUpdateProductCommand { Product = product });
            model = product.ToModel();
            return Created($"/api/products/{model.Id}", model);
        }

        [HttpPut("{id}")]
        [Consumes("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> Put(Guid id, [FromBody] ProductModel model)
        {
            var product = await _dispatcher.DispatchAsync(new GetProductQuery { Id = id, ThrowNotFoundIfNull = true });

            product.Code = model.Code;
            product.Name = model.Name;
            product.Description = model.Description;

            await _dispatcher.DispatchAsync(new AddUpdateProductCommand { Product = product });

            model = product.ToModel();

            return Ok(model);
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> Delete(Guid id)
        {
            var product = await _dispatcher.DispatchAsync(new GetProductQuery { Id = id, ThrowNotFoundIfNull = true });

            await _dispatcher.DispatchAsync(new DeleteProductCommand { Product = product });

            return Ok();
        }

        [HttpGet("{id}/auditlogs")]
        public async Task<ActionResult<IEnumerable<AuditLogEntryDTO>>> GetAuditLogs(Guid id)
        {
            var logs = await _dispatcher.DispatchAsync(new GetAuditEntriesQuery { ObjectId = id.ToString() });

            List<dynamic> entries = new List<dynamic>();
            ProductModel previous = null;
            foreach (var log in logs.OrderBy(x => x.CreatedDateTime))
            {
                var data = JsonConvert.DeserializeObject<ProductModel>(log.Log);
                var highLight = new
                {
                    Code = previous != null && data.Code != previous.Code,
                    Name = previous != null && data.Name != previous.Name,
                    Description = previous != null && data.Description != previous.Description,
                };

                var entry = new
                {
                    log.Id,
                    log.UserName,
                    Action = log.Action.Replace("_PRODUCT", string.Empty),
                    log.CreatedDateTime,
                    data,
                    highLight,
                };
                entries.Add(entry);

                previous = data;
            }

            return Ok(entries.OrderByDescending(x => x.CreatedDateTime));
        }
    }
}