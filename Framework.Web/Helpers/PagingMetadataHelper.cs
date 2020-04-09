using Framework.Core.Helpers;
using Framework.Web.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;

namespace Framework.Web.Helpers
{
    public enum ResourceUriType
    {
        PreviousPage,
        NextPage
    }
    public class PagingMetadataHelper
    {
        private readonly IUrlHelper urlHelper;

        public PagingMetadataHelper(IUrlHelper urlHelper)
        {
            this.urlHelper = urlHelper;
        }
        protected  string CreateResourceUri(           
            string actionName,
            PaginationParameters ResourceParams,
            ResourceUriType type
            )
        {
            switch (type)
            {
                case ResourceUriType.PreviousPage:
                    return urlHelper.Link(actionName,
                      new
                      {
                          pageNumber = ResourceParams.PageNumber - 1,
                          pageSize = ResourceParams.PageSize
                      });

                case ResourceUriType.NextPage:
                    return urlHelper.Link(actionName,
                      new
                      {
                          pageNumber = ResourceParams.PageNumber + 1,
                          pageSize = ResourceParams.PageSize
                      });
                default:
                    return urlHelper.Link(actionName,
                     new
                     {
                         pageNumber = ResourceParams.PageNumber,
                         pageSize = ResourceParams.PageSize
                     });
            }

        }


        public PaginationMetadata GetPagingMetadata<T>(PagedList<T> pagedList, string actionName,PaginationParameters parameters)
        {
            var previousePageLink = pagedList.HasPrevious ?
                this.CreateResourceUri(actionName, parameters, ResourceUriType.PreviousPage) : null;

            var nextPageLink = pagedList.HasNext ?
                this.CreateResourceUri(actionName, parameters, ResourceUriType.NextPage) : null;

            var paginationMetaData = new PaginationMetadata
            {
                TotalCount = pagedList.TotalCount,
                PageSize = pagedList.PageSize,
                CurrentPage = pagedList.CurrentPage,
                TotalPages = pagedList.TotalPages,
                PreviousePageLink = previousePageLink,
                NextPageLink = nextPageLink

            };
            return paginationMetaData;
        }
    }
}
