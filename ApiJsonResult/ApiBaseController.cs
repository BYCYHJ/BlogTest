using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiJsonResult
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class ApiBaseController : ControllerBase
    {
        /// <summary>
        /// 请求成功
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        protected ResponseJsonResult<T> Success<T>(T? data,string? msg = null)
        {
            var statusCode = data == null ? MyStatusCode.Success_NoContet : MyStatusCode.Success; 
            var responseResult = new ResponseJsonResult<T>(data) { 
                StatusCode= statusCode,
                Message = msg
            };
            return responseResult;
        }

        /// <summary>
        /// 请求失败
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="msg"></param>
        /// <returns></returns>
        protected ResponseJsonResult<T> BadRequest<T>(string? msg = null)
        {
            return new ResponseJsonResult<T>
            {
                StatusCode = MyStatusCode.BadRequest,
                Message = msg
            };
        }

        /// <summary>
        /// 无权限
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="msg"></param>
        /// <returns></returns>
        protected ResponseJsonResult<T> Unauthorized<T>(string? msg = null)
        {
            return new ResponseJsonResult<T>
            {
                StatusCode = MyStatusCode.Unauthorized,
                Message = msg
            };
        }

        /// <summary>
        /// 资源未找到
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="msg"></param>
        /// <returns></returns>
        protected ResponseJsonResult<T> NotFound<T>(string? msg = null)
        {
            return new ResponseJsonResult<T>
            {
                StatusCode = MyStatusCode.NotFound,
                Message = msg
            };
        }

        /// <summary>
        /// 自定义状态
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="status"></param>
        /// <param name="data"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        protected ResponseJsonResult<T> Result<T>(MyStatusCode status,T? data,string? msg = null)
        {
            return new ResponseJsonResult<T>
            {
                StatusCode = status,
                Data = data,
                Message = msg
            };
        }
    }
}
