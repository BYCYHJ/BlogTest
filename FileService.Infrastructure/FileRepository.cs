using FileService.Domain;
using FileService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileService.Infrastructure
{
    public class FileRepository : IFileRepository
    {
        private readonly FileDbContext _dbContext;

        public FileRepository(FileDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<ServerFile?> FindAsync(string hash)
        {
            return await _dbContext.Files.Where(f => f.FileSha256Hash == hash).FirstOrDefaultAsync();
        }

        public async Task<ServerFile?> FindAsync(Guid guid)
        {
            return await _dbContext.Files.Where(f => f.Id == guid).FirstOrDefaultAsync();
        }

        public async Task UploadAsync(ServerFile file)
        {
            await _dbContext.Files.AddAsync(file);
            _dbContext.SaveChanges();
        }
    }
}
