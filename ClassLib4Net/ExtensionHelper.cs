using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Web;

namespace ClassLib4Net
{
    /// <summary>
    /// 基础方法扩展类
    /// </summary>
    public static class ExtensionHelper
    {
        #region 转成基本的类型
        /// <summary>
		/// 转成基本的类型（支持可空类型）
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="str"></param>
        /// <param name="defaultValue">默认值</param>
        /// <returns></returns>
        public static T Convert<T>(this string str, T defaultValue)
        {
            if (String.IsNullOrEmpty(str))
                return defaultValue;

            var type = typeof(T);

            var typeName = type.FullName;

            if (type.Name == "Nullable`1")
            {
                var m = Regex.Match(typeName, @"((?<=\[)(\w+.\w+)(?=,))");

                type = Type.GetType(m.Value);
            }

            MethodInfo method = null;
            foreach (var m in type.GetMethods())
            {
                if (m.Name == "Parse" && m.GetParameters().Length == 1)
                {
                    method = m;
                    break;
                }
            }

            var result = defaultValue;
            try
            {
                if (method != null)
                    result = (T)method.Invoke(null, new[] { str });
            }
            catch
            {
                return defaultValue;
            }

            return result;
        }

		/// <summary>
		/// 转成基本的类型（支持可空类型）
		/// </summary>
		/// <typeparam name="T">类型</typeparam>
		/// <param name="obj"></param>
		/// <param name="defaultValue">默认值</param>
		/// <returns></returns>
		public static T ConvertObject<T>(this object obj, T defaultValue)
		{
			if (null == obj || System.DBNull.Value == obj)
				return defaultValue;

			return (obj.ToString()).Convert<T>(defaultValue);
		}
        #endregion

        #region 字符串转Guid对象
        /// <summary>
        /// 转换成Guid
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static Guid ToGuid(this string str)
        {
            try
            {
                return new Guid(str);
            }
            catch
            {
                return Guid.Empty;
            }
        }
        #endregion

        #region 转数字字符串
        /// <summary>
        /// 转成数字字符串
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string ToNumber(this string str)
        {
            if (String.IsNullOrEmpty(str))
                return "";

            return Regex.Replace(str, @"\D+", "");
        }
        #endregion

        #region 转成整数
        /// <summary>
        /// 转成整数
        /// </summary>
        /// <param name="str"></param>
        /// <param name="defaultValue">默认值</param>
        /// <returns></returns>
        /// 
        public static int ToInt(this string str, int defaultValue)
        {
            try
            {
                return int.Parse(str);
            }
            catch
            {
                return defaultValue;
            }
        }

        public static int ToInt(this object str, int defaultValue)
        {
            try
            {
                return ConvertHelper.GetInteger(str);
            }
            catch
            {
                return defaultValue;
            }
        }

        public static int ToInt(this string str)
        {
            try
            {
                return int.Parse(str);
            }
            catch
            {
                return -1;
            }
        }

        public static int? ToIntOrNull(this string str)
        {
            if (str == string.Empty)
                return null;
            return ToInt(str);
        }
        #endregion

        #region 将对象变量转成短整型变量的方法

        /// <summary>
        /// 将对象变量转成短整型变量的方法
        /// </summary>
        /// <param name="obj">对象变量</param>
        /// <returns>短整型变量</returns>
        public static short ToShortInt(this object obj)
        {
            return ConvertHelper.GetShortInt(ConvertHelper.GetString(obj));
        }
        #endregion

        #region 将对象变量转成64位整数型变量的方法
        /// <summary>
        /// 将对象变量转成64位整数型变量的方法
        /// </summary>
        /// <param name="obj">对象变量</param>
        /// <returns>64位整数型变量</returns>
        public static long ToLong(this object obj)
        {
            return ConvertHelper.GetLong(ConvertHelper.GetString(obj));
        }
        #endregion

        #region 将对象变量转成双精度浮点型变量的方法
        /// <summary>
        /// 将对象变量转成双精度浮点型变量的方法
        /// </summary>
        /// <param name="obj">对象变量</param>
        /// <returns>双精度浮点型变量</returns>
        public static double ToDouble(this object obj)
        {
            return ConvertHelper.GetDouble(ConvertHelper.GetString(obj));
        }
        #endregion

        #region 转成浮点类型
        /// <summary>
        /// 转成浮点类型
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static float ToFloat(this string str)
        {
            if (!String.IsNullOrEmpty(str))
            {
                str = Regex.Replace(str, "[^0-9^-^.]", string.Empty);
                if (str != "")
                    str = str.GetLeft(1) + str.Substring(1).Replace('-', '\0');

                var dotIndex = str.IndexOf('.');
                if (dotIndex == -1)
                    return float.Parse(str);
                str = str.Substring(0, dotIndex) + "." + str.Substring(dotIndex + 1).Replace('.', '\0');

                int result;
                int.TryParse(str, out result);
                return result;
            }
            return -1;
        }
        #endregion

        #region 将对象变量转成布尔型变量的方法
        /// <summary>
        /// 将对象变量转成布尔型变量的方法
        /// </summary>
        /// <param name="obj">对象变量</param>
        /// <returns>布尔型变量</returns>
        public static bool ToBoolean(this object obj)
        {
            return (obj == DBNull.Value || obj == null) ? false :
                ConvertHelper.GetString(obj).ToLower() == "true" ? true : false;
        }
        #endregion

        #region 将对象变量转成十进制数字变量的方法
        /// <summary>
        /// 将对象变量转成十进制数字变量的方法
        /// </summary>
        /// <param name="obj">对象变量</param>
        /// <returns>十进制数字变量</returns>
        public static decimal ToDecimal(this object obj)
        {
            return ConvertHelper.GetDecimal(ConvertHelper.GetString(obj));
        }
        #endregion

        #region 将对象变量转成日期时间型字符串变量的方法
        /// <summary>
        /// 将对象变量转成日期时间型字符串变量的方法
        /// </summary>
        /// <param name="obj">对象变量</param>
        /// <param name="sFormat">时间字符串格式，例：yyyy-MM-dd</param>
        /// <returns>时间型字符串变量</returns>
        public static string ToDateTimeString(this object obj, string sFormat)
        {
            return ConvertHelper.GetDateTime(obj).ToString(sFormat);
        }
        #endregion

        #region 将对象变量转成日期字符串变量的方法
        /// <summary>
        /// 将对象变量转成日期字符串变量的方法
        /// </summary>
        /// <param name="obj">对象变量</param>
        /// <returns>日期字符串变量</returns>
        public static string ToShortDateString(this object obj)
        {
            return ConvertHelper.GetDateTimeString(obj, "yyyy-MM-dd");
        }
        #endregion

        #region 将对象变量转成日期型变量的方法
        /// <summary>
        /// 将对象变量转成日期型变量的方法
        /// </summary>
        /// <param name="obj">对象变量</param>
        /// <returns>日期型变量</returns>
        public static DateTime ToDateTime(object obj)
        {
            DateTime result;
            DateTime.TryParse(ConvertHelper.GetString(obj), out result);
            return result;
        }
        #endregion

        #region 获取字符串左侧字符
        /// <summary>
        /// 获取字符串左侧字符
        /// </summary>
        /// <param name="str"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static string GetLeft(this string str, int length)
        {
            return String.IsNullOrEmpty(str) ? "" : (length >= str.Length ? str : str.Substring(0, length));
        }
        #endregion

        #region 获取字符串右侧字符
        /// <summary>
        /// 获取字符串右侧字符
        /// </summary>
        /// <param name="str"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static string GetRight(this string str, int length)
        {
            return String.IsNullOrEmpty(str) ? "" : (length >= str.Length ? str : str.Substring(str.Length - length));
        }
        #endregion

        #region Url code
        /// <summary>
		/// Url编码，空格转成%20
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
		public static string UrlEncode(this string str, System.Text.Encoding encoding)
		{
			string UrlEncode = "";
			UrlEncode = HttpUtility.UrlEncode(str, encoding);
			UrlEncode = UrlEncode.Replace("+", "%20");
			return UrlEncode;
		}
		/// <summary>
		/// Url编码，空格转成%20
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
		public static string UrlEncode(this string str)
		{
			return UrlEncode(str, Encoding.Default);
		}

        /// <summary>
        ///Url解码 
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string UrlDecode(this string str)
        {
            return HttpUtility.UrlDecode(str);
        }
        #endregion

		#region 时间扩展

		/// <summary>
		/// 时间格式获得中国星期
		/// </summary>
		/// <param name="date"></param>
		/// <returns></returns>
		public static string ChineseWeek(this DateTime date)
		{
			string enWeek = date.DayOfWeek.ToString();
			string cnWeek = string.Empty;
			switch (enWeek)
			{
				case "Monday":
					cnWeek = "星期一";
					break;
				case "Tuesday":
					cnWeek = "星期二";
					break;
				case "Wednesday":
					cnWeek = "星期三";
					break;
				case "Thursday":
					cnWeek = "星期四";
					break;
				case "Friday":
					cnWeek = "星期五";
					break;
				case "Saturday":
					cnWeek = "星期六";
					break;
				case "Sunday":
					cnWeek = "星期日";
					break;
			}
			return cnWeek;
		}

		/// <summary>
		/// Windows DateTime to Unix Timestamp
		/// </summary>
		/// <param name="time"></param>
		/// <returns></returns>
		public static double ToTimestamp(this DateTime time)
		{
			DateTime? _time = time;
			return ToTimestamp(_time).Value;
		}
		/// <summary>
		/// Windows DateTime to Unix Timestamp
		/// </summary>
		/// <param name="time"></param>
		/// <returns></returns>
		public static double? ToTimestamp(this DateTime? time)
		{
			if (time == null)
				return null;
			TimeSpan ts = (time.Value - new DateTime(1970, 1, 1, 0, 0, 0, 0).ToLocalTime());
			return (int)ts.TotalSeconds;
		}
		/// <summary>
		///  Unix Timestamp to Windows DateTime
		/// </summary>
		/// <param name="timestamp"></param>
		/// <returns></returns>
		public static DateTime ToDateTime(this long timestamp)
		{
			double? _timestamp = timestamp;
			return ToDateTime(_timestamp).Value;
		}
		/// <summary>
		///  Unix Timestamp to Windows DateTime
		/// </summary>
		/// <param name="timestamp"></param>
		/// <returns></returns>
		public static DateTime? ToDateTime(this long? timestamp)
		{
			double? _timestamp = timestamp;
			return ToDateTime(_timestamp);
		}
		/// <summary>
		///  Unix Timestamp to Windows DateTime
		/// </summary>
		/// <param name="timestamp"></param>
		/// <returns></returns>
		public static DateTime ToDateTime(this int timestamp)
		{
			double? _timestamp = timestamp;
			return ToDateTime(_timestamp).Value;
		}
		/// <summary>
		///  Unix Timestamp to Windows DateTime
		/// </summary>
		/// <param name="timestamp"></param>
		/// <returns></returns>
		public static DateTime? ToDateTime(this int? timestamp)
		{
			double? _timestamp = timestamp;
			return ToDateTime(_timestamp);
		}
		/// <summary>
		///  Unix Timestamp to Windows DateTime
		/// </summary>
		/// <param name="timestamp"></param>
		/// <returns></returns>
		public static DateTime ToDateTime(this double timestamp)
		{
			double? _timestamp = timestamp;
			return ToDateTime(_timestamp).Value;
		}
		/// <summary>
		/// Unix Timestamp to Windows DateTime
		/// </summary>
		/// <param name="timestamp"></param>
		/// <returns></returns>
		public static DateTime? ToDateTime(this double? timestamp)
		{
			if (timestamp == null)
				return null;
			DateTime startTime = new DateTime(1970, 1, 1, 0, 0, 0, 0);
			DateTime reTime = startTime.AddSeconds(timestamp.Value);
			return reTime.ToLocalTime();
		}

		/// <summary>
		/// 时间格式化
		/// </summary>
		/// <param name="time"></param>
		/// <param name="format"></param>
		/// <returns></returns>
		public static string FormatDateTime(this DateTime time, string format = "yyyy-MM-dd")
		{
			DateTime? _time = time;
			return FormatDateTime(_time, format);
		}

		/// <summary>
		/// 时间格式化
		/// </summary>
		/// <param name="time"></param>
		/// <param name="format"></param>
		/// <returns></returns>
		public static string FormatDateTime(this DateTime? time, string format = "yyyy-MM-dd")
		{
			string timeStr = string.Empty;
			if (time != null)
			{
				timeStr = time.Value.ToString(format);
			}
			return timeStr;
		}

		/// <summary>
		/// 将8位，14位，17位纯数字字符串转换成日期时间格式
		/// 例如：20101010101010100 转换成：2010-10-10 10:10:10.100
		/// 熊学浩
		/// 2013-07-12
		/// </summary>
		/// <param name="time">例如：20101010101010100</param>
		/// <returns></returns>
		public static string FormatDateTime(this string time)
		{
			if (string.IsNullOrEmpty(time)) return string.Empty;
			StringBuilder _time = new StringBuilder();
			if (time.Length >= 8)
			{
				_time.Append(time.Substring(0, 4));
				_time.Append("-");
				_time.Append(time.Substring(4, 2));
				_time.Append("-");
				_time.Append(time.Substring(6, 2));
			}
			if (time.Length >= 14)
			{
				_time.Append(" ");
				_time.Append(time.Substring(8, 2));
				_time.Append(":");
				_time.Append(time.Substring(10, 2));
				_time.Append(":");
				_time.Append(time.Substring(12, 2));
			}
			if (time.Length >= 17)
			{
				_time.Append(".");
				_time.Append(time.Substring(14, 3));
			}
			return _time.ToString();
		}

		#endregion 时间扩展
    }
}