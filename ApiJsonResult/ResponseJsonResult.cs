using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace ApiJsonResult
{
    public class ResponseJsonResult<T> : IActionResult
    {
        public MyStatusCode StatusCode {  get; set; } = MyStatusCode.Success;
        private string? msg;
        public string? Message { get 
            {
                return msg == null ? EnumHelper.GetDescription(StatusCode) : msg;
            }
            set
            {
                msg = value;
            } 
        }
        public T? Data { get; set; }

        public ResponseJsonResult() { }

        public ResponseJsonResult(T? data) {
            this.Data = data;
        }

        /// <summary>
        /// 隐式转换 T => ResponseJsonResult
        /// </summary>
        /// <param name="data"></param>
        public static implicit operator ResponseJsonResult<T>(T data)
        {
            return new ResponseJsonResult<T>(data);
        }

        public async Task ExecuteResultAsync(ActionContext context)
        {
            var services = context.HttpContext.RequestServices;
            var executor = services.GetRequiredService<IActionResultExecutor<ResponseJsonResult<T>>>();
            await executor.ExecuteAsync(context, this);
        }
    }
}
