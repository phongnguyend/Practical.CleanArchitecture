using ClassifiedAds.CrossCuttingConcerns.HtmlGenerator;
using RazorLight;
using System.Threading.Tasks;

namespace ClassifiedAds.Infrastructure.HtmlGenerators
{
    public class HtmlGenerator : IHtmlGenerator
    {
        private readonly IRazorLightEngine _razorLightEngine;

        public HtmlGenerator(IRazorLightEngine razorLightEngine)
        {
            _razorLightEngine = razorLightEngine;
        }

        public Task<string> GenerateAsync(string template, object model)
        {
            return _razorLightEngine.CompileRenderAsync(template, model);
        }
    }
}
