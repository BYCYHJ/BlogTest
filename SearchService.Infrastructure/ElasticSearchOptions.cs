using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SearchService.Infrastructure
{
    public class ElasticSearchOptions
    {
        public Uri Uri { get; set; }
        public string UserName {  get; set; }
        public string Password { get; set; }
        public ElasticSearchOptions() { }
        public ElasticSearchOptions(string uri, string userName, string password)
        {
            Uri = new Uri(uri);
            UserName = userName;
            Password = password;
        }
    }
}
