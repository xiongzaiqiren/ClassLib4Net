using System;
using System.Data;

/*
* 简单的ORM框架
* 熊学浩
* 2016-11-15
*/
namespace ClassLib4Net.Data.ORM
{
    /// <summary>
    /// DataRow映射助手
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DataRowMapperHelper<T> where T : class
    {
        protected virtual T CreateDomainObject()
        {
            T t = Activator.CreateInstance(typeof(T)) as T;
            return t;
        }

        #region Load
        protected virtual void DoLoad(T t, DataRow dataRow)
        {
            object o = null;
            foreach (var p in t.GetType().GetProperties())
            {
                if (null != p && p.CanWrite)
                {
                    o = dataRow[p.Name];
                    if (o != DBNull.Value)
                        p.SetValue(t, o);
                }
            }
        }
        public T Load(DataRow dataRow)
        {
            T t = CreateDomainObject();
            DoLoad(t, dataRow);
            return t;
        }
        #endregion

    }
}
