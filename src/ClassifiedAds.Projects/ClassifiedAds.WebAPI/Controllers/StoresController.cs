using ClassifiedAds.Domain;
using ClassifiedAds.Domain.Entities;
using ClassifiedAds.Domain.Services;
using ClassifiedAds.WebAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;

namespace ClassifiedAds.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StoresController : ControllerBase
    {
        private readonly IStoreService _storeService;
        private readonly IProductService _productService;
        private readonly ILogger _logger;

        public StoresController(IStoreService storeService, IProductService productService, ILogger<StoresController> logger)
        {
            _storeService = storeService;
            _productService = productService;
            _logger = logger;
        }

        [HttpGet("")]
        public IActionResult Get()
        {
            return Ok(_storeService.Get());
        }

        [HttpGet("{id}")]
        public IActionResult Get(Guid Id)
        {
            return Ok(_storeService.GetStoreIncludeProducts(Id));
        }

        [HttpPost("")]
        public IActionResult Post(StoreModel model)
        {
            var store = new Store
            {
                Name = model.Name,
                Location = new Domain.ValueObjects.Address(model.Location.Street, model.Location.City, model.Location.ZipCode),
                OpenedTime = model.OpenedTime,
                ClosedTime = model.ClosedTime,
            };
            _storeService.Add(store);
            return Ok(model);
        }

        [HttpPost("{storeId}/products")]
        public IActionResult AddProduct(Guid storeId, ProductInStore model)
        {
            var store = _storeService.GetStoreIncludeProducts(storeId);

            var product = _productService.Get().FirstOrDefault(x => x.Code == model.Code);

            var productInStore = store.Products.FirstOrDefault(x => x.ProductId == product.Id);

            if (productInStore != null)
            {
                productInStore.Quantity = model.Quantity;
            }
            else
            {
                store.Products.Add(new ProductInStore
                {
                    ProductId = product.Id,
                    Code = product.Code,
                    Quantity = model.Quantity,
                });
            }

            _storeService.Update(store);
            return Ok(model);
        }

        [HttpPost("{storeId}/orders/{orderId}/voters/{voterId}")]
        public IActionResult Vote(Guid storeId, Guid orderId, Guid voterId)
        {
            var @event = _storeService.GetStoreIncludeProducts(storeId);
            var session = @event.Products.FirstOrDefault(x => x.Id == orderId);
            //session.Voters.Add(new Voter { UserId = voterId });
            _storeService.Update(@event);
            return Ok(@event);
        }

        [HttpDelete("{storeId}/orders/{orderId}/voters/{voterId}")]
        public IActionResult UnVote(Guid storeId, Guid orderId, Guid voterId)
        {
            var @event = _storeService.GetStoreIncludeProducts(storeId);
            var session = @event.Products.FirstOrDefault(x => x.Id == orderId);
            //var vote = session.Voters.FirstOrDefault(x => x.UserId == voterId);
            //session.Voters.Remove(vote);
            _storeService.Update(@event);
            return Ok(@event);
        }
    }
}