using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlHelper
{
    [AttributeUsage(AttributeTargets.Property,AllowMultiple =false)]
    public  class MyPrimaryKeyAttribute : Attribute
    {
        public MyPrimaryKeyAttribute() { }
    }
}
