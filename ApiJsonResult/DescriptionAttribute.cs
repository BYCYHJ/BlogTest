using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiJsonResult
{
    [AttributeUsage(AttributeTargets.Field,AllowMultiple =false)]
    public class DescriptionAttribute : Attribute
    {
        public string Description { get; set; } 
        public DescriptionAttribute(string description) { 
            Description = description;
        }
    }
}
