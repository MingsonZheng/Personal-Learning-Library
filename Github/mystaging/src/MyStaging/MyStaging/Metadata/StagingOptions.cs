using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using MyStaging.Interface;

namespace MyStaging.Metadata
{
    public class StagingOptions
    {
        public StagingOptions(string name, string master, params string[] slaves)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException(nameof(name));

            if (string.IsNullOrEmpty(master))
                throw new ArgumentNullException(nameof(master));

            this.Master = master;
            this.Slaves = slaves;
            this.Name = name;
        }

        public string Name { get; }
        public ILogger Logger { get; set; }
        public IStagingConnection Connection { get; set; }
        public CacheOptions CacheOptions { get; set; }
        public ProviderType Provider { get; set; }
        public string Master { get; set; }
        public string[] Slaves { get; set; }
    }

    public class CacheOptions
    {
        /// <summary>
        ///  缓存前缀，默认值：mystaging_
        /// </summary>
        public string Prefix { get; set; } = "mystaging_";
        public IDistributedCache Cache { get; set; }
        /// <summary>
        ///  默认过期时间 60s,AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(60)
        /// </summary>
        public DistributedCacheEntryOptions Options { get; set; } = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(60)
        };
    }
}
