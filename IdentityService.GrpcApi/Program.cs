using IdentityService.GrpcApi.Services;
using IdentityService.Infrastructure;
using Microsoft.EntityFrameworkCore;
using RedisHelper;
using Zack.Commons;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddGrpc();

//mysql
builder.Services.AddDbContext<IdServiceDbContext>(opt =>
{
    var conStr = builder.Configuration.GetSection("MySqlConStr").Value!;
    opt.UseMySql(conStr, ServerVersion.Parse("8.0.34-mysql"));
});

//redis
builder.Services.AddSingleton<IRedisService, RedisService>();
builder.Services.AddStackExchangeRedisCache(opt =>
{
    opt.Configuration = builder.Configuration.GetSection("Redis").Value;
    opt.InstanceName = "BlogCache_";//前缀
});//redis缓存

//注册子程序服务
var arms = ReflectionHelper.GetAllReferencedAssemblies();
builder.Services.RunModuleInitializers(arms);

var app = builder.Build();

// Configure the HTTP request pipeline.
app.MapGrpcService<UserProtoService>();
app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

app.Run();
