using AutoMapper;
using ClassifiedAds.Application;
using ClassifiedAds.Application.Products.Commands;
using ClassifiedAds.Application.Products.Queries;
using ClassifiedAds.Domain.Entities;
using ClassifiedAds.WebAPI.Models.Products;
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
        private readonly Dispatcher _dispatcher;
        private readonly ILogger _logger;
        private readonly IMapper _mapper;

        public ProductsController(Dispatcher dispatcher, ILogger<ProductsController> logger, IMapper mapper)
        {
            _dispatcher = dispatcher;
            _logger = logger;
            _mapper = mapper;
        }

        [HttpGet]
        public ActionResult<IEnumerable<Product>> Get()
        {
            _logger.LogInformation("Getting all products");
            var products = _dispatcher.Dispatch(new GetProductsQuery());
            var model = _mapper.Map<List<ProductModel>>(products);
            return Ok(model);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<Product> Get(Guid id)
        {
            var product = _dispatcher.Dispatch(new GetProductQuery { Id = id, ThrowNotFoundIfNull = true });
            var model = _mapper.Map<ProductModel>(product);
            return Ok(model);
        }

        [HttpPost]
        [Consumes("application/json")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public ActionResult<Product> Post([FromBody] ProductModel model)
        {
            var product = _mapper.Map<Product>(model);
            _dispatcher.Dispatch(new AddUpdateProductCommand { Product = product });
            model = _mapper.Map<ProductModel>(product);
            return Created($"/api/products/{model.Id}", model);
        }

        [HttpPut("{id}")]
        [Consumes("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult Put(Guid id, [FromBody] ProductModel model)
        {
            var product = _dispatcher.Dispatch(new GetProductQuery { Id = id, ThrowNotFoundIfNull = true });

            product.Code = model.Code;
            product.Name = model.Name;
            product.Description = model.Description;

            _dispatcher.Dispatch(new AddUpdateProductCommand { Product = product });

            model = _mapper.Map<ProductModel>(product);

            return Ok(model);
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