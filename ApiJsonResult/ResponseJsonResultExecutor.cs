using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ApiJsonResult
{
    public class ResponseJsonResultExecutor<T> : IActionResultExecutor<T> where T : IActionResult
    {
        public async Task ExecuteAsync(ActionContext context, T jsonResult)
        {
            var response = context.HttpContext.Response;
            response.ContentType= "application/json;charset=utf-8";
            string json = JsonSerializer.Serialize(jsonResult);
            await response.WriteAsync(json);
        }
    }
}
