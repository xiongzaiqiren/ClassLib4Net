using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLib4Net.Encrypt
{
	/// <summary>
	/// SHA1方式加密与解密
	/// </summary>
	public class SHA1Helper
	{
        #region SHA1方式加密字符串的方法
        /// <summary>
        /// SHA1方式加密字符串的方法。
        /// 基于Sha1的自定义加密字符串方法：输入一个字符串，返回一个由40个字符组成的十六进制的哈希散列（字符串）。
        /// </summary>
        /// <param name="text">要进行加密的字符串</param>
        /// <param name="encoding">字符串编码格式</param>
        /// <returns>加密后的字符串</returns>
        public static string SHA1Encrypt(string text, System.Text.Encoding encoding)
		{
			if (string.IsNullOrWhiteSpace(text)) return string.Empty;
			try
			{
				byte[] cleanBytes = encoding.GetBytes(text);
				byte[] hashedBytes = System.Security.Cryptography.SHA1.Create().ComputeHash(cleanBytes);
				StringBuilder returnResult = new StringBuilder();
				foreach (var b in hashedBytes)
				{
					returnResult.AppendFormat("{0:x2}", b);
				}
				return returnResult.ToString();
			}
			catch (Exception ex)
			{
				throw new Exception("SHA1方式加密字符串失败。错误信息：" + ex.Message);
			}
		}

        /// <summary>
        /// SHA1方式加密字符串的方法（utf-8编码）。
        /// 基于Sha1的自定义加密字符串方法：输入一个字符串，返回一个由40个字符组成的十六进制的哈希散列（字符串）。
        /// </summary>
        /// <param name="text">要进行加密的字符串</param>
        /// <returns>加密后的字符串</returns>
        public static string SHA1Encrypt(string text)
		{
			if (string.IsNullOrWhiteSpace(text)) return string.Empty;
			return SHA1Encrypt(text, System.Text.Encoding.UTF8);
		}
		#endregion
	}
}
