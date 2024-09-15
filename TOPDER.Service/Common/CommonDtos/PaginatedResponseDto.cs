using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOPDER.Service.Common.CommonDtos
{
    public class PaginatedResponseDto<T>
    {
        public List<T> Items { get; set; }
        public int PageIndex { get; set; }
        public int TotalPages { get; set; }
        public bool HasPreviousPage { get; set; }
        public bool HasNextPage { get; set; }
        public PaginatedResponseDto(List<T> items, int pageIndex, int totalPages, bool hasPreviousPage, bool hasNextPage)
        {
            Items = items;
            PageIndex = pageIndex;
            TotalPages = totalPages;
            HasPreviousPage = hasPreviousPage;
            HasNextPage = hasNextPage;
        }
    }
}
