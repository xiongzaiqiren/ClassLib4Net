using System;
using System.Data;
using System.Reflection;

/*
* 简单的ORM框架
* 熊学浩
* 2016-11-15
*/
namespace ClassLib4Net.Data.ORM.SqlServer
{
    class SqlServerDataMapperAttribute
    {
    }

    #region MapperAttribute
    /// <summary>
    /// SqlServer数据表映射属性
    /// </summary>
    [AttributeUsage(AttributeTargets.All, AllowMultiple = true)]
    public class SqlServerTableMapperAttribute : DataTableMapperAttribute
    {
        /// <summary>
        /// SqlServer数据表映射属性构造函数
        /// </summary>
        /// <param name="LoadMode">数据加载模式</param>
        /// <param name="Name">名称</param>
        /// <param name="CanLoad">是否可装载</param>
        /// <param name="CanInsert">是否可插入</param>
        /// <param name="CanUpdate">是否可更新</param>
        /// <param name="CanDelete">是否可删除</param>
        public SqlServerTableMapperAttribute(LoadDataMode LoadMode, string Name, long Size = 0, string Describe = null, bool CanLoad = true, bool CanInsert = true, bool CanUpdate = true, bool CanDelete = false) : base(LoadMode, Name, Size, Describe, CanLoad, CanInsert, CanUpdate, CanDelete)
        {
        }
    }

    /// <summary>
    /// SqlServer数据表字段映射属性
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class SqlServerColumnMapperAttribute : DataColumnMapperAttribute
    {
        /// <summary>
        /// 字段类型
        /// </summary>
        public SqlDbType DbType { get; set; }

        /// <summary>
        /// SqlServer数据表字段映射属性构造函数
        /// </summary>
        /// <param name="DbType"></param>
        /// <param name="Size"></param>
        /// <param name="CanNull"></param>
        /// <param name="IsPrimaryKey"></param>
        /// <param name="IsForeignKey"></param>
        /// <param name="IsIdentity"></param>
        /// <param name="CanDefaultValue"></param>
        /// <param name="MappingType"></param>
        /// <param name="Name"></param>
        /// <param name="Describe"></param>
        /// <param name="CanLoad"></param>
        /// <param name="CanInsert"></param>
        /// <param name="CanUpdate"></param>
        /// <param name="CanDelete"></param>
        public SqlServerColumnMapperAttribute(SqlDbType DbType, long Size = 50, bool CanNull = true, bool IsPrimaryKey = false, bool IsForeignKey = false, bool IsIdentity = false, bool CanDefaultValue = false, MappingType MappingType = MappingType.Attribute, string Name = null, string Describe = null, bool CanLoad = true, bool CanInsert = true, bool CanUpdate = true, bool CanDelete = false) : base(Size, MappingType, IsPrimaryKey, IsForeignKey, IsIdentity, CanNull, CanDefaultValue, Name, Describe, CanLoad, CanInsert, CanUpdate, CanDelete)
        {
            this.DbType = DbType;
        }
    }
    #endregion


    #region MapperHelper
    /// <summary>
    /// SqlServer数据表映射助手
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class SqlServerTableMapperHelper<T> where T : class
    {
        /// <summary>
        /// 获取自定义属性集合
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public static SqlServerTableMapperAttribute[] GetCustomAttributes(T t)
        {
            SqlServerTableMapperAttribute[] os = null;
            os = (SqlServerTableMapperAttribute[])t.GetType().GetCustomAttributes(typeof(SqlServerTableMapperAttribute), false);
            return os;
        }
        /// <summary>
        /// 获取下标索引为0的自定义属性
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public static SqlServerTableMapperAttribute GetCustomAttribute(T t)
        {
            SqlServerTableMapperAttribute[] os = GetCustomAttributes(t);
            if (null != os && os.Length > 0)
                return os[0];
            return null;
        }
    }

    /// <summary>
    /// SqlServer数据表字段映射助手
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class SqlServerColumnMapperHelper<T> where T : class
    {
        /// <summary>
        /// 获取自定义属性集合
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public static SqlServerColumnMapperAttribute[] GetCustomAttributes(T t)
        {
            SqlServerColumnMapperAttribute[] os = null;
            os = (SqlServerColumnMapperAttribute[])t.GetType().GetCustomAttributes(typeof(SqlServerColumnMapperAttribute), false);
            return os;
        }
        /// <summary>
        /// 获取下标索引为0的自定义属性
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public static SqlServerColumnMapperAttribute GetCustomAttribute(T t)
        {
            SqlServerColumnMapperAttribute[] os = GetCustomAttributes(t);
            if (null != os && os.Length > 0)
                return os[0];
            return null;
        }

        /// <summary>
        /// 获取自定义属性集合
        /// </summary>
        /// <param name="PropertyInfo"></param>
        /// <returns></returns>
        public static SqlServerColumnMapperAttribute[] GetCustomAttributes(PropertyInfo PropertyInfo)
        {
            SqlServerColumnMapperAttribute[] os = null;
            os = (SqlServerColumnMapperAttribute[])PropertyInfo.GetCustomAttributes(typeof(SqlServerColumnMapperAttribute), false);
            return os;
        }
        /// <summary>
        /// 获取下标索引为0的自定义属性
        /// </summary>
        /// <param name="PropertyInfo"></param>
        /// <returns></returns>
        public static SqlServerColumnMapperAttribute GetCustomAttribute(PropertyInfo PropertyInfo)
        {
            SqlServerColumnMapperAttribute[] os = GetCustomAttributes(PropertyInfo);
            if (null != os && os.Length > 0)
                return os[0];
            return null;
        }

    }
    #endregion

}
