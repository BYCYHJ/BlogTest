﻿using FileService.Domain;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zack.Commons;

namespace FileService.Infrastructure
{
    public class ModuleInitializer : IModuleInitializer
    {
        public void Initialize(IServiceCollection services)
        {
            services.AddScoped<ServerFileService>();
            services.AddScoped<IFileRepository,FileRepository>();
            services.AddScoped<IFileStorage,FileStorage>();
        }
    }
}
