using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace ClassLib4Net
{
	/// <summary>
	/// HTML助手
	/// 熊学浩
	/// 2015-01-21
	/// </summary>
	public class HtmlHelper
	{
		#region 清除html标签

		/// <summary>
		/// 清除html标签
		/// </summary>
		/// <param name="htmlstring"></param>
		/// <returns></returns>
		public static string RemoveHtml(string htmlstring)
		{
			if (string.IsNullOrWhiteSpace(htmlstring))
				return string.Empty;
			//删除HTML
			htmlstring = Regex.Replace(htmlstring, @"<(.[^>]*)>", "", RegexOptions.IgnoreCase);
			htmlstring = Regex.Replace(htmlstring, @"([\r\n])[\s]+", "", RegexOptions.IgnoreCase);
			htmlstring = Regex.Replace(htmlstring, @"-->", "", RegexOptions.IgnoreCase);
			htmlstring = Regex.Replace(htmlstring, @"<!--.*", "", RegexOptions.IgnoreCase);
			htmlstring = Regex.Replace(htmlstring, @"&(quot|#34);", "\"", RegexOptions.IgnoreCase);
			htmlstring = Regex.Replace(htmlstring, @"&(amp|#38);", "&", RegexOptions.IgnoreCase);
			htmlstring = Regex.Replace(htmlstring, @"&(lt|#60);", "<", RegexOptions.IgnoreCase);
			htmlstring = Regex.Replace(htmlstring, @"&(gt|#62);", ">", RegexOptions.IgnoreCase);
			htmlstring = Regex.Replace(htmlstring, @"&(nbsp|#160);", " ", RegexOptions.IgnoreCase);
			htmlstring = Regex.Replace(htmlstring, @"&(iexcl|#161);", "\xa1", RegexOptions.IgnoreCase);
			htmlstring = Regex.Replace(htmlstring, @"&(cent|#162);", "\xa2", RegexOptions.IgnoreCase);
			htmlstring = Regex.Replace(htmlstring, @"&(pound|#163);", "\xa3", RegexOptions.IgnoreCase);
			htmlstring = Regex.Replace(htmlstring, @"&(copy|#169);", "\xa9", RegexOptions.IgnoreCase);
			htmlstring = Regex.Replace(htmlstring, @"&#(\d+);", "", RegexOptions.IgnoreCase);
			//htmlstring.Replace("<", "");
			//htmlstring.Replace(">", "");
			//htmlstring.Replace("\r\n", "");
			//htmlstring.Replace("&nbsp;", "");
			//htmlstring = System.Web.HttpContext.Current.Server.HtmlEncode(htmlstring).Trim();
			return htmlstring;
		}

		///<summary>
		///替换字符串中的特殊字符为html标记
		///</summary>
		///<param name="theString">需要进行替换的文本</param>
		///<returns>替换完的文本。</returns>
		public static string HtmlEncode(string theString)
		{
			if (string.IsNullOrWhiteSpace(theString))
				return string.Empty;
			theString = theString.Replace(">", "&gt;");
			theString = theString.Replace("<", "&lt;");
			theString = theString.Replace(" ", "&nbsp;");
			theString = theString.Replace("\"", "&quot;");
			theString = theString.Replace("\'", "&#39;");
			theString = theString.Replace("\n", "<br/>");
			theString = theString.Replace("&", "&amp;");
			return theString;
		}

		///<summary>
		///恢复字符串中的特殊字符
		///</summary>
		///<param name="theString">需要恢复的文本</param>
		///<returns>恢复好的文本。</returns>
		public static string HtmlDiscode(string theString)
		{
			if (string.IsNullOrWhiteSpace(theString))
				return string.Empty;
			theString = theString.Replace("&gt;", ">");
			theString = theString.Replace("&lt;", "<");
			theString = theString.Replace("&nbsp;", " ");
			theString = theString.Replace("&quot;", "\"");
			theString = theString.Replace("&#39;", "\'");
			theString = theString.Replace("<br/>", "\n");
			return theString;
		}

		/// <summary>
		/// 移除超文本链接A标签，保留链接文本
		/// </summary>
		/// <param name="htmlContent">html内容</param>
		/// <returns></returns>
		public static string RemoveAlinkTag(string htmlContent)
		{
			if (!string.IsNullOrEmpty(htmlContent))
				return Regex.Replace(htmlContent, @"<a[^>]*>|</\s*a>", ""); // 移除A标签，保留链接文本
			else return string.Empty;
		}

		/// <summary>
		/// 移除Style标签以及样式文本
		/// </summary>
		/// <param name="htmlContent">html内容</param>
		/// <returns></returns>
		public static string RemoveStyleTag(string htmlContent)
		{
			if (!string.IsNullOrEmpty(htmlContent))
				return Regex.Replace(htmlContent, @"<style([^>]*)>([^<]*)</\s*style>", "", RegexOptions.IgnoreCase); // 移除Style标签以及样式文本
			else return string.Empty;
		}

		/// <summary>
		/// 移除Script标签以及脚本
		/// </summary>
		/// <param name="htmlContent">html内容</param>
		/// <returns></returns>
		public static string RemoveScriptTag(string htmlContent)
		{
			if (!string.IsNullOrEmpty(htmlContent))
				return Regex.Replace(htmlContent, @"<script([^>]*)>([^<]*)</\s*script>", "", RegexOptions.IgnoreCase); // 移除Script标签以及样式文本
			else return string.Empty;
		}

		#endregion


		/// <summary>
		/// 取得HTML中所有图片的 URL。
		/// </summary>
		/// <param name="htmlText">HTML代码</param>
		/// <returns>图片的URL列表</returns>
		public static string[] GetHtmlImageUrlList(string htmlText)
		{
			// 定义正则表达式用来匹配 img 标签
			Regex regImg = new Regex(@"<img\b[^<>]*?\bsrc[\s\t\r\n]*=[\s\t\r\n]*[""']?[\s\t\r\n]*(?<imgUrl>[^\s\t\r\n""'<>]*)[^<>]*?/?[\s\t\r\n]*>", RegexOptions.IgnoreCase);

			// 搜索匹配的字符串
			MatchCollection matches = regImg.Matches(htmlText);

			int i = 0;
			string[] sUrlList = new string[matches.Count];

			// 取得匹配项列表
			foreach (Match match in matches)
				sUrlList[i++] = match.Groups["imgUrl"].Value;

			return sUrlList;
		}

		/// <summary>
		/// 取得HTML中所有图片的 img标签。
		/// </summary>
		/// <param name="htmlText">HTML代码</param>
		/// <returns>代码中的img标签列表</returns>
		public static string[] GetImgTag(string htmlText)
		{
			// 定义正则表达式用来匹配 img 标签
			Regex regImg = new Regex(@"<img[^>]*>", RegexOptions.IgnoreCase); // <img[\s\S]*?>

			// 搜索匹配的字符串
			MatchCollection matches = regImg.Matches(htmlText);

			int i = 0;
			string[] sUrlList = new string[matches.Count];

			// 取得匹配项列表
			foreach (Match match in matches)
				sUrlList[i++] = match.Value;

			return sUrlList;
		}

		/// <summary>
		/// 取得HTML中所有IMG标签的URL。
		/// </summary>
		/// <param name="ImgHtmlTag">HTML代码IMG标签</param>
		/// <returns>代码中的IMG标签URL列表</returns>
		public static string[] GetUrlByImgTag(string ImgHtmlTag)
		{
			// 定义正则表达式用来匹配 img 标签里面的url路径
			System.Text.RegularExpressions.Regex regImg = new System.Text.RegularExpressions.Regex(@"(?<=src\="")[^\s\t\r\n""'<>]*(?="")", System.Text.RegularExpressions.RegexOptions.IgnoreCase);

			// 搜索匹配的字符串
			System.Text.RegularExpressions.MatchCollection matches = regImg.Matches(ImgHtmlTag);

			int i = 0;
			string[] sUrlList = new string[matches.Count];

			// 取得匹配项列表
			foreach (System.Text.RegularExpressions.Match match in matches)
				sUrlList[i++] = match.Value;

			return sUrlList;
		}

		/// <summary>
		/// 获取绝对URL
		/// </summary>
		/// <param name="referenceUrl">参照物URL（当前网页URL）</param>
		/// <param name="virtualURL">要转化的虚拟URL（当前网页内的图片URL）</param>
		/// <returns>转化后的绝对URL</returns>
		public static string GetAbsoluteURL(string referenceUrl, string virtualURL)
		{
			if (string.IsNullOrWhiteSpace(referenceUrl) || string.IsNullOrWhiteSpace(virtualURL)) return string.Empty;

			referenceUrl = referenceUrl.Trim();
			virtualURL = virtualURL.Trim();
			if ((new Regex(@"^(http|https|ftp):\/\/", RegexOptions.IgnoreCase)).IsMatch(virtualURL))
				return virtualURL;

			string prefix = string.Empty, suffix = string.Empty;
			if ((new Regex(@"^(http|https|ftp):\/\/", RegexOptions.IgnoreCase)).IsMatch(referenceUrl))
			{
				/*
				 *  语法格式：   相对关系/部分路径/文件名
				 *  根据相对路径的参照点又可以分为相对文档的相对路径以及相对根目录的相对路径。
				 *  <1>相对文档：
				 *		这种路径的表现形式是根据目标文档所在目录和当前文档所在目录之间的关系的一种表现形式。“../”表示上一级目录，“./”表示当前目录。例如：“./img/test.gif“=“img/test.gif”。
				 *		例如：当前文档的路径是：test/project1/index.htm 。我们要找test目录下的html.htm 。而当前的目录是project1，我们要回到上一级目录中，所以路径是“../html.htm”。
				 *  <2>相对根目录：
				 *		这种路径根据目标文档相对于根目录的关系的一种表现形式。在这种表达式种的第一个字符是“/”，这个符号表示这个路径是一个相对于根目录的表达式。
				 *		例如：<a href="/test/html.htm">
				 */

				//1，相对根目录
				if ((new Regex(@"^/", RegexOptions.IgnoreCase)).IsMatch(virtualURL))
				{
					Match m = (new Regex(@"^(http|https|ftp):\/\/[^/]+", RegexOptions.IgnoreCase)).Match(referenceUrl);
					if (m != null && m.Success)
						prefix = m.Value;
					return prefix + virtualURL;
				}
				//2，相对文档
				else
				{
					//2.1，当前目录	“./“表示当前目录。例如：“./img/test.gif“=“img/test.gif”
					if ((new Regex(@"(^(\./)[^\./]+)|(^[^\./]+)", RegexOptions.IgnoreCase)).IsMatch(virtualURL))
					{
						suffix = (new Regex(@"^(\./)", RegexOptions.IgnoreCase)).Replace(virtualURL, ""); //移除开头的“./”
						return (new Regex(@"[^/]+$", RegexOptions.IgnoreCase)).Replace(referenceUrl, suffix);
					}
					//2.2，上级目录 “../”表示上一级目录
					else if ((new Regex(@"^(\.\./)+", RegexOptions.IgnoreCase)).IsMatch(virtualURL))
					{
						prefix = referenceUrl;
						//多个上级目录，逐级比对
						string[] tempArray = (new Regex(@"(\.\./)", RegexOptions.IgnoreCase)).Split(virtualURL);
						int length = tempArray.Length / 2; //3,5,7……
						for (int i = 1; i <= length; i++)
						{
							prefix = (new Regex(@"[^/]+/[^/]*$", RegexOptions.IgnoreCase)).Replace(prefix, "");
						}

						return (new Regex(@"^(\.\./)+", RegexOptions.IgnoreCase)).Replace(virtualURL, prefix);
					}
					else
					{
						return string.Empty;
					}
				}
			}
			return string.Empty;
		}

		#region 创建桌面快捷方式
		/// <summary> 
		/// 创建快捷方式 
		/// </summary> 
		/// <param name="Title">标题</param> 
		/// <param name="URL">URL地址</param> 
		public static void CreateShortcut(string Title, string URL)
		{
			string strFavoriteFolder;
			// “收藏夹”中 创建 IE 快捷方式 
			strFavoriteFolder = System.Environment.GetFolderPath(Environment.SpecialFolder.Favorites);
			CreateShortcutFile(Title, URL, strFavoriteFolder);
			// “ 桌面 ”中 创建 IE 快捷方式 
			strFavoriteFolder = System.Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
			CreateShortcutFile(Title, URL, strFavoriteFolder);
			// “ 链接 ”中 创建 IE 快捷方式 
			strFavoriteFolder = System.Environment.GetFolderPath(Environment.SpecialFolder.Favorites);
			if (System.IO.Directory.Exists(strFavoriteFolder))
			{
				if (System.IO.Directory.Exists(strFavoriteFolder + "\\Links"))
					strFavoriteFolder = strFavoriteFolder + "\\Links";
				else if (System.IO.Directory.Exists(strFavoriteFolder + "\\链接"))
					strFavoriteFolder = strFavoriteFolder + "\\链接";
				else if (System.IO.Directory.Exists(strFavoriteFolder + "\\收藏夹"))
					strFavoriteFolder = strFavoriteFolder + "\\收藏夹";
				else if (System.IO.Directory.Exists(strFavoriteFolder + "\\收藏夹栏"))
					strFavoriteFolder = strFavoriteFolder + "\\收藏夹栏";

				CreateShortcutFile(Title, URL, strFavoriteFolder);
			}
			//「开始」菜单中 创建 IE 快捷方式 
			strFavoriteFolder = System.Environment.GetFolderPath(Environment.SpecialFolder.StartMenu);
			CreateShortcutFile(Title, URL, strFavoriteFolder);
		}
		/// <summary> 
		/// 创建快捷方式 
		/// </summary> 
		/// <param name="Title">标题</param> 
		/// <param name="URL">URL地址</param> 
		/// <param name="SpecialFolder">特殊文件夹</param> 
		public static void CreateShortcutFile(string Title, string URL, string SpecialFolder)
		{
			if (string.IsNullOrWhiteSpace(SpecialFolder) || !System.IO.Directory.Exists(SpecialFolder)) return;
			// Create shortcut file, based on Title 
			System.IO.StreamWriter objWriter = System.IO.File.CreateText(SpecialFolder + "\\" + Title + ".url");
			// Write URL to file 
			objWriter.WriteLine("[InternetShortcut]");
			objWriter.WriteLine("URL=" + URL);
			// Close file 
			objWriter.Close();
		}
		#endregion

	}
}
