using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace ArcaiProject.Business.Helpers
{
    // Bu, veriyi tutan ana sınıf
    public class PagedList<T> : List<T>
    {
        public PagingMetadata Metadata { get; }

        public PagedList(List<T> items, int totalCount, int pageNumber, int pageSize)
        {
            Metadata = new PagingMetadata
            {
                TotalCount = totalCount,
                PageSize = pageSize,
                CurrentPage = pageNumber,
                TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
            };
            AddRange(items);
        }

        // Bu, sorguyu oluşturmak için kullanılacak helper metot
        public static async Task<PagedList<T>> CreateAsync(IQueryable<T> source, int pageNumber, int pageSize)
        {
            var totalCount = await source.CountAsync();
            var items = await source
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
            
            return new PagedList<T>(items, totalCount, pageNumber, pageSize);
        }
    }

    // Bu, meta veriyi tutan alt sınıf
    public class PagingMetadata
    {
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        
        public bool HasPrevious => CurrentPage > 1;
        public bool HasNext => CurrentPage < TotalPages;
    }
}

