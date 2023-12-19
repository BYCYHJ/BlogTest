using Identity.Domain;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdentityService.Infrastructure
{
    public class SendMockMail : ISendMail
    {
        private readonly ILogger<SendMockMail> logger;

        public SendMockMail(ILogger<SendMockMail> logger)
        {
            this.logger = logger;
        }

        public Task SendSmsMail(string phone, string[] args)
        {
            logger.LogWarning($"phone:{args[0]},{args[1]}");
            return Task.CompletedTask;
        }
    }
}
