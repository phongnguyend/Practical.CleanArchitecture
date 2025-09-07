using System;

namespace ClassifiedAds.Domain.Infrastructure.Messaging;

public class ConsumerException : Exception
{
    public bool Retryable { get; set; }
}
