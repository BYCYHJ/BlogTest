using Microsoft.AspNetCore.Mvc;
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
                return msg;
            }
            set
            {
                msg = value == null || value == String.Empty ? EnumHelper.GetDescription(StatusCode) : value;
            } 
        }
        public T? Data { get; set; }

        public ResponseJsonResult() { }

        public ResponseJsonResult(T? data) {
            this.Data = data;
        }

        /// <summary>
        /// 隐式转换
        /// </summary>
        /// <param name="data"></param>
        public static implicit operator ResponseJsonResult<T>(T data)
        {
            return new ResponseJsonResult<T>(data);
        }

        public Task ExecuteResultAsync(ActionContext context)
        {
            throw new NotImplementedException();
        }
    }
}
