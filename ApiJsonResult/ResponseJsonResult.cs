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

        //快速result设置
        private static readonly ResponseJsonResult<T> _failed = new ResponseJsonResult<T>() { StatusCode = MyStatusCode.BadRequest};
        private static readonly ResponseJsonResult<T> _success = new ResponseJsonResult<T>() { StatusCode=MyStatusCode.Success};
        public static ResponseJsonResult<T> Failed => _failed;
        public static ResponseJsonResult<T> Succeeded => _success;

        public ResponseJsonResult() { }

        public ResponseJsonResult(string? msg = null)
        {
            this.Message = msg;
        }

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

        //获取result执行者，以重写的Executor方法运行s
        public async Task ExecuteResultAsync(ActionContext context)
        {
            var services = context.HttpContext.RequestServices;
            var executor = services.GetRequiredService<IActionResultExecutor<ResponseJsonResult<T>>>();
            await executor.ExecuteAsync(context, this);
        }
    }
}
