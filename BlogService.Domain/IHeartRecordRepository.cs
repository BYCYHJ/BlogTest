using BlogService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogService.Domain
{
    public interface IHeartRecordRepository
    {
        Task<HeartRecord> GetHeartRecord(string userId,string objectId,HeartType type);
    }
}
