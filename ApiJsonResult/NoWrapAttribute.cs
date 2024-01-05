using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiJsonResult
{
    /// <summary>
    /// 表明不需要统一返回格式的类
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class,AllowMultiple =false)]
    public class NoWrapAttribute : Attribute
    {
    }
}
