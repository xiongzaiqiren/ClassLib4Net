using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Web;

namespace ClassLib4Net
{
    /// <summary>
    /// 服务器/系统信息
    /// </summary>
    public class SystemInfo
    {
        #region 获取服务器信息
        /// <summary>
        /// 获取服务器信息
        /// </summary>
        /// <returns></returns>
        public static string GetServerInfo()
        {
            string text = string.Empty;

            text += "<font>服务器OS：</font>" + Environment.OSVersion.ToString() + "<br />";
            text += "<font>CPU核心数：</font>" + Environment.GetEnvironmentVariable("NUMBER_OF_PROCESSORS") + "个核心<br />";
            text += "<font>CPU类型：</font>" + Environment.GetEnvironmentVariable("PROCESSOR_IDENTIFIER") + "<br />";
            text += "<font>IIS版本：</font>" + HttpContext.Current.Request.ServerVariables["SERVER_SOFTWARE"] + "<br />";
            text += "<font>服务器名：</font>" + HttpContext.Current.Server.MachineName + "<br />";
            text += "<font>服务器域名：</font>" + HttpContext.Current.Request.ServerVariables["SERVER_NAME"] + "<br />";
            text += "<font>服务器IP：</font>" + HttpContext.Current.Request.ServerVariables["LOCAL_ADDR"] + "<br />";
            text += "<font>虚拟服务绝对路径：</font>" + HttpContext.Current.Request.ServerVariables["APPL_PHYSICAL_PATH"] + "<br />";
            text += "<font>.Net版本：</font>" + ".NET CLR " + Environment.Version.ToString() + "<br />";
            text += "<font>脚本超时时间：</font>" + HttpContext.Current.Server.ScriptTimeout.ToString() + "秒<br />";
            text += "<font>开机运行时长：</font>" + ((Double)System.Environment.TickCount / 3600000).ToString("N2") + "小时<br />";
            text += "<font>Session总数：</font>" + HttpContext.Current.Session.Contents.Count.ToString() + "<br />";
            text += "<font>应用程序缓存总数：</font>" + HttpContext.Current.Cache.Count.ToString() + "<br />";

            /*
            text += "<br /><br /><br />";
            foreach (String item in HttpContext.Current.Request.ServerVariables)
            {
                text += string.Format("<font>{0}：</font>{1}<br />", item, HttpContext.Current.Request.ServerVariables[item]);
            }
            */
            return text;
        }

        /// <summary>
        /// 获取服务器OS
        /// </summary>
        /// <returns></returns>
        public static string OSVersion()
        {
            return Environment.OSVersion.ToString();
        }

        /// <summary>
        /// 获取服务器处理器个数
        /// </summary>
        /// <returns></returns>
        public static string NUMBER_OF_PROCESSORS()
        {
            return Environment.GetEnvironmentVariable("NUMBER_OF_PROCESSORS");
        }
        /// <summary>
        /// 获取服务器处理器标识
        /// </summary>
        /// <returns></returns>
        public static string PROCESSOR_IDENTIFIER()
        {
            return Environment.GetEnvironmentVariable("PROCESSOR_IDENTIFIER");
        }
        /// <summary>
        /// 获取服务器IIS版本
        /// </summary>
        /// <returns></returns>
        public static string SERVER_SOFTWARE()
        {
            return HttpContext.Current.Request.ServerVariables["SERVER_SOFTWARE"];
        }
        /// <summary>
        /// 获取服务器名
        /// </summary>
        /// <returns></returns>
        public static string MachineName()
        {
            return HttpContext.Current.Server.MachineName;
        }
        /// <summary>
        /// 获取服务器域名
        /// </summary>
        /// <returns></returns>
        public static string SERVER_NAME()
        {
            return HttpContext.Current.Request.ServerVariables["SERVER_NAME"];
        }
        /// <summary>
        /// 获取请求方法
        /// </summary>
        /// <returns></returns>
        public static string Request_Method()
        {
            return HttpContext.Current.Request.ServerVariables["Request_Method"];
        }
        /// <summary>
        /// 返回內容如：gzip,deflate
        /// </summary>
        /// <returns></returns>
        public static string Http_Accept_Language()
        {
            return HttpContext.Current.Request.ServerVariables["Http_Accept_Language"];
        }
        /// <summary>
        /// 支持的文件类型
        /// </summary>
        /// <returns></returns>
        public static string HTTP_Accept()
        {
            return HttpContext.Current.Request.ServerVariables["HTTP_Accept"];
        }
        /// <summary>
        /// 获取服务器IP
        /// </summary>
        /// <returns></returns>
        public static string LOCAL_ADDR()
        {
            return HttpContext.Current.Request.ServerVariables["LOCAL_ADDR"];
        }
        /// <summary>
        /// 获取虚拟服务绝对路径
        /// </summary>
        /// <returns></returns>
        public static string APPL_PHYSICAL_PATH()
        {
            return HttpContext.Current.Request.ServerVariables["APPL_PHYSICAL_PATH"];
        }
        /// <summary>
        /// 运行环境版本
        /// </summary>
        /// <returns></returns>
        public static string Version()
        {
            return Environment.Version.ToString();
        }
        /// <summary>
        /// 获取服务器允许的脚本超时时间（秒）
        /// </summary>
        /// <returns></returns>
        public static int ScriptTimeout()
        {
            return HttpContext.Current.Server.ScriptTimeout;
        }
        /// <summary>
        /// 获取服务器开机运行时长（小时）
        /// </summary>
        /// <returns></returns>
        public static string RuningTime()
        {
            return ((Double)System.Environment.TickCount / 3600000).ToString("N2");
        }
        /// <summary>
        /// 获取服务器Session总数
        /// </summary>
        /// <returns></returns>
        public static int SessionCount()
        {
            return HttpContext.Current.Session.Contents.Count;
        }
        /// <summary>
        /// 获取服务器Cache总数
        /// </summary>
        /// <returns></returns>
        public static int CacheCount()
        {
            return HttpContext.Current.Cache.Count;
        }

        #endregion

        #region 获取客户端信息
        /// <summary>
        /// 获取客户端IP
        /// </summary>
        /// <returns></returns>
        public static string Remote_Addr()
        {
            return HttpContext.Current.Request.ServerVariables.Get("Remote_Addr");
        }
        /// <summary>
        /// 获取客户端主机
        /// </summary>
        /// <returns></returns>
        public static string Remote_Host()
        {
            return HttpContext.Current.Request.ServerVariables.Get("Remote_Host");
        }
        /// <summary>
        /// 客户端浏览器
        /// </summary>
        /// <returns></returns>
        public static string Browser()
        {
            return HttpContext.Current.Request.Browser.Browser;
        }
        /// <summary>
        /// 客户端浏览器主版本号
        /// </summary>
        /// <returns></returns>
        public static int BrowserMajorVersion()
        {
            return HttpContext.Current.Request.Browser.MajorVersion;
        }
        /// <summary>
        /// 获取客户端操作系统
        /// </summary>
        /// <returns></returns>
        public static string BrowserPlatform()
        {
            return HttpContext.Current.Request.Browser.Platform;
        }
        #endregion

        #region 获取ip地址
        /// <summary>
        /// 获得本机ipv4地址
        /// </summary>
        /// <param name="ipAddressList"></param>
        /// <returns></returns>
        public static string GetIpv4(IPAddress[] ipAddressList)
        {
            string ip = string.Empty;
            if (ipAddressList.Length > 0)
            {
                foreach (IPAddress _ipAddress in ipAddressList)
                {
                    //从IP地址列表中筛选出IPv4类型的IP地址
                    //AddressFamily.InterNetwork表示此IP为IPv4,
                    //AddressFamily.InterNetworkV6表示此地址为IPv6类型
                    if (_ipAddress.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                    {
                        ip = _ipAddress.ToString();
                    }
                }
            }
            return ip;
        }
        /// <summary>
        /// 获得本机ipv4地址
        /// </summary>
        /// <returns></returns>
        public static string GetIpv4()
        {
            return GetIpv4(Dns.GetHostEntry(Dns.GetHostName()).AddressList);
        }

        /// <summary> 
        /// 获取当前请求的IP地址 
        /// </summary> 
        /// <returns></returns> 
        public static string GetIP()
        {
            /*
				http代理相关知识
				关键就在HTTP_X_FORWARDED_FOR
				使用不同种类代理服务器，上面的信息会有所不同：

				一、没有使用代理服务器的情况：
				REMOTE_ADDR = 您的 IP
				HTTP_VIA = 没数值或不显示
				HTTP_X_FORWARDED_FOR = 没数值或不显示

				二、使用透明代理服务器的情况：Transparent Proxies
				REMOTE_ADDR = 代理服务器 IP 
				HTTP_VIA = 代理服务器 IP
				HTTP_X_FORWARDED_FOR = 您的真实 IP
				这类代理服务器还是将您的信息转发给您的访问对象，无法达到隐藏真实身份的目的。

				三、使用普通匿名代理服务器的情况：Anonymous Proxies
				REMOTE_ADDR = 代理服务器 IP 
				HTTP_VIA =
			 */
            //获取IP地址 
            HttpRequest request = HttpContext.Current.Request;
            string ipAddress = string.Empty;
            if (request.ServerVariables["HTTP_X_FORWARDED_FOR"] == null || request.ServerVariables["HTTP_X_FORWARDED_FOR"] == "")
            {
                ipAddress = request.ServerVariables["REMOTE_ADDR"];
            }
            else if (request.ServerVariables["HTTP_X_FORWARDED_FOR"].IndexOf(",") >= 0)
            {
                int index = request.ServerVariables["HTTP_X_FORWARDED_FOR"].IndexOf(",");
                ipAddress = request.ServerVariables["HTTP_X_FORWARDED_FOR"].Substring(0, index - 1);
            }
            else if (request.ServerVariables["HTTP_X_FORWARDED_FOR"].IndexOf(";") >= 0)
            {
                int index = request.ServerVariables["HTTP_X_FORWARDED_FOR"].IndexOf(";");
                ipAddress = request.ServerVariables["HTTP_X_FORWARDED_FOR"].Substring(0, index - 1);
            }
            else
            {
                ipAddress = request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            }
            if (ipAddress == "127.0.0.1")
            {
                ipAddress = GetIpv4();
            }
            return ipAddress;
        }

        /// <summary>
        /// 获取ip地址
        /// 例如:112.168.121.123
        /// </summary>
        /// <param name="page">页面，一般为this</param>
        /// <returns></returns>
        public static string GetIP(System.Web.UI.Page page)
        {
            if (page != null)
                return page.Request.UserHostAddress;
            else
                return string.Empty;
        }
        /// <summary>
        /// 获取ip地址
        /// 例如:112.168.121.123
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns></returns>
        public static string GetIP(HttpRequestBase request)
        {
            if (request != null)
                return request.UserHostAddress;
            else
                return string.Empty;
        }
        /// <summary>
        /// 获取ip地址，获取权限显示的ip
        /// 例如:112.168.121.123
        /// 结果:112.*.*.123
        /// </summary>
        /// <param name="ip"></param>
        /// <returns></returns>
        public static string GetHiddenIP(string ip)
        {
            if (!string.IsNullOrEmpty(ip))
                return StringHelper.Split(ip, ".")[0] + ".*.*." + StringHelper.Split(ip, ".")[3];
            else
                return string.Empty;
        }
        #endregion

    }
}
