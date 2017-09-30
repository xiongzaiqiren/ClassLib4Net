using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;

namespace ClassLib4Net
{
    /// <summary>
    /// 枚举助手
    /// </summary>
    public class EnumHelper
    {
        /// <summary>
        /// 根据枚举类型获取描述
        /// </summary>
        /// <param name="value">类型</param>
        /// <returns>描述</returns>
        public static string GetEnumDescription(Enum value)
        {
            // Get the Description attribute value for the enum value  
            FieldInfo fi = value.GetType().GetField(value.ToString());
            DescriptionAttribute[] attributes = (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);
            if (attributes.Length > 0)
            {
                return attributes[0].Description;
            }
            else
            {
                return value.ToString();
            }
        }

    }

}
