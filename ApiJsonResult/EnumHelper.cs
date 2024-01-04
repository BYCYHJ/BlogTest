using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ApiJsonResult
{
    public static class EnumHelper
    {
        public static string? GetDescription<T>(T targetEnum) where T : Enum
        {
            var fieldInfo = typeof(T).GetField(targetEnum.ToString());
            DescriptionAttribute? attr = fieldInfo!.GetCustomAttribute(typeof(DescriptionAttribute),false) as DescriptionAttribute;
            if(attr != null) {
                return attr.Description;
            }
            return null;
        }
    }
}
