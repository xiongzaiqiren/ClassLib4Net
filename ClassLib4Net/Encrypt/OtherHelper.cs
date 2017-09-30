using System;
using System.Text;

namespace ClassLib4Net.Encrypt
{
	/// <summary>
	/// 其它方式加密与解密
	/// </summary>
	public class OtherHelper
	{
		#region 简单加密/解密
		/// <summary>
		/// 简单加密
		/// </summary>
		/// <param name="str"></param>
		/// <returns></returns>
		public string encode(string str)
		{
			string htext = "";

			for (int i = 0; i < str.Length; i++)
			{
				htext = htext + (char)(str[i] + 10 - 1 * 2);
			}
			return htext;
		}
		/// <summary>
		/// 简单解密
		/// </summary>
		/// <param name="str"></param>
		/// <returns></returns>
		public string decode(string str)
		{
			string dtext = "";

			for (int i = 0; i < str.Length; i++)
			{
				dtext = dtext + (char)(str[i] - 10 + 1 * 2);
			}
			return dtext;
		}
		#endregion
	}
}
