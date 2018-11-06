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
            #region 时间戳测试
            var t = new DateTime(2018, 11, 6, 11, 17, 9, 964);
            Console.WriteLine("目标时间：{0}", t.ToString("yyyy-MM-dd HH:mm:ss.fff"));
            var t1 = ClassLib4Net.ConvertHelper.TimeStamp(1541474229); //2018-11-06 11:17:09
            var t2 = ClassLib4Net.ConvertHelper.TimeStampByKind(t);
            Console.WriteLine("目标时间：{0}，时间戳：{1}", t1.ToString("yyyy-MM-dd HH:mm:ss.fff"), t2);

            var t3 = ClassLib4Net.ConvertHelper.TimeStampToLocalTime(1541474229964); //2018-11-06 11:17:09.964
            var t4 = ClassLib4Net.ConvertHelper.TimeStampToLocalTime(t);
            Console.WriteLine("目标时间：{0}，时间戳：{1}", t3.ToString("yyyy-MM-dd HH:mm:ss.fff"), t4); 
            #endregion

            //Cache.RedisTest.String();
            //Cache.RedisTest.Sort();

            Console.ReadKey();
        }
    }
}
