using System;
using System.Collections.Generic;
using System.Text;

namespace Framework.Core.Helpers
{
    public class PaginationParameters
    {
        const int maxPageSize = int.MaxValue;
        public int PageNumber { get; set; } = 1;

        protected int _pageSize = maxPageSize;
        public int PageSize
        {
            get
            {
                return _pageSize;
            }
            set
            {
                _pageSize = (value > maxPageSize) ? maxPageSize : value;
            }
        }
        //public bool NeedTotalCount { get; set; } = false;
    }
}
