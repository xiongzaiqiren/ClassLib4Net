using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestConsoleApplication.Cache
{
    public class RedisTest
    {
        public static void String()
        {
            Console.Write("开始Redis测试> ");
            var redis = new ClassLib4Net.Cache.Redis.RedisHelper();
            string key = "name";
            var r1 = redis.StringSet(key, "xiong", new TimeSpan(0, 30, 0));
            Console.WriteLine($"设置{key}，状态{r1}");
            var r2 = redis.StringGet(key);
            Console.WriteLine($"获取{key}={r2}");
        }
    }
}
