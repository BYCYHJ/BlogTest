using ApiJsonResult;
using BlogDomainCommons;
using BlogJWT;
using BlogRabbitHelper;
using BlogService.Infrastructure;
using BlogService.WebApi.Protos;
using Grpc.Net.Client;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Reflection;
using System.Text;
using Zack.Commons;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddSingleton(typeof(IActionResultExecutor<>),typeof(ResponseJsonResultExecutor<>));//为了统一格式

//MySql
builder.Services.AddDbContext<BlogServiceDbContext>(opt =>
{
    var conStr = builder.Configuration.GetSection("MySqlConStr").Value!;
    opt.UseMySql(conStr,ServerVersion.Parse("8.0.34-mysql"));
});

//RabbitMQ
builder.Services.Configure<RabbitMqOptions>(builder.Configuration.GetSection("RabbitMqOptions"));
builder.Services.AddRabbitMqHelper(exchangerType:"fanout",queueName:"BlogService",Assembly.GetExecutingAssembly());

//显式的指定HTTP/2不需要TLS支持
AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);
AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2Support", true);
//GRPC
builder.Services.AddGrpcClient<FileAPi.FileAPiClient>(options =>
{
    options.Address = new Uri(builder.Configuration.GetSection("FileServer").Value!);
});
builder.Services.AddGrpcClient<UserApi.UserApiClient>(options =>
{
    options.Address = new Uri(builder.Configuration.GetSection("UserServer").Value!);
});

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.Configure<MvcOptions>(options =>
{
    options.Filters.Add<UnitofWorkFilter>();//添加工作单元过滤器，用于当action完成时的统一SaveChanges
    options.Filters.Add<ResponseWrapperFilter>();//添加统一格式过滤器
    options.Filters.Add<ResultExceptionFilter>();//异常格式处理过滤器
});

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

builder.Services.AddHttpClient();

if (builder.Environment.IsDevelopment())
{
    #region Swagger授权按钮
    builder.Services.AddSwaggerGen(options =>
    {
        var scheme = new OpenApiSecurityScheme()
        {
            Description = "Authorization",
            Reference = new OpenApiReference()
            {
                Type = ReferenceType.SecurityScheme,
                Id = "Authorization"
            },
            Scheme = "oauth2",
            Name = "Authorization",
            In = ParameterLocation.Header,
            Type = SecuritySchemeType.ApiKey,
        };
        options.AddSecurityDefinition("Authorization", scheme);
        var requirement = new OpenApiSecurityRequirement();
        requirement[scheme] = new List<string>();
        options.AddSecurityRequirement(requirement);
    });
    #endregion
}

//注册子程序服务
var arms = ReflectionHelper.GetAllReferencedAssemblies();
builder.Services.RunModuleInitializers(arms);

var app = builder.Build();

app.UseRabbitMqHelper();//使用rabbitmq帮助类

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

//app.UseCors();

app.UseAuthorization();

app.MapControllers();

app.Run();
