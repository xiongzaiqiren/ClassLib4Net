using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

/// <summary>
/// Api模型
/// </summary>
namespace ClassLib4Net.Api
{
    #region interface
    /// <summary>
    /// 分页参数
    /// </summary>
    public interface IPaging
    {
        /// <summary>
        /// 页容量
        /// </summary>
        long PageSize { get; set; }
        /// <summary>
        /// 页索引
        /// </summary>
        long PageIndex { get; set; }
    }
    /// <summary>
    /// 分页总数
    /// </summary>
    public interface IPageCount
    {
        /// <summary>
        /// 分页总数
        /// </summary>
        long PageCount { get; set; }
    }
    /// <summary>
    /// 总数
    /// </summary>
    public interface ITotal
    {
        /// <summary>
        /// 总数
        /// </summary>
        long Total { get; set; }
    }
    /// <summary>
    /// 状态信息
    /// </summary>
    public interface IBase
    {
        /// <summary>
        /// 状态码
        /// </summary>
        long Status { get; set; }
        /// <summary>
        /// 信息
        /// </summary>
        string Message { get; set; }
    }
    #endregion

    #region api data
    /// <summary>
    /// 基本接口模型
    /// </summary>
    [Serializable]
    [DataContract]
    public class ApiModel : IBase
    {
        /// <summary>
        /// 状态码
        /// </summary>
        [DataMember(EmitDefaultValue = true, IsRequired = true)]
        public long Status { get; set; }
        /// <summary>
        /// 信息
        /// </summary>
        [DataMember(EmitDefaultValue = true, IsRequired = true)]
        public string Message { get; set; }

        public ApiModel() : base() { Status = 0; Message = ""; }
        public ApiModel(long status = 0, string message = "") : base() { Status = status; Message = message; }
    }

    /// <summary>
    /// 数据接口模型
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Serializable]
    [DataContract]
    public class ApiDataModel<T> : ApiModel
    {
        /// <summary>
        /// 数据
        /// </summary>
        [DataMember(EmitDefaultValue = true, IsRequired = true)]
        public T Data { get; set; }

        public ApiDataModel() : base() { Data = default(T); }
        public ApiDataModel(T data = default(T), long status = 0, string message = "") : base(status, message) { Data = data; }
    }

    /// <summary>
    /// 数据接口模型
    /// </summary>
    [Serializable]
    [DataContract]
    public class ApiDataModel : ApiDataModel<object>
    {
        public ApiDataModel() : base() { }
        public ApiDataModel(object data = default(object), long status = 0, string message = "") : base(data, status, message) { }
    }
    #endregion

    #region api list
    [Serializable]
    [DataContract]
    public abstract class abstractApiList : IPaging, IPageCount, ITotal
    {
        /// <summary>
        /// 列表数据
        /// </summary>
        [DataMember(EmitDefaultValue = true, IsRequired = true)]
        public virtual ICollection List { get; set; }
        /// <summary>
        /// 页容量
        /// </summary>
        [DataMember(EmitDefaultValue = true, IsRequired = true)]
        public virtual long PageSize { get; set; }
        /// <summary>
        /// 页索引
        /// </summary>
        [DataMember(EmitDefaultValue = true, IsRequired = true)]
        public virtual long PageIndex { get; set; }
        /// <summary>
        /// 总数
        /// </summary>
        [DataMember(EmitDefaultValue = true, IsRequired = true)]
        public virtual long Total { get; set; }

        /// <summary>
        /// 分页总数(根据总数和页容量自动计算)
        /// </summary>
        [DataMember(EmitDefaultValue = true, IsRequired = true)]
        public virtual long PageCount
        {
            get
            {
                if(PageSize < 1)
                    throw new ArgumentException("Must be a positive integer greater than zero.", nameof(PageSize));
                return (Total - 1) / PageSize + 1;
            }
            set { }
        }

        public abstractApiList() : base() { List = default(ICollection); }
        public abstractApiList(ICollection list = default(ICollection), long total = 0) : base() { List = list; Total = total; }
    }

    [Serializable]
    [DataContract]
    public class ApiListModel : abstractApiList, IBase
    {
        /// <summary>
        /// 状态码
        /// </summary>
        [DataMember(EmitDefaultValue = true, IsRequired = true)]
        public long Status { get; set; }
        /// <summary>
        /// 信息
        /// </summary>
        [DataMember(EmitDefaultValue = true, IsRequired = true)]
        public string Message { get; set; }

        public ApiListModel() : base() { List = default(ICollection); }
        public ApiListModel(ICollection list = default(ICollection), long total = 0) : base(list, total) { }
        public ApiListModel(ICollection list = default(ICollection), long total = 0, long status = 0, string message = "") : base(list, total) { Status = status; Message = message; }
    }

    /// <summary>
    /// 列表数据接口模型
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Obsolete("不推荐使用此泛型类，推荐使用" + nameof(ApiListModel))]
    [Serializable]
    [DataContract]
    public class ApiListModel<T> : ApiListModel
    {
        /// <summary>
        /// 列表数据
        /// </summary>
        [DataMember(EmitDefaultValue = true, IsRequired = true)]
        public new ICollection<T> List { get; set; }

        public ApiListModel() : base() { List = default(ICollection<T>); }
        public ApiListModel(ICollection<T> list = default(ICollection<T>), long total = 0, long status = 0, string message = "") { List = list; Total = total; Status = status; Message = message; }
    }
    #endregion

    #region ApiList
    /// <summary>
    /// 列表数据类型
    /// </summary>
    [Serializable]
    [DataContract]
    public class ApiList : abstractApiList
    {
        public ApiList() : base() { }
        public ApiList(ICollection list = default(ICollection), long total = 0) : base(list, total) { }
    }

    /// <summary>
    /// 列表数据类型
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Serializable]
    [DataContract]
    public class ApiList<T> : ApiList
    {
        /// <summary>
        /// 列表数据
        /// </summary>
        [DataMember(EmitDefaultValue = true, IsRequired = true)]
        public new ICollection<T> List { get; set; }

        public ApiList() { List = default(ICollection<T>); }
        public ApiList(ICollection<T> list = default(ICollection<T>), long total = 0) { List = list; Total = total; }
    }
    #endregion

    /// <summary>
    /// 数据列表接口模型
    /// </summary>
    [Serializable]
    [DataContract]
    public class ApiDataListModel : ApiDataModel<ApiList>
    {
        public ApiDataListModel() : base() { Data = new ApiList(); }
        public ApiDataListModel(ApiList data, long status = 0, string message = "") : base(data, status, message) { }
    }

    /// <summary>
    /// 数据列表接口模型
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Obsolete("不推荐使用此泛型类，推荐使用" + nameof(ApiDataListModel))]
    [Serializable]
    [DataContract]
    public class ApiDataListModel<T> : ApiDataModel<ApiList<T>>
    {
        public ApiDataListModel() : base() { Data = new ApiList<T>(); }
        public ApiDataListModel(ApiList<T> data, long total = 0, long status = 0, string message = "") : base(data, status, message) { }
    }

}
