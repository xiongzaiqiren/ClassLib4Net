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
    public sealed class wxuserDAL
    {
        #region Instance
        private static readonly wxuserDAL instance = new wxuserDAL();
        private wxuserDAL() { }
        internal static wxuserDAL Instance { get { return instance; } }
        #endregion

        private static NpgsqlMapperHelper<wxuser> mapper = new NpgsqlMapperHelper<wxuser>(DataRepository.DBConnectionString);

        #region Base Methods
        public object Insert(wxuser entity)
        {
            var _entity = LoadByWX_Openid(entity.wx_openid);
            if(_entity != null && _entity.id > 0)
            {
                entity = _entity;
                return _entity.id;
            }

            object result = 0;
            using(var ts = new TransactionScope())
            {
                result = mapper.Insert(entity);

                ts.Complete();
                return result;
            }
        }

        public IList<wxuser> LoadAll()
        {
            return mapper.LoadAll();
        }

        public int Update(wxuser entity)
        {
            return mapper.Update(entity);
        }

        #endregion

        #region Extension Methods
        public bool existWX_Openid(string wx_openid)
        {
            StringBuilder sql = new StringBuilder("SELECT COUNT(1) FROM wxuser WHERE wx_openid=@wx_openid;");
            List<NpgsqlParameter> parameters = new List<NpgsqlParameter>() {
                 new NpgsqlParameter() { ParameterName=mapper.paramNameSign+nameof(wxuser.wx_openid), Value = wx_openid, NpgsqlDbType =NpgsqlDbType.Varchar }
            };

            object result = NpgsqlHelper.ExecuteScalar(mapper.DBConnectionString, CommandType.Text, sql.ToString(), parameters.ToArray());
            if(null == result)
                return false;
            else
                return Lib.ConvertHelper.GetInteger(result) > 0;
        }

        public wxuser LoadByWX_Openid(string wx_openid)
        {
            StringBuilder sql = new StringBuilder("SELECT " + mapper.CanLoadColumnNamesString() + " FROM " + mapper.tableName + " WHERE wx_openid=@wx_openid ORDER BY id DESC LIMIT 1;");
            List<NpgsqlParameter> parameters = new List<NpgsqlParameter>() {
                 new NpgsqlParameter() { ParameterName=mapper.paramNameSign+nameof(wxuser.wx_openid), Value = wx_openid, NpgsqlDbType =NpgsqlDbType.Varchar }
            };

            using(NpgsqlDataReader reader = NpgsqlHelper.ExecuteReader(mapper.DBConnectionString, CommandType.Text, sql.ToString(), parameters.ToArray()))
            {
                if(reader.Read())
                    return mapper.Load(reader);
            }
            return null;
        }

        public wxuser LoadSubscribeInfoByWX_Openid(string wx_openid)
        {
            StringBuilder sql = new StringBuilder("SELECT id,wx_openid,wx_unionid,is_subscribe,subscribe_time,unsubscribe_time FROM wxuser WHERE wx_openid=@wx_openid ORDER BY id DESC LIMIT 1;");
            List<NpgsqlParameter> parameters = new List<NpgsqlParameter>() {
                 new NpgsqlParameter() { ParameterName=mapper.paramNameSign+nameof(wxuser.wx_openid), Value = wx_openid, NpgsqlDbType =NpgsqlDbType.Varchar }
            };

            using(NpgsqlDataReader reader = NpgsqlHelper.ExecuteReader(mapper.DBConnectionString, CommandType.Text, sql.ToString(), parameters.ToArray()))
            {
                if(reader.Read())
                    return mapper.Load(reader);
            }
            return null;
        }

        public int SubscribeByWX_Openid(string wx_openid, DateTime subscribe_time)
        {
            StringBuilder sql = new StringBuilder("UPDATE wxuser SET is_subscribe=@is_subscribe,subscribe_time=@subscribe_time WHERE wx_openid=@wx_openid;");
            List<NpgsqlParameter> parameters = new List<NpgsqlParameter>() {
                 new NpgsqlParameter() { ParameterName=mapper.paramNameSign+nameof(wxuser.wx_openid), Value = wx_openid, NpgsqlDbType = NpgsqlDbType.Varchar },

                 new NpgsqlParameter() { ParameterName=mapper.paramNameSign+nameof(wxuser.is_subscribe), Value = true, NpgsqlDbType = NpgsqlDbType.Boolean },
                 new NpgsqlParameter() { ParameterName=mapper.paramNameSign+nameof(wxuser.subscribe_time), Value = subscribe_time , NpgsqlDbType = NpgsqlDbType.TimestampTZ }
            };

            int result = NpgsqlHelper.ExecuteNonQuery(DataRepository.DBConnectionString, CommandType.Text, sql.ToString(), parameters.ToArray());
            return result;
        }
        public int UnSubscribeByWX_Openid(string wx_openid, DateTime unsubscribe_time)
        {
            StringBuilder sql = new StringBuilder("UPDATE wxuser SET is_subscribe=@is_subscribe,unsubscribe_time=@unsubscribe_time WHERE wx_openid=@wx_openid;");
            List<NpgsqlParameter> parameters = new List<NpgsqlParameter>() {
                 new NpgsqlParameter() { ParameterName=mapper.paramNameSign+nameof(wxuser.wx_openid), Value = wx_openid, NpgsqlDbType = NpgsqlDbType.Varchar },

                 new NpgsqlParameter() { ParameterName=mapper.paramNameSign+nameof(wxuser.is_subscribe), Value = false, NpgsqlDbType = NpgsqlDbType.Boolean },
                 new NpgsqlParameter() { ParameterName=mapper.paramNameSign+nameof(wxuser.unsubscribe_time), Value = unsubscribe_time , NpgsqlDbType = NpgsqlDbType.TimestampTZ }
            };

            int result = NpgsqlHelper.ExecuteNonQuery(DataRepository.DBConnectionString, CommandType.Text, sql.ToString(), parameters.ToArray());
            return result;
        }

        public int UpdateWXinfoByWX_Openid(wxuser entity)
        {
            List<NpgsqlParameter> parameters = new List<NpgsqlParameter>();

            StringBuilder updateColumn = new StringBuilder();
            updateColumn.AppendFormat("{0}=@{0},", nameof(wxuser.wx_openid));

            if(!string.IsNullOrWhiteSpace(entity.wx_unionid))
            {
                updateColumn.AppendFormat("{0}=@{0},", nameof(wxuser.wx_unionid));
                parameters.Add(new NpgsqlParameter() { ParameterName = mapper.paramNameSign + nameof(wxuser.wx_unionid), Value = entity.wx_unionid, NpgsqlDbType = NpgsqlDbType.Varchar });
            }
            if(entity.is_subscribe.HasValue)
            {
                updateColumn.AppendFormat("{0}=@{0},", nameof(wxuser.is_subscribe));
                parameters.Add(new NpgsqlParameter() { ParameterName = mapper.paramNameSign + nameof(wxuser.is_subscribe), Value = entity.is_subscribe, NpgsqlDbType = NpgsqlDbType.Boolean });
                if(entity.is_subscribe.Value)
                {
                    updateColumn.AppendFormat("{0}=@{0},", nameof(wxuser.subscribe_time));
                    updateColumn.AppendFormat("{0}=@{0},", nameof(wxuser.nickname));
                    updateColumn.AppendFormat("{0}=@{0},", nameof(wxuser.sex));
                    updateColumn.AppendFormat("{0}=@{0},", nameof(wxuser.language));
                    updateColumn.AppendFormat("{0}=@{0},", nameof(wxuser.headimgurl));

                    parameters.Add(new NpgsqlParameter() { ParameterName = mapper.paramNameSign + nameof(wxuser.subscribe_time), Value = entity.subscribe_time, NpgsqlDbType = NpgsqlDbType.TimestampTZ });
                    parameters.Add(new NpgsqlParameter() { ParameterName = mapper.paramNameSign + nameof(wxuser.nickname), Value = entity.nickname, NpgsqlDbType = NpgsqlDbType.Text });
                    parameters.Add(new NpgsqlParameter() { ParameterName = mapper.paramNameSign + nameof(wxuser.sex), Value = entity.sex, NpgsqlDbType = NpgsqlDbType.Integer });
                    parameters.Add(new NpgsqlParameter() { ParameterName = mapper.paramNameSign + nameof(wxuser.language), Value = entity.language, NpgsqlDbType = NpgsqlDbType.Varchar });
                    parameters.Add(new NpgsqlParameter() { ParameterName = mapper.paramNameSign + nameof(wxuser.headimgurl), Value = entity.headimgurl, NpgsqlDbType = NpgsqlDbType.Varchar });
                }
            }

            StringBuilder whereColumn = new StringBuilder();
            whereColumn.AppendFormat("{0}=@{0}", nameof(wxuser.wx_openid));
            parameters.Add(new NpgsqlParameter() { ParameterName = mapper.paramNameSign + nameof(wxuser.wx_openid), Value = entity.wx_openid, NpgsqlDbType = NpgsqlDbType.Varchar });

            int result = NpgsqlHelper.ExecuteNonQuery(DataRepository.DBConnectionString, CommandType.Text, string.Concat("UPDATE wxuser SET ", updateColumn.ToString().Trim(new Char[] { ' ', ',' }), " WHERE ", whereColumn.ToString().Trim(new Char[] { ' ', ',' })), parameters.ToArray());
            return result;
        }
        #endregion

    }
}
