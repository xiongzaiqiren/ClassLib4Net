using ClassLib4Net.Sample.Data.ORM.Npgsql.DAL;
using ClassLib4Net.Sample.Data.ORM.Npgsql.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ClassLib4Net.Sample.Data.ORM.Npgsql.BLL.Provider
{
    public sealed class wxuserBLL
    {
        #region Instance
        private static readonly wxuserBLL instance = new wxuserBLL();
        private wxuserBLL() { }
        internal static wxuserBLL Instance { get { return instance; } }
        #endregion

        #region Base Methods
        public object Insert(wxuser entity)
        {
            entity.creationtime = DateTime.Now;
            return DataRepository.wxuserDAL.Insert(entity);
        }

        public IList<wxuser> LoadAll()
        {
            return DataRepository.wxuserDAL.LoadAll();
        }


        public int Update(wxuser entity)
        {
            return DataRepository.wxuserDAL.Update(entity);
        }

        #endregion

        #region Extension Methods
        public bool existWX_Openid(string wx_openid)
        {
            if(string.IsNullOrWhiteSpace(wx_openid)) return false;
            return DataRepository.wxuserDAL.existWX_Openid(wx_openid);
        }
        public wxuser LoadByWX_Openid(string wx_openid)
        {
            if(string.IsNullOrWhiteSpace(wx_openid)) return null;
            return DataRepository.wxuserDAL.LoadByWX_Openid(wx_openid);
        }

        public wxuser LoadSubscribeInfoByWX_Openid(string wx_openid)
        {
            if(string.IsNullOrWhiteSpace(wx_openid)) return null;
            return DataRepository.wxuserDAL.LoadSubscribeInfoByWX_Openid(wx_openid);
        }
        public bool NeverSubscribe(wxuser entity)
        {
            if(null != entity && entity.id > 0)
            {
                if(entity.is_subscribe.HasValue && entity.is_subscribe.Value)
                    return false;
                else
                {
                    if(entity.subscribe_time.HasValue && entity.subscribe_time.Value.Year > 1970)
                        return false;
                    else if(entity.unsubscribe_time.HasValue && entity.unsubscribe_time.Value.Year > 1970)
                        return false;
                }
            }
            return true;
        }

        public int SubscribeByWX_Openid(string wx_openid, DateTime subscribe_time)
        {
            if(string.IsNullOrWhiteSpace(wx_openid) || subscribe_time.Year < 2016) return 0;
            return DataRepository.wxuserDAL.SubscribeByWX_Openid(wx_openid, subscribe_time);
        }
        public int UnSubscribeByWX_Openid(string wx_openid, DateTime unsubscribe_time)
        {
            if(string.IsNullOrWhiteSpace(wx_openid) || unsubscribe_time.Year < 2016) return 0;
            return DataRepository.wxuserDAL.UnSubscribeByWX_Openid(wx_openid, unsubscribe_time);
        }

        public int UpdateWXinfoByWX_Openid(wxuser entity)
        {
            if(null == entity || string.IsNullOrWhiteSpace(entity.wx_openid) || !entity.is_subscribe.HasValue) return 0;
            return DataRepository.wxuserDAL.UpdateWXinfoByWX_Openid(entity);
        }
        #endregion

    }
}
