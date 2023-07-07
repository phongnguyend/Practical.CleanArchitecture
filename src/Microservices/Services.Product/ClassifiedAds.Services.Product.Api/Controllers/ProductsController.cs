using ClassifiedAds.CrossCuttingConcerns.Csv;
using ClassifiedAds.Services.Product.Models;
using ClassifiedAds.Services.Product.RateLimiterPolicies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace ClassifiedAds.Services.Product.Controllers;

[EnableRateLimiting(RateLimiterPolicyNames.DefaultPolicy)]
[Authorize]
[Produces("application/json")]
[Route("api/[controller]")]
[ApiController]
public class ProductsController : ControllerBase
{
    private readonly ICsvReader<ProductModel> _productCsvReader;

    public ProductsController(
        ICsvReader<ProductModel> productCsvReader)
    {
        _productCsvReader = productCsvReader;
    }

    [HttpPost("importcsv")]
    public IActionResult ImportCsv([FromForm] UploadFileModel model)
    {
        using var stream = model.FormFile.OpenReadStream();
        var products = _productCsvReader.Read(stream);

        // TODO: import to database
        return Ok(products);
    }
}