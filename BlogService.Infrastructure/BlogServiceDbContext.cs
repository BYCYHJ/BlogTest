using BlogDomainCommons;
using BlogService.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BlogService.Infrastructure
{
    public class BlogServiceDbContext : DbContext
    {
        public DbSet<Blog> Blogs { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<HeartRecord> Hearts { get; set; }

        private readonly IMediator _mediator;

        //efcore使用
        internal BlogServiceDbContext(DbContextOptions options) : base(options)
        {
        }
        public BlogServiceDbContext(DbContextOptions options,IMediator mediator) : base(options) 
        {
            _mediator = mediator;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(this.GetType().Assembly);
        }


        /// <summary>
        /// 重写的SaveChangesAsync,包含了对于领域事件的发布
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns>数据库操作影响的行数</returns>
        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            //获取领域事件不为空的实体集合
            var domainEntities = this.ChangeTracker.Entries<IDomainEvents>()
                .Where(entry => entry.Entity.GetDomainEvents().Any());
            List<INotification> events = domainEntities.SelectMany(entry => entry.Entity.GetDomainEvents()).ToList();
            //操作保存数据库
            int result = await base.SaveChangesAsync(cancellationToken);
            //清空领域事件
            domainEntities.ToList().ForEach(e => { e.Entity.ClearDomainEvnets(); });
            //发布领域事件
            foreach (var item in events)
            {
                await _mediator.Publish(item,cancellationToken);
            }
            return result;
        }
    }
}
