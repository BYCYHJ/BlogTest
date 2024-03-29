﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SearchService.Domain
{
    public interface ISearchRepository
    {
        Task UpsertAsync(BlogRecord blog);
        Task DeleteAsync(Guid blogId);
        Task<SearchBlogResponse> SearchBlogAsync(string keyword,int index,int pageSize);
    }
}
