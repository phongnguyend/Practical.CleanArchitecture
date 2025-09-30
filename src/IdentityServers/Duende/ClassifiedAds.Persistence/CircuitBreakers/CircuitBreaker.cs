using ClassifiedAds.CrossCuttingConcerns.CircuitBreakers;
using System;
using System.Collections.Generic;

namespace ClassifiedAds.Persistence.CircuitBreakers;

public class CircuitBreaker : ICircuitBreaker
{
    public Guid Id { get; set; }

    public string Name { get; set; }

    public CircuitStatus Status { get; set; }

    public DateTimeOffset CreatedDateTime { get; set; }

    public DateTimeOffset LastStatusUpdated { get; set; }

    public ICollection<CircuitBreakerLog> CircuitBreakerLogs { get; set; }

    public void EnsureOkStatus()
    {
        if (Status == CircuitStatus.Open)
        {
            throw new CircuitBreakerOpenException();
        }
    }
}
