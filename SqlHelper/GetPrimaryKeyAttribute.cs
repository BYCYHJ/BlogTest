using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlHelper
{
    [AttributeUsage(AttributeTargets.Class,AllowMultiple =false)]
    public class GetPrimaryKeyAttribute : Attribute
    {
        public string PrimaryKeyName {  get; set; }
        public GetPrimaryKeyAttribute(string pkName) {
            PrimaryKeyName = pkName;
        }
    }
}
