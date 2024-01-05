using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ApiJsonResult
{
    /// <summary>
    /// 当程序出现异常时，将异常信息包装为ResponseJsonResult返回
    /// </summary>
    public class ResultExceptionFilter : ExceptionFilterAttribute
    {
        public override void OnException(ExceptionContext context)
        {
            Exception exception = context.Exception;
            var responseResult = new ResponseJsonResult<object>();
            responseResult.Data = exception.Message;
            responseResult.StatusCode = MyStatusCode.Internal_Server_Error;
            context.Result = new ContentResult
            {
                // 返回状态码设置为200，表示成功
                StatusCode = StatusCodes.Status200OK,
                // 设置返回格式
                ContentType = "application/json;charset=utf-8",
                Content = JsonSerializer.Serialize(responseResult)
            };
            context.ExceptionHandled = true;
        }
    }
}
