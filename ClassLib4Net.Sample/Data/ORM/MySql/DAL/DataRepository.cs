using ClassLib4Net.Sample.Data.ORM.MySql.DAL.Provider;

namespace ClassLib4Net.Sample.Data.ORM.MySql.DAL
{
   public class DataRepository
    {
        internal static string DBConnectionString { get { return ConfigHelper.GetConnectionString("MySqlConn"); } }

        public static epinfoDAL epinfoDAL { get { return epinfoDAL.Instance; } }

    }
}
