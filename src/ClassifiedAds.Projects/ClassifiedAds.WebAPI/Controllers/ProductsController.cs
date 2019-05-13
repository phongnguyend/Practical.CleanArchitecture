using System;
using System.Collections.Generic;
using ClassifiedAds.Domain.Entities;
using ClassifiedAds.DomainServices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ClassifiedAds.ApplicationServices;
using ClassifiedAds.ApplicationServices.Queries.Products;
using Microsoft.AspNetCore.Authorization;

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

        public ProductsController(IProductService productService, Dispatcher dispatcher)
        {
            _productService = productService;
            _dispatcher = dispatcher;
        }

        [HttpGet]
        public ActionResult<IEnumerable<Product>> Get()
        {
            var products = _dispatcher.Dispatch(new GetProductsQuery());
            return Ok(products);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<Product> Get(Guid id)
        {
            var product = _productService.GetById(id);
            if (product == null)
            {
                return NotFound();
            }

            return Ok(product);
        }

        [HttpPost]
        [Consumes("application/json")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public ActionResult<Product> Post([FromBody] Product model)
        {
            _productService.Create(model);
            return Created($"/api/products/{model.Id}", model);
        }


        [HttpPut("{id}")]
        [Consumes("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult Put(Guid id, [FromBody] Product model)
        {
            var product = _productService.GetById(id);
            if (product == null)
            {
                return NotFound();
            }

            product.Name = model.Name;

            _productService.Update(product);

            return Ok(product);
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult Delete(Guid id)
        {
            var product = _productService.GetById(id);
            if (product == null)
            {
                return NotFound();
            }

            _productService.Delete(product);

            return Ok();
        }
    }
}