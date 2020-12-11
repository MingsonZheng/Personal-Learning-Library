using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyStaging.Common;
using MyStaging.Metadata;
using Pgsql.Model;

namespace Pgsql
{
    public class ContextTest
    {
        const string connectionString = "Host=127.0.0.1;Port=5432;Username=postgres;Password=postgres;Database=mystaging;Pooling=true;Maximum Pool Size=1000;";
        StagingOptions options;
        PgsqlDbContext dbContext;
        public void Start()
        {
            options = new StagingOptions("user", connectionString);
            dbContext = new PgsqlDbContext(options);

            Insert();
            Delete();
            Update();
            Select();
            Transaction();
        }

        private void Insert()
        {
            List<User> users = new List<User>();
            for (int i = 0; i < 100; i++)
            {
                var user = new User
                {
                    id = ObjectId.NewId().ToString(),
                    age = 18,
                    createtime = DateTime.Now,
                    password = "123456",
                    nickname = "lgx",
                    IP = "127.0.0.1",
                    loginname = "lgx",
                    money = 0,
                    role = et_role.普通成员,
                    sex = true,
                    wealth = 0
                };
                users.Add(user);
            }

            var affrows = dbContext.User.Insert.AddRange(users).SaveChange();
            var firstUser = users[0];
            firstUser.id = ObjectId.NewId().ToString();
            dbContext.User.Insert.Add(firstUser);

            Console.WriteLine("AddRange={0}", affrows);
            Console.WriteLine("Add={0}", firstUser.id);
        }

        private void Delete()
        {
            var user = dbContext.User.Select.OrderByDescing(f => f.createtime).ToOne();
            dbContext.User.Delete.Where(f => f.id == user.id).SaveChange();

            var delete = dbContext.User.Select.Where(f => f.id == user.id).ToOne();
            Console.WriteLine("Delete==User:{0},{1}", user.id, delete == null);
        }

        private void Update()
        {
            var user = dbContext.User.Select.OrderByDescing(f => f.createtime).ToOne();
            var newUser = dbContext.User.Update.SetValue(f => f.age, 28)
                                                         .Where(f => f.id == user.id)
                                                         .SaveChange();

            Console.WriteLine("Update==user:{0},Age:old[{1}],new[{2}]", user.id, user.age, newUser.age);
        }

        private void Select()
        {
            for (int i = 1; i < 10; i++)
            {
                var userId = "5ddc9d8eb5ee485e50000001";
                var sum = dbContext.User.Select.Sum<long>(f => f.age);
                Console.WriteLine(sum);
                var user = dbContext.User.Select.Where(f => f.id == userId).ToOne();
                var users = dbContext.User.Select.ToList();
                Console.WriteLine("userid=={0}", user.id);
                Console.WriteLine("users=={0}--", users.Count);
                users.Clear();
            }
        }

        private void Transaction()
        {
            var user = new User
            {
                id = ObjectId.NewId().ToString(),
                age = 18,
                createtime = DateTime.Now,
                password = "123456",
                nickname = "lgx",
                IP = "127.0.0.1",
                loginname = "lgx",
                money = 0,
                role = et_role.普通成员,
                sex = true,
                wealth = 0
            };

            try
            {
                // 测试事务
                dbContext.BeginTransaction();
                dbContext.User.Insert.Add(user);
                List<User> li = new List<User>
                {
                    new User {
                        id=ObjectId.NewId(),
                        nickname = "test",
                        loginname="测试人员的姓名"
                    }
                };
                dbContext.User.Insert.AddRange(li).SaveChange();
                dbContext.User.Update.SetValue(a => a.nickname, "12345").Where(f => f.id == user.id).SaveChange();
                dbContext.CommitTransaction();

                var art = dbContext.User.Select.Where(f => f.id == "5df09d4db5ee485ac0000008").ToOne();
                art = dbContext.User.Update.SetValue(f => f.nickname, "马冬梅").Where(f => f.id == user.id).SaveChange();
                int affrows = dbContext.User.Delete.Where(f => f.id == user.id).SaveChange();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}
