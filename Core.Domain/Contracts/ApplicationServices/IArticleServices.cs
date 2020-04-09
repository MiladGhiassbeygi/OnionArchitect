using Core.Domain.Entities;
using Framework.Core.Helpers;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Core.Domain.Contracts.ApplicationServices
{
    public interface IArticleServices
    {
         Task<bool> Exist(long id, CancellationToken cancellationToken);
         Task<Article> FindAsync(long id,CancellationToken cancellationToken);
         Task<PagedList<Article>> GetPublishedArticlesAsync(CancellationToken cancellationToken,int pageNumber=1,int pageSize=int.MaxValue);
         Task<Result> RegisterArticlesAsync(Article article, CancellationToken cancellationToken);
         Task<Result> PublishArticleAsync(long articleId, CancellationToken cancellationToken);
        
    }
}
