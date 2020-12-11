using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;
using MyStaging.DataAnnotations;
using MyStaging.Metadata;

namespace MyStaging.Common
{
    public class CacheManager
    {
        private readonly CacheOptions cacheOpt = null;
        //   private readonly static JsonSerializerSettings JSON_SETTING = new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore };
        private readonly static JsonSerializerOptions JSON_SETTING = new JsonSerializerOptions { IgnoreNullValues = true };

        public CacheManager(CacheOptions options)
        {
            this.cacheOpt = options ?? throw new ArgumentNullException(nameof(CacheOptions));
        }

        public void SetItemCache<T>(T item, int seconds = 0)
        {
            if (item != null)
            {
                string id = null; // 仅缓存单个对象，此对象必须具有主键
                var properties = typeof(T).GetProperties().Where(f => f.GetCustomAttribute<PrimaryKeyAttribute>() != null).ToList();
                if (properties == null || properties.Count == 0) return;

                foreach (var pi in properties)
                {
                    id += pi.GetValue(item).ToString();
                }

                if (id == null) throw new ArgumentException(nameof(T) + "未设置主键，无法进行缓存.");
                DistributedCacheEntryOptions opt = cacheOpt.Options;
                if (seconds > 0) // 自定义缓存覆盖全局缓存
                {
                    opt = new DistributedCacheEntryOptions()
                    {
                        AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(60)
                    };
                }
                var key = FormatKey<T>(id);

                // var json = JsonConvert.SerializeObject(item, JSON_SETTING);               
                var json = JsonSerializer.Serialize(item, JSON_SETTING);
                var data = Encoding.UTF8.GetBytes(json);
                this.cacheOpt.Cache.Set(key, data, opt);
            }
        }

        public TResult GetItemCache<TResult, TParameter>(IList<TParameter> parameters) where TParameter : DbParameter
        {
            TResult obj = default;
            //仅针对主键进行缓存，无主键不缓存
            var pkCount = typeof(TResult).GetProperties().Where(f => f.GetCustomAttribute<PrimaryKeyAttribute>() != null).Count();
            if (parameters?.Count == pkCount && pkCount > 0)
            {
                var id = GetKeys<TParameter>(parameters);
                var key = FormatKey<TResult>(id);
                var data = this.cacheOpt.Cache.Get(key);
                if (data != null)
                {
                    var json = Encoding.UTF8.GetString(data);
                    //obj = JsonConvert.DeserializeObject<TResult>(json, JSON_SETTING);
                    obj = JsonSerializer.Deserialize<TResult>(json, JSON_SETTING);
                }
            }

            return obj;
        }

        public void RemoveItemCache<T>(string key)
        {
            key = FormatKey<T>(key);
            this.cacheOpt.Cache.Remove(key);
        }

        private static string GetKeys<TParameter>(IList<TParameter> parameters) where TParameter : DbParameter
        {
            var id = string.Empty;
            foreach (var p in parameters)
            {
                id += p.Value.ToString().Replace("/", "");
            }

            return id;
        }

        private string FormatKey<T>(string key)
        {
            return $"{cacheOpt.Prefix}{typeof(T).FullName}_{key}";
        }
    }
}
