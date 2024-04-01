using Identity.Domain;
using Identity.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.Internal;
using System.Text;

namespace IdentityService.Infrastructure
{
    public class IdentityRespository : IIdentityRepository
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<Role> _roleManager;

        public IdentityRespository(UserManager<User> userManager,RoleManager<Role> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        //错误的identityResult
        private IdentityResult ErrorResult(string msg)
        {
            IdentityError error = new IdentityError { Description=msg};
            return IdentityResult.Failed(error);
        }

        /// <summary>
        /// 添加指定角色到指定用户
        /// </summary>
        /// <param name="user"></param>
        /// <param name="role"></param>
        /// <returns></returns>
        public async Task<IdentityResult> AddRoleToUserAsync(User user, string role)
        {
            var result = await _userManager.AddToRoleAsync(user,role);
            return result;
        }
        
        /// <summary>
        /// 更改用户的电话
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="newPhone"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public async Task<IdentityResult> ChangePhoneNumAsync(User user, string newPhone, string token)
        {
            var result = await _userManager.ChangePhoneNumberAsync(user,newPhone,token);
            return result;
        }

        /// <summary>
        /// 更改密码
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="currentPwd"></param>
        /// <param name="newPwd"></param>
        /// <returns></returns>
        public async Task<IdentityResult> ChangePwdAsync(Guid userId, string currentPwd,string newPwd)
        {
            string id = userId.ToString();
            User? user = await FindOneUserByIdAsync(id);
            if(user is null)
            {
                return ErrorResult($"not found user with id:{userId}");
            }
            return await _userManager.ChangePasswordAsync(user,currentPwd,newPwd);
        }

        /// <summary>
        /// 检查密码是否正确
        /// </summary>
        /// <param name="name"></param>
        /// <param name="pwd"></param>
        /// <returns></returns>
        public async Task<SignInResult> CheckPasswordAsync(string name, string pwd)
        {
            User? user = await FindOneUserByNameAsync(name);
            //无用户返回失败
            if(user is null) return SignInResult.Failed;
            //用户被锁定返回失败
            if(await _userManager.IsLockedOutAsync(user))
            {
                return SignInResult.LockedOut;
            }
            bool result = await _userManager.CheckPasswordAsync(user, pwd);
            if (!result) 
            {
                await _userManager.AccessFailedAsync(user);//失败计数+1
                return SignInResult.Failed; 
            }
            return SignInResult.Success;
        }

        //创建新角色
        public Task<IdentityResult> CreateRoleAsync(string roleName)
        {
            Role newRole = new Role() { Name=roleName};
            return _roleManager.CreateAsync(newRole);
        }

        //创建用户，根据用户名和密码
        public async Task<IdentityResult> CreateUserAsync(string name, string password)
        {
            User newUser = new User(name);
            return await _userManager.CreateAsync(newUser,password);
        }

        //创建用户，根据手机号
        public async Task<IdentityResult> CreateUserByPhoneAsync(string phoneNumber)
        {
            User newUser = new User(phoneNumber:phoneNumber);
            string password = GenerateRandomPwd(12);
            return await _userManager.CreateAsync(newUser,password);
        }

        //根据手机号查找一个用户(多个时取第一个)
        public async Task<User?> FindOneByPhoneAsync(string phone)
        {
            var user = await _userManager.Users.FirstOrDefaultAsync(u => u.PhoneNumber == phone);
            return user;
        }

        //根据用户Id查找一个指定用户
        public async Task<User?> FindOneUserByIdAsync(string id)
        {
            return await _userManager.FindByIdAsync(id);
        }

        //根据用户名查找一个指定用户
        public async Task<User?> FindOneUserByNameAsync(string name)
        {
            return await _userManager.FindByNameAsync(name);
        }

        //根据用户名返回所有叫这个名字的用户
        public async Task<IEnumerable<User?>> FindUsersByName(string name)
        {
            return await _userManager.Users.Where(u => u.UserName == name).ToListAsync();
        }

        //返回密码重置令牌
        public async Task<string> GenerateResetPwdTokenAsync(User user)
        {
            return await _userManager.GeneratePasswordResetTokenAsync(user);
        }

        //返回该用户的所有角色
        public async Task<List<string>> GetUserRoleAsync(User user)
        {
            var roles = await _userManager.GetRolesAsync(user);
            return (List<string>)roles;
        }

        /// <summary>
        /// 软删除
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<IdentityResult> RemoveUserAsync(Guid userId)
        {
            User? user = await FindOneUserByIdAsync(userId.ToString());
            if (user is null) return ErrorResult($"not found the user with id {userId.ToString()}");
            user.SoftDelete();
            var result = await _userManager.UpdateAsync(user);
            return result;
        }

        /// <summary>
        /// 生成随机密码
        /// </summary>
        /// <returns></returns>
        private string GenerateRandomPwd(int length)
        {
            const string validChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890!@#$%^&*()_+-=[]{}|;:,.<>?";
            var random = new Random();
            var result = new StringBuilder(length);

            for (int i = 0; i < length; i++)
            {
                result.Append(validChars[random.Next(validChars.Length)]);
            }

            return result.ToString();
        }

        /// <summary>
        /// 更新用户信息
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<IdentityResult> UpdateUserAsync(User user)
        {
           return await  _userManager.UpdateAsync(user);
        }
    }
}
