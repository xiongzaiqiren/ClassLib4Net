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
    /// 作者：熊学浩
    /// 最后修改时间：2018/01/06
    /// </summary>
    public class CookieHelper
    {
        /// <summary>
        /// 构建HttpCookie实例（未初始化Value或Values属性）
        /// </summary>
        /// <param name="Name">Cookie 的名称</param>
        /// <param name="Minutes">Cookie 的过期时间（分钟）。</param>
        /// <param name="Path">要与此 Cookie 一起传输的虚拟路径。 默认为 /，也就是服务器根目录。</param>
        /// <param name="Domain">设置将此 Cookie 与其关联的域。默认值为当前域。（例如.baidu.com，如果是当前域名则该参数不要设置）</param>
        /// <param name="HttpOnly">如果 Cookie 具有 HttpOnly 特性且不能通过客户端脚本访问，则为 true；否则为 false。 默认值为 false。</param>
        /// <param name="Secure">如果通过 SSL 连接 (HTTPS) 传输 Cookie，则为 true；否则为 false。 默认值为 false。</param>
        /// <param name="Shareable">确定 cookie 是否允许参与输出缓存。</param>
        /// <returns></returns>
        public static HttpCookie BuildCookie(string Name, double Minutes = 20D, string Path = "/", string Domain = "", bool HttpOnly = false, bool Secure = false, bool Shareable = false)
        {
            HttpCookie cookie = new HttpCookie(Name)
            {
                Expires = DateTime.Now.AddMinutes(Minutes),
                Path = string.IsNullOrEmpty(Path) ? "/" : Path,
                Domain = string.IsNullOrEmpty(Domain) ? string.Empty : Domain,
                HttpOnly = HttpOnly,
                Secure = Secure,
                Shareable = Shareable
            };
            return cookie;
        }
        /// <summary>
        /// 设置或增加HttpCookie
        /// </summary>
        /// <param name="cookie">HttpCookie</param>
        public static void SetCookie(HttpCookie cookie)
        {
            if(null == cookie) throw new ArgumentNullException("cookie is null.");
            if(string.IsNullOrWhiteSpace(cookie.Name)) throw new ArgumentNullException("cookie name is null.");
            if(null == HttpContext.Current || null == HttpContext.Current.Response || null == HttpContext.Current.Response.Cookies) throw new NullReferenceException("HttpContext.Current.Response.Cookies is null.");
            if(HttpContext.Current.Response.Cookies.AllKeys.Contains(cookie.Name))
                HttpContext.Current.Response.Cookies.Set(cookie);
            else
                HttpContext.Current.Response.Cookies.Add(cookie);
        }

        #region Cookie
        /// <summary>
        /// 根据key获取value
        /// </summary>
        /// <param name="key">key</param>
        /// <returns></returns>
        [Obsolete("请使用：" + nameof(CookieHelper.GetCookie), false)]
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
        [Obsolete("请使用：" + nameof(CookieHelper.SetCookie), false)]
        public static string GetValue(string key, Func<string> getValueFunc)
        {
            var val = GetValue(key);
            if(val == null)
            {
                val = getValueFunc();
                var cookie = HttpContext.Current.Request.Cookies[key];
                if(cookie == null)
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
        [Obsolete("请使用：" + nameof(CookieHelper.SetCookie), false)]
        public static void SetValue(string key, Func<string> getValueFunc)
        {
            SetValue(key, getValueFunc());
        }

        /// <summary>
        /// 设置键值对
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        [Obsolete("请使用：" + nameof(CookieHelper.SetCookie), false)]
        public static void SetValue(string key, object value)
        {
            if(value == null)
            {
                HttpContext.Current.Response.Cookies.Remove(key);
                return;
            }
            var cookie = HttpContext.Current.Request.Cookies[key];
            if(cookie != null)
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
        /// <param name="name"></param>
        public static void Remove(string name)
        {
            if(null != HttpContext.Current)
            {
                if(null != HttpContext.Current.Response && null != HttpContext.Current.Response.Cookies && HttpContext.Current.Response.Cookies.AllKeys.Contains(name))
                {
                    HttpContext.Current.Response.Cookies.Remove(name);
                }

                if(null != HttpContext.Current.Request && null != HttpContext.Current.Request.Cookies && HttpContext.Current.Request.Cookies.AllKeys.Contains(name))
                {
                    var cookie = HttpContext.Current.Request.Cookies[name];
                    cookie.Expires = DateTime.Now.AddDays(-1);
                    if(HttpContext.Current.Response.Cookies.AllKeys.Contains(cookie.Name))
                    {
                        HttpContext.Current.Response.Cookies.Set(cookie);
                    }
                    else
                    {
                        HttpContext.Current.Response.Cookies.Add(cookie);
                    }

                    HttpContext.Current.Request.Cookies.Remove(name);
                }
            }
        }

        /// <summary>
        /// 获取Cookie
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static string GetCookie(string name)
        {
            if(null == HttpContext.Current || null == HttpContext.Current.Request || null == HttpContext.Current.Request.Cookies)
                return string.Empty;
            if(HttpContext.Current.Request.Cookies.AllKeys.Contains(name))
            {
                HttpCookie cookie = HttpContext.Current.Request.Cookies.Get(name);
                if(!string.IsNullOrWhiteSpace(cookie.Value))
                {
                    return cookie.Value;
                }
                else if(cookie.Values != null && cookie.Values.Count > 0)
                {
                    StringBuilder result = new StringBuilder();
                    foreach(var item in cookie.Values)
                    {
                        if(item != null)
                            result.Append(item.ToString() + "&");
                    }
                    return string.IsNullOrEmpty(result.ToString()) ? string.Empty : result.ToString().Substring(result.ToString().Length - 1, 1);
                }
            }
            return string.Empty;
        }

        /// <summary>
        /// 根据key获取Cookie模型
        /// </summary>
        /// <typeparam name="T">可序列化的对象类型</typeparam>
        /// <param name="name"></param>
        /// <returns></returns>
        public static T Get<T>(string name) where T : class, new()
        {
            if(HttpContext.Current.Request.Cookies.AllKeys.Contains(name))
            {
                var cookie = HttpContext.Current.Request.Cookies[name];
                if(!string.IsNullOrWhiteSpace(cookie.Value))
                {
                    return JsonHelper.DeSerialize<T>(HttpUtility.UrlDecode(cookie.Value));
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
        /// 设置Cookie
        /// </summary>
        /// <typeparam name="T">可序列化的对象类型</typeparam>
        /// <param name="obj">可序列化的对象实体</param>
        /// <param name="Name">Cookie 的名称</param>
        /// <param name="Minutes">Cookie 的过期时间（分钟）。</param>
        /// <param name="Path">要与此 Cookie 一起传输的虚拟路径。 默认为 /，也就是服务器根目录。</param>
        /// <param name="Domain">设置将此 Cookie 与其关联的域。默认值为当前域。（例如.baidu.com，如果是当前域名则该参数不要设置）</param>
        /// <param name="HttpOnly">如果 Cookie 具有 HttpOnly 特性且不能通过客户端脚本访问，则为 true；否则为 false。 默认值为 false。</param>
        /// <param name="Secure">如果通过 SSL 连接 (HTTPS) 传输 Cookie，则为 true；否则为 false。 默认值为 false。</param>
        /// <param name="Shareable">确定 cookie 是否允许参与输出缓存。</param>
        public static void Set<T>(T obj, string Name, double Minutes = 20D, string Path = "/", string Domain = "", bool HttpOnly = false, bool Secure = false, bool Shareable = false) where T : class, new()
        {
            string JsonStr = JsonHelper.Serialize(obj);
            HttpCookie cookie = BuildCookie(Name, Minutes, Path, Domain, HttpOnly, Secure, Shareable);
            cookie.Value = HttpUtility.UrlEncode(JsonStr);
            SetCookie(cookie);
        }

        /// <summary>
        /// 是否正常启用Cookie
        /// </summary>
        /// <returns></returns>
        public static bool CookieIsEnable()
        {
            try
            {
                SetCookie("OK", "CookieIsEnable"); //1分钟
                if(string.IsNullOrWhiteSpace(GetCookie("CookieIsEnable")))
                    return false;
                else
                    return true;
            }
            catch(Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// 设置Cookie
        /// </summary>
        /// <param name="Value">可以ToString()的object实体</param>
        /// <param name="Name">Cookie 的名称</param>
        /// <param name="Minutes">Cookie 的过期时间（分钟）。</param>
        /// <param name="Path">要与此 Cookie 一起传输的虚拟路径。 默认为 /，也就是服务器根目录。</param>
        /// <param name="Domain">设置将此 Cookie 与其关联的域。默认值为当前域。（例如.baidu.com，如果是当前域名则该参数不要设置）</param>
        /// <param name="HttpOnly">如果 Cookie 具有 HttpOnly 特性且不能通过客户端脚本访问，则为 true；否则为 false。 默认值为 false。</param>
        /// <param name="Secure">如果通过 SSL 连接 (HTTPS) 传输 Cookie，则为 true；否则为 false。 默认值为 false。</param>
        /// <param name="Shareable">确定 cookie 是否允许参与输出缓存。</param>
        public static void SetCookie(object Value, string Name, double Minutes = 20D, string Path = "/", string Domain = "", bool HttpOnly = false, bool Secure = false, bool Shareable = false)
        {
            if(null == Value) throw new ArgumentNullException("Value is null.");
            HttpCookie cookie = BuildCookie(Name, Minutes, Path, Domain, HttpOnly, Secure, Shareable);
            cookie.Value = Value.ToString();
            SetCookie(cookie);
        }

        /// <summary>
        /// 设置Cookie
        /// </summary>
        /// <param name="Value">Dictionary</param>
        /// <param name="Name">Cookie 的名称</param>
        /// <param name="Minutes">Cookie 的过期时间（分钟）。</param>
        /// <param name="Path">要与此 Cookie 一起传输的虚拟路径。 默认为 /，也就是服务器根目录。</param>
        /// <param name="Domain">设置将此 Cookie 与其关联的域。默认值为当前域。（例如.baidu.com，如果是当前域名则该参数不要设置）</param>
        /// <param name="HttpOnly">如果 Cookie 具有 HttpOnly 特性且不能通过客户端脚本访问，则为 true；否则为 false。 默认值为 false。</param>
        /// <param name="Secure">如果通过 SSL 连接 (HTTPS) 传输 Cookie，则为 true；否则为 false。 默认值为 false。</param>
        /// <param name="Shareable">确定 cookie 是否允许参与输出缓存。</param>
        public static void SetCookie(System.Collections.Generic.Dictionary<string, string> Value, string Name, double Minutes = 20D, string Path = "/", string Domain = "", bool HttpOnly = false, bool Secure = false, bool Shareable = false)
        {
            if(null == Value) throw new ArgumentNullException("Value is null.");
            HttpCookie cookie = BuildCookie(Name, Minutes, Path, Domain, HttpOnly, Secure, Shareable);
            foreach(var item in Value)
            {
                cookie.Values.Add(item.Key, item.Value);
            }
            SetCookie(cookie);
        }

        /// <summary>
        /// 设置Cookie
        /// </summary>
        /// <param name="Value">Dictionary</param>
        /// <param name="Name">Cookie 的名称</param>
        /// <param name="Minutes">Cookie 的过期时间（分钟）。</param>
        /// <param name="Path">要与此 Cookie 一起传输的虚拟路径。 默认为 /，也就是服务器根目录。</param>
        /// <param name="Domain">设置将此 Cookie 与其关联的域。默认值为当前域。（例如.baidu.com，如果是当前域名则该参数不要设置）</param>
        /// <param name="HttpOnly">如果 Cookie 具有 HttpOnly 特性且不能通过客户端脚本访问，则为 true；否则为 false。 默认值为 false。</param>
        /// <param name="Secure">如果通过 SSL 连接 (HTTPS) 传输 Cookie，则为 true；否则为 false。 默认值为 false。</param>
        /// <param name="Shareable">确定 cookie 是否允许参与输出缓存。</param>
        public static void SetCookie(System.Collections.Hashtable Value, string Name, double Minutes = 20D, string Path = "/", string Domain = "", bool HttpOnly = false, bool Secure = false, bool Shareable = false)
        {
            if(null == Value) throw new ArgumentNullException("Value is null.");
            HttpCookie cookie = BuildCookie(Name, Minutes, Path, Domain, HttpOnly, Secure, Shareable);
            foreach(System.Collections.DictionaryEntry item in Value)
            {
                cookie.Values.Add(item.Key.ToString(), item.Value.ToString());
            }
            SetCookie(cookie);
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