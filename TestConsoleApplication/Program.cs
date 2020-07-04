using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestConsoleApplication
{
    class Program
    {
        static void Main(string[] args)
        {
            /*
            #region 时间戳测试
            var t = new DateTime(2018, 11, 6, 11, 17, 9, 964);
            Console.WriteLine("目标时间：{0}", t.ToString("yyyy-MM-dd HH:mm:ss.fff"));
            var t1 = ClassLib4Net.ConvertHelper.TimeStamp(1541474229); //2018-11-06 11:17:09
            var t2 = ClassLib4Net.ConvertHelper.TimeStampByKind(t);
            Console.WriteLine("目标时间：{0}，时间戳：{1}", t1.ToString("yyyy-MM-dd HH:mm:ss.fff"), t2);

            var t3 = ClassLib4Net.ConvertHelper.TimeStampToLocalTime(1541474229964); //2018-11-06 11:17:09.964
            var t4 = ClassLib4Net.ConvertHelper.TimeStampToLocalTime(t);
            Console.WriteLine("目标时间：{0}，时间戳：{1}", t3.ToString("yyyy-MM-dd HH:mm:ss.fff"), t4);

            string gmt = ClassLib4Net.ConvertHelper.GMTString(t);
            Console.WriteLine("本地时间：" + t.ToString("yyyy-MM-dd HH:mm:ss.fff") + "     GMTString转换后的时间：" + gmt);
            string gmtZ = ClassLib4Net.ConvertHelper.GMTByTimezone(t);
            Console.WriteLine("本地时间：" + t.ToString("yyyy-MM-dd HH:mm:ss.fff") + "     GMTByTimezone转换后的时间：" + gmtZ);
            Console.WriteLine("     GMTByTimezone的时间：" + gmtZ + "本地时间：" + ClassLib4Net.ConvertHelper.GMT2Local(gmtZ).ToString("yyyy-MM-dd HH:mm:ss.fff"));

            const string s = "123456";
            Console.WriteLine("密码：" + s);
            string md5 = ClassLib4Net.Encrypt.MD5Helper.MD5Encrypt(s);
            Console.WriteLine("Md5：" + md5);
            Console.WriteLine("长度：" + md5.Length);
            string sha1 = ClassLib4Net.Encrypt.SHA1Helper.SHA1Encrypt(s);
            Console.WriteLine("Sha1：" + sha1);
            Console.WriteLine("长度：" + sha1.Length);
            string des = ClassLib4Net.Encrypt.DESHelper.Encrypt(s);
            Console.WriteLine("DES：" + des);
            Console.WriteLine("长度：" + des.Length);
            
            #endregion
            */

            Image.ImageTest.WaterMarkByText();

            //Cache.RedisTest.String();
            //Cache.RedisTest.Sort();

            Console.ReadKey();
        }
    }
}
