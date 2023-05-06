using Grpc.Net.Client;
using System.Net.Http;

namespace ClassifiedAds.Infrastructure.Grpc;

public class ChannelFactory
{
    public static GrpcChannel Create(string address)
    {
        var channel = GrpcChannel.ForAddress(address,
            new GrpcChannelOptions
            {
                HttpClient = new HttpClient(new HttpClientHandler
                {
                    ServerCertificateCustomValidationCallback = (message, cert, chain, errors) =>
                    {
                        // TODO: verify the Certificate
                        return true;
                    },
                }),
            });

        return channel;
    }
}
