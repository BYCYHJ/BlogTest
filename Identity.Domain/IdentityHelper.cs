using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.AspNetCore.Identity
{
    public static class IdentityHelper
    {
        /// <summary>
        /// 将错误集变为一个字符串，便于返回
        /// </summary>
        /// <param name="errors">错误集</param>
        /// <returns></returns>
        public static string SumErrors(this IEnumerable<IdentityError> errors)
        {
            var strs = errors.Select(e =>
            {
                return $"code:{e.Code},error:{e.Description}";
            });
            return string.Join("\n ", strs);
        }
    }
}
