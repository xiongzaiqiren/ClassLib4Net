using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace ClassLib4Net
{
	/// <summary>
	/// Xml助手
	/// </summary>
	public class XmlHelper
	{
		#region XML序列化
		/// <summary>
		/// 将对象序列化成xml字符串
		/// </summary>
		/// <typeparam name="T">对象类型</typeparam>
		/// <param name="o">对象实例</param>
		/// <returns>xml字符串</returns>
		public static string Serialize<T>(T o)
		{
			if (o == null) return string.Empty;
			string result = string.Empty;
			try
			{
				using (MemoryStream memoryStream = new MemoryStream())
				{
					XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
					ns.Add("", ""); //Add an empty namespace and empty value
					XmlWriterSettings writerSet = new XmlWriterSettings()
					{
						CloseOutput = false,
						Encoding = System.Text.Encoding.UTF8,
						//OmitXmlDeclaration = true, //忽略XML声明 <?xml version="1.0" encoding="utf-8"?>
						Indent = false
					};
					XmlWriter writer = XmlWriter.Create(memoryStream, writerSet);
					XmlSerializer formatter = new XmlSerializer(typeof(T));
					formatter.Serialize(writer, o, ns);
					writer.Close();

					memoryStream.Position = 0;
					StreamReader reader = new StreamReader(memoryStream);
					result = reader.ReadToEnd();
					memoryStream.Close();
				}
			}
			catch (Exception Ex)
			{
				throw Ex;
			}
			return result;
		}

		/// <summary>
		/// 将对象序列化成xml字符串
		/// </summary>
		/// <param name="o">对象实例</param>
		/// <returns>xml字符串</returns>
		public static string Serialize(object o)
		{
			if (o == null || o == System.DBNull.Value) return string.Empty;
			string result = string.Empty;
			using (MemoryStream memoryStream = new MemoryStream())
			{
				XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
				ns.Add("", ""); //Add an empty namespace and empty value
				XmlWriterSettings writerSet = new XmlWriterSettings()
				{
					CloseOutput = false,
					Encoding = System.Text.Encoding.UTF8,
					//OmitXmlDeclaration = true, //忽略XML声明 <?xml version="1.0" encoding="utf-8"?>
					Indent = false
				};
				XmlWriter writer = XmlWriter.Create(memoryStream, writerSet);
				XmlSerializer formatter = new XmlSerializer(o.GetType());
				formatter.Serialize(writer, o, ns);
				writer.Close();

				memoryStream.Position = 0;
				StreamReader reader = new StreamReader(memoryStream);
				result = reader.ReadToEnd();
				memoryStream.Close();
			}
			return result;
		}

		/// <summary>
		/// 将对象序列化到指定xml文件
		/// </summary>
		/// <typeparam name="T">对象类型</typeparam>
		/// <param name="o">对象实例</param>
		/// <param name="filePath">完整xml文件路径</param>
		public static void Serialize<T>(T o, string filePath)
		{
			try
			{
				XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
				ns.Add("", ""); //Add an empty namespace and empty value
				XmlSerializer formatter = new XmlSerializer(typeof(T));
				StreamWriter sw = new StreamWriter(filePath, false);
				formatter.Serialize(sw, o, ns);
				sw.Flush();
				sw.Close();
			}
			catch (Exception Ex)
			{
				throw Ex;
			}
		}
		#endregion

		#region XML反序列化
		/// <summary>
		/// 将xml字符串反序列化成对象
		/// </summary>
		/// <typeparam name="T">对象类型</typeparam>
		/// <param name="xmlText">xml字符串文本</param>
		/// <returns>对象</returns>
		public static T DeSerialize<T>(string xmlText)
		{
			try
			{
				TextReader textReader = new StringReader(xmlText);
				XmlSerializer formatter = new XmlSerializer(typeof(T), string.Empty);
				T o = (T)formatter.Deserialize(textReader);
				textReader.Close();
				textReader.Dispose();
				return o;
			}
			catch (Exception Ex)
			{
				throw Ex;
			}
			return default(T);
		}

		/// <summary>
		/// 将指定的xml文件反序列化成对象
		/// </summary>
		/// <typeparam name="T">对象类型</typeparam>
		/// <param name="filePath">完整xml文件路径</param>
		/// <returns>对象</returns>
		public static T DeSerializeByPath<T>(string filePath)
		{
			try
			{
				XmlSerializer formatter = new XmlSerializer(typeof(T));
				StreamReader sr = new StreamReader(filePath);
				T o = (T)formatter.Deserialize(sr);
				sr.Close();
				return o;
			}
			catch (Exception Ex)
			{
				throw Ex;
			}
			return default(T);
		}
		#endregion

		#region Xml实体值转义
		/*
		* CDATA标签用于说明数据不被XML解析器解析。
		* 下面是五个在XML文件中预定义好的实体：
		* &lt;	< 小于号
		* &gt; > 大于号
		* &amp; & 和
		* &apos; ' 单引号
		* &quot; " 双引号
		* 实体必须以符号"&"开头，以符号";"结尾。
		* 注意: 只有"<" 字符和"&"字符对于XML来说是严格禁止使用的。剩下的都是合法的，为了减少出错，使用实体是个好习惯。
		*/

		/// <summary>
		/// Xml实体值转义标签字符，将标签等特殊字符转义成预定义实体
		/// </summary>
		/// <param name="XmlText">Xml实体值</param>
		/// <returns></returns>
		public static string XmlEncode(string XmlText)
		{
			if (string.IsNullOrWhiteSpace(XmlText)) return string.Empty;
			return XmlText
				.Replace("<", "&lt;")
				.Replace(">", "&gt;")
				.Replace("&", "&amp;")
				.Replace("'", "&apos;")
				.Replace("\"", "&quot;");
		}
		/// <summary>
		/// Xml值反转义标签字符，将预定义实体反转义成原标签等特殊字符
		/// </summary>
		/// <param name="XmlText">Xml实体值</param>
		/// <returns></returns>
		public static string XmlDecode(string XmlText)
		{
			if (string.IsNullOrWhiteSpace(XmlText)) return string.Empty;
			return XmlText
				.Replace("&lt;", "<")
				.Replace("&gt;", ">")
				.Replace("&amp;", "&")
				.Replace("&apos;", "'")
				.Replace("&quot;", "\"");
		}
		#endregion

		/// <summary>
		/// 接收XML，调用示例：System.Xml.XmlDocument xml = WeiXinAPI.XmlHelper.GetXML(Request, Request.ContentEncoding)
		/// </summary>
		/// <param name="request">System.Web.HttpRequestBase</param>
		/// <param name="charset">编码格式</param>
		/// <returns>XML</returns>
		public static System.Xml.XmlDocument LoadXML(System.Web.HttpRequestBase request, System.Text.Encoding charset)
		{
			try
			{
				System.IO.Stream inputstream = request.InputStream;
				byte[] b = new byte[inputstream.Length];
				inputstream.Read(b, 0, (int)inputstream.Length);
				string inputstr = (charset != null ? charset : System.Text.Encoding.UTF8).GetString(b);
				System.Xml.XmlDocument xml = new System.Xml.XmlDocument();
				xml.LoadXml(inputstr);

				return xml;
			}
			catch (Exception Ex)
			{
				throw Ex;
			}
		}

	}
}
