using BlogService.Domain;
using BlogService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogService.Infrastructure
{
    public class HeartRecordRepository : IHeartRecordRepository
    {
        private readonly BlogServiceDbContext _dbContext;

        public HeartRecordRepository(BlogServiceDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<HeartRecord> GetHeartRecord(string userId, string objectId, HeartType type)
        {
            HeartRecord record = new HeartRecord(userId, objectId, type);
            string sql = @$"
INSERT INTO blog_heart_record 
VALUES ('{userId}','{objectId}','{type.ToString()}','{record.CreateOnTime.ToString("yyyy-MM-dd HH:mm:ss")}',0)            
            ";
            await _dbContext.Database.ExecuteSqlRawAsync(sql);
            return record;
        }
    }
}
