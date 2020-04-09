using Core.Domain.Abstractions.Entities;
using Framework.Core.Helpers;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Core.Domain.Contracts.Repositories
{
    
    public interface IGenericRepository<TEntity> where TEntity : class, IEntity
    {
        bool Exist(params object[] ids);
        Task<bool> ExistAsync(CancellationToken cancellationToken, params object[] ids);
        void Add(TEntity entity);
        Task AddAsync(TEntity entity, CancellationToken cancellationToken);
        void AddRange(IEnumerable<TEntity> entities);
        Task AddRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken);
        void Attach(TEntity entity);
        void Delete(TEntity entity);        
        void DeleteRange(IEnumerable<TEntity> entities);
        void Detach(TEntity entity);
        TEntity GetById(params object[] ids);
        Task<TEntity> GetByIdAsync(CancellationToken cancellationToken, params object[] ids);
        PagedList<TEntity> Get(int pageNumber= 1, int pageSize = int.MaxValue);
        Task<PagedList<TEntity>> GetAsync(CancellationToken cancellationToken,int pageNumber = 1, int pageSize = int.MaxValue);
        PagedList<TEntity> GetByCondition(Expression<Func<TEntity, bool>> query,int pageNumber = 1, int pageSize = int.MaxValue);
        Task<PagedList<TEntity>> GetByConditionAsync(Expression<Func<TEntity, bool>> query, CancellationToken cancellationToken, int pageNumber = 1, int pageSize = int.MaxValue);
        void LoadCollection<TProperty>(TEntity entity, Expression<Func<TEntity, IEnumerable<TProperty>>> collectionProperty) where TProperty : class;
        Task LoadCollectionAsync<TProperty>(TEntity entity, Expression<Func<TEntity, IEnumerable<TProperty>>> collectionProperty, CancellationToken cancellationToken) where TProperty : class;
        void LoadReference<TProperty>(TEntity entity, Expression<Func<TEntity, TProperty>> referenceProperty) where TProperty : class;
        Task LoadReferenceAsync<TProperty>(TEntity entity, Expression<Func<TEntity, TProperty>> referenceProperty, CancellationToken cancellationToken) where TProperty : class;
        void Update(TEntity entity);      
        void UpdateRange(IEnumerable<TEntity> entities);
       
    }
}
