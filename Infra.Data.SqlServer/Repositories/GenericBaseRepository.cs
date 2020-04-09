using AutoMapper;
using Core.Domain.Abstractions.Entities;
using Core.Domain.Contracts.Repositories;
using Framework.Core.Helpers;
using Framework.Infra.Data;
using Infra.Data.SqlServer.DbContexts;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Infra.Data.SqlServer.Repositories
{
    public class GenericBaseRepository<TEntity> : BaseRepository<TEntity>,IGenericRepository<TEntity>
        where TEntity : class, IEntity
    {


        public GenericBaseRepository(OnionArchitectDbContext dbContext) : base(dbContext)
        {

        }

        #region Async Method
        public async Task<bool> ExistAsync(CancellationToken cancellationToken, params object[] ids)
        {
            TEntity entity = await dbSet.FindAsync(ids);
            return entity != default(TEntity);
        }
        public async virtual Task<TEntity> GetByIdAsync(CancellationToken cancellationToken, params object[] ids)
        {
            return await dbSet.FindAsync(ids, cancellationToken);
        }

        public async Task<PagedList<TEntity>> GetByConditionAsync(Expression<Func<TEntity, bool>> query, CancellationToken cancellationToken, int pageNumber = 1, int pageSize = int.MaxValue)
        {
            var result = dbSet.Where(query);
            return await PageingHelper.CreatePagedListAsync<TEntity>(result, pageNumber, pageSize, cancellationToken);
        }
        public async Task<PagedList<TEntity>> GetAsync(CancellationToken cancellationToken, int pageNumber = 1, int pageSize = int.MaxValue)
        {
            var result = dbSet;
            return await PageingHelper.CreatePagedListAsync<TEntity>(result, pageNumber, pageSize, cancellationToken);
        }
        public virtual async Task AddAsync(TEntity entity, CancellationToken cancellationToken)
        {
            await dbSet.AddAsync(entity, cancellationToken);

        }
        public virtual async Task AddRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken)
        {

            await dbSet.AddRangeAsync(entities, cancellationToken).ConfigureAwait(false);

        }
        #endregion

        #region Sync Methods
        public bool Exist(params object[] ids)
        {
            TEntity entity = dbSet.Find(ids);
            return entity != default(TEntity);
        }
        public virtual TEntity GetById(params object[] ids)
        {
            return dbSet.Find(ids);
        }
        public PagedList<TEntity> Get(int pageNumber = 1, int pageSize = int.MaxValue)
        {
            var result = dbSet;
            return  PageingHelper.CreatePagedList<TEntity>(result, pageNumber, pageSize);
        }
        public PagedList<TEntity> GetByCondition(Expression<Func<TEntity, bool>> query, int pageNumber = 1, int pageSize = int.MaxValue)
        {
            var result = dbSet.Where(query);
            return PageingHelper.CreatePagedList<TEntity>(result, pageNumber, pageSize);
        }
        public virtual void Add(TEntity entity)
        {

            dbSet.Add(entity);

        }

        public virtual void AddRange(IEnumerable<TEntity> entities)
        {

            dbSet.AddRange(entities);

        }

        public virtual void Update(TEntity entity)
        {

            dbSet.Update(entity);

        }

        public virtual void UpdateRange(IEnumerable<TEntity> entities)
        {

            dbSet.UpdateRange(entities);

        }

        public virtual void Delete(TEntity entity)
        {

            dbSet.Remove(entity);

        }

        public virtual void DeleteRange(IEnumerable<TEntity> entities)
        {

            dbSet.RemoveRange(entities);

        }
        #endregion

        #region Attach & Detach
        public virtual void Detach(TEntity entity)
        {

            var entry = dbContext.Entry(entity);

        }

        public virtual void Attach(TEntity entity)
        {

            if (dbContext.Entry(entity).State == EntityState.Detached)
                dbSet.Attach(entity);
        }
        #endregion

        #region Explicit Loading
        public virtual async Task LoadCollectionAsync<TProperty>(TEntity entity, Expression<Func<TEntity, IEnumerable<TProperty>>> collectionProperty, CancellationToken cancellationToken)
            where TProperty : class
        {
            Attach(entity);

            var collection = dbContext.Entry(entity).Collection(collectionProperty);
            if (!collection.IsLoaded)
                await collection.LoadAsync(cancellationToken).ConfigureAwait(false);
        }

        public virtual void LoadCollection<TProperty>(TEntity entity, Expression<Func<TEntity, IEnumerable<TProperty>>> collectionProperty)
            where TProperty : class
        {
            Attach(entity);
            var collection = dbContext.Entry(entity).Collection(collectionProperty);
            if (!collection.IsLoaded)
                collection.Load();
        }

        public virtual async Task LoadReferenceAsync<TProperty>(TEntity entity, Expression<Func<TEntity, TProperty>> referenceProperty, CancellationToken cancellationToken)
            where TProperty : class
        {
            Attach(entity);
            var reference = dbContext.Entry(entity).Reference(referenceProperty);
            if (!reference.IsLoaded)
                await reference.LoadAsync(cancellationToken).ConfigureAwait(false);
        }

        public virtual void LoadReference<TProperty>(TEntity entity, Expression<Func<TEntity, TProperty>> referenceProperty)
            where TProperty : class
        {
            Attach(entity);
            var reference = dbContext.Entry(entity).Reference(referenceProperty);
            if (!reference.IsLoaded)
                reference.Load();
        }

       

      
        #endregion

    }
}
