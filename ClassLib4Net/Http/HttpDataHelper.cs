using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace ClassLib4Net.Http
{
	/// <summary>
	/// HTTP数据传输助手
	/// </summary>
	public class HttpDataHelper
	{
		/// <summary>
		/// http Get
		/// </summary>
		/// <param name="url"></param>
		/// <param name="postParameter"></param>
		/// <returns></returns>
		public static string HttpWebRequestGet(string url, List<UrlParameter> postParameter = null)
		{
			return HttpWebRequest(url, false, postParameter);
		}

		/// <summary>
		/// http Post
		/// </summary>
		/// <param name="url"></param>
		/// <param name="postParameter"></param>
		/// <returns></returns>
		public static string HttpWebRequestPost(string url, List<UrlParameter> postParameter)
		{
			return HttpWebRequest(url, true, postParameter);
		}

		/// <summary>
		/// 下载远程文
		/// </summary>
		/// <param name="url">远程文件url</param>
		/// <param name="savePath">本地保存路径</param>
		/// <param name="fileName">保存文件名</param>
		/// <param name="isAsync">是否异步下载</param>
		/// <param name="repeatTimesForSync">同步下载重试次数</param>
		public static void HttpDownloadFile(string url, string savePath, string fileName, bool isAsync = false, int repeatTimesForSync = 1)
		{
			if (!Directory.Exists(savePath))
			{
				Directory.CreateDirectory(savePath);
			}
			savePath = GetDirPath(savePath) + fileName;

			WebClient webClient = new WebClient() { Encoding = Encoding.UTF8 };

			if (!isAsync)
			{
				for (int i = 1; i <= repeatTimesForSync; i++)
				{
					try
					{
						webClient.DownloadFile(url, savePath);
						break;
					}
					catch (Exception err)
					{
						if (i == repeatTimesForSync)
						{
							throw err;
						}
					}
				}
			}
			else
			{
				string asyncToken = url + "|" + savePath + "|" + DateTime.Now.ToString("yyyyMMddhhmmsss");
				webClient.DownloadFileAsync(new Uri(url), savePath, asyncToken);
			}
		}

		/// <summary>
		/// 从url获得数据
		/// </summary>
		/// <param name="url"></param>
		/// <param name="isPost"></param>
		/// <param name="postParameter"></param>
		/// <returns></returns>
		public static string HttpWebRequest(string url, bool isPost = false, List<UrlParameter> postParameter = null)
		{
			string reStr = string.Empty;
			WebClient webClient = new WebClient() { Encoding = Encoding.UTF8 };
			string postStr = string.Empty;
			if (postParameter != null)
			{
				postStr = string.Join("&", postParameter.Select(c => c.key + "=" + c.value));
			}

			if (!isPost)
			{
				url += url.IndexOf("?") > -1 ? "&" : "?" + postStr;
				reStr = webClient.DownloadString(url);
			}
			else
			{
				webClient.Headers.Add("Content-Type", "application/x-www-form-urlencoded");
				reStr = webClient.UploadString(url, postStr);
			}

			return reStr;
		}
		/// <summary>
		/// 从url获得数据
		/// </summary>
		/// <param name="url"></param>
		/// <param name="isPost"></param>
		/// <returns></returns>
		public static string HttpWebRequest(string url, bool isPost = false)
		{
			string reStr = string.Empty;
			WebClient webClient = new WebClient() { Encoding = Encoding.UTF8 };
			if (!isPost)
			{
				reStr = webClient.DownloadString(url);
			}
			else
			{
				webClient.Headers.Add("Content-Type", "text/xml");
				reStr = webClient.DownloadString(url);
			}

			return reStr;
		}
		/// <summary>
		/// 获得文件夹路径 例 c:\haha => c:\haha\
		/// </summary>
		/// <param name="path"></param>
		/// <returns></returns>
		public static string GetDirPath(string path)
		{
			if (path[path.Length - 1] != System.IO.Path.DirectorySeparatorChar)
			{
				path += System.IO.Path.DirectorySeparatorChar;
			}
			return path;
		}

		/// <summary>
		/// URL参数过滤正则表达式
		/// </summary>
		public static string Reg_UrlFilter = @"(\s{1,}(and|or|xp_cmdshell|sysname|sp_addlogin|rename|restore)\s{1,})|(\s{1,}(count|chr|mid|char|db_name)\s*)|(\s*(exec|drop|insert|select|delete|update|truncate|declare|master|net user)\s{1,})|(script|')"; //防止SQL注入

		/// <summary>
		/// 获取url参数列表
		/// 熊学浩
		/// 2013-07-02
		/// </summary>
		/// <param name="url">传入url参数，例如：Request.Url.AbsoluteUri </param>
		/// <returns></returns>
		public static Dictionary<string, string> HttpWebRequest(string url)
		{
			if (string.IsNullOrEmpty(url)) return null;
			url = System.Web.HttpUtility.UrlDecode(url, System.Text.Encoding.Default);
			Dictionary<string, string> paras = new Dictionary<string, string>();
			System.Text.RegularExpressions.Regex regex = new System.Text.RegularExpressions.Regex(Reg_UrlFilter);
			string[] QueryStr;
			string[] QueryList;
			string[] QueryValue;
			if (url.Contains("?"))
			{
				QueryStr = null;
				QueryStr = url.Split('?');
				if (!string.IsNullOrEmpty(QueryStr[1]))
				{
					QueryList = null;
					QueryList = QueryStr[1].Split('&');
					foreach (string s in QueryList)
					{
						if (s.Contains('='))
						{
							QueryValue = null;
							QueryValue = s.Split('=');
							if (!paras.Keys.Contains(QueryValue[0].Trim()))
								paras.Add(QueryValue[0].Trim(), regex.Replace(QueryValue[1].Trim(), ""));
						}
						else
						{
							if (!paras.Keys.Contains(s.Trim()))
								paras.Add(s.Trim(), "");
						}
					}
				}
			}

			return paras;
		}

		/// <summary>
		/// 获取get/post请求参数列表
		/// 熊学浩
		/// 2013-07-17
		/// </summary>
		/// <param name="request">System.Web.HttpRequestBase</param>
		/// <returns></returns>
		public static Dictionary<string, string> HttpWebRequest(System.Web.HttpRequestBase request)
		{
			Dictionary<string, string> paras = new Dictionary<string, string>();
			System.Text.RegularExpressions.Regex regex = new System.Text.RegularExpressions.Regex(Reg_UrlFilter);
			string key = "", value = "";
			if (string.Compare(request.HttpMethod.ToUpper(), "GET") == 0)
			{
				try
				{
					for (int i = 0; i < (request.QueryString).AllKeys.Length; i++)
					{
						if (string.IsNullOrEmpty((request.QueryString).AllKeys[i])) continue;
						key = (request.QueryString).AllKeys[i];
						value = request.QueryString[key];
						key = System.Web.HttpUtility.UrlDecode(key, System.Text.Encoding.UTF8).Trim();
						value = System.Web.HttpUtility.UrlDecode(value, System.Text.Encoding.UTF8).Trim();
						paras.Add(key, regex.Replace(value, ""));
					}
				}
				catch (Exception Ex)
				{
					throw Ex;
				}
			}
			else if (string.Compare(request.HttpMethod.ToUpper(), "POST") == 0)
			{
				try
				{
					for (int i = 0; i < (request.Form).AllKeys.Length; i++)
					{
						if (string.IsNullOrEmpty((request.Form).AllKeys[i])) continue;
						key = (request.Form).AllKeys[i];
						value = request.Form[key];
						key = System.Web.HttpUtility.UrlDecode(key, System.Text.Encoding.UTF8).Trim();
						value = System.Web.HttpUtility.UrlDecode(value, System.Text.Encoding.UTF8).Trim();
						paras.Add(key, regex.Replace(value, " "));
					}
				}
				catch (Exception Ex)
				{
					throw Ex;
				}
			}
			else if (string.Compare(request.HttpMethod.ToUpper(), "HEAD") == 0) { }
			if (paras.Count == 0)
			{
				try
				{
					for (int i = 0; i < (request.QueryString).AllKeys.Length; i++)
					{
						if (string.IsNullOrEmpty((request.QueryString).AllKeys[i])) continue;
						key = (request.QueryString).AllKeys[i];
						value = request.QueryString[key];
						key = System.Web.HttpUtility.UrlDecode(key, System.Text.Encoding.UTF8).Trim();
						value = System.Web.HttpUtility.UrlDecode(value, System.Text.Encoding.UTF8).Trim();
						paras.Add(key, regex.Replace(value, " "));
					}
				}
				catch (Exception Ex)
				{
					throw Ex;
				}
			}
			return paras;
		}

		/// <summary>
		/// 获取get/post请求文件参数列表
		/// 熊学浩
		/// 2013-07-19
		/// </summary>
		/// <param name="request">System.Web.HttpRequestBase</param>
		/// <returns></returns>
		public static List<FileParameter> HttpWebRequestFile(System.Web.HttpRequestBase request)
		{
			List<FileParameter> paras = new List<FileParameter>();
			FileParameter file;
			for (int i = 0; i < request.Files.Count; i++)
			{
				if (request.Files[i] == null) continue;
				file = new FileParameter();
				file.key = System.Web.HttpUtility.UrlDecode(request.Files.AllKeys[i], System.Text.Encoding.UTF8).Trim();
				file.fileBase = request.Files[i];
				paras.Add(file);
			}
			return paras;
		}

		/// <summary>
		/// 接收HTTP流传输的图片
		/// </summary>
		/// <param name="request"></param>
		/// <param name="flag"></param>
		/// <returns>Image</returns>
		public static System.Drawing.Image HttpWebRequestImage(System.Web.HttpRequestBase request)
		{
			if (request.Files.Count > 0)
			{
				var img = System.Drawing.Image.FromStream(request.Files[0].InputStream);
				return img;
			}
			else if (request.ContentLength > 0 && request.InputStream != null)
			{
				var img = System.Drawing.Image.FromStream(request.InputStream);
				return img;
			}
			else return null;
		}

		/// <summary>
		/// 接收HTTP流传输的字符串数据包
		/// </summary>
		/// <param name="request"></param>
		/// <param name="encode"></param>
		/// <returns></returns>
		public static string HttpWebRequestInputStream(System.Web.HttpRequestBase request, Encoding encode)
		{
			if (request == null || request.InputStream == null || !request.InputStream.CanRead) return string.Empty;
			string result = string.Empty;
			Stream _InputStream = request.InputStream;
			using (StreamReader sr = new StreamReader(_InputStream, encode))
			{
				result = sr.ReadToEnd();
				sr.Close();
				sr.Dispose();
			}
			return result;
		}

		/// <summary>
		/// 发送get/post请求,当包含文件类型的数据时默认为post方式请求
		/// 注意：如果传输文件参数，则必须文件存在且可读
		/// 熊学浩
		/// 2013-07-18
		/// </summary>
		/// <param name="url">目标url</param>
		/// <param name="data">要发送的数据 (格式为：name1=value1&amp;name2=value2)</param>
		/// <param name="datafile">要发送的文件数据 (格式为：file1=C:/test.mp3&amp;file2=C:/test.jpg)</param>
		/// <param name="method">请求方式 get/post</param>
		/// <param name="encode">编码格式 System.Text.Encoding</param>
		/// <returns></returns>
		public static string HttpWebRequest(string url, string data, string datafile, string method, System.Text.Encoding encode)
		{
			string ret = string.Empty;
			if (string.IsNullOrEmpty(url)) throw new Exception("目标url不能为空！");
			encode = encode != null ? encode : System.Text.Encoding.UTF8; //默认utf-8编码
			try
			{
				if (string.IsNullOrEmpty(datafile))
				{
					/*** 字符串类型的请求，get/post方式 ***/
					if (string.IsNullOrEmpty(method)) method = "GET";
					else method = method.Trim().ToUpper() == "POST" ? "POST" : "GET";
					if (method == "GET")
					{
						if (url.IndexOf("?") > 0)
						{
							if (data.IndexOf("&") == 0)
							{
								url += "&" + data;
							}
						}
						else
							url += "?" + data;
					}

					System.Net.HttpWebRequest webReq = (System.Net.HttpWebRequest)System.Net.WebRequest.Create(new Uri(url));
					webReq.Method = method;
					webReq.Timeout = 500000;

					if (webReq.Method == "GET")
					{
						webReq.ContentType = "text/html;charset=" + (encode == System.Text.Encoding.UTF8 ? "UTF-8" : "GB2312");
					}
					else if (webReq.Method == "POST")
					{
						byte[] byteArray = encode.GetBytes(data);
						webReq.ContentType = "application/x-www-form-urlencoded";
						webReq.ContentLength = byteArray.Length;
						System.IO.Stream newStream = webReq.GetRequestStream();
						newStream.Write(byteArray, 0, byteArray.Length);
						newStream.Close();
					}

					System.Net.HttpWebResponse webRes = (System.Net.HttpWebResponse)webReq.GetResponse();
					System.IO.StreamReader sr = new System.IO.StreamReader(webRes.GetResponseStream(), encode);
					ret = sr.ReadToEnd();
					sr.Close();
					sr.Dispose();
					webRes.Close();
				}
				else
				{
					/*** 文件请求，默认post方式 ***/
					method = "POST";
					string[] parasList, parasListSub;

					long length = 0;
					string boundary = "--" + DateTime.Now.Ticks.ToString("x");

					System.Net.HttpWebRequest webReq = (System.Net.HttpWebRequest)System.Net.WebRequest.Create(url);
					webReq.ContentType = "multipart/form-data; boundary=" + boundary;
					webReq.Method = method;
					webReq.Timeout = 500000;
					webReq.KeepAlive = true;
					webReq.Credentials = System.Net.CredentialCache.DefaultCredentials;

					System.IO.Stream memStream = new System.IO.MemoryStream();

					byte[] boundarybytes = System.Text.Encoding.ASCII.GetBytes("\r\n--" + boundary + "\r\n");
					memStream.Write(boundarybytes, 0, boundarybytes.Length);
					length += boundarybytes.Length;

					string headerTemplate = "";
					string header = "";
					byte[] headerbytes;
					//写入字符串类型参数
					if (!string.IsNullOrEmpty(data))
					{
						headerTemplate = "Content-Disposition: form-data; name=\"{0}\" \r\n\r\n{1}";
						parasList = System.Text.RegularExpressions.Regex.Split(System.Web.HttpUtility.UrlDecode(data, encode).Trim(), "&");
						foreach (string sub in parasList)
						{
							parasListSub = System.Text.RegularExpressions.Regex.Split(sub, "=");
							if (parasListSub.Length < 2) continue;
							header = string.Format(headerTemplate, parasListSub[0], parasListSub[1]);
							headerbytes = encode.GetBytes(header);
							memStream.Write(headerbytes, 0, headerbytes.Length);
							length += headerbytes.Length;

							memStream.Write(boundarybytes, 0, boundarybytes.Length);
							length += boundarybytes.Length;
						}
					}

					//写入文件类型参数
					if (!string.IsNullOrEmpty(datafile))
					{
						headerTemplate = "Content-Disposition: form-data; name=\"{0}\"; filename=\"{1}\" \r\nContent-Type: application/octet-stream\r\n\r\n";
						parasList = System.Text.RegularExpressions.Regex.Split(System.Web.HttpUtility.UrlDecode(datafile, encode).Trim(), "&");
						foreach (string sub in parasList)
						{
							parasListSub = System.Text.RegularExpressions.Regex.Split(sub, "=");
							if (parasListSub.Length < 2) continue;
							header = string.Format(headerTemplate, parasListSub[0], parasListSub[1]);
							headerbytes = encode.GetBytes(header);
							memStream.Write(headerbytes, 0, headerbytes.Length);
							length += headerbytes.Length;

							System.IO.FileStream fileStream = new System.IO.FileStream(parasListSub[1], System.IO.FileMode.Open, System.IO.FileAccess.Read);
							byte[] buffer = new byte[1024];
							int bytesRead = 0;
							while ((bytesRead = fileStream.Read(buffer, 0, buffer.Length)) != 0)
							{
								memStream.Write(buffer, 0, bytesRead);
								length += bytesRead;
							}

							memStream.Write(boundarybytes, 0, boundarybytes.Length);
							length += boundarybytes.Length;

							fileStream.Close();
						}
					}
					webReq.ContentLength = memStream.Length;

					System.IO.Stream s = webReq.GetRequestStream();
					memStream.Position = 0;
					byte[] tempBuffer = new byte[memStream.Length];
					memStream.Read(tempBuffer, 0, tempBuffer.Length);
					memStream.Close();
					s.Write(tempBuffer, 0, tempBuffer.Length);
					s.Close();

					System.Net.HttpWebResponse webRes = (System.Net.HttpWebResponse)webReq.GetResponse();
					System.IO.StreamReader sr = new System.IO.StreamReader(webRes.GetResponseStream(), encode);
					ret = sr.ReadToEnd();
					sr.Close();
					sr.Dispose();
					webRes.Close();
				}
			}
			catch (Exception ex)
			{
				throw ex;
			}
			return ret;
		}

		/// <summary>
		/// 发送get/post请求,当包含文件类型的数据时默认为post方式请求
		/// 熊学浩
		/// 2013-07-22
		/// </summary>
		/// <param name="url">目标url</param>
		/// <param name="data">要发送的数据 (格式为：name1=value1&amp;name2=value2)</param>
		/// <param name="datafile">要发送的二进制文件数据 格式为：FileByteParameter </param>
		/// <param name="method">请求方式 get/post</param>
		/// <param name="encode">编码格式 System.Text.Encoding</param>
		/// <returns></returns>
		public static string HttpWebRequest(string url, string data, List<FileByteParameter> datafile, string method, System.Text.Encoding encode)
		{
			if (datafile == null || datafile.Count == 0) { return HttpWebRequest(url, data, "", method, encode); }
			if (string.IsNullOrEmpty(url)) throw new Exception("目标url不能为空！");
			encode = encode != null ? encode : System.Text.Encoding.UTF8; //默认utf-8编码
			string ret = string.Empty;
			try
			{
				/*** 文件请求，默认post方式 ***/
				if (string.IsNullOrEmpty(method)) method = "POST";
				else method = method.Trim().ToUpper() == "GET" ? "GET" : "POST";

				string[] parasList, parasListSub;
				long length = 0;
				string boundary = "--" + DateTime.Now.Ticks.ToString("x");

				System.Net.HttpWebRequest webReq = (System.Net.HttpWebRequest)System.Net.WebRequest.Create(url);
				webReq.ContentType = "multipart/form-data; boundary=" + boundary;
				webReq.Method = method;
				webReq.Timeout = 500000;
				webReq.KeepAlive = true;
				webReq.Credentials = System.Net.CredentialCache.DefaultCredentials;

				System.IO.Stream memStream = new System.IO.MemoryStream();
				byte[] boundarybytes = System.Text.Encoding.ASCII.GetBytes("\r\n--" + boundary + "\r\n");
				memStream.Write(boundarybytes, 0, boundarybytes.Length);
				length += boundarybytes.Length;

				string headerTemplate = "";
				string header = "";
				byte[] headerbytes;

				//写入字符串类型参数
				if (!string.IsNullOrEmpty(data))
				{
					headerTemplate = "Content-Disposition: form-data; name=\"{0}\" \r\n\r\n{1}";
					parasList = System.Text.RegularExpressions.Regex.Split(System.Web.HttpUtility.UrlDecode(data, encode).Trim(), "&");
					foreach (string sub in parasList)
					{
						parasListSub = System.Text.RegularExpressions.Regex.Split(sub, "=");
						if (parasListSub.Length < 2) continue;
						header = string.Format(headerTemplate, parasListSub[0], parasListSub[1]);
						headerbytes = encode.GetBytes(header);
						memStream.Write(headerbytes, 0, headerbytes.Length);
						length += headerbytes.Length;

						memStream.Write(boundarybytes, 0, boundarybytes.Length);
						length += boundarybytes.Length;
					}
				}

				//写入文件类型参数
				headerTemplate = "Content-Disposition: form-data; name=\"{0}\"; filename=\"{1}\" \r\nContent-Type: application/octet-stream\r\n\r\n";
				foreach (FileByteParameter sub in datafile)
				{
					if (sub.filebyte == null || sub.filebyte.Length == 0) continue;
					header = string.Format(headerTemplate, sub.key, sub.filename);
					headerbytes = encode.GetBytes(header);
					memStream.Write(headerbytes, 0, headerbytes.Length);
					length += headerbytes.Length;

					memStream.Write(sub.filebyte, 0, sub.filebyte.Length);
					length += sub.filebyte.Length;

					memStream.Write(boundarybytes, 0, boundarybytes.Length);
					length += boundarybytes.Length;
				}

				webReq.ContentLength = memStream.Length;

				System.IO.Stream s = webReq.GetRequestStream();
				memStream.Position = 0;
				byte[] tempBuffer = new byte[memStream.Length];
				memStream.Read(tempBuffer, 0, tempBuffer.Length);
				memStream.Close();
				s.Write(tempBuffer, 0, tempBuffer.Length);
				s.Close();

				System.Net.HttpWebResponse webRes = (System.Net.HttpWebResponse)webReq.GetResponse();
				System.IO.StreamReader sr = new System.IO.StreamReader(webRes.GetResponseStream(), encode);
				ret = sr.ReadToEnd();
				sr.Close();
				sr.Dispose();
				webRes.Close();
			}
			catch (Exception ex)
			{
				throw ex;
			}
			return "";
		}

		/// <summary>
		/// 发送图片
		/// </summary>
		/// <param name="url"></param>
		/// <param name="postData"></param>
		/// <param name="encode"></param>
		/// <returns></returns>
		public static string HttpWebRequest(string url, byte[] postData, System.Text.Encoding encode)
		{
			string reStr = string.Empty;

			System.Net.WebClient webClient = new System.Net.WebClient() { Encoding = encode };
			webClient.Headers.Add("Content-Type", "image/jpeg");
			webClient.Headers.Add("ContentLength", postData.Length.ToString());
			byte[] reByte = webClient.UploadData(url, postData);
			reStr = encode.GetString(reByte);

			return reStr;
		}

		/// <summary>
		/// 读取文件并转换为二进制
		/// 熊学浩
		/// 2013-07-22
		/// </summary>
		/// <param name="path">文件物理路径</param>
		/// <returns>二进制数据</returns>
		public static byte[] OpenFile(string path)
		{
			if (string.IsNullOrEmpty(path)) return null;
			if (!System.IO.File.Exists(path)) throw new Exception("找不到指定的文件！");
			Byte[] byteArray = null;
			try
			{
				System.IO.FileStream file = new System.IO.FileStream(path, System.IO.FileMode.Open, System.IO.FileAccess.Read);
				byteArray = new byte[file.Length];
				file.Read(byteArray, 0, byteArray.Length);
				file.Close();
			}
			catch (Exception Ex)
			{
				byteArray = null;
				throw Ex;
			}
			return byteArray;
		}

		/// <summary>
		/// 从Host获取顶级（一级）域名
		/// （例如：www.1xuezhe.exuezhe.com将返回exuezhe.com）
		/// </summary>
		/// <param name="host">Request.Url.Host</param>
		/// <returns></returns>
		public static string GetDomainByHost(string host)
		{
			if (string.IsNullOrEmpty(host)) return string.Empty;
			System.Text.RegularExpressions.Regex regex = new System.Text.RegularExpressions.Regex(@"\w+\.\w{2,3}$", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
			System.Text.RegularExpressions.Match match = regex.Match(host);
			if (match != null && match.Success)
				return match.Value;
			return string.Empty;
		}
		/// <summary>
		/// 从URL中获取域名
		/// （例如：http://www.1xuezhe.exuezhe.com/activitys/2048SKDS/new.html将返回www.1xuezhe.exuezhe.com）
		/// </summary>
		/// <param name="url"></param>
		/// <returns></returns>
		public static string GetDomainByURL(string url)
		{
			if (string.IsNullOrEmpty(url)) return string.Empty;
			System.Text.RegularExpressions.Regex regex = new System.Text.RegularExpressions.Regex(@"\/\/[\w\.]+\/", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
			System.Text.RegularExpressions.Match match = regex.Match(url);
			if (match != null && match.Success)
			{
				return match.Value.Trim(new Char[] { '/' });
			}
			return string.Empty;
		}

	}

	/// <summary>
	/// url 参数
	/// 熊学浩
	/// 2013-07-18
	/// </summary>
	public class UrlParameter
	{
		/// <summary>
		/// 参数键
		/// </summary>
		public string key { get; set; }
		/// <summary>
		/// 参数值
		/// </summary>
		public string value { get; set; }
	}

	/// <summary>
	/// file 参数
	/// 熊学浩
	/// 2013-07-18
	/// </summary>
	public class FileParameter
	{
		/// <summary>
		/// 作为参数传递的key
		/// 保存的结果文件名(不含后缀名)
		/// </summary>
		public string key { get; set; }
		/// <summary>
		/// 文件参数
		/// </summary>
		public System.Web.HttpPostedFileBase fileBase { get; set; }
	}

	/// <summary>
	/// file Byte参数
	/// 熊学浩
	/// 2013-07-21
	/// </summary>
	public class FileByteParameter
	{
		/// <summary>
		/// 作为参数传递的key
		/// 保存的结果文件名(不含后缀名)
		/// </summary>
		public string key { get; set; }
		/// <summary>
		/// 文件名（含后缀名）
		/// </summary>
		public string filename { get; set; }
		/// <summary>
		/// 文件的二进制数组
		/// </summary>
		public byte[] filebyte { get; set; }
	}

}