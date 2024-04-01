using ApiJsonResult;
using BlogJWT;
using FileService.Infrastructure;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Zack.Commons;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

//mysql
builder.Services.AddDbContext<FileDbContext>(opt =>
{
    var conStr = builder.Configuration.GetSection("MySqlConStr").Value!;
    opt.UseMySql(conStr, ServerVersion.Parse("8.0.34-mysql"));
});

//自定义返回格式
builder.Services.AddSingleton(typeof(IActionResultExecutor<>), typeof(ResponseJsonResultExecutor<>));

//JWT
builder.Services.Configure<JWTOptions>(builder.Configuration.GetSection("JWT"));
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
{
    var jwtOpt = builder.Configuration.GetSection("JWT").Get<JWTOptions>();
    byte[] bytes = Encoding.UTF8.GetBytes(jwtOpt!.Key);
    var secKey = new SymmetricSecurityKey(bytes);
    options.TokenValidationParameters = new()
    {
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateIssuerSigningKey = true,
        ValidateLifetime = true,
        IssuerSigningKey = secKey
    };
});

builder.Services.Configure<MvcOptions>(opt =>
{
    opt.Filters.Add<ResponseWrapperFilter>();//统一格式过滤器
    opt.Filters.Add<ResultExceptionFilter>();//异常处理过滤器
});

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//注册子程序服务
var arms = ReflectionHelper.GetAllReferencedAssemblies();
builder.Services.RunModuleInitializers(arms);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
