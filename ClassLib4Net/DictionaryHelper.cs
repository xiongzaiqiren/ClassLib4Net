using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ClassLib4Net
{
    /// <summary>
    /// 字典帮助类
    /// </summary>
    public class DictionaryHelper
    {
        #region XML与参数字典
        /// <summary>
        /// 字典转换XML字符串
        /// </summary>
        /// <param name="dictionary">字典</param>
        /// <returns>XML字符串</returns>
        public static string DictionaryToXml(Dictionary<string, string> dictionary)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("<xml>");
            foreach (KeyValuePair<string, string> pair in dictionary)
            {
                if (string.IsNullOrWhiteSpace(pair.Key) || string.IsNullOrWhiteSpace(pair.Value)) continue;
                if (System.Text.RegularExpressions.Regex.IsMatch(pair.Value, "[<>&\'\"]+"))
                    sb.Append("<" + pair.Key + "><![CDATA[" + pair.Value + "]]></" + pair.Key + ">");
                else
                    sb.Append("<" + pair.Key + ">" + pair.Value + "</" + pair.Key + ">");
            }
            sb.Append("</xml>");
            return sb.ToString();
        }

        /// <summary>
        /// XML文档转换字典
        /// </summary>
        /// <param name="doc">XmlDocument</param>
        /// <returns></returns>
        public static Dictionary<string, string> XmlToDictionary(System.Xml.XmlDocument doc)
        {
            Dictionary<string, string> dictionary = new Dictionary<string, string>();
            foreach (System.Xml.XmlElement element in doc.DocumentElement.ChildNodes)
            {
                string key = element.Name;
                string value = element.InnerText;
                if (value != "")
                {
                    dictionary.Add(key, value);
                }
            }
            return dictionary;
        }
        #endregion

    }
}