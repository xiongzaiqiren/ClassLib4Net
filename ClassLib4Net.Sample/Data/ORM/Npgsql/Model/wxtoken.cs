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
    [NpgsqlTableMapper(LoadDataMode.Table, "wxtoken")]
    public class wxtoken
    {
        [NpgsqlColumnMapper(NpgsqlDbType.Bigint, long.MaxValue, false, true, false, true, true)]
        public long id { get; set; }

        [NpgsqlColumnMapper(NpgsqlDbType.Char, 11, false, true, false, false)]
        public string name { get; set; }

        [NpgsqlColumnMapper(NpgsqlDbType.Varchar, 20, CanLoad = true, CanUpdate = false, CanDelete = false)]
        public string appid { get; set; }

        [NpgsqlColumnMapper(NpgsqlDbType.Varchar, 512, CanLoad = true, CanUpdate = true, CanDelete = false)]
        public string token { get; set; }

        [NpgsqlColumnMapper(NpgsqlDbType.Integer, int.MaxValue, CanLoad = true, CanUpdate = true, CanDelete = false)]
        public int expires_in { get; set; }

        [NpgsqlColumnMapper(NpgsqlDbType.TimestampTZ, 30, CanLoad = true, CanUpdate = true, CanDelete = false)]
        public DateTime expires_time { get; set; }

        [NpgsqlColumnMapper(NpgsqlDbType.TimestampTZ, 30, CanLoad = true, CanUpdate = true, CanDelete = false)]
        public DateTime creationtime { get; set; }

    }
}
