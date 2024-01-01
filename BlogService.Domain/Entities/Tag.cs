using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogService.Domain.Entities
{
    public class Tag
    {
        public string TagName {  get; init; }
        public Tag(string tagName)
        {
            TagName = tagName;
        }
    }
}
