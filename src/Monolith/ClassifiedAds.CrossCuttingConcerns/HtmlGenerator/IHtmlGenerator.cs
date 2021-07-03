using System.Threading.Tasks;

namespace ClassifiedAds.CrossCuttingConcerns.HtmlGenerator
{
    public interface IHtmlGenerator
    {
        Task<string> GenerateAsync(string template, object model);
    }
}
