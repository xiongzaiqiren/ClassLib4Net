using System;
using System.Collections.Generic;
using Enyim.Caching;
using Enyim.Caching.Memcached;

namespace ClassLib4Net.Cache
{
	/// <summary>
	/// Memcached缓存管理
	/// </summary>
	public class MemcachedCacheManager : ICacheManager
	{
		static MemcachedClient client = new MemcachedClient();

		#region ICacheManager 成员
		/// <summary>
		/// 添加缓存项
		/// </summary>
		/// <param name="key"></param>
		/// <param name="value"></param>
		/// <param name="expireMin">过期时长（分钟）</param>
		/// <returns></returns>
		public bool Add(string key, object value, int expireMin)
		{
			return client.Store(StoreMode.Set, key, value, TimeSpan.FromMinutes(expireMin));
		}

		/// <summary>
		/// 添加缓存项
		/// </summary>
		/// <param name="key"></param>
		/// <param name="value"></param>
		/// <param name="expireMin">过期时长（分钟）</param>
		/// <returns></returns>
		public bool Add(string key, object value, double expireMin)
		{
			return client.Store(StoreMode.Set, key, value, TimeSpan.FromMinutes(expireMin));
		}

		/// <summary>
		/// 添加缓存项
		/// </summary>
		/// <param name="key"></param>
		/// <param name="value"></param>
		/// <returns></returns>
		public bool Add(string key, object value)
		{
			return client.Store(StoreMode.Set, key, value);
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
			return client.Store(StoreMode.Set, key, timeSpan);
		}

		/// <summary>
		/// 获取缓存项值
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		public object Get(string key)
		{
			return client.Get(key);
		}

		/// <summary>
		/// 获取缓存项值
		/// </summary>
		/// <typeparam name="T">缓存项类型</typeparam>
		/// <param name="key"></param>
		/// <returns></returns>
		public T Get<T>(string key)
		{
			return client.Get<T>(key);
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
				return client.Get(keys) ?? new Dictionary<string, object>();
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
			return client.Remove(key);
		}
		#endregion
	}
}
