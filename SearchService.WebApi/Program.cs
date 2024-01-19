using ApiJsonResult;
using BlogJWT;
using BlogRabbitHelper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using SearchService.Domain;
using SearchService.Infrastructure;
using System.Reflection;
using System.Text;
using Zack.Commons;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

//blog搜索仓储
builder.Services.AddScoped<ISearchRepository,SearchBlogRepository>();

//ResponseJsonResult的专属执行器
builder.Services.AddSingleton(typeof(IActionResultExecutor<>), typeof(ResponseJsonResultExecutor<>));

//elastic search连接配置
builder.Services.Configure<ElasticSearchOptions>(builder.Configuration.GetSection("ElasticSearchOptions"));

//RabbitMQ配置
builder.Services.Configure<RabbitMqOptions>(builder.Configuration.GetSection("RabbitMqOptions"));
builder.Services.AddRabbitMqHelper(exchangerType:"fanout",queueName: "BlogService",Assembly.GetExecutingAssembly());

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.Configure<JWTOptions>(builder.Configuration.GetSection("JWT"));
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
{
    var jwtOpt = builder.Configuration.GetSection("JWT").Get<JWTOptions>();
    byte[] bytes = Encoding.UTF8.GetBytes(jwtOpt.Key);
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

//cors
//builder.Services.AddCors(opt =>
//{
//    opt.AddDefaultPolicy(builder =>
//    {
//        string[] method = { "GET", "POST", "PATCH", "PUT", "DELETE" };
//        builder.WithOrigins("*").AllowCredentials().AllowAnyHeader().WithMethods(method);
//    });
//});

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

app.UseRabbitMqHelper();//RabbitMQ帮助类

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//app.UseCors();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
