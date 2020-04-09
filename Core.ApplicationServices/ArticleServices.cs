using Core.Domain.Contracts.ApplicationServices;
using Core.Domain.Contracts.Repositories;
using Core.Domain.Entities;
using Framework.Core.Helpers;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Core.ApplicationServices
{
    public class ArticleServices : IArticleServices
    {
        private readonly IUnitOfWork unitOfWork;

        public ArticleServices(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }
        public async Task<Article> FindAsync(long id, CancellationToken cancellationToken)
        {
 
            Article article= await unitOfWork.ArticleRepository.GetByIdAsync(cancellationToken,id);
            return article;
        }
        public async Task<PagedList<Article>> GetPublishedArticlesAsync(CancellationToken cancellationToken, int pageNumber = 1, int pageSize = int.MaxValue)
        {
            PagedList<Article> articles = await unitOfWork.ArticleRepository.GetByConditionAsync(a=>a.Published==true,cancellationToken,pageNumber,pageSize);
            return articles;
        }
        public async Task<Result> RegisterArticlesAsync(Article article, CancellationToken cancellationToken)
        {
            await unitOfWork.ArticleRepository.AddAsync(article,cancellationToken);
            await unitOfWork.CommitAsync(cancellationToken);
            return Result.Ok();
        }
        public async Task<Result> PublishArticleAsync(long articleId, CancellationToken cancellationToken)
        {
            if (articleId == default(long))
                return Result.Fail<Article>("شناسه مقاله نامعتبر است");
            Article article =await unitOfWork.ArticleRepository.GetByIdAsync(cancellationToken,articleId);
            article.Published = true;
            unitOfWork.ArticleRepository.Update(article);
            await unitOfWork.CommitAsync(cancellationToken);
            return Result.Ok();
        }

        public Task<bool> Exist(long id, CancellationToken cancellationToken)
        {
            return unitOfWork.ArticleRepository.ExistAsync(cancellationToken, id);
        }
    }
}
