using System;

namespace ClassifiedAds.Domain.Repositories;

public interface IConcurrencyHandler<TEntity>
{
    void SetRowVersion(TEntity entity, byte[] version);

    bool IsDbUpdateConcurrencyException(Exception ex);
}
