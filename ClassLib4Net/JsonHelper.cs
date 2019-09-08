using System;
using System.IO;
using System.Runtime.Serialization.Json;

using System.Web.Script.Serialization;

namespace ClassLib4Net
{
    /// <summary>
    /// Json帮助类
    /// </summary>
    public static class JsonHelper
    {
        static readonly JavaScriptSerializer Serializer = new JavaScriptSerializer();

        /// <summary>
        /// 将一个对象转换成json格式字符串
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <returns></returns>
        public static string ToJson<T>(this T t)
        {
            return Serializer.Serialize(t);
        }

        /// <summary>
        /// 将json格式字符串转换成强类型
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="json"></param>
        /// <returns></returns>
        public static T ToObject<T>(this string json)
        {
            return Serializer.Deserialize<T>(json);
        }

		#region 获取Json某一key值
		/// <summary>
		/// 从json字符串中获取key的值
		/// </summary>
		/// <param name="jsonString"></param>
		/// <param name="key"></param>
		/// <returns></returns>
		public static string GetJosnValue(this string jsonString, string key)
		{
			string result = string.Empty;
			if (!string.IsNullOrEmpty(jsonString))
			{
				key = "\"" + key.Trim('"') + "\"";
				int index = jsonString.IndexOf(key) + key.Length + 1;
				if (index > key.Length + 1)
				{
					//先截逗号，若是最后一个，截“｝”号，取最小值

					int end = jsonString.IndexOf(',', index);
					if (end == -1)
					{
						end = jsonString.IndexOf('}', index);
					}
					//index = json.IndexOf('"', index + key.Length + 1) + 1;
					result = jsonString.Substring(index, end - index);
					//过滤引号或空格
					result = result.Trim(new char[] { '"', ' ', '\'' });
				}
			}
			return result;
		} 
		#endregion

		#region JSON序列化
		/// <summary>
		/// 将对象序列化成json字符串
		/// </summary>
		/// <typeparam name="T">对象类型</typeparam>
		/// <param name="o">对象实例</param>
		/// <returns>json字符串</returns>
		public static string Serialize<T>(this T o)
		{
			if (o == null) return string.Empty;
			string result = string.Empty;
			try
			{
				using (MemoryStream memoryStream = new MemoryStream())
				{
					DataContractJsonSerializer json = new DataContractJsonSerializer(typeof(T));
					json.WriteObject(memoryStream, o);
					result = System.Text.Encoding.UTF8.GetString(memoryStream.ToArray());

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
		/// 将对象序列化成json字符串
		/// </summary>
		/// <param name="o">对象实例</param>
		/// <returns>json字符串</returns>
		public static string Serialize(this object o)
		{
			if (o == null || o == System.DBNull.Value) return string.Empty;
			string result = string.Empty;
			using (MemoryStream memoryStream = new MemoryStream())
			{
				DataContractJsonSerializer json = new DataContractJsonSerializer(o.GetType());
				json.WriteObject(memoryStream, o);
				result = System.Text.Encoding.UTF8.GetString(memoryStream.ToArray());

				memoryStream.Close();
			}
			return result;
		}
        #endregion

        #region JSON反序列化
        /// <summary>
        /// 将字节流（原json字符串）反序列化成对象
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="buffer">字节流（原json字符串）</param>
        /// <returns></returns>
        public static T DeSerialize<T>(this byte[] buffer)
        {
            try
            {
                using(MemoryStream memoryStream = new MemoryStream(buffer))
                {
                    DataContractJsonSerializer json = new DataContractJsonSerializer(typeof(T));
                    T o = (T)json.ReadObject(memoryStream);

                    memoryStream.Close();
                    return o;
                }
            }
            catch(Exception Ex)
            {
                throw Ex;
            }
            return default(T);
        }

        /// <summary>
        /// 将json字符串（默认UTF8编码）反序列化成对象
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="jsonText">json字符串文本（默认UTF8编码）</param>
        /// <returns>对象</returns>
        public static T DeSerialize<T>(this string jsonText)
		{
			try
			{
				using (MemoryStream memoryStream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(jsonText)))
				{
					DataContractJsonSerializer json = new DataContractJsonSerializer(typeof(T));
					T o = (T)json.ReadObject(memoryStream);

					memoryStream.Close();
					return o;
				}
			}
			catch (Exception Ex)
			{
				throw Ex;
			}
			return default(T);
		}

		#endregion
    }
}