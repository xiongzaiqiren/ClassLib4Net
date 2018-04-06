using ClassLib4Net.Sample.Data.ORM.Npgsql.DAL;
using ClassLib4Net.Sample.Data.ORM.Npgsql.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ClassLib4Net.Sample.Data.ORM.Npgsql.BLL.Provider
{
    public sealed class wxtokenBLL
    {
        #region Instance
        private static readonly wxtokenBLL instance = new wxtokenBLL();
        private wxtokenBLL() { }
        internal static wxtokenBLL Instance { get { return instance; } }
        #endregion

        #region Base Methods
        public object Insert(wxtoken entity)
        {
            return DataRepository.wxtokenDAL.Insert(entity);
        }

        public IList<wxtoken> LoadAll()
        {
            return DataRepository.wxtokenDAL.LoadAll();
        }


        public int Update(wxtoken entity)
        {
            return DataRepository.wxtokenDAL.Update(entity);
        }

        #endregion

        #region Extension Methods
        public wxtoken LoadByName(string name, string appid)
        {
            if(string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(appid)) return null;
            return DataRepository.wxtokenDAL.LoadByName(name, appid);
        }

        public int UpdateByName(wxtoken entity)
        {
            if(null == entity || string.IsNullOrWhiteSpace(entity.name) || string.IsNullOrWhiteSpace(entity.appid)) return 0;

            entity.creationtime = DateTime.Now;
            return DataRepository.wxtokenDAL.UpdateByName(entity);
        }
        #endregion

    }
}
