using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ClassifiedAds.Domain.Entities;
using ClassifiedAds.DomainServices.Repositories;
using ClassifiedAds.DomainServices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ClassifiedAds.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IProductService _productService;

        public ProductsController(IUnitOfWork unitOfWork, IProductService productService)
        {
            _unitOfWork = unitOfWork;
            _productService = productService;
        }

        [HttpGet]
        public ActionResult<IEnumerable<Product>> Get()
        {
            return Ok(_productService.GetProducts());
        }

        [HttpGet("{id}")]
        public ActionResult<Product> Get(Guid id)
        {
            var product = _productService.GetById(id);
            if (product == null)
                return NotFound();
            return Ok(product);
        }

        [HttpPost]
        public ActionResult<Product> Post([FromBody] Product model)
        {
            _productService.Create(model);
            _unitOfWork.SaveChanges();
            return Ok(model);
        }


        [HttpPut("{id}")]
        public ActionResult Put(Guid id, [FromBody] Product model)
        {
            var product = _productService.GetById(id);
            if (product == null)
                return NotFound();

            product.Name = model.Name;

            _unitOfWork.SaveChanges();

            return Ok(product);
        }

        [HttpDelete("{id}")]
        public ActionResult Delete(Guid id)
        {
            var product = _productService.GetById(id);
            if (product == null)
                return NotFound();

            _productService.Delete(product);
            _unitOfWork.SaveChanges();

            return Ok();
        }
    }
}