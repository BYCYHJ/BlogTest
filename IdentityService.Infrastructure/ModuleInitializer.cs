using BlogJWT;
using Identity.Domain;
using Identity.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zack.Commons;

namespace IdentityService.Infrastructure
{
    internal class ModuleInitializer : IModuleInitializer
    {
        public void Initialize(IServiceCollection services)
        {
            services.AddScoped<IIdentityRepository, IdentityRespository>();
            services.AddScoped<IdentityDomainService>();
            services.AddScoped<ITokenService, TokenService>();

            #region EFCore.identity
            services.AddIdentityCore<User>(opt =>
            {
                opt.Password.RequiredLength = 6;
                opt.Password.RequireDigit = false;
                opt.Password.RequireLowercase = false;
                opt.Password.RequireUppercase = false;
                opt.Password.RequireNonAlphanumeric = false;
                //验证码设置
                opt.Tokens.PasswordResetTokenProvider = TokenOptions.DefaultEmailProvider;
                opt.Tokens.ChangePhoneNumberTokenProvider = TokenOptions.DefaultPhoneProvider;
            });
            var idBuilder = new IdentityBuilder(typeof(User),typeof(Role),services);
            idBuilder.AddEntityFrameworkStores<IdServiceDbContext>()
                .AddUserManager<UserManager<User>>()
                .AddRoleManager<RoleManager<Role>>();
            #endregion
        }
    }
}
