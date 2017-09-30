using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.IO;
using System.Web.Script.Serialization;
using System.Xml;

#region Common{xml:Serialize/string:Deserialize}
/// <summary>
/// 本类提供了对象的常用扩展方法
/// </summary>
public static class ObjectHelper
{
	/// <summary>
	/// 对可序列化对象（[Serializable]）进行序列化，默认使用常用的UTF8编码
	/// </summary>
	/// <param name="obj">序列化对象</param>
	/// <param name="encoding">Encoding,默认使用UTF8</param>
	/// <returns>XML字符串</returns>
	public static string DefaultSerialize(this object obj, Encoding encoding = null)
	{
		encoding = encoding ?? Encoding.UTF8;
		XmlSerializer xmlSerializer = new XmlSerializer(obj.GetType());
		MemoryStream stream = new MemoryStream();
		xmlSerializer.Serialize(stream, obj);
		byte[] buf = stream.ToArray();
		string xml = encoding.GetString(buf);
		stream.Close();
		return xml;
	}
	/// <summary>
	/// 对可序列化对象（[Serializable]）进行序列化，默认使用常用的UTF8编码，同时在版本信息里加入编码声明
	/// </summary>
	/// <param name="obj">序列化对象</param>
	/// <param name="encoding">Encoding,默认使用UTF8</param>
	/// <returns>XML字符串</returns>
	public static string DefaultSerialize2(this object obj, Encoding encoding = null)
	{
		encoding = encoding ?? Encoding.UTF8;
		using (MemoryStream stream = new MemoryStream())
		{
			using (XmlTextWriter writer = new XmlTextWriter(stream, encoding))
			{
				writer.Formatting = Formatting.Indented;
				XmlSerializer xmlSerializer = new XmlSerializer(obj.GetType());
				xmlSerializer.Serialize(writer, obj);
				byte[] buf = stream.ToArray();
				var xml = encoding.GetString(buf);
				return xml;
			}
		}
	}

	/// <summary>
	/// 将字符串反序列化成指定对象，默认使用常用的UTF8编码
	/// </summary>
	/// <typeparam name="T">目标对象</typeparam>
	/// <param name="str">反序化的字符串对象</param>
	/// <param name="encoding">Encoding,默认使用UTF8</param>
	/// <returns>指定对象</returns>
	public static T DefaultDeserialize<T>(this string str, Encoding encoding = null)
	{
		encoding = encoding ?? Encoding.UTF8;
		XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));
		byte[] buf = encoding.GetBytes(str);
		MemoryStream stream = new MemoryStream(buf);
		T obj = (T)xmlSerializer.Deserialize(stream);
		stream.Close();
		return obj;
	}
	/// <summary>
	/// 将当前对象通过json"弱"的特性强制转换成另一个结构有重叠的对象
	/// </summary>
	/// <typeparam name="T">目标对象类型</typeparam>
	/// <param name="obj">转换对象</param>
	/// <returns>目标对象</returns>
	public static T JsonCast<T>(this object obj)
	{
		JavaScriptSerializer jsonSerializer = new JavaScriptSerializer();
		string jsonString = jsonSerializer.Serialize(obj);
		return jsonSerializer.Deserialize<T>(jsonString);
	}

}
#endregion