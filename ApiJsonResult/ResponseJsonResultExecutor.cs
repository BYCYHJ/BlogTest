using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiJsonResult
{
    public class ResponseJsonResultExecutor<T> : IActionResultExecutor<T> where T : ResponseJsonResult<object>
    {
        public Task ExecuteAsync(ActionContext context, T jsonResult)
        {
            var response = context.HttpContext.Response;
            if()
        }
    }
}
