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

        public class SortedModel
        {
            public int ID { get; set; }
            public string Name { get; set; }
            public int Number { get; set; }
            public DateTime createtime { get; set; }
        }
        public static void Sort()
        {
            Console.Write("开始Redis测试> ");
            var redis = new ClassLib4Net.Cache.Redis.RedisHelper();
            var db = redis.GetDatabase();

            string key = "MySortedSet";

            for(int i = 1; i <= 100; i++)
            {
                var entity = new SortedModel() { ID = i, Name = $"I am {i}", Number = i, createtime = DateTime.Now };
                var r = db.SortedSetAdd(key, ClassLib4Net.JsonHelper.Serialize(entity), entity.Number);
            }
            
            Console.WriteLine($"模拟设置{db.SortedSetLength(key)}个排序项已完成");

            var rr = db.SortedSetRangeByRank(key, 0, 10, StackExchange.Redis.Order.Descending);
            foreach(var item in rr)
            {
                Console.WriteLine($"获取{key}={item}");
            }
            
        }

    }
}
