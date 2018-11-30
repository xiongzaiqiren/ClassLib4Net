using System;

namespace ClassLib4Net.Encrypt
{
	/// <summary>
	/// MD5方式加密
	/// </summary>
	public class MD5Helper
	{
        #region MD5方式加密字符串的方法
        /// <summary>
        /// MD5方式加密字符串的方法。
        /// 基于Md5的自定义加密字符串方法：输入一个字符串，返回一个由32个字符组成的十六进制的哈希散列（字符串）。
        /// </summary>
        /// <param name="text">要进行加密的字符串</param>
        /// <param name="encoding">字符串编码格式</param>
        /// <returns>加密后的字符串</returns>
        public static string MD5Encrypt(string text, System.Text.Encoding encoding)
		{
			if (string.IsNullOrWhiteSpace(text)) return string.Empty;
			try
			{
				System.Text.StringBuilder sb = new System.Text.StringBuilder(32);
				System.Security.Cryptography.MD5 md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
				byte[] t = md5.ComputeHash(encoding.GetBytes(text));
				for (int i = 0; i < t.Length; i++)
				{
					sb.Append(t[i].ToString("x").PadLeft(2, '0'));
				}
				return sb.ToString();
			}
			catch (Exception ex)
			{
				throw new Exception("MD5方式加密字符串失败。错误信息：" + ex.Message);
			}
		}

        /// <summary>
        /// MD5方式加密字符串的方法（utf-8编码）。
        /// 基于Md5的自定义加密字符串方法：输入一个字符串，返回一个由32个字符组成的十六进制的哈希散列（字符串）。
        /// </summary>
        /// <param name="text">要进行加密的字符串</param>
        /// <returns>加密后的字符串</returns>
        public static string MD5Encrypt(string text)
		{
			if (string.IsNullOrWhiteSpace(text)) return string.Empty;
			return MD5Encrypt(text, System.Text.Encoding.UTF8);
		}
        #endregion

        #region 标准MD5加密
        /// <summary>
        /// 使用默认编码进行标准MD5加密。
        /// 基于Md5的自定义加密字符串方法：输入一个字符串，返回一个由32个字符组成的十六进制的哈希散列（字符串）。
        /// </summary>
        /// <param name="strText">text</param>
        /// <returns>md5 Encrypt string</returns>
        public static string MD5Hash(string strText)
		{
			return MD5Hash(strText, System.Text.Encoding.Default);
		}
        /// <summary>
        /// 标准MD5加密。
        /// 基于Md5的自定义加密字符串方法：输入一个字符串，返回一个由32个字符组成的十六进制的哈希散列（字符串）。
        /// </summary>
        /// <param name="strText">text</param>
        /// <param name="encoding">编码方式</param>
        /// <returns>md5 Encrypt string</returns>
        public static string MD5Hash(string strText, System.Text.Encoding encoding)
		{
			System.Security.Cryptography.MD5 MD5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
			byte[] datSource = encoding.GetBytes(strText);
			byte[] newSource = MD5.ComputeHash(datSource);
			string byte2String = null;
			for (int i = 0; i < newSource.Length; i++)
			{
				string thisByte = newSource[i].ToString("x");
				if (thisByte.Length == 1) thisByte = "0" + thisByte;
				byte2String += thisByte;
			}
			return byte2String;
		}
		#endregion
	}
}
