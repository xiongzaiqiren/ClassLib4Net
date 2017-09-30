using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Web;
using System.IO;

namespace ClassLib4Net
{
	/// <summary>
	/// 对文件进行处理的类
	/// </summary>
	public class FileHelper
	{
		#region 得到文件名的方法
		/// <summary>
		/// 得到文件名的方法
		/// </summary>
		/// <param name="str">完整文件名</param>
		/// <returns>文件名</returns>
		public static string GetFileName(string str)
		{
			int index = str.LastIndexOf('\\');
			if (index != -1)
				return str.Substring(index + 1);
			else
				return "";
		}
		#endregion

		#region 创建目录的方法
		/// <summary>
		/// 创建目录的方法
		/// </summary>
		/// <param name="path">要创建目录的路径</param>
		public static void CreateDirectory(string path)
		{
			try
			{
				path = path.Replace("/", @"\");
				if (!Directory.Exists(path))
				{
					Directory.CreateDirectory(path);
				}
			}
			catch (Exception ex)
			{
				throw new Exception("在'" + path + "'下创建目录失败。错误信息：" + ex.Message);
			}
		}
		#endregion

		#region 取扩展名的方法
		/// <summary>
		/// 取扩展名的方法
		/// </summary>
		/// <param name="_srcName">完整文件名</param>
		/// <returns>扩展名</returns>
		public static string GetSuffix(string _srcName)
		{
			int pos = _srcName.LastIndexOf(".");
			if (pos != -1)
				_srcName = _srcName.ToLower().Substring(pos);
			else
				_srcName = ".uitv";

			return _srcName;
		}
		#endregion

		#region 根据时间合成文件名的方法
		/// <summary>
		/// 根据时间合成文件名的方法
		/// </summary>
		/// <param name="_srcName">完整文件名</param>
		/// <returns>根据时间生成的文件名</returns>
		public static string BuildFileNameByTime(string _srcName)
		{
			return BuildFileNameByTime(_srcName, "");
		}
		/// <summary>
		/// 根据时间合成文件名的方法
		/// </summary>
		/// <param name="_srcName">完整文件名</param>
		/// <param name="_tgtName">自定义完整文件名</param>
		/// <returns>文件名</returns>
		public static string BuildFileNameByTime(string _srcName, string _tgtName)
		{
			DateTime dt = DateTime.Now;
			string fileName;
			if (_tgtName != null & _tgtName.Trim().Length > 0)
				fileName = _tgtName;
			else
				fileName = dt.Year.ToString()
					+ dt.Month.ToString()
					+ dt.Day.ToString()
					+ dt.Hour.ToString()
					+ dt.Minute.ToString()
					+ dt.Second.ToString()
					+ dt.Millisecond.ToString();
			return fileName + GetSuffix(_srcName);
		}
		#endregion

		#region 删除文件的方法
		/// <summary>
		/// 删除文件的方法
		/// </summary>
		/// <param name="_src">完整文件名</param>
		public static void RemoveFile(string _src)
		{
			try
			{
				if (File.Exists(_src))
					File.Delete(_src);
			}
			catch (Exception ex)
			{
				throw new Exception("删除文件'" + _src + "'失败。错误信息：" + ex.Message);
			}
		}
		#endregion

		#region 重命名文件的方法
		/// <summary>
		/// 重命名文件的方法
		/// </summary>
		/// <param name="_src">完整文件名</param>
		/// <param name="_newName">新完整文件名</param>
		public static void ReNameFile(string _src, string _newName)
		{
			try
			{
				int pos = _src.LastIndexOf(@"\");
				if (pos != -1)
				{
					string path = _src.Substring(0, pos);
					path = path + @"\" + _newName + GetSuffix(_src);
					if (File.Exists(_src))
					{
						File.Copy(_src, path, true);
						File.Delete(_src);
					}
				}
			}
			catch (Exception ex)
			{
				throw new Exception("重命名文件'" + _src + "'失败。错误信息：" + ex.Message);
			}
		}
		#endregion

		#region 往文件里追加内容的方法
		/// <summary>
		/// 往文件里追加内容的方法
		/// </summary>
		/// <param name="_content">要追加的内容</param>
		/// <param name="path">完整文件名</param>
		public static void AppendFile(string _content, string path)
		{
			AppendFile(_content, path, Encoding.Default);
		}
		/// <summary>
		/// 往文件里追加内容的方法
		/// </summary>
		/// <param name="_content">要追加的内容</param>
		/// <param name="path">完整文件名</param>
		/// <param name="encoding">文件编码</param>
		public static void AppendFile(string _content, string path, Encoding encoding)
		{
			try
			{
				int pos = path.LastIndexOf(@"\");
				if (pos != -1)
				{
					string dir = path.Substring(0, pos);
					if (!Directory.Exists(dir))
					{
						Directory.CreateDirectory(dir);
					}

					using (FileStream fs = new FileStream(path, FileMode.Append))
					{
						byte[] content = encoding.GetBytes(_content);
						fs.Write(content, 0, content.Length);
					}
				}
			}
			catch (Exception ex)
			{
				throw new Exception("往文件'" + path + "'追加内容失败。错误信息：" + ex.Message);
			}
		}
		#endregion

		#region 将内容写入文件中，自动创建文件和目录的方法
		/// <summary>
		/// 将内容写入文件中，自动创建文件和目录的方法
		/// </summary>
		/// <param name="_content">要写入的内容</param>
		/// <param name="path">完整文件名</param>
		public static void UpdateFile(string _content, string path)
		{
			UpdateFile(_content, path, Encoding.Default);
		}
		/// <summary>
		/// 将内容写入文件中，自动创建文件和目录的方法
		/// </summary>
		/// <param name="_content">要写入的内容</param>
		/// <param name="path">完整文件名</param>
		/// <param name="encoding">文件编码</param>
		public static void UpdateFile(string _content, string path, Encoding encoding)
		{
			try
			{
				path = path.Replace("/", @"\");
				int pos = path.LastIndexOf(@"\");
				if (pos != -1)
				{
					string dir = path.Substring(0, pos);
					if (!Directory.Exists(dir))
					{
						Directory.CreateDirectory(dir);
					}

					using (FileStream fs = new FileStream(path, FileMode.Create))
					{
						byte[] content = encoding.GetBytes(_content);
						fs.Write(content, 0, content.Length);
					}
				}
			}
			catch (Exception ex)
			{
				throw new Exception("创建文件'" + path + "'失败。错误信息：" + ex.Message);
			}
		}
		#endregion

		#region 读出模版数据的方法
		/// <summary>
		/// 读出模版数据的方法
		/// </summary>
		/// <param name="modelAbsPath">模版地址</param>
		/// <returns>模版内容</returns>
		public static string ReadPageModel(string modelAbsPath)
		{
			return ReadPageModel(modelAbsPath, Encoding.Default);
		}
		/// <summary>
		/// 读出模版数据的方法
		/// </summary>
		/// <param name="modelAbsPath">模版地址</param>
		/// <param name="encoding">文件编码</param>
		/// <returns>模版内容</returns>
		public static string ReadPageModel(string modelAbsPath, Encoding encoding)
		{
			try
			{
				StringBuilder sbModel = new StringBuilder();
				using (StreamReader sr = new StreamReader(modelAbsPath, encoding))
				{
					sbModel.Append(sr.ReadToEnd());
				}
				return sbModel.ToString();
			}
			catch (Exception ex)
			{
				throw new Exception("读出模板'" + modelAbsPath + "'失败。错误信息：" + ex.Message);
			}
		}
		#endregion

		#region 保存上传文件的方法
		/// <summary>
		/// 保存上传文件的方法
		/// </summary>
		/// <param name="_file">文件对象</param>
		/// <param name="_folder">目录文件夹</param>
		/// <param name="_absPath">回传保存绝对路径</param>
		public static void SaveFileToServer(HttpPostedFile _file, string fileLocalPath, ref string _absPath)
		{
			try
			{
				DateTime now = DateTime.Now;
				_absPath = now.ToShortDateString().Replace("-", "") + @"\";

				if (!Directory.Exists(fileLocalPath + _absPath))
					Directory.CreateDirectory(fileLocalPath + _absPath);
				_absPath += System.Guid.NewGuid() + GetSuffix(_file.FileName);

				_file.SaveAs(fileLocalPath + _absPath);
				_absPath = _absPath.Replace("\\", "/");
			}
			catch (Exception ex)
			{
				throw new Exception("保存文件'" + fileLocalPath + "'失败。错误信息：" + ex.Message);
			}
		}
		#endregion

		#region 文件下载的方法
		/// <summary>
		/// 文件下载的方法
		/// </summary>
		/// <param name="FullFileName">完整文件名</param>
		/// <param name="RealFileName">下载显示的文件名</param>
		public static void FileDownload(string FullFileName, string RealFileName)
		{
			FileInfo DownloadFile = new FileInfo(FullFileName);
			HttpContext.Current.Response.Clear();
			HttpContext.Current.Response.ClearHeaders();
			HttpContext.Current.Response.Buffer = false;
			HttpContext.Current.Response.ContentType = "application/octet-stream";
			HttpContext.Current.Response.AppendHeader("Content-Disposition", "attachment;filename=" + HttpUtility.UrlEncode(RealFileName, Encoding.UTF8));
			HttpContext.Current.Response.AppendHeader("Content-Length", DownloadFile.Length.ToString());
			HttpContext.Current.Response.WriteFile(DownloadFile.FullName);
			HttpContext.Current.Response.Flush();
			HttpContext.Current.Response.End();
		}
		#endregion

		#region 删除文件夹及其旗下所有文件
		/// <summary>
		/// 删除文件夹及其旗下所有文件
		/// </summary>
		/// <param name="dir"></param>
		public static void DeleteFolder(string dir)
		{
			try
			{
				foreach (string d in Directory.GetFileSystemEntries(dir))
				{
					if (File.Exists(d))
					{
						FileInfo fi = new FileInfo(d);
						if (fi.Attributes.ToString().IndexOf("ReadOnly") != -1)
							fi.Attributes = FileAttributes.Normal;
						File.Delete(d);//直接删除其中的文件   
					}
					else
					{
						DeleteFolder(d);//递归删除子文件夹   
					}
				}
				Directory.Delete(dir);//删除已空文件夹  
			}
			catch { }
		}
        #endregion

        #region 剪切文件到指定目录
        /// <summary>
        /// 剪切文件到指定目录
        /// </summary>
        /// <param name="sourcePath">源文件路径</param>
        /// <param name="destPath">目标文件路径</param>
        /// <param name="overwrite">覆盖，如果目标文件可以被覆盖，则为true；否则false。</param>
        /// <returns></returns>
        public static bool Cut(string sourcePath, string destPath, bool overwrite = false)
        {
            if (!File.Exists(sourcePath)) return false;
            File.Copy(sourcePath, destPath, false);
            File.Delete(sourcePath);
            return true;
        }
        #endregion


        #region 扩展
        /// <summary>
        /// byte[]转换成Stream
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static Stream BytesToStream(byte[] bytes)
		{
			Stream stream = new MemoryStream(bytes);
			return stream;
		}
		/// <summary>
		/// 文件转换成byte[]
		/// </summary>
		/// <param name="path">完整路径</param>
		/// <returns></returns>
		public static byte[] FileToBytes(string path)
		{
			using (FileStream fileStream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
			{
				// 读取文件的 byte[] 
				byte[] bytes = new byte[fileStream.Length];
				fileStream.Read(bytes, 0, bytes.Length);
				fileStream.Close();
				fileStream.Dispose();
				return bytes;
			}
		}
		/// <summary>
		/// Stream转换成byte[]
		/// </summary>
		/// <param name="stream"></param>
		/// <returns></returns>
		public static byte[] StreamToBytes(Stream stream)
		{
			byte[] bytes = new byte[stream.Length];
			stream.Read(bytes, 0, bytes.Length);
			// 设置当前流的位置为流的开始 
			stream.Seek(0, SeekOrigin.Begin);
			return bytes;
		}
		/// <summary>
		/// Stream写入文件
		/// </summary>
		/// <param name="stream"></param>
		/// <param name="path">完整路径</param>
		public static void StreamToFile(Stream stream, string path)
		{
			// 把 Stream 转换成 byte[] 
			byte[] bytes = new byte[stream.Length];
			stream.Read(bytes, 0, bytes.Length);
			// 设置当前流的位置为流的开始 
			stream.Seek(0, SeekOrigin.Begin);
			// 把 byte[] 写入文件 
			using (FileStream fs = new FileStream(path, FileMode.Create))
			{
				using (BinaryWriter bw = new BinaryWriter(fs))
				{
					bw.Write(bytes);
					bw.Dispose();
					bw.Close();
				}
				fs.Dispose();
				fs.Close();
			}
		}
		/// <summary>
		/// 文件转换成Stream
		/// </summary>
		/// <param name="path">完整路径</param>
		/// <returns></returns>
		public static Stream FileToStream(string path)
		{
			using (FileStream fileStream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
			{
				// 读取文件的 byte[] 
				byte[] bytes = new byte[fileStream.Length];
				fileStream.Read(bytes, 0, bytes.Length);
				fileStream.Close();
				// 把 byte[] 转换成 Stream 
				Stream stream = new MemoryStream(bytes);
				return stream;
			}
		}
		#endregion

	}
}