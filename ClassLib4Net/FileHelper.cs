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
        /// <param name="physicalPath">包含文件名的物理路径</param>
        /// <returns>文件名</returns>
        [Obsolete("建议使用 System.IO.Path.GetFileNameWithoutExtension")]
        public static string GetFileName(string physicalPath)
        {
            int index = physicalPath.LastIndexOf('\\');
            if(index != -1)
                return physicalPath.Substring(index + 1);
            else
                return "";
        }
        #endregion

        #region 创建目录的方法
        /// <summary>
        /// 如果目录不存在则创建目录的方法
        /// </summary>
        /// <param name="path">要创建目录路径（“/”会自动转换成“\”）</param>
        public static void CreateDirectory(string path)
        {
            try
            {
                path = path.Replace("/", @"\");
                if(!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
            }
            catch(Exception ex)
            {
                throw new Exception("在'" + path + "'下创建目录失败。错误信息：" + ex.Message);
            }
        }
        #endregion

        #region 取扩展名的方法
        /// <summary>
        /// 取扩展名的方法
        /// </summary>
        /// <param name="path">完整文件名</param>
        /// <returns>扩展名</returns>
        [Obsolete("建议使用 System.IO.Path.GetExtension")]
        public static string GetSuffix(string path)
        {
            int pos = path.LastIndexOf(".");
            if(pos >= 0)
                path = path.ToLower().Substring(pos);
            else
                path = ".uitv";

            return path;
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
        /// 如果文件存在则删除文件的方法
        /// </summary>
        /// <param name="physicalPath">完整文件名</param>
        public static void RemoveFile(string physicalPath)
        {
            try
            {
                if(File.Exists(physicalPath))
                    File.Delete(physicalPath);
            }
            catch(Exception ex)
            {
                throw new Exception("删除文件'" + physicalPath + "'失败。错误信息：" + ex.Message);
            }
        }
        #endregion

        #region 重命名文件的方法
        /// <summary>
        /// 重命名文件的方法
        /// </summary>
        /// <param name="physicalPath">完整文件名</param>
        /// <param name="newName">新文件名(不包含后缀名)</param>
        public static void RenameFile(string physicalPath, string newName)
        {
            try
            {
                int pos = physicalPath.LastIndexOf(@"\");
                if(pos != -1)
                {
                    string path = physicalPath.Substring(0, pos);
                    path = path + @"\" + newName + GetSuffix(physicalPath);
                    if(File.Exists(physicalPath))
                    {
                        File.Copy(physicalPath, path, true);
                        File.Delete(physicalPath);
                    }
                }
            }
            catch(Exception ex)
            {
                throw new Exception("重命名文件'" + physicalPath + "'失败。错误信息：" + ex.Message);
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
        /// <param name="content">要追加的内容</param>
        /// <param name="physicalPath">完整文件名</param>
        /// <param name="encoding">文件编码</param>
        public static void AppendFile(string content, string physicalPath, Encoding encoding)
        {
            try
            {
                int pos = physicalPath.LastIndexOf(@"\");
                if(pos != -1)
                {
                    string dir = physicalPath.Substring(0, pos);
                    CreateDirectory(dir);
                    using(FileStream fs = new FileStream(physicalPath, FileMode.Append))
                    {
                        byte[] _content = encoding.GetBytes(content);
                        fs.Write(_content, 0, content.Length);
                    }
                }
            }
            catch(Exception ex)
            {
                throw new Exception("往文件'" + physicalPath + "'追加内容失败。错误信息：" + ex.Message);
            }
        }
        #endregion

        #region 将内容写入文件中，自动创建文件和目录的方法
        /// <summary>
        /// 将内容写入文件中，自动创建文件和目录的方法
        /// </summary>
        /// <param name="content">要写入的内容</param>
        /// <param name="physicalPath">完整文件名</param>
        public static void UpdateFile(string content, string physicalPath)
        {
            UpdateFile(content, physicalPath, Encoding.Default);
        }
        /// <summary>
        /// 将内容写入文件中，自动创建文件和目录的方法
        /// </summary>
        /// <param name="content">要写入的内容</param>
        /// <param name="physicalPath">完整文件名</param>
        /// <param name="encoding">文件编码</param>
        public static void UpdateFile(string content, string physicalPath, Encoding encoding)
        {
            try
            {
                physicalPath = physicalPath.Replace("/", @"\");
                int pos = physicalPath.LastIndexOf(@"\");
                if(pos != -1)
                {
                    string dir = physicalPath.Substring(0, pos);
                    CreateDirectory(dir);
                    using(FileStream fs = new FileStream(physicalPath, FileMode.Create))
                    {
                        byte[] _content = encoding.GetBytes(content);
                        fs.Write(_content, 0, content.Length);
                    }
                }
            }
            catch(Exception ex)
            {
                throw new Exception("创建文件'" + physicalPath + "'失败。错误信息：" + ex.Message);
            }
        }
        #endregion

        #region 读出模版数据的方法
        /// <summary>
        /// 读取文件内容
        /// </summary>
        /// <param name="physicalPath">要读取的完整文件路径</param>
        /// <returns>模版内容</returns>
        public static string ReadFile(string physicalPath)
        {
            return ReadFile(physicalPath, Encoding.Default);
        }
        /// <summary>
        /// 读取文件内容
        /// </summary>
        /// <param name="physicalPath">要读取的完整文件路径</param>
        /// <param name="encoding">文件编码</param>
        /// <returns>模版内容</returns>
        public static string ReadFile(string physicalPath, Encoding encoding)
        {
            try
            {
                StringBuilder sb = new StringBuilder();
                using(StreamReader sr = new StreamReader(physicalPath, encoding))
                {
                    sb.Append(sr.ReadToEnd());
                }
                return sb.ToString();
            }
            catch(Exception ex)
            {
                throw new Exception("读出模板'" + physicalPath + "'失败。错误信息：" + ex.Message);
            }
        }
        #endregion

        #region 保存上传文件的方法
        /// <summary>
        /// 保存上传文件的方法
        /// </summary>
        /// <param name="file">文件对象</param>
        /// <param name="fileLocalPath">目录文件夹</param>
        /// <param name="absPath">回传保存绝对路径</param>
        [Obsolete("不建议使用")]
        public static void SaveFileToServer(HttpPostedFile file, string fileLocalPath, ref string absPath)
        {
            try
            {
                DateTime now = DateTime.Now;
                absPath = now.ToShortDateString().Replace("-", "") + @"\";

                if(!Directory.Exists(fileLocalPath + absPath))
                    Directory.CreateDirectory(fileLocalPath + absPath);
                absPath += System.Guid.NewGuid() + GetSuffix(file.FileName);

                file.SaveAs(fileLocalPath + absPath);
                absPath = absPath.Replace("\\", "/");
            }
            catch(Exception ex)
            {
                throw new Exception("保存文件'" + fileLocalPath + "'失败。错误信息：" + ex.Message);
            }
        }
        /// <summary>
        /// 保存上传文件的方法
        /// </summary>
        /// <param name="file">要保存的文件</param>
        /// <param name="physicalFolder">保存到物理文件夹（示例：D:\WebSite\MyWeb\）</param>
        /// <param name="relativePath">基于物理文件夹下的相对路径（示例：/Upload/MyFiles/）</param>
        /// <param name="namingRules">文件命名规则</param>
        /// <returns>是否保存成功</returns>
        public static bool SaveFileToServer(HttpPostedFile file, string physicalFolder, ref string relativePath, NamingRules namingRules = NamingRules.Guid)
        {
            if(null == file) throw new ArgumentNullException("file参数为空");
            if(string.IsNullOrWhiteSpace(physicalFolder)) throw new ArgumentNullException("physicalFolder参数为空");
            try
            {
                string suffixName = GetSuffix(file.FileName);
                string fileName = string.Empty;
                switch(namingRules)
                {
                    case NamingRules.Guid: fileName = Guid.NewGuid().ToString(); break;
                    case NamingRules.GuidWithoutCrossLine: fileName = Guid.NewGuid().ToString().Replace("-", ""); break;
                    case NamingRules.DateTime: fileName = DateTime.Now.ToString("yyyyMMddHHmmssffffff"); break;
                    case NamingRules.OriginalFileName: fileName = Path.GetFileNameWithoutExtension(file.FileName); break;
                    case NamingRules.FromRelativePath:
                        if(string.IsNullOrWhiteSpace(relativePath)) throw new ArgumentNullException("relativePath参数为空");
                        suffixName = Path.GetExtension(relativePath);
                        fileName = Path.GetFileNameWithoutExtension(relativePath);
                        if(string.IsNullOrWhiteSpace(suffixName) || string.IsNullOrWhiteSpace(fileName)) throw new ArgumentNullException("当前命名规则下relativePath参数必需包含完整文件名");
                        break;
                }

                physicalFolder = string.Concat(physicalFolder.Trim().TrimEnd(new Char[] { '/', '\\' }), "\\");
                relativePath = string.Concat(relativePath.Trim().Trim(new Char[] { '/', '\\' }), "/");

                string _relativePath = relativePath.Replace("/", "\\");

                if(!Directory.Exists(Path.Combine(physicalFolder, _relativePath)))
                    Directory.CreateDirectory(Path.Combine(physicalFolder, _relativePath));

                file.SaveAs(Path.Combine(physicalFolder, _relativePath, fileName + suffixName));
                relativePath = Path.Combine(relativePath, fileName + suffixName);

                return true;
            }
            catch(Exception ex)
            {
                throw new Exception("保存文件'" + physicalFolder + "'失败。错误信息：" + ex.Message);
            }
        }

        /// <summary>
        /// 保存上传文件的方法
        /// </summary>
        /// <param name="file">要保存的文件</param>
        /// <param name="physicalFolder">保存到物理文件夹（示例：D:\WebSite\MyWeb\）</param>
        /// <param name="relativePath">基于物理文件夹下的相对路径（示例：/Upload/MyFiles/）</param>
        /// <param name="namingRules">文件命名规则</param>
        /// <returns>是否保存成功</returns>
        public static bool SaveFileToServer(HttpPostedFileBase file, string physicalFolder, ref string relativePath, NamingRules namingRules = NamingRules.Guid)
        {
            if(null == file) throw new ArgumentNullException("file参数为空");
            if(string.IsNullOrWhiteSpace(physicalFolder)) throw new ArgumentNullException("physicalFolder参数为空");
            try
            {
                string suffixName = GetSuffix(file.FileName);
                string fileName = string.Empty;
                switch(namingRules)
                {
                    case NamingRules.Guid: fileName = Guid.NewGuid().ToString(); break;
                    case NamingRules.GuidWithoutCrossLine: fileName = Guid.NewGuid().ToString().Replace("-", ""); break;
                    case NamingRules.DateTime: fileName = DateTime.Now.ToString("yyyyMMddHHmmssffffff"); break;
                    case NamingRules.OriginalFileName: fileName = Path.GetFileNameWithoutExtension(file.FileName); break;
                    case NamingRules.FromRelativePath:
                        if(string.IsNullOrWhiteSpace(relativePath)) throw new ArgumentNullException("relativePath参数为空");
                        suffixName = Path.GetExtension(relativePath);
                        fileName = Path.GetFileNameWithoutExtension(relativePath);
                        if(string.IsNullOrWhiteSpace(suffixName) || string.IsNullOrWhiteSpace(fileName)) throw new ArgumentNullException("当前命名规则下relativePath参数必需包含完整文件名");
                        break;
                }

                physicalFolder = string.Concat(physicalFolder.Trim().TrimEnd(new Char[] { '/', '\\' }), "\\");
                relativePath = string.Concat(relativePath.Trim().Trim(new Char[] { '/', '\\' }), "/");

                string _relativePath = relativePath.Replace("/", "\\");
                
                if(!Directory.Exists(Path.Combine(physicalFolder, _relativePath)))
                    Directory.CreateDirectory(Path.Combine(physicalFolder, _relativePath));

                file.SaveAs(Path.Combine(physicalFolder, _relativePath, fileName + suffixName));
                relativePath = Path.Combine(relativePath, fileName + suffixName);

                return true;
            }
            catch(Exception ex)
            {
                throw new Exception("保存文件'" + physicalFolder + "'失败。错误信息：" + ex.Message);
            }
        }
        /// <summary>
        /// 文件命名规则 Naming rules
        /// </summary>
        public enum NamingRules : byte
        {
            /// <summary>
            /// NewGuid
            /// </summary>
            Guid = 0,
            /// <summary>
            /// Guid without a cross line
            /// </summary>
            GuidWithoutCrossLine = 1,
            /// <summary>
            /// DateTime
            /// </summary>
            DateTime = 2,
            /// <summary>
            /// OriginalFileName
            /// </summary>
            OriginalFileName = 3,
            /// <summary>
            /// From RelativePath
            /// </summary>
            FromRelativePath = 4
        }

        /// <summary>
        /// 保存上传文件的方法
        /// </summary>
        /// <param name="file">要保存的文件</param>
        /// <param name="physicalFolder">保存到物理文件夹（示例：D:\WebSite\MyWeb\）</param>
        /// <param name="relativePath">基于物理文件夹下的相对路径（示例：/Upload/MyFiles/）</param>
        /// <param name="namingRules">文件命名规则</param>
        /// <returns>是否保存成功</returns>
        public static bool SaveFileToServer(HttpPostedFileWrapper file, string physicalFolder, ref string relativePath, NamingRules namingRules = NamingRules.Guid)
        {
            return SaveFileToServer((HttpPostedFileBase)file, physicalFolder, ref relativePath, namingRules);
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
        /// <param name="path">路径</param>
        public static void DeleteFolder(string path)
        {
            try
            {
                foreach(string d in Directory.GetFileSystemEntries(path))
                {
                    if(File.Exists(d))
                    {
                        FileInfo fi = new FileInfo(d);
                        if(fi.Attributes.ToString().IndexOf("ReadOnly") != -1)
                            fi.Attributes = FileAttributes.Normal;
                        File.Delete(d);//直接删除其中的文件   
                    }
                    else
                    {
                        DeleteFolder(d);//递归删除子文件夹   
                    }
                }
                Directory.Delete(path);//删除已空文件夹  
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