using System;
using System.IO;
using System.Net;
using System.Web;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

using System.Net.Security;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;

namespace ClassLib4Net.Http
{
	/// <summary>
	/// HTTP助手
	/// 熊学浩
	/// 2015-01-20
	/// </summary>
	public class HttpHelper
	{
		/// <summary>
		/// HttpRequest请求方式
		/// </summary>
		public enum Method
		{
			/// <summary>
			/// GET方式
			/// </summary>
			GET,
			/// <summary>
			/// POST方式
			/// </summary>
			POST
		}

		#region HTTP请求
		/// <summary>
		/// HTTP请求（get方式）默认utf-8编码
		/// 作者：熊学浩
		/// 时间：2014-5-27
		/// </summary>
		/// <param name="url">请求地址</param>
		/// <returns></returns>
		public static string Request(string url)
		{
			return Request(url, string.Empty, Method.GET, 5000, System.Text.Encoding.UTF8);
		}
		/// <summary>
		/// HTTP请求（get方式）默认utf-8编码
		/// 作者：熊学浩
		/// 时间：2014-5-27
		/// </summary>
		/// <param name="url">请求地址</param>
		/// <param name="timeout">请求超时时间（毫秒）</param>
		/// <returns></returns>
		public static string Request(string url, int timeout)
		{
			return Request(url, string.Empty, Method.GET, timeout, System.Text.Encoding.UTF8);
		}
		/// <summary>
		/// HTTP请求（get方式）默认utf-8编码
		/// 作者：熊学浩
		/// 时间：2014-5-27
		/// </summary>
		/// <param name="url">请求地址</param>
		/// <param name="timeout">请求超时时间（毫秒）</param>
		/// <param name="encode">编码</param>
		/// <returns></returns>
		public static string Request(string url, int timeout, System.Text.Encoding encode)
		{
			return Request(url, string.Empty, Method.GET, timeout, encode);
		}

		/// <summary>
		/// HTTP请求
		/// 作者：熊学浩
		/// 时间：2014-5-27
		/// </summary>
		/// <param name="url">请求地址</param>
		/// <param name="data">请求参数</param>
		/// <param name="method">请求方式</param>
		/// <param name="timeout">请求超时时间（毫秒）</param>
		/// <param name="encode">编码</param>
		/// <returns>HTTP请求返回结果</returns>
		public static string Request(string url, string data, Method method, int timeout, System.Text.Encoding encode)
		{
			string result = string.Empty;
			if (string.IsNullOrEmpty(url)) throw new Exception("目标url不能为空！");
			encode = encode != null ? encode : System.Text.Encoding.UTF8; //默认utf-8编码

			try
			{
				url = Uri.UnescapeDataString(url);
			}
			catch { };
			Regex re = new Regex("(?<h>[^\x00-\xff]+)");
			Match mc = re.Match(url);
			if (mc.Success)
			{
				string han = mc.Groups["h"].Value;
				//url = url.Replace(han, System.Web.HttpUtility.UrlEncode(han, Encoding.GetEncoding("GB2312")));
				url = url.Replace(han, System.Web.HttpUtility.UrlEncode(han, encode));
			}

			ServicePointManager.ServerCertificateValidationCallback = ValidateServerCertificate; //解决“基础连接已经关闭: 未能为SSL/TLS 安全通道建立信任关系”
			HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
			request.UseDefaultCredentials = true;
			request.Method = method.ToString();

			if (method == Method.GET)
			{
				request.ContentType = "text/html;charset=" + (encode == System.Text.Encoding.UTF8 ? "UTF-8" : "GB2312");
			}
			else if (method == Method.POST)
			{
				request.ContentType = "application/x-www-form-urlencoded";
				byte[] byteArray = encode.GetBytes(data);
				request.ContentLength = byteArray.Length;
				System.IO.Stream newStream = request.GetRequestStream();
				newStream.Write(byteArray, 0, byteArray.Length);
				newStream.Close();
			}
			request.UserAgent = "Mozilla/4.0 (compatible; MSIE 8.0; Windows NT 5.0; .NET CLR 1.1.4322; .NET CLR 2.0.50215;)";
			request.CookieContainer = new CookieContainer();
			request.AllowAutoRedirect = true;
			request.MaximumAutomaticRedirections = 3;
			if (method == Method.GET)
			{
				//request.KeepAlive = true;
				request.Timeout = timeout > 0 ? timeout : 5000; //5秒
			}
			else
			{
				request.KeepAlive = true;
				request.Timeout = timeout > 0 ? timeout : 50000; //50秒
			}

			try
			{
				HttpWebResponse response = (HttpWebResponse)request.GetResponse();

				//网页编码
				string CharSet = string.Empty;
				if (response.ContentType.ToLower().IndexOf("charset=") != -1)
				{
					try
					{
						CharSet = response.ContentType.Substring(response.ContentType.ToLower().IndexOf("charset=") + "charset=".Length);
					}
					catch
					{
						CharSet = Encoding.Default.BodyName;
					}
				}
				else if (response.ContentType.ToLower().IndexOf("encoding=") != -1)
				{
					try
					{
						CharSet = response.ContentType.Substring(response.ContentType.ToLower().IndexOf("encoding=") + "encoding=".Length);
					}
					catch
					{
						CharSet = Encoding.Default.BodyName;
					}
				}
				else
				{
					CharSet = response.CharacterSet;
				}

				var buffer = GetBytes(response);
				result = GetStringFromBuffer(buffer, ref CharSet);

				response.Close();
				request.Abort();
			}
			catch (Exception ex)
			{
				throw ex;
			}

			return result;
		}

		#region 不同网页编码处理
		/// <summary>
		/// 内存流，将流拷贝到byte数组中保存起来。
		/// 把流拷贝到内存里面可以重复使用。
		/// 作者：熊学浩
		/// 时间：2014-03-27
		/// </summary>
		/// <param name="response"></param>
		/// <returns></returns>
		private static byte[] GetBytes(WebResponse response)
		{
			var length = (int)response.ContentLength;
			byte[] data;

			using (var memoryStream = new MemoryStream())
			{
				var buffer = new byte[0x100];
				using (var rs = response.GetResponseStream())
				{
					for (var i = rs.Read(buffer, 0, buffer.Length); i > 0; i = rs.Read(buffer, 0, buffer.Length))
					{
						memoryStream.Write(buffer, 0, i);
					}
				}
				data = memoryStream.ToArray();
			}

			return data;
		}
		/// <summary>
		/// 从字节数组编码字符串
		/// 作者：熊学浩
		/// 时间：2014-03-27
		/// </summary>
		/// <param name="buffer"></param>
		/// <param name="charSet"></param>
		/// <returns></returns>
		private static string GetStringFromBuffer(byte[] buffer, ref string charSet)
		{
			if (string.IsNullOrEmpty(charSet) || string.Compare(charSet.Trim().ToUpper(), "ISO-8859-1") == 0)
			{
				charSet = GetEncodingFromBody(buffer);
			}

			try
			{
				var encoding = Encoding.GetEncoding(charSet);
				var str = encoding.GetString(buffer);
				return str;
			}
			catch (Exception ex)
			{
				return string.Empty;
			}
		}
		/// <summary>
		/// 当无法从Http Header中获得编码信息是就用ASCII编码从buffer中获得字符串。我们知道HTML的标签都是字母，使用ASCII编码虽然中文或者其他双字节字符会出现乱码，但是HTML标签还是能够解析出来。这样我们就可以检测HTML的meta标签从而获得charset。
		/// 作者：熊学浩
		/// 时间：2014-03-27
		/// </summary>
		/// <param name="buffer"></param>
		/// <returns></returns>
		private static string GetEncodingFromBody(byte[] buffer)
		{
			Regex regex = new Regex("charset=(?<encoding>[^=]+)?\"", RegexOptions.IgnoreCase | RegexOptions.Compiled);
			var str = Encoding.ASCII.GetString(buffer);
			var regMatch = regex.Match(str);
			if (regMatch.Success)
			{
				var charSet = regMatch.Groups["encoding"].Value.Trim();
				return charSet.Replace("\"", "").Trim();
			}

			return Encoding.ASCII.BodyName;
		}
		#endregion

		/// <summary>
		/// 解决“请求被中止: 未能创建 SSL/TLS 安全通道。”的问题
		/// http://radiumwong.iteye.com/blog/684118
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="certificate"></param>
		/// <param name="chain"></param>
		/// <param name="sslPolicyErrors"></param>
		/// <returns></returns>
		private static bool ValidateServerCertificate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
		{
			//总是接受
			return true;
		}
		#endregion

		#region 检查Request.Headers
		/// <summary>
		/// 检查Request.Headers请求头是否是正确的域名来源。（不区分大小写）
		/// Request.Headers["Host"]
		/// </summary>
		/// <param name="Request">Request</param>
		/// <param name="Host">Host值例如：www.1xuezhe.com</param>
		/// <returns></returns>
		public static bool CheckHeadersByHost(HttpRequestBase Request, string Host)
		{
			if (string.IsNullOrWhiteSpace(Host)) return false;
			if (Request != null && Request.Headers != null && Request.Headers.Count > 0)
			{
				if (Request.Headers["Host"] != null)
				{
					string host = Request.Headers["Host"]; //www.1xuezhe.com
					if (host.Trim().ToLower() == Host.Trim().ToLower())
						return true;
				}
			}
			return false;
		}
		/// <summary>
		/// 检查Request.Headers请求头是否是正确的域名来源。（不区分大小写）
		/// Request.Headers["Origin"]
		/// </summary>
		/// <param name="Request">Request</param>
		/// <param name="Origin">Origin值例如：http://www.1xuezhe.com</param>
		/// <returns></returns>
		public static bool CheckHeadersByOrigin(HttpRequestBase Request, string Origin)
		{
			if (string.IsNullOrWhiteSpace(Origin)) return false;
			if (Request != null && Request.Headers != null && Request.Headers.Count > 0)
			{
				if (Request.Headers["Origin"] != null)
				{
					string origin = Request.Headers["Origin"]; //http://www.1xuezhe.com
					if (origin.Trim().ToLower() == Origin.Trim().ToLower())
						return true;
				}
			}
			return false;
		}
		/// <summary>
		/// 检查Request.Headers请求头是否是正确的域名来源。（不区分大小写，全部转换小写字符比较时采用IndexOf算法）
		/// Request.Headers["Referer"]
		/// </summary>
		/// <param name="Request">Request</param>
		/// <param name="Referer">Referer值例如：http://www.1xuezhe.com/news </param>
		/// <returns></returns>
		public static bool CheckHeadersByReferer(HttpRequestBase Request, string Referer)
		{
			if (string.IsNullOrWhiteSpace(Referer)) return false;
			if (Request != null && Request.Headers != null && Request.Headers.Count > 0)
			{
				if (Request.Headers["Referer"] != null)
				{
					string referer = Request.Headers["Referer"]; //http://www.1xuezhe.com/member
					if (referer.Trim().ToLower().IndexOf(Referer.Trim().ToLower()) >= 0)
						return true;
				}
			}
			return false;
		}
		#endregion

		#region HTTP下载文件
		private static readonly char[] InvalidFileNameChars = new[]
                                                                  {
                                                                      '"',
                                                                      '<',
                                                                      '>',
                                                                      '|',
                                                                      '\0',
                                                                      '\u0001',
                                                                      '\u0002',
                                                                      '\u0003',
                                                                      '\u0004',
                                                                      '\u0005',
                                                                      '\u0006',
                                                                      '\a',
                                                                      '\b',
                                                                      '\t',
                                                                      '\n',
                                                                      '\v',
                                                                      '\f',
                                                                      '\r',
                                                                      '\u000e',
                                                                      '\u000f',
                                                                      '\u0010',
                                                                      '\u0011',
                                                                      '\u0012',
                                                                      '\u0013',
                                                                      '\u0014',
                                                                      '\u0015',
                                                                      '\u0016',
                                                                      '\u0017',
                                                                      '\u0018',
                                                                      '\u0019',
                                                                      '\u001a',
                                                                      '\u001b',
                                                                      '\u001c',
                                                                      '\u001d',
                                                                      '\u001e',
                                                                      '\u001f',
                                                                      ':',
                                                                      '*',
                                                                      '?',
                                                                      '\\',
                                                                      '/'
                                                                  };
		/// <summary>
		/// 清理文件名中无效的字符
		/// 熊学浩
		/// 2015-01-25
		/// </summary>
		/// <param name="fileName"></param>
		/// <returns></returns>
		public static string CleanInvalidFileName(string fileName)
		{
			fileName = fileName + "";
			fileName = InvalidFileNameChars.Aggregate(fileName, (current, c) => current.Replace(c + "", ""));

			if (fileName.Length > 1)
				if (fileName[0] == '.')
					fileName = "dot" + fileName.TrimStart('.');

			return fileName;
		}

		/// <summary>
		/// 从网上下载文件，到本地，并进行保存
		/// 熊学浩
		/// 2015-01-25
		/// </summary>
		/// <param name="url">网络资源文件绝对URL</param>
		/// <param name="savePath">保存位置绝对路径</param>
		/// <returns>保存位置绝对路径</returns>
		public static string HttpDownloadFile(string url, string savePath)
		{
			string fileName = string.Empty, folderPath = string.Empty;
			Match m_fileName = (new Regex(@"[^\\]+$", RegexOptions.IgnoreCase)).Match(savePath);
			if (m_fileName != null && m_fileName.Success)
				fileName = m_fileName.Value;
			folderPath = savePath.Replace(fileName, "");
			if (!Directory.Exists(folderPath)) //如果文件夹路径不存在就创建
				Directory.CreateDirectory(folderPath);

			try
			{
				Uri downUri = new Uri(url);
				//建立一个WEB请求，返回HttpWebRequest对象      
				HttpWebRequest hwr = (HttpWebRequest)WebRequest.Create(downUri);
				//设置接收对象的范围为0-10000000字节。
				//hwr.AddRange(0, 10000000);

				//流对象使用完后自动关闭
				using (Stream stream = hwr.GetResponse().GetResponseStream())
				{
					//文件流，流信息读到文件流中，读完关闭
					using (FileStream fs = File.Create(savePath)) // @"c:\gif.png"
					{
						//建立字节组，并设置它的大小是多少字节
						byte[] bytes = new byte[102400];
						int n = 1;
						while (n > 0)
						{
							//一次从流中读多少字节，并把值赋给Ｎ，当读完后，Ｎ为０,并退出循环
							n = stream.Read(bytes, 0, 10240);
							fs.Write(bytes, 0, n);　//将指定字节的流信息写入文件流中
						}
					}
				}
			}
			catch (Exception Ex)
			{
				throw Ex;
			}
			return savePath;
		}
		#endregion

		#region WebProxy Web代理
		/// <summary>
		/// 给HttpWebRequest添加代理服务
		/// 熊学浩
		/// 2015-01-26
		/// </summary>
		/// <param name="request">HttpWebRequest</param>
		/// <param name="proxyAddress">代理服务器的地址</param>
		/// <param name="BypassList">设置一个地址数组，这些地址不使用代理服务器</param>
		/// <param name="BypassProxyOnLocal">设置一个值，该值指示是否对本地地址不使用代理服务器</param>
		/// <param name="NetworkCredentialUserName">网关用户名，设置提交给代理服务器进行身份验证的凭据</param>
		/// <param name="NetworkCredentialPassword">网关密码，设置提交给代理服务器进行身份验证的凭据</param>
		/// <returns></returns>
		public static HttpWebRequest SetProxy(HttpWebRequest request, string proxyAddress, string[] BypassList, bool BypassProxyOnLocal, string NetworkCredentialUserName, string NetworkCredentialPassword)
		{
			if (request == null) return null;
			if (string.IsNullOrWhiteSpace(proxyAddress)) return request;

			WebProxy proxyObject = new WebProxy();
			proxyObject.Address = new Uri(proxyAddress);

			if (BypassList != null && BypassList.Length > 0)
				proxyObject.BypassList = BypassList;
			proxyObject.BypassProxyOnLocal = BypassProxyOnLocal;
			if (!string.IsNullOrWhiteSpace(NetworkCredentialUserName) && !string.IsNullOrWhiteSpace(NetworkCredentialPassword))
			{
				proxyObject.Credentials = new NetworkCredential(NetworkCredentialUserName, NetworkCredentialPassword);
				proxyObject.UseDefaultCredentials = true;
			}

			request.Proxy = proxyObject;
			return request;
		}
		/// <summary>
		/// 给HttpWebRequest添加代理服务
		/// 熊学浩
		/// 2015-01-26
		/// </summary>
		/// <param name="request">HttpWebRequest</param>
		/// <param name="proxyAddress">代理服务器的地址</param>
		/// <param name="BypassList">设置一个地址数组，这些地址不使用代理服务器</param>
		/// <param name="NetworkCredentialUserName">网关用户名，设置提交给代理服务器进行身份验证的凭据</param>
		/// <param name="NetworkCredentialPassword">网关密码，设置提交给代理服务器进行身份验证的凭据</param>
		/// <returns></returns>
		public static HttpWebRequest SetProxy(HttpWebRequest request, string proxyAddress, string[] BypassList, string NetworkCredentialUserName, string NetworkCredentialPassword)
		{
			return SetProxy(request, proxyAddress, BypassList, true, NetworkCredentialUserName, NetworkCredentialPassword);
		}
		/// <summary>
		/// 给HttpWebRequest添加代理服务
		/// 熊学浩
		/// 2015-01-26
		/// </summary>
		/// <param name="request">HttpWebRequest</param>
		/// <param name="proxyAddress">代理服务器的地址</param>
		/// <param name="NetworkCredentialUserName">网关用户名，设置提交给代理服务器进行身份验证的凭据</param>
		/// <param name="NetworkCredentialPassword">网关密码，设置提交给代理服务器进行身份验证的凭据</param>
		/// <returns></returns>
		public static HttpWebRequest SetProxy(HttpWebRequest request, string proxyAddress, string NetworkCredentialUserName, string NetworkCredentialPassword)
		{
			return SetProxy(request, proxyAddress, null, true, NetworkCredentialUserName, NetworkCredentialPassword);
		}
		/// <summary>
		/// 给HttpWebRequest添加代理服务
		/// 熊学浩
		/// 2015-01-26
		/// </summary>
		/// <param name="request">HttpWebRequest</param>
		/// <param name="proxyAddress">代理服务器的地址</param>
		/// <param name="BypassList">设置一个地址数组，这些地址不使用代理服务器</param>
		/// <returns></returns>
		public static HttpWebRequest SetProxy(HttpWebRequest request, string proxyAddress, string[] BypassList)
		{
			return SetProxy(request, proxyAddress, BypassList, true, string.Empty, string.Empty);
		}
		/// <summary>
		/// 给HttpWebRequest添加代理服务
		/// 熊学浩
		/// 2015-01-26
		/// </summary>
		/// <param name="request">HttpWebRequest</param>
		/// <param name="proxyAddress">代理服务器的地址</param>
		/// <returns></returns>
		public static HttpWebRequest SetProxy(HttpWebRequest request, string proxyAddress)
		{
			return SetProxy(request, proxyAddress, null, true, string.Empty, string.Empty);
		}
		#endregion

	}
}
