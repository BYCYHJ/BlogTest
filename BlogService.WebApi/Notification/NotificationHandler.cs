using BlogDomainCommons;
using BlogService.Domain;
using BlogService.Domain.Entities;
using BlogService.Infrastructure;
using MediatR;

namespace BlogService.WebApi.Notification
{
    public class NotificationHandler : INotificationHandler<LoveNotification>
    {
        private readonly CommentDomainService _commentDomainService;

        public NotificationHandler(CommentDomainService commentDomainService)
        {
            _commentDomainService = commentDomainService;
        }

        /// <summary>
        /// 进行 用户-喜欢 表记录创建
        /// </summary>
        /// <param name="notification"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        [UnitofWork(new Type[] { typeof(BlogServiceDbContext) })]
        public async Task Handle(LoveNotification notification, CancellationToken cancellationToken)
        {
            switch (notification.type)
            {
                case HeartType.Blog: { break; }
                case HeartType.Comment: {
                        await _commentDomainService.CreateLoveCommentRecordAsync(notification.userId,notification.objectId);
                        break; 
                    }
                default: { break; }
            }
            //发布消息，进行通知
        }
    }
}
