using System;

namespace ClassifiedAds.CrossCuttingConcerns.CircuitBreakers;

public interface ICircuitBreakerManager
{
    ICircuitBreaker GetCircuitBreaker(string name, TimeSpan openTime);

    void LogSuccess(ICircuitBreaker circuitBreaker);

    void LogFailure(ICircuitBreaker circuitBreaker, int maximumNumberOfFailures, TimeSpan period);
}
