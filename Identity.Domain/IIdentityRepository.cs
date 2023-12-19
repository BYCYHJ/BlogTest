using Identity.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Domain
{
    public interface IIdentityRepository
    {
        Task<User?> FindOneUserByIdAsync(string id);//根据id查找一个用户
        Task<User?> FindOneUserByNameAsync(string name);//根据用户名查找一个用户
        Task<IEnumerable<User?>> FindUsersByName(string name);//查找所有该用户名的用户
        Task<User?> FindOneByPhoneAsync(string phone);//根据手机号查找用户
        Task<List<string>> GetUserRoleAsync(User user);//获取角色

        Task<IdentityResult> CreateUserAsync(string name,string password);//创建用户，通过用户名、密码
        Task<IdentityResult> CreateUserByPhoneAsync(string phoneNumber);//创建用户，通过手机号
        Task<IdentityResult> CreateRoleAsync(string roleName);//创建角色

        /// <summary>
        /// 生成密码重置令牌
        /// </summary>
        /// <returns></returns>
        Task<string> GenerateResetPwdTokenAsync(User user);

        /// <summary>
        /// 重置密码
        /// </summary>
        /// <param name="userId">用户账户</param>
        /// <param name="newPhone">新手机号</param>
        /// <param name="token">重置令牌</param>
        /// <returns></returns>
        Task<IdentityResult> ChangePhoneNumAsync(string userId,string newPhone,string token);

        Task<IdentityResult> ChangePwdAsync(Guid userId,string currentPwd,string newPwd);//更改用户密码

        /// <summary>
        /// 添加角色到用户中
        /// </summary>
        /// <param name="user"></param>
        /// <param name="role"></param>
        /// <returns></returns>
        Task<IdentityResult> AddRoleToUserAsync(User user,Role role);

        /// <summary>
        /// 删除用户(软删除)
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task<IdentityResult> RemoveUserAsync(Guid userId);

        Task<SignInResult> CheckPasswordAsync(string name, string pwd);//检查用户名和密码是否匹配


    }
}
