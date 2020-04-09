using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Core.Domain.Contracts.ApplicationServices;
using Core.Domain.Entities;
using Framework.Core.Helpers;
using Framework.Web.Api;
using Framework.Web.Filters;
using Framework.Web.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace OnionArchitect.Api.V1.Controllers
{
    
    public class ArticleController :BaseController
    {
        private readonly PagingMetadataHelper pagingMetadataHelper;
        private readonly IArticleServices articleServices;
        private readonly IMapper mapper;

        public ArticleController(PagingMetadataHelper pagingMetadataHelper,IArticleServices articleServices, IMapper mapper)
            : base(pagingMetadataHelper)
        {
            this.pagingMetadataHelper = pagingMetadataHelper;
            this.articleServices = articleServices;
            this.mapper = mapper;
        }

        [LogRequest]
        [HttpGet(Name=nameof(GetArticles))]
        public async Task<IActionResult> GetArticles([FromQuery]PaginationParameters paginationParameters,CancellationToken cancellationToken)
        {
            PagedList<Article> articles = await articleServices.GetPublishedArticlesAsync(cancellationToken, paginationParameters.PageNumber, paginationParameters.PageSize);

            var paginationMetadata =
                this.pagingMetadataHelper.GetPagingMetadata(articles, nameof(GetArticles), paginationParameters);

            Response.Headers.Add("X-Pagination",
                Newtonsoft.Json.JsonConvert.SerializeObject(paginationMetadata));

            return Ok(articles);
        }
    }
}