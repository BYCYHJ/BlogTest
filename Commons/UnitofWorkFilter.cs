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
    /// <summary>
    /// 保持一致性，如果执行失败不进行结果更新
    /// </summary>
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
                    try
                    {
                        await dbCtx.SaveChangesAsync();
                    }
                    //捕获并发更新冲突
                    catch(DbUpdateConcurrencyException ex)
                    {
                        //检查受到影响的实体
                        foreach(var entry in ex.Entries)
                        {
                            //重新加载尝试重新更新
                            entry.Reload();
                        }
                        await dbCtx.SaveChangesAsync();//保存结果,进行更新
                    }
                }
            }
        }
    }
}
