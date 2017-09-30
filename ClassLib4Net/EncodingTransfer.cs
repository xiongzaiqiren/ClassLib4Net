using System;
using System.Text;

namespace ClassLib4Net
{
    /// <summary>
    /// 编码转换
    /// </summary>
    public class EncodingTransfer
    {
        /// <summary>
        /// 把字符串从一种编码转换到另一种编码
        /// </summary>
        /// <param name="fromText">需要转换的字符串</param>
        /// <param name="fromEncoding">原始编码</param>
        /// <param name="toEncoding">转换后的编码</param>
        /// <returns>转换结果</returns>
        public static string TransferEncoding(string fromText, Encoding fromEncoding, Encoding toEncoding)
        {
            string rst = string.Empty;
            byte[] fromTextBytes = fromEncoding.GetBytes(fromText);
            byte[] rstAsciiBytes = Encoding.Convert(fromEncoding, toEncoding, fromTextBytes);
            char[] rstAsciiChars = new char[toEncoding.GetCharCount(rstAsciiBytes, 0, rstAsciiBytes.Length)];
            toEncoding.GetChars(rstAsciiBytes, 0, rstAsciiBytes.Length, rstAsciiChars, 0);
            rst = new string(rstAsciiChars);
            return rst;
        }

        /// <summary>
        /// 把以base64编码的字符串转换为byte[]数组
        /// </summary>
        /// <param name="base64String">以base64编码的字符串</param>
        /// <returns></returns>
        public static byte[] FromBase64String(string base64String)
        {
            string[] strs = new string[] { "", "=", "==", "===", "" };
            base64String = base64String + strs[4 - (base64String.Length % 4)];
            return Convert.FromBase64String(base64String);
        }
    }
}
