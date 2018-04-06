using ClassLib4Net.Sample.Data.ORM.Npgsql.BLL.Provider;

namespace ClassLib4Net.Sample.Data.ORM.Npgsql.BLL
{
    public class BLLRepository
    {
        public static wxtokenBLL wxtokenBLL { get { return wxtokenBLL.Instance; } }
        public static wxuserBLL wxuserBLL { get { return wxuserBLL.Instance; } }
        
    }
}
