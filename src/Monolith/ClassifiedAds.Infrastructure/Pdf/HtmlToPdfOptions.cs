namespace ClassifiedAds.Infrastructure.Pdf;

public class HtmlToPdfOptions
{
    public string BaseUrl { get; set; } = "http://localhost:8130/playwright/";

    public int TimeoutSeconds { get; set; } = 120;
}
