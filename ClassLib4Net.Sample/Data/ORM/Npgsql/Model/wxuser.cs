using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using NpgsqlTypes;
using ClassLib4Net.Data.ORM.Npgsql;
using ClassLib4Net.Data.ORM;

namespace ClassLib4Net.Sample.Data.ORM.Npgsql.Model
{
    [NpgsqlTableMapper(LoadDataMode.Table, "wxuser")]
    public class wxuser
    {
        [NpgsqlColumnMapper(NpgsqlDbType.Bigint, long.MaxValue, false, true, false, true, true)]
        public long id { get; set; }

        [NpgsqlColumnMapper(NpgsqlDbType.Varchar, 28)]
        public string wx_openid { get; set; }

        [NpgsqlColumnMapper(NpgsqlDbType.Varchar, 29)]
        public string wx_unionid { get; set; }

        [NpgsqlColumnMapper(NpgsqlDbType.Boolean)]
        public bool? is_subscribe { get; set; }

        [NpgsqlColumnMapper(NpgsqlDbType.TimestampTZ)]
        public DateTime? subscribe_time { get; set; }
        [NpgsqlColumnMapper(NpgsqlDbType.TimestampTZ, CanUpdate = false)]
        public DateTime? unsubscribe_time { get; set; }

        [NpgsqlColumnMapper(NpgsqlDbType.Text)]
        public string nickname { get; set; }
        [NpgsqlColumnMapper(NpgsqlDbType.Integer)]
        public int? sex { get; set; }
        [NpgsqlColumnMapper(NpgsqlDbType.Varchar, 10)]
        public string language { get; set; }
        [NpgsqlColumnMapper(NpgsqlDbType.Varchar, 512)]
        public string headimgurl { get; set; }

        [NpgsqlColumnMapper(NpgsqlDbType.TimestampTZ, 30, CanLoad = true, CanUpdate = false, CanDelete = false)]
        public DateTime creationtime { get; set; }

    }
}
