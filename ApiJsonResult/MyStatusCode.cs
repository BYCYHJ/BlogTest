namespace ApiJsonResult
{
    public enum MyStatusCode
    {
        [Description("请求成功")]
        Success = 200,

        [Description("请求成功")]
        Success_NoContet = 204,

        [Description("请求失败")]
        BadRequest = 400,

        [Description("请求失败，无权限")]
        Unauthorized = 401,

        [Description("服务器拒绝了请求")]
        Forbidden = 403,

        [Description("未找到资源")]
        NotFound = 404,

        [Description("服务器内部错误")]
        Internal_Server_Error = 500,

        [Description("服务器不支持该请求")]
        Not_Implemented = 501,
    }
}
