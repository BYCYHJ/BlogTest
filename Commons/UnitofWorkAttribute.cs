using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogDomainCommons
{
    [AttributeUsage(AttributeTargets.Method)]
    public class UnitofWorkAttribute :Attribute
    {
        public Type[] DbContextType { get; init; }
        public UnitofWorkAttribute(Type[] dbContextTypes) { 
            DbContextType = dbContextTypes;
        }
    }
}
