using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Orleans.Grains
{
    public class SessionControlGrain : Grain, ISessionControlGrain
    {
        private List<string> LoginUsers { get; set; } = new List<string>();

        public Task Login(string userId)
        {
            //获取当前Grain的身份标识(因为ISessionControlGrain身份标识为string类型，GetPrimaryKeyString()); 
            var appName = this.GetPrimaryKeyString();

            LoginUsers.Add(userId);//未加锁

            Console.WriteLine($"Current active users count of {appName} is {LoginUsers.Count}");
            return Task.CompletedTask;
        }

        public Task Logout(string userId)
        {
            //获取当前Grain的身份标识
            var appName = this.GetPrimaryKey();
            LoginUsers.Remove(userId);

            Console.WriteLine($"Current active users count of {appName} is {LoginUsers.Count}");
            return Task.CompletedTask;
        }

        public Task<int> GetActiveUserCount()
        {
            return Task.FromResult(LoginUsers.Count);
        }
    }
}
