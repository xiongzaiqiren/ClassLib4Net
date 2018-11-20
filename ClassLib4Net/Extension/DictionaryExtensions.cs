using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ClassLib4Net.Extension.Dictionary
{
    /// <summary>
    /// 字典扩展
    /// </summary>
    public static class DictionaryExtensions
    {
        /// <summary>
        /// 根据key获取value
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dictionary"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static T GetValue<T>(this Dictionary<string, T> dictionary, string key)
        {
            return GetValue(dictionary, key, default(T));
        }
        /// <summary>
        /// 根据key获取value
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dictionary"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static T GetValue<T>(this SortedDictionary<string, T> dictionary, string key)
        {
            return GetValue(dictionary, key, default(T));
        }

        /// <summary>
        /// 根据key获取value
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dictionary"></param>
        /// <param name="key"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static T GetValue<T>(this Dictionary<string, T> dictionary, string key, T defaultValue)
        {
            return dictionary == null ? default(T) : dictionary.ContainsKey(key) ? dictionary[key] : defaultValue;
        }
        /// <summary>
        /// 根据key获取value
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dictionary"></param>
        /// <param name="key"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static T GetValue<T>(this SortedDictionary<string, T> dictionary, string key, T defaultValue)
        {
            return dictionary == null ? default(T) : dictionary.ContainsKey(key) ? dictionary[key] : defaultValue;
        }

        /// <summary>
        /// 添加键值对数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dictionary"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static Dictionary<string, T> AddValue<T>(this Dictionary<string, T> dictionary, string key, T value)
        {
            if(dictionary.ContainsKey(key))
                dictionary[key] = value;
            else
                dictionary.Add(key, value);
            return dictionary;
        }
        /// <summary>
        /// 添加键值对数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dictionary"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static SortedDictionary<string, T> AddValue<T>(this SortedDictionary<string, T> dictionary, string key, T value)
        {
            if(dictionary.ContainsKey(key))
                dictionary[key] = value;
            else
                dictionary.Add(key, value);
            return dictionary;
        }

        /// <summary>
        /// 添加键值对数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dictionary"></param>
        /// <param name="keys"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        public static Dictionary<string, T> AddValues<T>(this Dictionary<string, T> dictionary, string[] keys, T[] values)
        {
            var i = 0;
            foreach(var key in keys)
            {
                dictionary.AddValue(key, i >= values.Length ? default(T) : values[i]);
                i++;
            }
            return dictionary;
        }
        /// <summary>
        /// 添加键值对数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dictionary"></param>
        /// <param name="keys"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        public static SortedDictionary<string, T> AddValues<T>(this SortedDictionary<string, T> dictionary, string[] keys, T[] values)
        {
            var i = 0;
            foreach(var key in keys)
            {
                dictionary.AddValue(key, i >= values.Length ? default(T) : values[i]);
                i++;
            }
            return dictionary;
        }

        #region 参数字典排序
        /// <summary>
        /// 将集合内非空参数值的参数按照参数名ASCII码从小到大排序（字典序）
        /// </summary>
        /// <param name="dictionary">参数（键值对）字典</param>
        /// <param name="isAsc">参数（键值对）字典的主键排序方式</param>
        /// <returns></returns>
        public static Dictionary<string, string> Sort(this Dictionary<string, string> dictionary, bool isAsc = true)
        {
            Dictionary<string, string> newDictionarys = new Dictionary<string, string>();
            foreach(KeyValuePair<string, string> item in dictionary)
            {
                if(!string.IsNullOrWhiteSpace(item.Key) && !string.IsNullOrWhiteSpace(item.Value))
                {
                    newDictionarys.Add(item.Key, item.Value);
                }
            }
            if(isAsc)
                return newDictionarys.OrderBy(o => o.Key).ToDictionary(o => o.Key, p => p.Value); //将集合内非空参数值的参数按照参数名ASCII码从小到大排序（字典序）
            else
                return newDictionarys.OrderByDescending(o => o.Key).ToDictionary(o => o.Key, p => p.Value); //将集合内非空参数值的参数按照参数名ASCII码从大到小排序（字典降序）
        }

        /// <summary>
        /// 将参数字典转换成URL键值对的格式（即 key1=value1&amp;key2=value2…）
        /// </summary>
        /// <param name="dictionary">参数（键值对）字典</param>
        /// <param name="removeEmpty">是否排除空项（健或值为空或空字符串）</param>
        /// <param name="urlEncode">是否Value进行URL编码</param>
        /// <returns></returns>
        public static string ToURLParameter(this Dictionary<string, string> dictionary, bool removeEmpty = false, bool urlEncode = false)
        {
            if(dictionary == null || dictionary.Count < 1) return string.Empty;
            StringBuilder sb = new StringBuilder();
            foreach(KeyValuePair<string, string> item in dictionary)
            {
                if(removeEmpty && (string.IsNullOrWhiteSpace(item.Key) || string.IsNullOrWhiteSpace(item.Value)))
                    continue;
                sb.AppendFormat("{0}={1}&", item.Key, urlEncode ? System.Web.HttpUtility.UrlEncode(item.Value) : item.Value);
            }
            return sb.ToString().Trim(new char[] { '&' });
        }
        #endregion

        #region SortedDictionary
        /// <summary>
        /// 将参数字典转换成URL键值对的格式（即 key1=value1&amp;key2=value2…）
        /// </summary>
        /// <param name="sd">参数（键值对）字典</param>
        /// <param name="removeEmpty">是否排除空项（健或值为空或空字符串）</param>
        /// <param name="urlEncode">是否Value进行URL编码</param>
        /// <returns></returns>
        public static string ToURLParameter(this SortedDictionary<string, string> sd, bool removeEmpty = false, bool urlEncode = false)
        {
            if(null == sd || sd.Count < 1) return string.Empty;
            StringBuilder sb = new StringBuilder();
            foreach(KeyValuePair<string, string> item in sd)
            {
                if(removeEmpty && (string.IsNullOrWhiteSpace(item.Key) || string.IsNullOrWhiteSpace(item.Value)))
                    continue;
                sb.AppendFormat("&{0}={1}", item.Key, urlEncode ? System.Web.HttpUtility.UrlEncode(item.Value) : item.Value);
            }
            return sb.ToString().Trim(new Char[] { '&' });
        }
        #endregion

    }
}
