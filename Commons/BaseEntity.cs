using MassTransit;
using MediatR;
using System.ComponentModel.DataAnnotations.Schema;

namespace BlogDomainCommons
{
    public class BaseEntity : IDomainEvents
    {
        public Guid Id { get; init; } //主键Id
        public DateTime CreateOnTime { get; init; } //创建时间
        public DateTime UpdateTime {  get; private set; } //修改时间
        [NotMapped]
        private List<INotification> _domainEvents = new();//领域事件


        public BaseEntity() {
            this.Id = NewId.NextGuid();//使用有序Guid，方便索引
            this.CreateOnTime = DateTime.Now;
        }

        //修改更新时间
        public void ChangeUpdateTime()
        {
            this.UpdateTime = DateTime.Now;
        }

        //添加领域事件
        public void AddDomainEvents(INotification notification)
        {
            this._domainEvents.Add(notification);
        }

        //清空领域事件
        public void ClearDomainEvnets()
        {
            this._domainEvents.Clear();
        }

        //获取领域事件
        public IEnumerable<INotification> GetDomainEvents()
        {
            return this._domainEvents;
        }

    }
}
