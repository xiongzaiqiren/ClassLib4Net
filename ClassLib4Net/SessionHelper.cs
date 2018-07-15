using System;
using System.Web;

namespace ClassLib4Net
{
    /// <summary>
    /// Session帮助类
    /// </summary>
    public class SessionHelper
    {
        /// <summary>
        /// 根据key获取value
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public static T GetValue<T>(string key)
        {
            var sessions = HttpContext.Current.Session;
            if (sessions == null)
                return default(T);
            var obj = sessions[key];
            if (obj == null)
                return default(T);
            return (T) obj;
        }

        /// <summary>
        /// 设置键值对
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
		public static void SetValue(string key, object value)
        {
            if(string.IsNullOrWhiteSpace(key)) throw new ArgumentNullException("key is null.");
            if(null == value || DBNull.Value == value) throw new ArgumentNullException("value is null.");
            if(null == HttpContext.Current.Session) throw new NullReferenceException(nameof(HttpContext.Current.Session));

            var obj = HttpContext.Current.Session[key];
            if(null != obj)
                HttpContext.Current.Session[key] = value;
            else
                HttpContext.Current.Session.Add(key, value);
        }

        /// <summary>
        /// 根据key移除缓存
        /// </summary>
        /// <param name="key"></param>
        public static void Remove(string key)
        {
            if(null == HttpContext.Current.Session) throw new NullReferenceException(nameof(HttpContext.Current.Session));
            HttpContext.Current.Session.Remove(key);
            HttpContext.Current.Session.Abandon();
        }
    }
}