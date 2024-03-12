using ChatService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatService.Infrastructure
{
    public class ChatServiceDbContext : DbContext
    {
        public DbSet<Message> Messages { get; set; }

        //以Message作为模型类,创建多个对话表DbSet
        //private readonly Dictionary<string, DbSet<Message>> ChatDbSets = new Dictionary<string, DbSet<Message>>();
        //动态创建DbSet
        //public DbSet<Message> GetDbSet(string tableName)
        //{
        //    //没有则添加后返回
        //    if (!ChatDbSets.ContainsKey(tableName)) {
        //        ChatDbSets[tableName] = Set<Message>();
        //    }
        //    return ChatDbSets[tableName];
        //}
        public ChatServiceDbContext(DbContextOptions<ChatServiceDbContext> options) : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(this.GetType().Assembly);
        }
    }
}
