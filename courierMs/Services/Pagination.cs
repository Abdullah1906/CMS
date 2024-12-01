using System;
using Microsoft.EntityFrameworkCore;

namespace courierMs.Services
{
    public class Pagination<T> where T:class
    {
        public int PageIndex { get; private set; }
        public int TotalPages { get; private set; }
        public List<T> Items { get; private set; }
        public Pagination(List<T> items, int count, int pageIndex, int pageSize)
        {
            PageIndex = pageIndex;
            TotalPages = (int)Math.Ceiling(count / (double)pageSize);
            Items = items;
        }

        public bool HasPreviousPage => PageIndex > 1;
        public bool HasNextPage => PageIndex < TotalPages;

        public static async Task<Pagination<T>> CreateAsync(IQueryable<T> source,int pageIndex,int pageSize)
        {
            var count = await source.CountAsync();
            var items = await source.Skip((pageIndex-1)*pageSize).Take(pageSize).ToListAsync();
            return new Pagination<T>(items,count,pageIndex, pageSize);
        }
    }
}
