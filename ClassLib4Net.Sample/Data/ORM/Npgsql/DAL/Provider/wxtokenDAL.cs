using ClassLib4Net.Data;
using ClassLib4Net.Data.ORM.Npgsql;
using ClassLib4Net.Sample.Data.ORM.Npgsql.Model;
using Npgsql;
using NpgsqlTypes;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Transactions;

namespace ClassLib4Net.Sample.Data.ORM.Npgsql.DAL.Provider
{
    public sealed class wxtokenDAL
    {
        #region Instance
        private static readonly wxtokenDAL instance = new wxtokenDAL();
        private wxtokenDAL() { }
        internal static wxtokenDAL Instance { get { return instance; } }
        #endregion

        private static NpgsqlMapperHelper<wxtoken> mapper = new NpgsqlMapperHelper<wxtoken>(DataRepository.DBConnectionString);

        #region Base Methods
        public object Insert(wxtoken entity)
        {
            return mapper.Insert(entity);
        }

        public IList<wxtoken> LoadAll()
        {
            return mapper.LoadAll();
        }

        public int Update(wxtoken entity)
        {
            return mapper.Update(entity);
        }

        #endregion

        #region Extension Methods
        public wxtoken LoadByName(string name, string appid)
        {
            StringBuilder sql = new StringBuilder("SELECT " + mapper.CanLoadColumnNamesString() + " FROM " + mapper.tableName + " WHERE name=@name AND appid=@appid ORDER BY expires_time DESC LIMIT 1;");
            List<NpgsqlParameter> parameters = new List<NpgsqlParameter>() {
                 new NpgsqlParameter() { ParameterName=mapper.paramNameSign+nameof(wxtoken.name), Value = name, NpgsqlDbType = NpgsqlDbType.Varchar },
                 new NpgsqlParameter() { ParameterName=mapper.paramNameSign+nameof(wxtoken.appid), Value = appid, NpgsqlDbType = NpgsqlDbType.Varchar }
            };

            using(NpgsqlDataReader reader = NpgsqlHelper.ExecuteReader(mapper.DBConnectionString, CommandType.Text, sql.ToString(), parameters.ToArray()))
            {
                if(reader.Read())
                    return mapper.Load(reader);
            }
            return null;
        }

        public int UpdateByName(wxtoken entity)
        {
            StringBuilder sql = new StringBuilder("UPDATE wxtoken SET token=@token,expires_in=@expires_in,expires_time=@expires_time,creationtime=@creationtime WHERE name=@name AND appid=@appid;");
            List<NpgsqlParameter> parameters = new List<NpgsqlParameter>() {
                 new NpgsqlParameter() { ParameterName=mapper.paramNameSign+nameof(wxtoken.name), Value = entity.name, NpgsqlDbType = NpgsqlDbType.Varchar },
                 new NpgsqlParameter() { ParameterName=mapper.paramNameSign+nameof(wxtoken.appid), Value = entity.appid, NpgsqlDbType = NpgsqlDbType.Varchar },

                 new NpgsqlParameter() { ParameterName=mapper.paramNameSign+nameof(wxtoken.token), Value = entity.token, NpgsqlDbType = NpgsqlDbType.Varchar },
                 new NpgsqlParameter() { ParameterName=mapper.paramNameSign+nameof(wxtoken.expires_in), Value = entity.expires_in , NpgsqlDbType = NpgsqlDbType.Integer },
                 new NpgsqlParameter() { ParameterName=mapper.paramNameSign+nameof(wxtoken.expires_time), Value = entity.expires_time, NpgsqlDbType = NpgsqlDbType.TimestampTZ },
                 new NpgsqlParameter() { ParameterName=mapper.paramNameSign+nameof(wxtoken.creationtime), Value = entity.creationtime, NpgsqlDbType = NpgsqlDbType.TimestampTZ }
            };

            int result = NpgsqlHelper.ExecuteNonQuery(DataRepository.DBConnectionString, CommandType.Text, sql.ToString(), parameters.ToArray());
            return result;
        }
        #endregion

    }
}
