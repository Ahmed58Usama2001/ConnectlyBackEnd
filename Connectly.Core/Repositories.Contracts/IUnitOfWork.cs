﻿
namespace Connectly.Core.Repositories.Contracts;

public interface IUnitOfWork: IAsyncDisposable
{
    IGenericRepository<TEntity> Repository<TEntity>() where TEntity : BaseEntity;

    Task<int> CompleteAsync();
}

