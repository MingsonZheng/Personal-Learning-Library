using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using Microsoft.Extensions.Logging;
using MyStaging.Common;
using MyStaging.Interface;
using MyStaging.Metadata;

namespace MyStaging.Core
{
    public class InternalDbContext
    {
        public readonly static string[] INTERFACES = { "ISelectBuilder`1", "IUpdateBuilder`1", "IInsertBuilder`1", "IDeleteBuilder`1", "IStagingConnection" };
        public readonly static ConcurrentDictionary<string, Type> SERVICES = new ConcurrentDictionary<string, Type>();
        public readonly static object objlock = new object();

        public static void Initializer(StagingOptions options)
        {
            if (SERVICES.Count == 0)
            {
                lock (objlock)
                {
                    if (SERVICES.Count > 0)
                        return;

                    var fileName = "MyStaging." + options.Provider + ".dll";
                    var providerFile = System.IO.Directory.GetFiles(System.IO.Directory.GetCurrentDirectory(), fileName, SearchOption.AllDirectories).FirstOrDefault();
                    if (string.IsNullOrEmpty(providerFile))
                        throw new FileNotFoundException($"找不到提供程序，确保已经引用程序集：{fileName}");

                    var assembly = Assembly.LoadFrom(providerFile);
                    var types = assembly.GetTypes();
                    // 初始化桥接对象
                    foreach (var type in types)
                    {
                        var interfaces = type.GetInterfaces();
                        foreach (var inter in interfaces)
                        {
                            if (INTERFACES.Contains(inter.Name))
                            {
                                SERVICES.TryAdd(inter.Name, type);
                                break;
                            }
                        }
                    }
                }
            }
        }

        public static Type GetInterfaceType(string typeName, Type genericType)
        {
            if (InternalDbContext.SERVICES.ContainsKey(typeName))
            {
                return SERVICES[typeName].MakeGenericType(genericType);
            }
            return null;
        }
    }

    public abstract class DbContext
    {
        public DbContext(StagingOptions options, ProviderType provider)
        {
            options.Provider = provider;
            Initializer(options);
        }

        public DbContext(StagingOptions options)
        {
            Initializer(options);
        }

        public void Initializer(StagingOptions options)
        {
            CheckNotNull.NotNull(options, nameof(options));
            CheckNotNull.NotNull(options.Name, nameof(options.Name));

            Options = options;
            if (options.CacheOptions != null && options.CacheOptions.Cache != null)
            {
                CacheManager = new CacheManager(options.CacheOptions);
            }

            InternalDbContext.Initializer(Options);
            var properties = this.GetType().GetProperties();
            foreach (var pi in properties)
            {
                if (pi.PropertyType.Name == "DbSet`1")
                {
                    var piObject = Activator.CreateInstance(pi.PropertyType);
                    pi.SetValue(this, piObject);
                    var genericType = pi.PropertyType.GenericTypeArguments[0];
                    var childreProperties = pi.PropertyType.GetProperties();
                    foreach (var children in childreProperties)
                    {
                        var interfaceType = InternalDbContext.GetInterfaceType(children.PropertyType.Name, genericType);
                        if (interfaceType != null)
                        {
                            children.SetValue(piObject, Activator.CreateInstance(interfaceType, this));
                        }
                    }
                }
            }
            InternalDbContext.SERVICES.TryGetValue(typeof(IStagingConnection).Name, out Type iop);
            Options.Connection = (IStagingConnection)Activator.CreateInstance(iop);
            this.Refresh(Options.Master, Options.Slaves);
        }

        public SQLExecute Execute => CreateExecute();
        /// <summary>
        ///  获取或者设置数据库连接池对象
        /// </summary>
        public DbConnection Connection { get; set; }

        /// <summary>
        ///  获取事务集
        /// </summary>
        public ConcurrentDictionary<int, DbTransaction> Trans = new ConcurrentDictionary<int, DbTransaction>();

        /// <summary>
        ///  获取当前线程产生的数据库事务
        /// </summary>
        public virtual DbTransaction CurrentThreadTransaction
        {
            get
            {
                int tid = Thread.CurrentThread.ManagedThreadId;
                if (Trans.ContainsKey(tid) && Trans[tid] != null)
                    return Trans[tid];
                return null;
            }
        }

        /// <summary>
        ///  缓存管理
        /// </summary>
        public CacheManager CacheManager { get; set; } = null;

        /// <summary>
        ///  脚手架设置选项
        /// </summary>
        public StagingOptions Options { get; set; }

        /// <summary>
        ///  刷新数据库连接
        /// </summary>
        /// <param name="connectionMaster"></param>
        /// <param name="connectionSlaves"></param>
        public void Refresh(string connectionMaster, string[] connectionSlaves = null)
        {
            Options.Connection.Refresh(Options.Name, connectionMaster, connectionSlaves);
        }

        /// <summary>
        ///  此函数只能在读写数据库连接中进行
        /// </summary>
        /// <param name="action"></param>
        public void Transaction(Action action)
        {
            try
            {
                BeginTransaction();
                action?.Invoke();
                CommitTransaction();
            }
            catch (Exception ex)
            {
                WriteLog(ex);
                RollBackTransaction();
                throw;
            }
        }

        /// <summary>
        ///  记录连接异常日志
        /// </summary>
        /// <param name="message"></param>
        public void WriteLog(string message)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(message);
            Console.ForegroundColor = ConsoleColor.White;
            Options.Logger?.LogError(message);
        }

        public void WriteLog(Exception ex)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(ex.Message);
            sb.AppendLine(ex.StackTrace);
            if (ex.Data["DbConnection"] is DbConnection dbConnection)
            {
                sb.AppendLine(dbConnection.ConnectionString);
            }

            if (ex.InnerException != null)
            {
                sb.AppendLine(ex.InnerException.Message);
                sb.AppendLine(ex.InnerException.StackTrace);
            }

            WriteLog(sb.ToString());
        }

        private SQLExecute CreateExecute()
        {
            if (CurrentThreadTransaction != null)
            {
                this.Connection = CurrentThreadTransaction.Connection;
            }
            else
            {
                Connection = Options.Connection.GetConnection(Options.Name, byMaster);
            }

            return new SQLExecute(Connection, CurrentThreadTransaction);
        }

        /// <summary>
        ///  添加仅从已配置的主数据源中查询数据的约束
        /// </summary>
        /// <returns></returns>
        public DbContext ByMaster()
        {
            byMaster = !byMaster;
            return this;
        }

        private bool byMaster;

        /// <summary>
        ///  在当前线程上开始执行事务
        /// </summary>
        public virtual DbTransaction BeginTransaction()
        {
            if (CurrentThreadTransaction != null)
                CommitTransaction(true);

            CreateExecute();
            if (Connection.State != ConnectionState.Open)
                Connection.Open();

            DbTransaction tran = Connection.BeginTransaction();
            int tid = Thread.CurrentThread.ManagedThreadId;
            if (Trans.ContainsKey(tid))
                CommitTransaction();
            else
                Trans.TryAdd(tid, tran);

            return tran;
        }

        /// <summary>
        ///  提交当前线程上执行的事务
        /// </summary>
        public virtual DbTransaction CommitTransaction() => CommitTransaction(true);

        /// <summary>
        ///  将当前线程上的事务进行回滚
        /// </summary>
        public virtual DbTransaction RollBackTransaction() => CommitTransaction(false);

        /// <summary>
        ///  可控制的事务提交
        /// </summary>
        /// <param name="iscommit">true=提交事务，false=回滚事务</param>
        public virtual DbTransaction CommitTransaction(bool iscommit)
        {
            int tid = Thread.CurrentThread.ManagedThreadId;
            if (Trans.TryRemove(tid, out DbTransaction tran))
            {
                using var connection = tran.Connection;
                if (iscommit)
                    tran.Commit();
                else
                    tran.Rollback();
            }
            return tran;
        }
    }
}
