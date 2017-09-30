using System;
using System.Web;
using System.Text.RegularExpressions;
using System.IO;

namespace ClassLib4Net
{
    /// <summary>
    /// Request帮助类
    /// </summary>
    public class RequestHelper
    {
        /// <summary>
        /// 根据路径获取映射路径
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string MapPath(string path)
        {
            if (HttpContext.Current != null)
            {
                return HttpContext.Current.Server.MapPath(path);
            }
            else //非web程序引用
            {
                path = Regex.Replace(path, @"\A~?/", "").Replace("/", @"\");
                return Path.Combine(AppDomain.CurrentDomain.SetupInformation.ApplicationBase, path);
            }
        }

        /// <summary>
        /// 根据key获取url参数值
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string Query(string key)
        {
            var str = HttpContext.Current.Request.QueryString[key];
            return String.IsNullOrEmpty(str) ? string.Empty : HttpUtility.UrlDecode(Safe(str.Trim()));
        }

        /// <summary>
        /// 获取原始url
        /// </summary>
        /// <returns></returns>
        public static string RawUrl()
        {
            string rawUrl = HttpContext.Current.Request.ServerVariables["HTTP_X_REWRITE_URL"];
            if (string.IsNullOrEmpty(rawUrl))
            {
                return HttpContext.Current.Request.RawUrl;
            }
            else
            {
                return string.Format("http://{0}{1}", HttpContext.Current.Request.Url.Host, rawUrl);
            }
        }

        /// <summary>
        /// 获取当前url参数部分
        /// </summary>
        /// <returns></returns>
        public static string UrlParams()
        {
            var url = RawUrl();
            return url.Substring(url.IndexOf('?') + 1);
        }

        /// <summary>
        /// 根据参数key获取参数值
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string Params(string key)
        {
            var val = HttpContext.Current.Request.Params[key];
            return val ?? string.Empty;
        }

        /// <summary>
        /// 获取key获取Form值
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string Form(string key)
        {
            var str = HttpContext.Current.Request.Form[key];
            return str == null ? string.Empty : str.Trim();
        }

        /// <summary>
        /// url安全过滤
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string Safe(string str)
        {
            return str.Replace("'", string.Empty);
        }

        /// <summary>
        /// 移除Html标签
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string NonHtml(string str)
        {
            return Regex.Replace(str, @"</?[^<]+>", "");
        }

        /// <summary>
        /// 获取客户端ip
        /// </summary>
        /// <returns></returns>
        public static string GetIp(HttpContext context = null)
        {
            context = context ?? HttpContext.Current;
            if (context == null) return "127.0.0.1";
            var remoteAddr = context.Request.ServerVariables["REMOTE_ADDR"];
            var httpVia = context.Request.ServerVariables["HTTP_VIA"];
            var ip = String.IsNullOrEmpty(httpVia) ? remoteAddr : httpVia;
            return ip == "::1" ? "127.0.0.1" : ip;
        }

        /// <summary>
        /// 从页面请求获取整型
        /// </summary>
        /// <param name="str">请求key</param>
        /// <param name="defaultValue">默认值</param>
        /// <returns></returns>
        public static int GetInt(string str, int defaultValue = 0, HttpContext context = null)
        {
            context = context ?? HttpContext.Current;
            int result;
            if (!int.TryParse(context.Request[str], out result)) result = defaultValue;
            return result;
        }

        /// <summary>
        /// 从页面请求获取时间
        /// </summary>
        /// <param name="str">请求key</param>
        /// <param name="defatulValue">默认值</param>
        /// <returns>获得的日期</returns>
        public static DateTime GetDate(string str, DateTime defatulValue)
        {
            DateTime date;
            if (!DateTime.TryParse(HttpContext.Current.Request[str], out date)) date = defatulValue;
            return date;
        }

        /// <summary>
        /// 301跳转
        /// </summary>
        /// <param name="url">要跳转的地址</param>
        public static void Rediret301(string url)
        {
            Rediret301(HttpContext.Current, url);
        }

        /// <summary>
        /// 301跳转
        /// </summary>
        /// <param name="context">http上下文</param>
        /// <param name="url">要跳转的地址</param>
        public static void Rediret301(HttpContext context, string url)
        {
            context.Response.Clear();
            context.Response.Status = "301 Moved Permanently";
            context.Response.AddHeader("Location", url);
            context.Response.End();
        }

    }
}