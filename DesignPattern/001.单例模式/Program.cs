using System;

namespace _001.单例模式
{
    /*
     * 定义：
     *（Singleton Pattern）确保某一个类只有一个实例，而且自行实例化并向整个系统提供这个实例
     *
     * 应用场景：
     * 避免产生多个对象消耗过多的资源（特别是一个对象需要频繁的创建和销毁时）
     * 提供一个全局访问点，常常被用来管理系统中共享的资源(作为一个Manager)
     *
     * 实现方式：
     * 一、静态变量初始化（饿汉模式）
     * 应用场景：适用于单线程应用程序
     * 二、延迟初始化（懒汉模式）
     * 三、锁机制（推荐）
     * 四、泛型单例模式
     */
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
        }
    }

    /// <summary>
    /// 单例模式实现方式一：
    /// 静态变量初始化
    /// </summary>
    public class Singleton1
    {
        /// <summary>
        /// 定义为静态变量，由所有对象共享
        /// </summary>
        private static Singleton1 instance = new Singleton1();

        /// <summary>
        /// 构造函数私有化，禁止外部类实例化该类对象
        /// </summary>
        private Singleton1()
        {

        }

        public static Singleton1 Instance()
        {
            return instance;
        }
    }

    /// <summary>
    /// 单例模式实现方式二：
    /// 延迟初始化
    /// </summary>
    public class Singleton2
    {
        /// <summary>
        /// 定义为静态变量，由所有对象共享
        /// </summary>
        private static Singleton2 _instance;

        /// <summary>
        /// 构造函数私有化，禁止外部类实例化该类对象
        /// </summary>
        private Singleton2()
        {

        }

        public static Singleton2 Instance()
        {
            return _instance ??= new Singleton2();
        }
    }

    /// <summary>
    /// 单例模式实现方式三：
    /// 锁机制，确保多线程只产生一个实例
    /// </summary>
    public class Singleton3
    {
        private static Singleton3 _instance;

        private static readonly object Locker = new object();

        private Singleton3()
        {
        }

        public static Singleton3 Instance()
        {
            //没有第一重 instance == null 的话，每一次有线程进入 GetInstance()时，均会执行锁定操作来实现线程同步，
            //非常耗费性能 增加第一重instance ==null 成立时的情况下执行一次锁定以实现线程同步
            if (_instance == null)
            {
                lock (Locker)
                {
                    //Double-Check Locking 双重检查锁定
                    if (_instance == null)
                    {
                        _instance = new Singleton3();
                    }
                }
            }

            return _instance;
        }
    }

    /// <summary>
    /// 泛型单例模式的实现
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class GenericSingleton<T> where T : class //,new ()
    {
        private static T instance;

        private static readonly object Locker = new object();

        public static T GetInstance()
        {
            //没有第一重 instance == null 的话，每一次有线程进入 GetInstance()时，均会执行锁定操作来实现线程同步，
            //非常耗费性能 增加第一重instance ==null 成立时的情况下执行一次锁定以实现线程同步
            if (instance == null)
            {
                //Double-Check Locking 双重检查锁定
                lock (Locker)
                {
                    if (instance == null)
                    {
                        //instance = new T();
                        //需要非公共的无参构造函数，不能使用new T() ,new不支持非公共的无参构造函数 
                        instance = Activator.CreateInstance(typeof(T), true) as T; //第二个参数防止异常：“没有为该对象定义无参数的构造函数。”
                    }
                }
            }

            return instance;
        }
    }
}
