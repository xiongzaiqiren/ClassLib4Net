using ClassLib4Net.Sample.Data.ORM.Npgsql.DAL.Provider;

namespace ClassLib4Net.Sample.Data.ORM.Npgsql.DAL
{
    public class DataRepository
    {
        internal static string DBConnectionString { get { return ConfigHelper.GetConnectionString("PostgreSqlConn"); } }

        public static wxtokenDAL wxtokenDAL { get { return wxtokenDAL.Instance; } }
        public static wxuserDAL wxuserDAL { get { return wxuserDAL.Instance; } }

    }
}
