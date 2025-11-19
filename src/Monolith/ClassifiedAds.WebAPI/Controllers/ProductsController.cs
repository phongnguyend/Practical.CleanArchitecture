using ClassifiedAds.Application;
using ClassifiedAds.Application.AuditLogEntries.DTOs;
using ClassifiedAds.Application.AuditLogEntries.Queries;
using ClassifiedAds.Application.Products.Commands;
using ClassifiedAds.Application.Products.DTOs;
using ClassifiedAds.Application.Products.Queries;
using ClassifiedAds.CrossCuttingConcerns.Csv;
using ClassifiedAds.CrossCuttingConcerns.Pdf;
using ClassifiedAds.Domain.Entities;
using ClassifiedAds.Domain.Repositories;
using ClassifiedAds.Infrastructure.AI;
using ClassifiedAds.WebAPI.Authorization;
using ClassifiedAds.WebAPI.Models.Products;
using ClassifiedAds.WebAPI.RateLimiterPolicies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Data.SqlTypes;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Text.Json;
using System.Threading.Tasks;

namespace ClassifiedAds.WebAPI.Controllers;

[EnableRateLimiting(RateLimiterPolicyNames.DefaultPolicy)]
[Authorize]
[Produces("application/json")]
[Route("api/[controller]")]
[ApiController]
public class ProductsController : ControllerBase
{
    private readonly Dispatcher _dispatcher;
    private readonly ILogger _logger;
    private readonly IPdfWriter<ExportProductsToPdf> _pdfWriter;
    private readonly ICsvWriter<ExportProductsToCsv> _productCsvWriter;
    private readonly ICsvReader<ImportProductsFromCsv> _productCsvReader;
    private readonly IRepository<ProductEmbedding, Guid> _productEmbeddingRepository;
    private readonly EmbeddingService _embeddingService;

    public ProductsController(Dispatcher dispatcher,
        ILogger<ProductsController> logger,
        IPdfWriter<ExportProductsToPdf> pdfWriter,
        ICsvWriter<ExportProductsToCsv> productCsvWriter,
        ICsvReader<ImportProductsFromCsv> productCsvReader,
        IRepository<ProductEmbedding, Guid> productEmbeddingRepository,
        EmbeddingService embeddingService)
    {
        _dispatcher = dispatcher;
        _logger = logger;
        _pdfWriter = pdfWriter;
        _productCsvWriter = productCsvWriter;
        _productCsvReader = productCsvReader;
        _productEmbeddingRepository = productEmbeddingRepository;
        _embeddingService = embeddingService;
    }

    [Authorize(Permissions.GetProducts)]
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Product>>> Get()
    {
        _logger.LogInformation("Getting all products");
        var products = await _dispatcher.DispatchAsync(new GetProductsQuery());
        var model = products.ToModels();
        return Ok(model);
    }

    [Authorize(Permissions.GetProducts)]
    [HttpGet("vectorsearch")]
    public async Task<ActionResult<IEnumerable<ProductModel>>> VectorSearch(string searchText)
    {
        var embeddingRs = await _embeddingService.GenerateAsync(searchText);
        var embedding = new SqlVector<float>(embeddingRs.Vector);

        var products = _productEmbeddingRepository.GetQueryableSet()
                .OrderBy(x => EF.Functions.VectorDistance("cosine", x.Embedding, embedding))
                .Take(5)
                .Select(x => new ProductModel
                {
                    Id = x.Product.Id,
                    Code = x.Product.Code,
                    Name = x.Product.Name,
                    Description = x.Product.Description,
                    SimilarityScore = EF.Functions.VectorDistance("cosine", x.Embedding, embedding)
                }).ToList();

        return Ok(products);
    }

    [Authorize(Permissions.GetProduct)]
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ProductModel>> Get(Guid id)
    {
        var product = await _dispatcher.DispatchAsync(new GetProductQuery { Id = id, ThrowNotFoundIfNull = true });
        var model = product.ToModel();

        var embedding = _productEmbeddingRepository.GetQueryableSet().Where(x => x.ProductId == id)
            .Select(x => new
            {
                x.Text,
                x.Embedding,
                x.TokenDetails,
                x.CreatedDateTime,
                x.UpdatedDateTime,
            })
            .FirstOrDefault();

        if (embedding != null)
        {
            model.ProductEmbedding = new ProductEmbeddingModel
            {
                Text = embedding.Text,
                Embedding = JsonSerializer.Serialize(embedding.Embedding.Memory),
                TokenDetails = embedding.TokenDetails,
                CreatedDateTime = embedding.CreatedDateTime,
                UpdatedDateTime = embedding.UpdatedDateTime,
            };

            var similarProducts = _productEmbeddingRepository.GetQueryableSet()
                .Where(x => x.ProductId != id)
                .OrderBy(x => EF.Functions.VectorDistance("cosine", x.Embedding, embedding.Embedding))
                .Take(5)
                .Select(x => new SimilarProductModel
                {
                    Id = x.Product.Id,
                    Code = x.Product.Code,
                    Name = x.Product.Name,
                    Description = x.Product.Description,
                    SimilarityScore = EF.Functions.VectorDistance("cosine", x.Embedding, embedding.Embedding)
                }).ToList();

            model.SimilarProducts = similarProducts;
        }

        return Ok(model);
    }

    [Authorize(Permissions.AddProduct)]
    [HttpPost]
    [Consumes("application/json")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public async Task<ActionResult<Product>> Post([FromBody] ProductModel model)
    {
        var product = model.ToEntity();
        await _dispatcher.DispatchAsync(new AddUpdateProductCommand { Product = product });
        model = product.ToModel();
        return Created($"/api/products/{model.Id}", model);
    }

    [Authorize(Permissions.UpdateProduct)]
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

    [Authorize(Permissions.DeleteProduct)]
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> Delete(Guid id)
    {
        var product = await _dispatcher.DispatchAsync(new GetProductQuery { Id = id, ThrowNotFoundIfNull = true });

        await _dispatcher.DispatchAsync(new DeleteProductCommand { Product = product });

        return Ok();
    }

    [Authorize(Permissions.GetProductAuditLogs)]
    [HttpGet("{id}/auditlogs")]
    public async Task<ActionResult<IEnumerable<AuditLogEntryDTO>>> GetAuditLogs(Guid id)
    {
        var logs = await _dispatcher.DispatchAsync(new GetAuditEntriesQuery { ObjectId = id.ToString() });

        List<dynamic> entries = new List<dynamic>();
        ProductDTO previous = null;
        foreach (var log in logs.OrderBy(x => x.CreatedDateTime))
        {
            var data = JsonSerializer.Deserialize<ProductDTO>(log.Log);
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

    [HttpGet("exportaspdf")]
    public async Task<IActionResult> ExportAsPdf()
    {
        var products = await _dispatcher.DispatchAsync(new GetProductsQuery());
        var bytes = await _pdfWriter.GetBytesAsync(new ExportProductsToPdf { Products = products });
        return File(bytes, MediaTypeNames.Application.Octet, "Products.pdf");
    }

    [HttpGet("exportascsv")]
    public async Task<IActionResult> ExportAsCsv()
    {
        var products = await _dispatcher.DispatchAsync(new GetProductsQuery());
        using var stream = new MemoryStream();
        await _productCsvWriter.WriteAsync(new ExportProductsToCsv { Products = products }, stream);
        return File(stream.ToArray(), MediaTypeNames.Application.Octet, "Products.csv");
    }

    [HttpPost("importcsv")]
    public async Task<IActionResult> ImportCsv([FromForm] UploadFileModel model)
    {
        using var stream = model.FormFile.OpenReadStream();
        var result = await _productCsvReader.ReadAsync(stream);

        // TODO: import to database
        return Ok(result.Products);
    }
}