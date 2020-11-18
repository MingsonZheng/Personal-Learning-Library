using Microsoft.Extensions.Configuration;
using System;
using System.Timers;

namespace ConfigurationCustom
{
    // ConfigurationProvider 集成自 IConfigurationProvider
    class MyConfigurationProvider : ConfigurationProvider
    {

        Timer timer;

        public MyConfigurationProvider() : base()
        {
            // 用一个线程模拟配置发生变化，每三秒钟执行一次，告诉我们要重新加载配置
            timer = new Timer();
            timer.Elapsed += Timer_Elapsed;
            timer.Interval = 3000;
            timer.Start();
        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e) => Load(true);

        public override void Load() => Load(false);

        /// <summary>
        /// 加载数据
        /// </summary>
        /// <param name="reload">是否重新加载数据</param>
        void Load(bool reload)
        {
            // Data 表示 Key-value 数据，这是由 ConfigurationProvider 提供的一个数据承载的集合
            // 我们把最新的时间填充进去
            Data["lastTime"] = DateTime.Now.ToString();
            if (reload)
            {
                base.OnReload();
            }
        }
    }
}