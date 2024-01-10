using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Nest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zack.Commons;

namespace SearchService.Infrastructure
{
    public class ModuleInitializer : IModuleInitializer
    {
        public void Initialize(IServiceCollection services)
        {
            //ElasticSesarch连接
            services.AddScoped<IElasticClient>(sp =>
            {
                var options = sp.GetRequiredService<IOptions<ElasticSearchOptions>>().Value;
                var connectionSettings = new ConnectionSettings(options.Uri);
                connectionSettings.BasicAuthentication(options.UserName, options.Password);
                connectionSettings.DefaultIndex("index");
                var esClient = new ElasticClient(connectionSettings);
                return esClient;
            });
        }
    }
}
