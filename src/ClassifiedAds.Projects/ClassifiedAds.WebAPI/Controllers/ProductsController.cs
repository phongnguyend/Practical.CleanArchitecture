using ClassifiedAds.Application;
using ClassifiedAds.Application.Commands.Products;
using ClassifiedAds.Application.Queries.Products;
using ClassifiedAds.Domain.Entities;
using ClassifiedAds.Domain.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;

namespace ClassifiedAds.WebAPI.Controllers
{
    [Authorize]
    [Produces("application/json")]
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService;
        private readonly Dispatcher _dispatcher;
        private readonly ILogger _logger;

        public ProductsController(IProductService productService, Dispatcher dispatcher, ILogger<ProductsController> logger)
        {
            _productService = productService;
            _dispatcher = dispatcher;
            _logger = logger;
        }

        [HttpGet]
        public ActionResult<IEnumerable<Product>> Get()
        {
            _logger.LogInformation("Getting all products");
            var products = _dispatcher.Dispatch(new GetProductsQuery());
            return Ok(products);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<Product> Get(Guid id)
        {
            var product = _dispatcher.Dispatch(new GetProductQuery { Id = id, ThrowNotFoundIfNull = true });
            return Ok(product);
        }

        [HttpPost]
        [Consumes("application/json")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public ActionResult<Product> Post([FromBody] Product model)
        {
            _dispatcher.Dispatch(new AddUpdateProductCommand { Product = model });
            return Created($"/api/products/{model.Id}", model);
        }

        [HttpPut("{id}")]
        [Consumes("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult Put(Guid id, [FromBody] Product model)
        {
            var product = _dispatcher.Dispatch(new GetProductQuery { Id = id, ThrowNotFoundIfNull = true });

            product.Code = model.Code;
            product.Name = model.Name;
            product.Description = model.Description;

            _dispatcher.Dispatch(new AddUpdateProductCommand { Product = product });

            return Ok(product);
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult Delete(Guid id)
        {
            var product = _dispatcher.Dispatch(new GetProductQuery { Id = id, ThrowNotFoundIfNull = true });

            _dispatcher.Dispatch(new DeleteProductCommand { Product = product });

            return Ok();
        }
    }
}