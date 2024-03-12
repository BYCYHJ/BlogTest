

using ChatService.Domain;
using ChatService.Domain.Entities;

namespace ChatService.WebApi.HostService
{
    public class DataHostedService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;

        public DataHostedService(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        //定时读取redis中的数据并上传至mysql做持久化处理
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            //每30s读取一次redis并且将redis数据上传至mysql
            while(!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using (var scope = _scopeFactory.CreateScope())
                    {
                        //注入chatDomainService
                        var _chatService = scope.ServiceProvider.GetRequiredService<ChatDomainService>();
                        //获取redis中的所有key
                        List<string> allKeys = (List<string>)await _chatService.GetPatternKeyAsync("*");
                        foreach (string key in allKeys)
                        {
                            //防止因为IHostedService为singleton而chatService为Scoped所造成的生命周期问题

                            //获取key中的所有数据
                            var messages = await _chatService.GetRedisMsgsAsync(key);
                            //上传到mysql
                            await _chatService.UpdateToSql(key, messages);
                            //删除key
                            await _chatService.DeleteRedisList(key);
                        }
                    }
                }catch (Exception ex)
                {
                    await Console.Out.WriteLineAsync(ex.Message);
                }
                await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);
            }
        }
    }
}
