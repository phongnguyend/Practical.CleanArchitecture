using Azure;
using Azure.AI.Vision.ImageAnalysis;
using Microsoft.Extensions.Configuration;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ClassifiedAds.Infrastructure.AI;

public class ImageAnalysisService
{
    private readonly IConfiguration _configuration;

    public ImageAnalysisService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    private static ImageAnalysisClient CreateImageAnalysisClient(string endpoint, string key)
    {
        var client = new ImageAnalysisClient(new Uri(endpoint), new AzureKeyCredential(key));
        return client;
    }

    public async Task<ImageAnalysisResult> AnalyzeImageAsync(byte[] bytes, CancellationToken cancellationToken = default)
    {
        string key = _configuration["ImageAnalysis:AzureAIVision:ApiKey"]!;
        string endpoint = _configuration["ImageAnalysis:AzureAIVision:Endpoint"]!;

        // Create a client
        var client = CreateImageAnalysisClient(endpoint, key);

        // Creating a list that defines the features to be extracted from the image.
        VisualFeatures features = VisualFeatures.Caption | VisualFeatures.DenseCaptions | VisualFeatures.Tags;

        // Analyze the image
        var result = await client.AnalyzeAsync(new BinaryData(bytes), visualFeatures: features, cancellationToken: cancellationToken);

        return new ImageAnalysisResult
        {
            Tags = result.Value.Tags.Values.Select(x => new Tag
            {
                Name = x.Name,
                Confidence = x.Confidence
            }).ToArray(),
            Caption = new Caption
            {
                Text = result.Value.Caption.Text,
                Confidence = result.Value.Caption.Confidence
            },
            DenseCaptions = result.Value.DenseCaptions.Values.Select(x => new Caption
            {
                Text = x.Text,
                Confidence = x.Confidence
            }).ToArray()
        };
    }
}

public class ImageAnalysisResult
{
    public Tag[] Tags { get; set; }

    public Caption Caption { get; set; }

    public Caption[] DenseCaptions { get; set; }
}

public class Tag
{
    public string Name { get; set; }

    public float Confidence { get; set; }
}

public class Caption
{
    public string Text { get; set; }

    public float Confidence { get; set; }
}
