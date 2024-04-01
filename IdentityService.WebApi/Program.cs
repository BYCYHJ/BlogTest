using BlogJWT;
using Identity.Domain;
using IdentityService.Infrastructure;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using RedisHelper;
using StackExchange.Redis;
using System.Text;
using Zack.Commons;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

//���ݿ�
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
    opt.InstanceName = "BlogCache_";//ǰ׺
});//redis����

builder.Services.AddHttpClient();


//���ŷ���
if (builder.Environment.IsProduction())
{
    //builder.Services.AddScoped<ISendMail,>();
    builder.Services.AddScoped<ISendMail, SendMockMail>();//ģ����ŷ���
}
else
{
    builder.Services.AddScoped<ISendMail, SendMockMail>();//ģ����ŷ���
    #region Swagger��Ȩ��ť
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

//JWT
builder.Services.Configure<JWTOptions>(builder.Configuration.GetSection("JWT"));//JWT����
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
   .AddJwtBearer(options =>
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

//CORS
//builder.Services.AddCors(opt =>
//{
//    opt.AddDefaultPolicy(builder =>
//    {
//        string[] method = { "GET", "POST", "PATCH", "PUT", "DELETE" };
//        builder.WithOrigins("*").AllowCredentials().AllowAnyOrigin().WithMethods(method);
//    });
//});

var arms = ReflectionHelper.GetAllReferencedAssemblies();//���г���
builder.Services.RunModuleInitializers(arms);//ע�����


builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


//app.UseCors();//Cors

app.UseHttpsRedirection();

app.UseAuthentication();//token��֤
app.UseAuthorization();

app.MapControllers();

app.Run();
