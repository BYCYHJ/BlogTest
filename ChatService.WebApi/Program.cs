using ChatService.Infrastructure;
using ChatService.WebApi.HostService;
using ChatService.WebApi.Hubs;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using RedisHelper;
using Zack.Commons;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

//Redis
var redisConStr = builder.Configuration.GetSection("Redis").Value;
builder.Services.AddRedisListHelper(redisConStr!,1);//添加redisListHelper，arg1:服务器地址,arg2:db库(默认为0)

//MySql
builder.Services.AddDbContext<ChatServiceDbContext>(opt =>
{
    var conStr = builder.Configuration.GetSection("MySqlConStr").Value!;
    opt.UseMySql(conStr, ServerVersion.Parse("8.0.34-mysql"));
});

//SignalR
builder.Services.AddSignalR();
builder.Services.AddSingleton<ChatUserIdProvider>();//添加自定义userId映射

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddHostedService<DataHostedService>();//托管服务

//注册子程序服务
var arms = ReflectionHelper.GetAllReferencedAssemblies();
builder.Services.RunModuleInitializers(arms);

var app = builder.Build();

//添加Hub
app.MapHub<MessageHub>("/messageHub");

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
