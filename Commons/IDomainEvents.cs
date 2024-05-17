using MediatR;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogDomainCommons
{
    public interface IDomainEvents
    {
        /// <summary>
        /// 添加领域事件
        /// </summary>
        /// <param name="notification"></param>
        public void AddDomainEvents(INotification notification);

        /// <summary>
        /// 清空领域事件
        /// </summary>
        public void ClearDomainEvnets();

        /// <summary>
        /// 获取领域事件
        /// </summary>
        /// <returns></returns>
        public IEnumerable<INotification> GetDomainEvents();
    }
}
