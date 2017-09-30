using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Reflection;
using System.Xml;

namespace ClassLib4Net
{
    /// <summary>
    /// List转换数据集类
    /// </summary>
    public class ListHelper
    {
        #region  xml转换成List
        /// <summary>
        /// xml转换成List
        /// </summary>
        /// <typeparam name="T">转换类型</typeparam>
        /// <param name="xml">xml字符串</param>
        /// <param name="headtag">xml tag</param>
        /// <returns>List结果集</returns>
        public static List<T> XmlToObjList<T>(string xml, string headtag) where T : new()
        {
            var list = new List<T>();
            var doc = new XmlDocument();
            PropertyInfo[] propinfos = null;
            doc.LoadXml(xml);
            var nodelist = doc.SelectNodes(headtag);
            if (nodelist != null)
            {
                foreach (XmlNode node in nodelist)
                {
                    var entity = new T();
                    //初始化propertyinfo
                    if (propinfos == null)
                    {
                        var objtype = entity.GetType();
                        propinfos = objtype.GetProperties();
                    }
                    //填充entity类的属性
                    foreach (var propinfo in propinfos)
                    {
                        var cnode = node.SelectSingleNode(propinfo.Name);
                        if (cnode != null)
                        {
                            var v = cnode.InnerXml;
                            propinfo.SetValue(entity, Convert.ChangeType(v, propinfo.PropertyType), null);
                        }
                        else
                        {
                            if (node.FirstChild != null && !node.FirstChild.HasChildNodes)
                            {
                                var v = node.InnerText;
                                propinfo.SetValue(entity, Convert.ChangeType(v, propinfo.PropertyType), null);
                            }
                        }
                    }
                    list.Add(entity);
                }
            }
            return list;
        }
        #endregion

        #region  List结果集转换成xml字符串
        /// <summary>
        /// List结果集转换成xml字符串
        /// </summary>
        /// <typeparam name="T">待转换类型</typeparam>
        /// <param name="enitities">结果集</param>
        /// <param name="headtag">xml外层标签</param>
        /// <returns>xml字符串</returns>
        public static string ObjListToXml<T>(List<T> enitities, string headtag)
        {
            var sb = new StringBuilder();
            PropertyInfo[] propinfos = null;
            sb.AppendLine("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
            sb.AppendLine("<" + headtag + ">");
            foreach (T obj in enitities)
            {
                //初始化propertyinfo
                if (propinfos == null)
                {
                    Type objtype = obj.GetType();
                    propinfos = objtype.GetProperties();
                }

                sb.AppendLine("<item>");
                foreach (var propinfo in propinfos)
                {
                    sb.Append("<");
                    sb.Append(propinfo.Name);
                    sb.Append(">");
                    sb.Append(propinfo.GetValue(obj, null));
                    sb.Append("</");
                    sb.Append(propinfo.Name);
                    sb.AppendLine(">");
                }
                sb.AppendLine("</item>");
            }
            sb.AppendLine("</" + headtag + ">");
            return sb.ToString();
        }
        #endregion
    }
}