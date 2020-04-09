using Core.Domain.Contracts.Repositories;
using Core.Domain.Contracts.Repositoriess;
using Infra.Data.SqlServer.DbContexts;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Infra.Data.SqlServer.Repositories
{
    public class UnitOfWork :IUnitOfWork
    {
        private readonly OnionArchitectDbContext _dbContext;

        public UnitOfWork(OnionArchitectDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        //=============================================================
        private IArticleRepository _articleRepository;
        public IArticleRepository ArticleRepository
        {
            get
            {
                return _articleRepository = _articleRepository ?? new ArticleRepository(_dbContext);
            }
        }
        //=============================================================
        public void Commit()
        {
            _dbContext.SaveChanges();
        }

        private bool disposed = false;
        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    _dbContext.Dispose();
                }
            }
            this.disposed = true;
        }
        public Task<int> CommitAsync(CancellationToken cancellationToken)
        {
            return _dbContext.SaveChangesAsync(cancellationToken);
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        public Task CommitAsync()
        {
            return _dbContext.SaveChangesAsync();
        }

    }

}
