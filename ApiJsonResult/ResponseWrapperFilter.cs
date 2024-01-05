using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ApiJsonResult
{
    /// <summary>
    /// 将action返回的结果全部转换为ResponseJsonResult类型
    /// </summary>
    public class ResponseWrapperFilter : ActionFilterAttribute
    {
        public override void OnActionExecuted(ActionExecutedContext context)
        {
            //若标注了NoWrap说明不需要统一返回格式
            var descriptor = context.ActionDescriptor as ControllerActionDescriptor;
            var actionWrapAttr = descriptor?.MethodInfo.GetCustomAttributes(typeof(NoWrapAttribute), false).FirstOrDefault();
            var controllerWrapAttr = descriptor?.ControllerTypeInfo.GetCustomAttributes(typeof(NoWrapAttribute), false).FirstOrDefault();
            //若controller包含NoWrap特性或者action包含NoWrap特性，就不进行格式统一
            if (actionWrapAttr != null || controllerWrapAttr != null)
            {
                return;
            }

            //以下为返回格式统一
            var responseJsonResult = new ResponseJsonResult<object>();
            //若返回结果为object
            if (context.Result is ObjectResult)
            {
                ObjectResult? result = context.Result as ObjectResult;
                //如果result本来就是ResponseJsonResult则不进行处理
                if (result.DeclaredType == typeof(ResponseJsonResult<>))
                {
                    return;
                }
                string statusCode = result.StatusCode == null  ? "200" : result.StatusCode.ToString()!;//状态码
                responseJsonResult.StatusCode = statusCode == null ? 
                    MyStatusCode.Success : 
                    (MyStatusCode)Enum.Parse(typeof(MyStatusCode), statusCode);
                responseJsonResult.Data = result.Value;
                context.Result = responseJsonResult;
            }
            else if (context.Result is EmptyResult)//如果返回结果为空
            {
                responseJsonResult.StatusCode = MyStatusCode.Success_NoContet;
                context.Result = responseJsonResult;
            }
            return;
        }
    }
}
