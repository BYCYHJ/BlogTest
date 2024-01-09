using Microsoft.Extensions.DependencyInjection;
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
                var options = sp.GetRequiredService<ElasticSearchOptions>();
                var connectionSettings = new ConnectionSettings(options.Uri);
                connectionSettings.BasicAuthentication(options.UserName, options.Password);
                var esClient = new ElasticClient(connectionSettings);
                return esClient;
            });
        }
    }
}
