using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;

namespace ClassLib4Net
{
	/// <summary>
	/// 自定义日志
	/// </summary>
	class LogCustom
	{
		private static readonly object SyncRoot = new object();
		#region 记录自定义日志信息的方法
		/// <summary>
		/// 记录自定义日志信息的方法
		/// </summary>
		/// <param name="content">日志信息</param>
		public static void Log(string content)
		{
			string path = AppDomain.CurrentDomain.BaseDirectory;
			if (HttpContext.Current != null)
			{
				path = HttpContext.Current.Server.MapPath("~/");
			}
			path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "LogCustom/");

			Log(content, path);
		}

		/// <summary>
		/// 记录自定义日志信息的方法
		/// </summary>
		/// <param name="content">日志信息</param>
		/// <param name="logLocalPath">日志文件物理路径</param>
		public static void Log(string content, string logLocalPath)
		{
			lock (SyncRoot)
			{
				string FilePath = logLocalPath + DateTime.Now.ToShortDateString().Replace("/", "") + ".txt";
				FileHelper.AppendFile(content, FilePath);
			}
		}
		#endregion
	}
}
