using ClassLib4Net.Sample.Data.ORM.MySql.BLL.Provider;

namespace ClassLib4Net.Sample.Data.ORM.MySql.BLL
{
   public class BLLRepository
    {
        public static epinfoBLL epinfoBLL { get { return epinfoBLL.Instance; } }
    }
}
