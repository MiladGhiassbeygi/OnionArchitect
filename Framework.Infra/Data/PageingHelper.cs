using AutoMapper;
using AutoMapper.QueryableExtensions;
using Framework.Core.Helpers;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Framework.Infra.Data
{
    public  static class PageingHelper
    {
        public static async Task<PagedList<TSource>> CreatePagedListAsync<TSource>(IQueryable<TSource> source, int pageNumber, int pageSize, CancellationToken cancellationToken)
        {
            var count = source.Count();
            var items = await source.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync(cancellationToken);
            return new PagedList<TSource>(items, count, pageNumber, pageSize);
        }

        public static async Task<PagedList<TDestination>> CreatePagedListAsync<TSource,TDestination>(IQueryable<TSource> source, int pageNumber, int pageSize, IMapper mapper,CancellationToken cancellationToken)
        {
            var count = source.Count();
            var items = await source.Skip((pageNumber - 1) * pageSize).Take(pageSize).ProjectTo<TDestination>(mapper.ConfigurationProvider).ToListAsync(cancellationToken);
            return new PagedList<TDestination>(items, count, pageNumber, pageSize);
        }
        public static PagedList<TSource> CreatePagedList<TSource>(IQueryable<TSource> source, int pageNumber, int pageSize)
        {
            var count = source.Count();
            var items =  source.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
            return new PagedList<TSource>(items, count, pageNumber, pageSize);
        }

        public static PagedList<TDestination> CreatePagedList<TSource, TDestination>(IQueryable<TSource> source, int pageNumber, int pageSize, IMapper mapper)
        {
            var count = source.Count();
            var items =  source.Skip((pageNumber - 1) * pageSize).Take(pageSize).ProjectTo<TDestination>(mapper.ConfigurationProvider).ToList();
            return new PagedList<TDestination>(items, count, pageNumber, pageSize);
        }
    }
}
