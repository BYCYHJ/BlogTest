using ChatService.Domain;
using ChatService.Infrastructure;
using ChatService.WebApi.HostService;
using ChatService.WebApi.Hubs;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using RedisHelper;
using System.Reflection;
using Zack.Commons;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

//Redis
var redisConStr = builder.Configuration.GetSection("Redis").Value;
builder.Services.AddRedisListHelper(redisConStr!,1);//���redisListHelper��arg1:��������ַ,arg2:db��(Ĭ��Ϊ0)

//MySql
builder.Services.AddDbContext<ChatServiceDbContext>(opt =>
{
    var conStr = builder.Configuration.GetSection("MySqlConStr").Value!;
    opt.UseMySql(conStr, ServerVersion.Parse("8.0.34-mysql"));
});

//SignalR
builder.Services.AddSignalR(hubOpt =>
{
    hubOpt.ClientTimeoutInterval = TimeSpan.FromSeconds(120);//�ͻ���120sδ������Ϣ����ʱ�Ͽ�����
});
//builder.Services.AddSingleton<IUserIdProvider,ChatUserIdProvider>();//����Զ���userIdӳ��

//���MediatR
//builder.Services.AddMediatR(configuration => configuration.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

//����ڴ滺��
builder.Services.AddMemoryCache();

//ӳ����
builder.Services.AddSingleton(typeof(ConnectionMapping<>));

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddHostedService<DataHostedService>();//�йܷ���

//ע���ӳ������
var arms = ReflectionHelper.GetAllReferencedAssemblies();
builder.Services.RunModuleInitializers(arms);

var app = builder.Build();

//���Hub
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
