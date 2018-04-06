using ClassLib4Net.Sample.Data.ORM.MySql.DAL;
using ClassLib4Net.Sample.Data.ORM.MySql.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLib4Net.Sample.Data.ORM.MySql.BLL.Provider
{
    public sealed class epinfoBLL
    {
        #region Instance
        private static readonly epinfoBLL instance = new epinfoBLL();
        private epinfoBLL() { }
        internal static epinfoBLL Instance { get { return instance; } }
        #endregion

        #region Base Methods
        public object Insert(epinfo entity)
        {
            return DataRepository.epinfoDAL.Insert(entity);
        }

        public IList<epinfo> LoadAll()
        {
            return DataRepository.epinfoDAL.LoadAll();
        }


        public int Update(epinfo entity)
        {
            return DataRepository.epinfoDAL.Update(entity);
        }

        #endregion

        #region Extension Methods
        public epinfo LoadByName(string epGuid, string epName)
        {
            if(string.IsNullOrWhiteSpace(epGuid) || string.IsNullOrWhiteSpace(epName)) return null;
            return DataRepository.epinfoDAL.LoadByName(epGuid, epName);
        }

        public int UpdateNameByGuid(string epGuid, string epName)
        {
            if(string.IsNullOrWhiteSpace(epGuid) || string.IsNullOrWhiteSpace(epName)) return 0;
            return DataRepository.epinfoDAL.UpdateNameByGuid(epGuid, epName);
        }
        #endregion

    }
}
