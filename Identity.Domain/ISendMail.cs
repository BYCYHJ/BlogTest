using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Domain
{
    public interface ISendMail
    {
        /// <summary>
        /// 发送短信
        /// </summary>
        /// <param name="phone"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        Task SendSmsMail(string phone,string[] args);
    }
}
