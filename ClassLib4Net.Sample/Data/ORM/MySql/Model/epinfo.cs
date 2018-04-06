using ClassLib4Net.Data.ORM;
using ClassLib4Net.Data.ORM.MySql;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ClassLib4Net.Sample.Data.ORM.MySql.Model
{

    /// <summary>
    /// epinfo是实体表/查询结果/存储过程返回结果/数据集等
    /// </summary>
    [MySqlTableMapper(LoadDataMode.Table, nameof(epinfo))]
    public class epinfo
    {
        [MySqlColumnMapper(MySqlDbType.Int32, Name = nameof(epinfo.ID), IsPrimaryKey = true, IsIdentity = true)]
        public int ID { get; set; }



        [MySqlColumnMapper(MySqlDbType.VarChar, 64, CanNull = false)]
        public string epGuid { get; set; }

        [MySqlColumnMapper(MySqlDbType.Text)]
        public string epName { get; set; }

        [MySqlColumnMapper(MySqlDbType.Text)]
        public string epDescription { get; set; }

        [MySqlColumnMapper(MySqlDbType.VarChar, 30)]
        public string epShortName { get; set; }

    }
}
