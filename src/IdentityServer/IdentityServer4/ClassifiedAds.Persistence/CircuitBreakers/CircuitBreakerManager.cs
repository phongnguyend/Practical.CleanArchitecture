using ClassifiedAds.CrossCuttingConcerns.CircuitBreakers;
using ClassifiedAds.CrossCuttingConcerns.DateTimes;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace ClassifiedAds.Persistence.CircuitBreakers;

public class CircuitBreakerManager : ICircuitBreakerManager, IDisposable
{
    private readonly AdsDbContext _dbContext;
    private readonly IDateTimeProvider _dateTimeProvider;

    public CircuitBreakerManager(IDbContextFactory<AdsDbContext> dbContextFactory, IDateTimeProvider dateTimeProvider)
    {
        _dbContext = dbContextFactory.CreateDbContext();
        _dateTimeProvider = dateTimeProvider;
    }

    public ICircuitBreaker GetCircuitBreaker(string name, TimeSpan openTime)
    {
        CircuitBreaker GetCircuitBreakerByName()
        {
            return _dbContext.Set<CircuitBreaker>().Where(x => x.Name.Equals(name)).FirstOrDefault();
        }

        var circuitBreaker = GetCircuitBreakerByName();
        if (circuitBreaker == null)
        {
            try
            {
                circuitBreaker = new CircuitBreaker
                {
                    Name = name,
                    Status = CircuitStatus.Closed,
                    CreatedDateTime = _dateTimeProvider.Now,
                    LastStatusUpdated = _dateTimeProvider.Now,
                };

                _dbContext.Set<CircuitBreaker>().Add(circuitBreaker);
                _dbContext.SaveChanges();
            }
            catch (Exception)
            {
                circuitBreaker = GetCircuitBreakerByName();
                if (circuitBreaker == null)
                {
                    throw;
                }
            }
        }

        if (circuitBreaker.Status == CircuitStatus.Open && circuitBreaker.LastStatusUpdated + openTime <= _dateTimeProvider.Now)
        {
            circuitBreaker.Status = CircuitStatus.HalfOpen;
            circuitBreaker.LastStatusUpdated = _dateTimeProvider.Now;
            _dbContext.SaveChanges();
        }

        return circuitBreaker;
    }

    public void LogFailure(ICircuitBreaker circuitBreaker, int maximumNumberOfFailures, TimeSpan period)
    {
        _dbContext.Set<CircuitBreakerLog>().Add(new CircuitBreakerLog
        {
            CircuitBreakerId = ((CircuitBreaker)circuitBreaker).Id,
            Status = ((CircuitBreaker)circuitBreaker).Status,
            Succeeded = false,
            CreatedDateTime = _dateTimeProvider.OffsetNow,
        });

        UpdateCircuitBreakerStatus(circuitBreaker, circuitBreaker.Status == CircuitStatus.HalfOpen, CircuitStatus.Open);

        _dbContext.SaveChanges();

        if (circuitBreaker.Status == CircuitStatus.Closed)
        {
            var sinceLastTime = _dateTimeProvider.OffsetNow - period;
            var numberOfFailures = _dbContext.Set<CircuitBreakerLog>().Where(x => x.CircuitBreakerId == ((CircuitBreaker)circuitBreaker).Id
            && x.Succeeded == false && x.CreatedDateTime >= sinceLastTime).Count();

            UpdateCircuitBreakerStatus(circuitBreaker, numberOfFailures >= maximumNumberOfFailures, CircuitStatus.Open);

            _dbContext.SaveChanges();
        }
    }

    public void LogSuccess(ICircuitBreaker circuitBreaker)
    {
        _dbContext.Set<CircuitBreakerLog>().Add(new CircuitBreakerLog
        {
            CircuitBreakerId = ((CircuitBreaker)circuitBreaker).Id,
            Status = ((CircuitBreaker)circuitBreaker).Status,
            Succeeded = true,
            CreatedDateTime = _dateTimeProvider.OffsetNow,
        });

        UpdateCircuitBreakerStatus(circuitBreaker, circuitBreaker.Status == CircuitStatus.HalfOpen, CircuitStatus.Closed);

        _dbContext.SaveChanges();
    }

    private void UpdateCircuitBreakerStatus(ICircuitBreaker circuitBreaker, bool shouldUpdate, CircuitStatus circuitStatus)
    {
        if (!shouldUpdate)
        {
            return;
        }

        circuitBreaker.Status = circuitStatus;
        circuitBreaker.LastStatusUpdated = _dateTimeProvider.OffsetNow;
    }

    public void Dispose()
    {
        _dbContext.Dispose();
    }
}
