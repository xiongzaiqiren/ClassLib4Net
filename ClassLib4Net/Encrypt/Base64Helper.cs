using System;
using System.Text;

namespace ClassLib4Net.Encrypt
{
	/// <summary>
	/// Base64方式加密与解密
	/// </summary>
	public class Base64Helper
	{
		#region Base64加密解密
		/// <summary>
		/// Base64加密
		/// </summary>
		/// <param name="input">需要加密的字符串</param>
		/// <returns></returns>
		public static string Base64Encrypt(string input)
		{
			return Base64Encrypt(input, new UTF8Encoding());
		}

		/// <summary>
		/// Base64加密
		/// </summary>
		/// <param name="input">需要加密的字符串</param>
		/// <param name="encode">字符编码</param>
		/// <returns></returns>
		public static string Base64Encrypt(string input, Encoding encode)
		{
			return Convert.ToBase64String(encode.GetBytes(input));
		}

		/// <summary>
		/// Base64解密
		/// </summary>
		/// <param name="input">需要解密的字符串</param>
		/// <returns></returns>
		public static string Base64Decrypt(string input)
		{
			return Base64Decrypt(input, new UTF8Encoding());
		}

		/// <summary>
		/// Base64解密
		/// </summary>
		/// <param name="input">需要解密的字符串</param>
		/// <param name="encode">字符的编码</param>
		/// <returns></returns>
		public static string Base64Decrypt(string input, Encoding encode)
		{
			return encode.GetString(Convert.FromBase64String(input));
		}
		#endregion
	}
}
