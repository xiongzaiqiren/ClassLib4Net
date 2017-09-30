using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;

namespace ClassLib4Net
{
    /// <summary>
    /// Cookie帮助类
    /// </summary>
    public class CookieHelper
    {
        #region Cookie第一种写法
        /// <summary>
        /// 根据key获取value
        /// </summary>
        /// <param name="key">key</param>
        /// <returns></returns>
        public static string GetValue(string key)
        {
            var cookie = HttpContext.Current.Request.Cookies[key];
            return cookie == null ? null : cookie.Value.UrlDecode();
        }

        /// <summary>
        /// 根据key获取value，未获取到值执行赋值函数
        /// </summary>
        /// <param name="key"></param>
        /// <param name="getValueFunc"></param>
        /// <returns></returns>
        public static string GetValue(string key, Func<string> getValueFunc)
        {
            var val = GetValue(key);
            if (val == null)
            {
                val = getValueFunc();
                var cookie = HttpContext.Current.Request.Cookies[key];
                if (cookie == null)
                {
                    SetValue(key, val);
                    return val;
                }
            }
            return val;
        }

        /// <summary>
        /// 设置键值对
        /// </summary>
        /// <param name="key"></param>
        /// <param name="getValueFunc"></param>
        public static void SetValue(string key, Func<string> getValueFunc)
        {
            SetValue(key, getValueFunc());
        }

        /// <summary>
        /// 设置键值对
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public static void SetValue(string key, object value)
        {
            if (value == null)
            {
                HttpContext.Current.Response.Cookies.Remove(key);
                return;
            }
            var cookie = HttpContext.Current.Request.Cookies[key];
            if (cookie != null)
            {
                cookie.Value = value.ToString();
                HttpContext.Current.Response.AppendCookie(cookie);
            }
            else
                HttpContext.Current.Response.AppendCookie(new HttpCookie(key, value.ToString().UrlEncode()));
        }

        /// <summary>
        /// 根据key移除项
        /// </summary>
        /// <param name="key"></param>
        public static void Remove(string key)
        {
            if (null != HttpContext.Current)
            {
                if (null != HttpContext.Current.Request && null != HttpContext.Current.Request.Cookies && HttpContext.Current.Request.Cookies.AllKeys.Contains(key))
                {
                    HttpContext.Current.Request.Cookies.Remove(key);
                }
                if (null != HttpContext.Current.Response && null != HttpContext.Current.Response.Cookies && HttpContext.Current.Response.Cookies.AllKeys.Contains(key))
                {
                    HttpContext.Current.Response.Cookies.Remove(key);
                }
            }
        }

        /// <summary>
        /// 根据key获取Cookie模型
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public static T Get<T>(string key) where T : class, new()
        {
            if (HttpContext.Current.Request.Cookies[key] != null)
            {
                if (!string.IsNullOrEmpty(HttpContext.Current.Request.Cookies[key].Value))
                {
                    return JsonHelper.DeSerialize<T>(HttpUtility.UrlDecode(HttpContext.Current.Request.Cookies[key].Value));
                }
                else
                {
                    return new T();
                }
            }
            else
            {
                return new T();
            }
        }

        /// <summary>
        /// 设置指定key的Cookie模型
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <param name="key"></param>
        /// <param name="days">过期时间（天）</param>
        /// <param name="path">路径</param>
        /// <param name="domain">域（例如.baidu.com，如果是当前域名则该参数不要设置）</param>
        /// <param name="secure">http传输保密（注意：为true时javascript中可能取不到相应的Cookie）</param>
        public static void Set<T>(T obj, string key, double days = 0d, string path = "/", string domain = null, bool secure = false) where T : class, new()
        {
            string JsonStr = JsonHelper.Serialize(obj);
            //HttpCookie cookie = new HttpCookie(key, HttpUtility.UrlEncode(JsonStr));
            HttpCookie cookie = SetCookie(key, days, path, domain, secure);
            cookie.Value = HttpUtility.UrlEncode(JsonStr);
            HttpContext.Current.Response.Cookies.Add(cookie);
        }
        #endregion

        #region Cookie第二种写法
        /// <summary>
        /// 是否正常启用Cookie
        /// </summary>
        /// <returns></returns>
        public static bool CookieIsEnable()
        {
            try
            {
                SetCookie("CookieIsEnable", "OK", (1 / 24 / 60)); //1分钟
                if (string.IsNullOrEmpty(GetCookie("CookieIsEnable")))
                    return false;
                else
                    return true;
            }
            catch (Exception Ex)
            {
                return false;
            }
        }

        private static HttpCookie SetCookie(string name, double days, string path, string domain, bool secure)
        {
            HttpCookie cookie = new HttpCookie(name);
            if (0d != days)
                cookie.Expires = DateTime.Now.AddDays(days);
            cookie.Path = string.IsNullOrEmpty(path) ? "/" : path;
            cookie.Domain = string.IsNullOrEmpty(domain) ? string.Empty : domain;
            cookie.Secure = secure;
            return cookie;
        }

        /// <summary>
        /// 设置Cookie
        /// 熊学浩
        /// 2014年6月9日
        /// </summary>
        /// <param name="name">名称</param>
        /// <param name="value">存储对象Dictionary集合</param>
        /// <param name="days">过期时间（天）</param>
        /// <param name="path">路径</param>
        /// <param name="domain">域（例如.baidu.com，如果是当前域名则该参数不要设置）</param>
        /// <param name="secure">http传输保密（注意：为true时javascript中可能取不到相应的Cookie）</param>
        public static void SetCookie(string name, System.Collections.Generic.Dictionary<string, string> value, double days = 0d, string path = "/", string domain = null, bool secure = false)
        {
            if (string.IsNullOrEmpty(name) || value == null) return;

            HttpCookie cookie = SetCookie(name, days, path, domain, secure);
            foreach (var item in value)
            {
                cookie.Values.Add(item.Key, item.Value);
            }
            if (HttpContext.Current.Response.Cookies.AllKeys.Contains(name))
            {
                HttpContext.Current.Response.Cookies.Remove(name); //无法真正删除
                HttpContext.Current.Response.Cookies[name].Expires = DateTime.Now.AddDays(-1);
            }
            HttpContext.Current.Response.Cookies.Add(cookie);
        }
        /// <summary>
        /// 设置Cookie
        /// 熊学浩
        /// 2014年6月9日
        /// </summary>
        /// <param name="name">名称</param>
        /// <param name="value">存储对象 Hashtable表</param>
        /// <param name="days">过期时间（天）</param>
        /// <param name="path">路径</param>
        /// <param name="domain">域（例如.baidu.com，如果是当前域名则该参数不要设置）</param>
        /// <param name="secure">http传输保密（注意：为true时javascript中可能取不到相应的Cookie）</param>
        public static void SetCookie(string name, System.Collections.Hashtable value, double days = 0d, string path = "/", string domain = null, bool secure = false)
        {
            if (string.IsNullOrEmpty(name) || value == null) return;

            HttpCookie cookie = SetCookie(name, days, path, domain, secure);
            foreach (System.Collections.DictionaryEntry item in value)
            {
                cookie.Values.Add(item.Key.ToString(), item.Value.ToString());
            }
            if (HttpContext.Current.Response.Cookies.AllKeys.Contains(name))
            {
                HttpContext.Current.Response.Cookies.Remove(name);
                HttpContext.Current.Response.Cookies[name].Expires = DateTime.Now.AddDays(-1);
            }
            HttpContext.Current.Response.Cookies.Add(cookie);
        }

        /// <summary>
        /// 设置Cookie
        /// 熊学浩
        /// 2014年6月9日
        /// </summary>
        /// <param name="name">名称</param>
        /// <param name="value">存储对象Dictionary集合</param>
        /// <param name="days">过期时间（天）</param>
        public static void SetCookie(string name, System.Collections.Generic.Dictionary<string, string> value, double days = 0d)
        {
            SetCookie(name, value, days, string.Empty, string.Empty, false);
        }
        /// <summary>
        /// 设置Cookie
        /// 熊学浩
        /// 2014年6月9日
        /// </summary>
        /// <param name="name">名称</param>
        /// <param name="value">存储对象 Hashtable表</param>
        /// <param name="days">过期时间（天）</param>
        public static void SetCookie(string name, System.Collections.Hashtable value, double days = 0d)
        {
            SetCookie(name, value, days, string.Empty, string.Empty, false);
        }

        /// <summary>
        /// 设置Cookie
        /// 熊学浩
        /// 2014年6月9日
        /// </summary>
        /// <param name="name">名称</param>
        /// <param name="value">存储对象（可以被.ToString()处理的非集合对象）</param>
        /// <param name="days">过期时间（天）</param>
        /// <param name="path">路径</param>
        /// <param name="domain">域（例如.baidu.com，如果是当前域名则该参数不要设置）</param>
        /// <param name="secure">http传输保密（注意：为true时javascript中可能取不到相应的Cookie）</param>
        public static void SetCookie(string name, object value, double days = 0d, string path = "/", string domain = null, bool secure = false)
        {
            if (string.IsNullOrEmpty(name) || value == null) return;

            HttpCookie cookie = SetCookie(name, days, path, domain, secure);
            cookie.Value = value.ToString();

            if (HttpContext.Current.Response.Cookies.AllKeys.Contains(name))
            {
                HttpContext.Current.Response.Cookies.Remove(name);
                HttpContext.Current.Response.Cookies[name].Expires = DateTime.Now.AddDays(-1);
            }
            HttpContext.Current.Response.Cookies.Add(cookie);
        }

        /// <summary>
        /// 设置Cookie
        /// 熊学浩
        /// 2014年6月9日
        /// </summary>
        /// <param name="name">名称</param>
        /// <param name="value">存储对象（可以被.ToString()处理的非集合对象）</param>
        /// <param name="days">过期时间（天）</param>
        public static void SetCookie(string name, object value, double days = 0d)
        {
            SetCookie(name, value, days, string.Empty, string.Empty, false);
        }

        /// <summary>
        /// 获取Cookie
        /// 熊学浩
        /// 2014年6月9日
        /// </summary>
        /// <param name="name">名称</param>
        /// <returns></returns>
        public static string GetCookie(string name)
        {
            HttpCookie cookie = HttpContext.Current.Request.Cookies.Get(name);
            if (cookie != null)
            {
                if (!string.IsNullOrEmpty(cookie.Value))
                {
                    return cookie.Value;
                }
                else if (cookie.Values != null && cookie.Values.Count > 0)
                {
                    StringBuilder result = new StringBuilder();
                    foreach (var item in cookie.Values)
                    {
                        if (item != null)
                            result.Append(item.ToString() + "&");
                    }
                    return string.IsNullOrEmpty(result.ToString()) ? string.Empty : result.ToString().Substring(result.ToString().Length - 1, 1);
                }
            }
            return string.Empty;
        }
        #endregion

        #region CookieContainer
        /// <summary>
        /// 读取全部Cookie
        /// </summary>
        /// <param name="container">CookieContainer</param>
        /// <returns></returns>
        public static ICollection<Cookie> GetAllCookies(CookieContainer container)
        {
            ICollection<Cookie> listCookies = new List<Cookie>();

            Hashtable table = (Hashtable)container.GetType().InvokeMember("m_domainTable",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.GetField |
                System.Reflection.BindingFlags.Instance, null, container, new object[] { });

            foreach (object pathList in table.Values)
            {
                SortedList lstCookieCol = (SortedList)pathList.GetType().InvokeMember("m_list",
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.GetField
                    | System.Reflection.BindingFlags.Instance, null, pathList, new object[] { });
                foreach (CookieCollection colCookies in lstCookieCol.Values)
                    foreach (Cookie c in colCookies) listCookies.Add(c);
            }

            return listCookies;
        }
        /// <summary>
        /// 填充CookieContainer
        /// </summary>
        /// <param name="cookies">IEnumerable</param>
        /// <param name="cookieContainer">ref CookieContainer</param>
        /// <returns></returns>
        public static CookieContainer FillCookieContainer(IEnumerable<Cookie> cookies, ref CookieContainer cookieContainer)
        {
            if (null == cookies || !cookies.Any()) return cookieContainer;
            if (null == cookieContainer) cookieContainer = new CookieContainer();
            foreach (var cookie in cookies)
                cookieContainer.Add(cookie);

            return cookieContainer;
        }
        #endregion
    }
}