using System;
using System.Data;
using System.Reflection;

/*
* 简单的ORM框架
* 熊学浩
* 2016-11-15
*/
namespace ClassLib4Net.Data.ORM
{
    #region MapperAttribute
    /// <summary>
    /// 简易数据映射属性
    /// </summary>
    [AttributeUsage(AttributeTargets.All, AllowMultiple = true)]
    public class DataMapperAttribute : Attribute
    {
        public virtual string Name { get; set; }
        public virtual string Describe { get; set; }
        public virtual long Size { get; set; }

        
        public virtual bool CanLoad { get; set; }
        public virtual bool CanInsert { get; set; }
        public virtual bool CanUpdate { get; set; }
        public virtual bool CanDelete { get; set; }

        /// <summary>
        /// 简易数据映射属性构造函数
        /// </summary>
        public DataMapperAttribute(string Name, long Size, string Describe = null, bool CanLoad = true, bool CanInsert = true, bool CanUpdate = true, bool CanDelete = false)
        {
            this.Name = Name;
            this.Describe = Describe;
            this.Size = Size;
            this.CanLoad = CanLoad;
            this.CanInsert = CanInsert;
            this.CanUpdate = CanUpdate;
            this.CanDelete = CanDelete;
        }

    }

    /// <summary>
    /// 简易数据表映射属性
    /// </summary>
    [AttributeUsage(AttributeTargets.All, AllowMultiple = true)]
    public class DataTableMapperAttribute : DataMapperAttribute
    {
        public LoadDataMode LoadMode { get; set; }

        /// <summary>
        /// 简易数据表映射属性构造函数
        /// </summary>
        public DataTableMapperAttribute(LoadDataMode LoadMode, string Name, long Size = 0, string Describe = null, bool CanLoad = true, bool CanInsert = true, bool CanUpdate = true, bool CanDelete = false) : base(Name, Size, Describe, CanLoad, CanInsert, CanUpdate, CanDelete)
        {
            this.LoadMode = LoadMode;
            switch (this.LoadMode)
            {
                case LoadDataMode.ComplexQuery:
                case LoadDataMode.View:
                case LoadDataMode.StoredProcedure:
                case LoadDataMode.Enums:
                    CanInsert = false;
                    CanUpdate = false;
                    CanDelete = false;
                    break;
            }

        }
    }

    /// <summary>
    /// 简易数据表字段映射属性
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class DataColumnMapperAttribute : DataMapperAttribute
    {
        public MappingType MappingType { get; set; }
        
        /// <summary>
        /// 是主键
        /// </summary>
        public bool IsPrimaryKey { get; set; }
        /// <summary>
        /// 是外键
        /// </summary>
        public bool IsForeignKey { get; set; }
        /// <summary>
        /// 是ID标识(自增标识)
        /// </summary>
        public bool IsIdentity { get; set; }
        /// <summary>
        /// 可空
        /// </summary>
        public bool CanNull { get; set; }
        /// <summary>
        /// 可自动初始化(默认值)
        /// </summary>
        public bool CanDefaultValue { get; set; }

        /// <summary>
        /// 简易数据表字段映射属性构造函数
        /// </summary>
        public DataColumnMapperAttribute(long Size, MappingType MappingType = MappingType.Attribute, bool IsPrimaryKey = false, bool IsForeignKey = false, bool IsIdentity = false, bool CanNull = true, bool CanDefaultValue = false, string Name = null, string Describe = null, bool CanLoad = true, bool CanInsert = true, bool CanUpdate = true, bool CanDelete = false) : base(Name, Size, Describe, CanLoad, CanInsert, CanUpdate, CanDelete)
        {
            this.MappingType = MappingType;
            this.IsPrimaryKey = IsPrimaryKey;
            this.IsForeignKey = IsForeignKey;
            this.IsIdentity = IsIdentity;
            this.CanNull = CanNull;
            this.CanDefaultValue = CanDefaultValue;

            if (this.IsPrimaryKey || this.IsForeignKey)
            {
                this.CanNull = false;
                this.CanDelete = false;
            }
            if (this.CanDefaultValue)
            {
                this.CanNull = true;
            }
            if (this.IsIdentity)
            {
                this.CanNull = true;
                this.CanInsert = false;
                this.CanUpdate = false;
            }
        }
    }
    #endregion


    #region MapperHelper
    /// <summary>
    /// 数据表映射助手
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DataTableMapperHelper<T> where T : class
    {
        public static DataTableMapperAttribute[] GetCustomAttributes(T t)
        {
            DataTableMapperAttribute[] os = null;
            os = (DataTableMapperAttribute[])t.GetType().GetCustomAttributes(typeof(DataTableMapperAttribute), false);
            return os;
        }
        public static DataTableMapperAttribute GetCustomAttribute(T t)
        {
            DataTableMapperAttribute[] os = GetCustomAttributes(t);
            if (null != os && os.Length > 0)
                return os[0];
            return null;
        }
    }

    /// <summary>
    /// 数据表字段映射助手
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DataColumnMapperHelper<T> where T : class
    {
        public static DataColumnMapperAttribute[] GetCustomAttributes(T t)
        {
            DataColumnMapperAttribute[] os = null;
            os = (DataColumnMapperAttribute[])t.GetType().GetCustomAttributes(typeof(DataColumnMapperAttribute), false);
            return os;
        }
        public static DataColumnMapperAttribute GetCustomAttribute(T t)
        {
            DataColumnMapperAttribute[] os = GetCustomAttributes(t);
            if (null != os && os.Length > 0)
                return os[0];
            return null;
        }

        
        public static DataColumnMapperAttribute[] GetCustomAttributes(PropertyInfo PropertyInfo)
        {
            DataColumnMapperAttribute[] os = null;
            os = (DataColumnMapperAttribute[])PropertyInfo.GetCustomAttributes(typeof(DataColumnMapperAttribute), false);
            return os;
        }
        public static DataColumnMapperAttribute GetCustomAttribute(PropertyInfo PropertyInfo)
        {
            DataColumnMapperAttribute[] os = GetCustomAttributes(PropertyInfo);
            if (null != os && os.Length > 0)
                return os[0];
            return null;
        }

    } 
    #endregion

}
