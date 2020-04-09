using Core.Domain.Abstractions.Entities;
using Infra.Data.SqlServer.DbContexts;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infra.Data.SqlServer.Repositories
{
    public class BaseRepository<TEntity> where TEntity : class, IEntity
    {
        protected readonly OnionArchitectDbContext dbContext;
        public DbSet<TEntity> dbSet; 
        public BaseRepository(OnionArchitectDbContext dbContext)
        {
            this.dbContext = dbContext;
            this.dbSet = dbContext.Set<TEntity>();
        }
        
    }
}
