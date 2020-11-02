using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Orleans.Grains
{
    public class SessionControlGrain : Grain<LoginState>, ISessionControlGrain
    {
        public Task Login(string userId)
        {
            //获取当前Grain的身份标识(因为ISessionControlGrain身份标识为string类型，GetPrimaryKeyString()); 
            var appName = this.GetPrimaryKeyString();
            this.State.LoginUsers.Add(userId);
            this.WriteStateAsync();

            Console.WriteLine($"Current active users count of {appName} is {this.State.Count}");
            return Task.CompletedTask;
        }

        public Task Logout(string userId)
        {
            //获取当前Grain的身份标识
            var appName = this.GetPrimaryKey();
            this.State.LoginUsers.Remove(userId);
            this.WriteStateAsync();

            Console.WriteLine($"Current active users count of {appName} is {this.State.Count}");
            return Task.CompletedTask;
        }

        public Task<int> GetActiveUserCount()
        {
            return Task.FromResult(this.State.Count);
        }
    }

    /// <summary>
    /// 登录状态
    /// </summary>
    public class LoginState
    {
        public List<string> LoginUsers { get; set; } = new List<string>();

        public int Count => LoginUsers.Count;
    }
}
