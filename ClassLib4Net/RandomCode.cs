using System;
using System.Text;

namespace ClassLib4Net
{
	/// <summary>
	/// RandomCode 随机代码
	/// 熊学浩
	/// </summary>
	public class RandomCode
	{
		/// <summary>
		/// 随机代码基本字符集
		/// </summary>
		private static readonly string[] basechar ={"0","1","2","3","4","5","6","7","8","9",
                                    "A","B","C","D","E","F","G","H","I","J","K","L","M","N",
                                    "O","P","Q","R","S","T","U","V","W","X","Y","Z",
                                    "a","b","c","d","e","f","g","h","i","j","k","l","m","n",
                                    "o","p","q","r","s","t","u","v","w","x","y","z"
                                   };
        /// <summary>
        /// 静态只读的随机对象，确保连续产生的随机数的随机性
        /// </summary>
		public static readonly Random random = new Random();

		/// <summary>
		/// 字符串中连续相同的字符的个数
		/// </summary>
		/// <param name="text">字符串</param>
		/// <returns>字符串中连续相同的字符的个数</returns>
		public static int sameChar(string text)
		{
			if (!string.IsNullOrEmpty(text) && text.Length > 1)
			{
				int flag = 0; //字符串中相同的字符数
				char[] ArrChar = text.ToCharArray();
				for (int i = 0; i < ArrChar.Length - 1; i++) //判断连续相同的字符（末尾字符不用判断）
				{
					if (ArrChar[i] == ArrChar[i + 1])
						flag++;
				}
				return flag;
			}
			return 0;
		}

        /// <summary>
        /// 创建随机代码（数字与字母组成）
        /// </summary>
        /// <param name="length">随机代码长度</param>
        /// <param name="onlyUpper">英文字符仅大写</param>
        /// <returns>随机代码</returns>
        public static string createRandomCode(int length, bool onlyUpper = true)
        {
            int range = 0;
            if (onlyUpper)
            {
                range = 36;
            }
            else
            {
                range = basechar.Length;
            }

            StringBuilder code = new StringBuilder();
            for (int i = 0; i < length; i++)
            {
                code.Append(basechar[random.Next(0, range)]);
            }

            if (length > 1 && !string.IsNullOrEmpty(code.ToString()))
            {
                if (sameChar(code.ToString()) > length - 1) //至少有一个字符不相同
                {
                    return createRandomCode(length, onlyUpper);
                }
            }

            return code.ToString();
        }

		/// <summary>
		/// 创建随机代码（纯数字）
		/// </summary>
		/// <param name="length">随机代码长度</param>
		/// <returns>随机代码</returns>
		public static string createRandomDigital(int length)
		{
			string buffer = "0123456789";// 随机字符中也可以为汉字（任何）
			StringBuilder sb = new StringBuilder();
			int range = buffer.Length;
			for (int i = 0; i < length; i++)
			{
				sb.Append(buffer.Substring(random.Next(range), 1));
			}
			return sb.ToString();
		}
        /// <summary>
        /// 创建随机代码（纯英文字母）
        /// </summary>
        /// <param name="length">随机代码长度</param>
        /// <param name="onlyUpper">英文字符仅大写</param>
        /// <returns>随机代码</returns>
        public static string createRandomEnglishAlphabet(int length, bool onlyUpper = true)
        {
            string buffer = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
            StringBuilder sb = new StringBuilder();
            int range = 0;
            if (onlyUpper)
            {
                range = 26;
            }
            else
            {
                range = buffer.Length;
            }

            for (int i = 0; i < length; i++)
            {
                sb.Append(buffer.Substring(random.Next(range), 1));
            }
            return sb.ToString();
        }

        #region 生成随机的数字
        /// <summary>
        /// 创建随机代码（递归生成纯数字随机字符串）
        /// </summary>
        /// <param name="length">生成字符的个数</param>
        /// <returns>string</returns>
        public static string GenerateRandomDigital(int length)
        {
            string Vchar = "0,1,2,3,4,5,6,7,8,9";
            string[] VcArray = Vchar.Split(',');
            string VNum = ""; //由于字符串很短，就不用StringBuilder了

            int temp = -1; //记录上次随机数值，尽量避免生产几个一样的随机数


            //采用一个简单的算法以保证生成随机数的不同
            Random rand = new Random();
            for (int i = 1; i < length + 1; i++)
            {
                if (temp != -1)
                {
                    rand = new Random(i * temp * unchecked((int)DateTime.Now.Ticks));
                }
                int t = rand.Next(VcArray.Length);
                if (temp != -1 && temp == t)
                {
                    return GenerateRandomDigital(length);
                }
                temp = t;
                VNum += VcArray[t];
            }
            return VNum;
        }
        #endregion
    }
}
