using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Caching;

namespace ClassLib4Net.Cache
{
	/// <summary>
	/// 默认缓存管理器
	/// </summary>
	public class CacheManager : ICacheManager
	{
		/// <summary>
		/// 获取当前缓存
		/// </summary>
		public static ICacheManager Current
		{
			get
			{

				if (dict.ContainsKey(CacheManagerConfig.CacheManagerType) && CacheManagerConfig.CacheManagerType != "Default")
				{
					return dict[CacheManagerConfig.CacheManagerType];
				}
				else
				{
					return dict["Default"];
				}
			}
		}

		static Dictionary<string, ICacheManager> dict = new Dictionary<string, ICacheManager>();

		static CacheManager()
		{
			dict.Add("Default", new CacheManager());
			dict.Add("Memcached", new MemcachedCacheManager());
		}

		System.Web.Caching.Cache cache = HttpRuntime.Cache;

		/// <summary>
		/// 添加缓存项
		/// </summary>
		/// <param name="key"></param>
		/// <param name="value"></param>
		/// <param name="expireMin">过期时长（分钟）</param>
		/// <returns></returns>
		public bool Add(string key, object value, int expireMin)
		{
			return cache.Add(key, value, null, DateTime.Now.AddMinutes(expireMin), TimeSpan.Zero, CacheItemPriority.Normal, null) != null;
		}

		/// <summary>
		/// 添加缓存项
		/// </summary>
		/// <param name="key"></param>
		/// <param name="value"></param>
		/// <returns></returns>
		public bool Add(string key, object value)
		{
			return cache.Add(key, value, null, DateTime.Now.AddMinutes(10), TimeSpan.Zero, CacheItemPriority.Normal, null) != null;
		}

		/// <summary>
		/// 添加缓存项
		/// </summary>
		/// <param name="key"></param>
		/// <param name="value"></param>
		/// <param name="timeSpan">过期时长</param>
		/// <returns></returns>
		public bool Add(string key, object value, TimeSpan timeSpan)
		{
			return cache.Add(key, value, null, DateTime.MaxValue, timeSpan, CacheItemPriority.Normal, null) != null;
		}

		/// <summary>
		/// 获取缓存项值
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		public object Get(string key)
		{
			return cache[key];
		}

		/// <summary>
		/// 获取缓存项值
		/// </summary>
		/// <typeparam name="T">缓存项类型</typeparam>
		/// <param name="key"></param>
		/// <returns></returns>
		public T Get<T>(string key)
		{
			if (cache[key] == null) return default(T);
			return (T)cache[key];
		}

		/// <summary>
		/// 获取多个缓存项
		/// </summary>
		/// <param name="keys">缓存键列表</param>
		/// <returns></returns>
		public IDictionary<string, object> Get(params string[] keys)
		{
			if (keys != null)
			{
				return keys.ToDictionary(k => k, k => cache[k]) ?? new Dictionary<string, object>();
			}
			else
			{
				return new Dictionary<string, object>();
			}
		}

		/// <summary>
		/// 删除缓存项
		/// </summary>
		/// <param name="key"></param>
		/// <returns>缓存项值</returns>
		public object Remove(string key)
		{
			return cache.Remove(key);
		}
	}
}
