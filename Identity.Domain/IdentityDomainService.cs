﻿using BlogJWT;
using Identity.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using RedisHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Domain
{
    public class IdentityDomainService
    {
        private readonly IIdentityRepository _identityRepository;
        private readonly ITokenService _tokenService;
        private readonly IOptions<JWTOptions> _jwtOpt;
        private readonly IRedisService _redisCache;//redis缓存

        public IdentityDomainService(IIdentityRepository identityRepository,
            ITokenService tokenService,
            IOptions<JWTOptions> jwtOptions,
            IRedisService redisCache)
        {
            _identityRepository = identityRepository;
            _tokenService = tokenService;
            _jwtOpt = jwtOptions;
            _redisCache = redisCache;
        }

        /// <summary>
        /// 检查用户名和密码
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="pwd"></param>
        /// <returns></returns>
        public async Task<(SignInResult checkResult,string? token)> CheckUserAndPasswordAsync(string userName,string pwd)
        {
            var findUserResult = await _identityRepository.FindOneUserByNameAsync(userName);
            if(findUserResult is null)//无用户
            {
                return (SignInResult.Failed,null);
            }
            var matchPwdResult = await _identityRepository.CheckPasswordAsync(userName,pwd);
            if(matchPwdResult.Succeeded == false)//密码不匹配
            {
                return (SignInResult.Failed,null);
            }
            var token = await GetTokenAsync(findUserResult);
            return (SignInResult.Success,token);
        }

        /// <summary>
        /// 创建手机验证码并保存在缓存中
        /// </summary>
        /// <param name="phone"></param>
        /// <returns></returns>
        public async Task<(SignInResult,string? code)> CreatePhoneTokenCodeAsync(string phone)
        {
            string code =  new Random().Next(100000,999999).ToString();
            string key = $"Identity_Phone_{phone}";
            await _redisCache.SetAsync(key,code,60);//保存至数据库,过期时间为60s
            return (SignInResult.Success,code);
        }


        /// <summary>
        ///检查手机登录验证码是否正确
        /// </summary>
        /// <param name="phone"></param>
        /// <param name="tokenCode"></param>
        /// <returns></returns>
        public async Task<SignInResult> CheckPhoneAndTokenAsync(string phone, string tokenCode)
        {
            string key = $"Identity_Phone_{phone}";
            var cacheCode = await _redisCache.GetAsync(key);
            if(cacheCode != tokenCode)
            {
                return SignInResult.Failed;
            }
            return SignInResult.Success;
        }


        /// <summary>
        /// 获取登录token
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public async Task<string> GetTokenAsync(User user)
        {
            var roles = await _identityRepository.GetUserRoleAsync(user);//用户角色
            //包含了用户id、姓名和角色
            var claims = new List<Claim>();
            claims.Add(new Claim(ClaimTypes.NameIdentifier,user.Id.ToString()));
            claims.Add(new Claim(ClaimTypes.Name,user.UserName!));
            foreach(var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role,role));
            }
            string token = _tokenService.BuildToken(claims,_jwtOpt.Value);
            return token;
        }

        /// <summary>
        /// 更新用户信息
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public async Task<IdentityResult> UpdateUserInfoAsync(User user)
        {
            return await _identityRepository.UpdateUserAsync(user);
        }

        public async Task<User?> FindOneByIdAsync(string id)
        {
            return await _identityRepository.FindOneUserByIdAsync(id);
        }

        /// <summary>
        /// 获取多个用户信息
        /// </summary>
        /// <param name="userIds"></param>
        /// <returns></returns>
        public IEnumerable<(string id, string name, string? avatarUrl)> FindBulkUsers(IEnumerable<string> userIds)
        {
            return _identityRepository.FindBulkUsers(userIds);
        }
    }
}
