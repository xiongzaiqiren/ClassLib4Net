using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web.Mvc;

namespace ClassLib4Net.Extension.Enum
{
    /// <summary>
    /// 枚举扩展
    /// </summary>
    public static class EnumExtensions
    {
        /// <summary>
        /// 根据枚举成员获取DescriptionAttribute的属性Description
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        public static string GetDescription(object o)
        {
            //获取枚举的Type类型对象
            Type t = o.GetType();

            //获取枚举的所有字段
            FieldInfo[] ms = t.GetFields();

            //遍历所有枚举的所有字段
            foreach (FieldInfo f in ms)
            {
                if (f.Name != o.ToString())
                {
                    continue;
                }
                ////  Description
                //  //第二个参数true表示查找EnumDisplayNameAttribute的继承链
                //  if (f.IsDefined(typeof(EnumDisplayNameAttribute), true))
                //  {
                //      return
                //        (f.GetCustomAttributes(typeof(EnumDisplayNameAttribute), true)[0] as EnumDisplayNameAttribute)
                //          .DisplayName;
                //  } 
                FieldInfo fi = o.GetType().GetField(o.ToString());
                try
                {
                    var attributes = (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);
                    return (attributes != null && attributes.Length > 0) ? attributes[0].Description : o.ToString();
                }
                catch
                {
                    return "(Unknow)";
                }
            }

            //如果没有找到自定义属性，直接返回属性项的名称
            return o.ToString();
        }
    }

    #region 多语言实现方法

    /// <summary>
    /// 定义枚举多语言属性
    /// </summary>
    [AttributeUsage(AttributeTargets.All, AllowMultiple = true)]
    public class MultiLanguageAttribute : Attribute
    {
        /// <summary>
        /// 带参数的构造方法
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public MultiLanguageAttribute(object key, string value)
        {
            this.key = key;
            this.value = value;
        }

        /// <summary>
        /// 属性
        /// </summary>
        public object key { get; private set; }
        /// <summary>
        /// 属性值
        /// </summary>
        public string value { get; private set; }
    }

    #region 扩展enum属性

    /// <summary>
    /// 多语言扩展
    /// </summary>
    public static class MultiLanguageExtensions
    {
        /// <summary>
        /// 当前语言值
        /// </summary>
        /// <param name="enumObject"></param>
        /// <returns></returns>
        public static string Language(this System.Enum enumObject)
        {
            return Language(enumObject, CurrentCultureName.Replace('-', '_'));
        }

        /// <summary>
        /// 枚举指定语言显示
        /// </summary>
        /// <param name="enumObject"></param>
        /// <param name="language"></param>
        /// <returns></returns>
        public static string Language(this System.Enum enumObject, string language)
        {
            string cultureName = language.Replace('-', '_');

            Type t = enumObject.GetType();
            string s = enumObject.ToString();
            MultiLanguageAttribute[] os = null;
            try
            {
                os = (MultiLanguageAttribute[])t.GetField(s).GetCustomAttributes(typeof(MultiLanguageAttribute), false);
            }
            catch { }
            if (os != null)
            {
                foreach (MultiLanguageAttribute o in os)
                {
                    if (o.key.ToString() == cultureName)
                    {
                        s = o.value;
                    }
                }
            }

            return s;
        }

        /// <summary>
        /// 枚举转换为字典，并翻译为当前语言
        /// </summary>
        /// <param name="em</param>
        /// <returns></returns>
        public static IDictionary<int, string> ToIEnumerable(this System.Enum enumObject)
        {
            string cultureName = CurrentCultureName.Replace('-', '_'); //当前系统语言
            IDictionary<int, string> dic = new Dictionary<int, string>();

            Array array = System.Enum.GetValues(enumObject.GetType());//当前枚举的所有项
            Type t = enumObject.GetType();//当前枚举类型
            foreach (int val in array)
            {
                string emName = System.Enum.GetName(enumObject.GetType(), val);
                string emKey = emName;
                MultiLanguageAttribute[] os = null;
                try
                {
                    os = (MultiLanguageAttribute[])t.GetField(emName).GetCustomAttributes(typeof(MultiLanguageAttribute), false);
                }
                catch { }
                if (os != null)
                {
                    foreach (MultiLanguageAttribute la in os)
                    {
                        if (la.key.ToString() == cultureName)
                        {
                            emName = la.value;
                            break;
                        }
                    }
                }
                dic.Add(val, emName);
            }

            return dic;
        }

        /// <summary>
        /// 枚举转换为selectlist，并翻译为当前语言
        /// </summary>
        /// <param name="enumObject"></param>
        /// <returns></returns>
        public static SelectList ToSelectList(this System.Enum enumObject)
        {
            IDictionary<int, string> dic = enumObject.ToIEnumerable();
            return new SelectList(dic, "Key", "Value");
        }
        /// <summary>
        /// 枚举转换为selectlist，设置默认值，并翻译为当前语言
        /// </summary>
        /// <param name="enumObject"></param>
        /// <param name="defaultValue">默认值类型为Int32</param>
        /// <returns></returns>
        public static List<SelectListItem> ToSelectList(this System.Enum enumObject, object defaultValue)
        {
            List<SelectListItem> selectList = ToSelectList(enumObject).ToList();
            if (null != defaultValue && DBNull.Value != defaultValue)
            {
                int _defaultValue = Int32.Parse(defaultValue.ToString());
                foreach (SelectListItem item in selectList)
                {
                    if (_defaultValue == Int32.Parse(item.Value))
                    {
                        item.Selected = true;
                    }
                }
            }
            return selectList;
        }

        /// <summary>
        /// 当前用户地区编码
        /// </summary>
        public static string CurrentCultureName
        {
            get
            {
                return System.Threading.Thread.CurrentThread.CurrentCulture.Name;
            }
        }

    }

    #endregion

    #endregion

}
