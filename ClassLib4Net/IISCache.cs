using System;
using System.Web;

namespace ClassLib4Net
{
    /// <summary>
    /// IIS缓存管理（HttpContext.Current.Cache：为当前 HTTP 请求获取Cache对象）
    /// </summary>
    [Obsolete("请使用：" + nameof(CacheHelper), false)]
    public class IISCache
    {
        #region IIS缓存系统
        /// <summary>
        /// 获取IIS缓存
        /// </summary>
        /// <param name="cacheKey">缓存键</param>
        /// <param name="isCache">是否IIS缓存</param>
        /// <returns></returns>
        public static object GetCache(string cacheKey, bool isCache)
        {
            object rtn = null;
            if (isCache && System.Web.HttpContext.Current != null && System.Web.HttpContext.Current.Cache[cacheKey] != null)
            {
                rtn = System.Web.HttpContext.Current.Cache[cacheKey];
                return rtn;
            }
            return rtn;
        }
        /// <summary>
        /// 根据键移除IIS缓存
        /// </summary>
        /// <param name="cacheKey">缓存键</param>
        /// <param name="isCache">是否IIS缓存</param>
        public static void RemoveCacheByKey(string cacheKey, bool isCache)
        {
            if (isCache && System.Web.HttpContext.Current != null && System.Web.HttpContext.Current.Cache[cacheKey] != null)
            {
                System.Web.HttpContext.Current.Cache.Remove(cacheKey);
            }
        }
        /// <summary>
        /// 移除全部IIS缓存
        /// </summary>
        /// <param name="isCache">是否IIS缓存</param>
        public static void RemoveAllCache(bool isCache)
        {
            if (isCache && System.Web.HttpRuntime.Cache != null && System.Web.HttpRuntime.Cache.Count > 0)
            {
                System.Web.Caching.Cache _cache = System.Web.HttpRuntime.Cache;
                System.Collections.IDictionaryEnumerator CacheEnum = _cache.GetEnumerator();
                while (CacheEnum.MoveNext())
                {
                    string _key = CacheEnum.Key.ToString();
                    _cache.Remove(_key);
                }
            }
        }

        /// <summary>
        /// 设置（新增）IIS缓存
        /// </summary>
        /// <param name="cacheKey">缓存键</param>
        /// <param name="objectValue">缓存值</param>
        /// <param name="isCache">是否IIS缓存</param>
        /// <param name="cacheTime">过期时间（分钟）</param>
        public static void SetCache(string cacheKey, object objectValue, bool isCache, int cacheTime)
        {
            if (objectValue != null && isCache && System.Web.HttpContext.Current != null)
            {
                System.Web.HttpContext.Current.Cache.Add(cacheKey, objectValue, null, DateTime.Now.AddMinutes(cacheTime), System.Web.Caching.Cache.NoSlidingExpiration, System.Web.Caching.CacheItemPriority.High, null);
            }
        }

        /// <summary>
        /// 修改（新增）IIS缓存
        /// </summary>
        /// <param name="cacheKey">缓存键</param>
        /// <param name="objectValue">缓存值</param>
        /// <param name="isCache">是否IIS缓存</param>
        /// <param name="cacheTime">过期时间（分钟）</param>
        public static void UpdateCache(string cacheKey, object objectValue, bool isCache, int cacheTime)
        {
            RemoveCacheByKey(cacheKey, isCache);
            SetCache(cacheKey, objectValue, isCache, cacheTime);
        }
        #endregion
    }
}
