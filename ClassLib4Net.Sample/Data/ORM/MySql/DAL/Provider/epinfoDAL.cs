using ClassLib4Net.Data.ORM.MySql;
using ClassLib4Net.Sample.Data.ORM.MySql.Model;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Transactions;

namespace ClassLib4Net.Sample.Data.ORM.MySql.DAL.Provider
{
    public sealed class epinfoDAL
    {
        #region Instance
        private static readonly epinfoDAL instance = new epinfoDAL();
        private epinfoDAL() { }
        internal static epinfoDAL Instance { get { return instance; } }
        #endregion

        private static MySqlMapperHelper<epinfo> mapper = new MySqlMapperHelper<epinfo>(DataRepository.DBConnectionString);

        #region Base Methods
        public object Insert(epinfo entity)
        {
            return mapper.Insert(entity);
        }

        public IList<epinfo> LoadAll()
        {
            return mapper.LoadAll();
        }

        public int Update(epinfo entity)
        {
            return mapper.Update(entity);
        }

        #endregion

        #region Extension Methods
        public epinfo LoadByName(string epGuid, string epName)
        {
            StringBuilder sql = new StringBuilder();
            sql.AppendFormat("SELECT {0} FROM {1} WHERE ", mapper.CanLoadColumnNamesString(), mapper.tableName);

            sql.AppendFormat(" {1}={0}{1}", mapper.paramNameSign, nameof(epinfo.epName));
            sql.AppendFormat(" AND {1}={0}{1}", mapper.paramNameSign, nameof(epinfo.epGuid));

            sql.AppendFormat(" ORDER BY {0} DESC LIMIT 1;", nameof(epinfo.ID));
            IList<MySqlParameter> parameters = new List<MySqlParameter>() {
                 new MySqlParameter() { ParameterName=mapper.paramNameSign+nameof(epinfo.epName), Value = epName, MySqlDbType = MySqlDbType.VarChar },
                 new MySqlParameter() { ParameterName=mapper.paramNameSign+nameof(epinfo.epGuid), Value = epGuid, MySqlDbType = MySqlDbType.Text }
            };

            using(MySqlDataReader reader = ClassLib4Net.Data.MySqlHelper.ExecuteReader(mapper.DBConnectionString, CommandType.Text, sql.ToString(), parameters.ToArray()))
            {
                if(reader.Read())
                    return mapper.Load(reader);
            }
            return null;
        }

        public int UpdateNameByGuid(string epGuid, string epName)
        {
            StringBuilder sql = new StringBuilder();
            sql.AppendFormat("UPDATE {0} SET epName={1}epName WHERE epGuid={1}epGuid;", mapper.tableName, mapper.paramNameSign);

            IList<MySqlParameter> parameters = new List<MySqlParameter>() {
                 new MySqlParameter() { ParameterName=mapper.paramNameSign+nameof(epinfo.epGuid), Value = epGuid, MySqlDbType = MySqlDbType.VarChar,Size= 64 },
                 new MySqlParameter() { ParameterName=mapper.paramNameSign+nameof(epinfo.epName), Value = epName, MySqlDbType = MySqlDbType.Text }
            };

            int result = ClassLib4Net.Data.MySqlHelper.ExecuteNonQuery(DataRepository.DBConnectionString, CommandType.Text, sql.ToString(), parameters.ToArray());
            return result;
        }
        #endregion

    }
}
