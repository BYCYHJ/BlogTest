using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BlogDomainCommons
{
    public class UnitofWorkFilter : IAsyncActionFilter
    {

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var result = await next();
            if(result == null)
            {
                return;
            }
            var actionDescriptor = context.ActionDescriptor as ControllerActionDescriptor;
            if(actionDescriptor == null)
            {
                return;
            }
            var types = actionDescriptor.MethodInfo.GetCustomAttribute<UnitofWorkAttribute>();
            if(types == null)
            {
                return;
            }
            foreach( var type in types.DbContextType)
            {
                //获取DbContext实例
                var dbCtx = context.HttpContext.RequestServices.GetService(type) as DbContext;
                if (dbCtx != null)
                {
                    dbCtx.SaveChanges();
                }
            }
        }
    }
}
