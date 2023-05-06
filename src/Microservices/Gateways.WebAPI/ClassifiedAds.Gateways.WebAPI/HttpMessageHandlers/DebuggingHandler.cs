using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace ClassifiedAds.Gateways.WebAPI.HttpMessageHandlers;

public class DebuggingHandler : DelegatingHandler
{
    public DebuggingHandler()
    {
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var response = await base.SendAsync(request, cancellationToken);
        return response;
    }
}
