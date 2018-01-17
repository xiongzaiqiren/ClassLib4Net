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

        public ApiModel() { Status = 0; Message = ""; }
        public ApiModel(long status = 0, string message = "") { Status = status; Message = message; }
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
    /// <summary>
    /// 列表数据接口模型
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Serializable]
    [DataContract]
    public class ApiListModel<T> : ApiModel, IPaging, IPageCount, ITotal
    {
        /// <summary>
        /// 列表数据
        /// </summary>
        [DataMember(EmitDefaultValue = true, IsRequired = true)]
        public ICollection<T> List { get; set; }
        /// <summary>
        /// 页容量
        /// </summary>
        [DataMember(EmitDefaultValue = true, IsRequired = true)]
        public long PageSize { get; set; }
        /// <summary>
        /// 页索引
        /// </summary>
        [DataMember(EmitDefaultValue = true, IsRequired = true)]
        public long PageIndex { get; set; }
        /// <summary>
        /// 总数
        /// </summary>
        [DataMember(EmitDefaultValue = true, IsRequired = true)]
        public long Total { get; set; }

        /// <summary>
        /// 分页总数(根据总数和页容量自动计算)
        /// </summary>
        [DataMember(EmitDefaultValue = true, IsRequired = true)]
        public long PageCount { get { return (Total - 1) / PageSize + 1; } set { } }

        public ApiListModel() : base() { List = default(ICollection<T>); }
        public ApiListModel(ICollection<T> list = default(ICollection<T>), long total = 0, long status = 0, string message = "") : base(status, message) { List = list; Total = total; }
    }

    /// <summary>
    /// 列表数据接口模型
    /// </summary>
    [Serializable]
    [DataContract]
    public class ApiListModel : ApiListModel<object>
    {
        public ApiListModel() : base() { }
        public ApiListModel(ICollection<object> list = default(ICollection<object>), long total = 0, long status = 0, string message = "") : base(list, total, status, message) { }
    }
    #endregion

    #region ApiListData
    /// <summary>
    /// 列表数据类型
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Serializable]
    [DataContract]
    public class ApiListData<T> : IPaging, IPageCount, ITotal
    {
        /// <summary>
        /// 列表数据
        /// </summary>
        [DataMember(EmitDefaultValue = true, IsRequired = true)]
        public ICollection<T> List { get; set; }
        /// <summary>
        /// 页容量
        /// </summary>
        [DataMember(EmitDefaultValue = true, IsRequired = true)]
        public long PageSize { get; set; }
        /// <summary>
        /// 页索引
        /// </summary>
        [DataMember(EmitDefaultValue = true, IsRequired = true)]
        public long PageIndex { get; set; }
        /// <summary>
        /// 总数
        /// </summary>
        [DataMember(EmitDefaultValue = true, IsRequired = true)]
        public long Total { get; set; }

        /// <summary>
        /// 分页总数(根据总数和页容量自动计算)
        /// </summary>
        [DataMember(EmitDefaultValue = true, IsRequired = true)]
        public long PageCount { get { return (Total - 1) / PageSize + 1; } set { } }

        public ApiListData() { List = default(ICollection<T>); }
        public ApiListData(ICollection<T> list = default(ICollection<T>), long total = 0) { List = list; Total = total; }
    }

    /// <summary>
    /// 列表数据类型
    /// </summary>
    [Serializable]
    [DataContract]
    public class ApiListData : IPaging, IPageCount, ITotal
    {
        /// <summary>
        /// 列表数据
        /// </summary>
        [DataMember(EmitDefaultValue = true, IsRequired = true)]
        public ICollection List { get; set; }
        /// <summary>
        /// 页容量
        /// </summary>
        [DataMember(EmitDefaultValue = true, IsRequired = true)]
        public long PageSize { get; set; }
        /// <summary>
        /// 页索引
        /// </summary>
        [DataMember(EmitDefaultValue = true, IsRequired = true)]
        public long PageIndex { get; set; }
        /// <summary>
        /// 总数
        /// </summary>
        [DataMember(EmitDefaultValue = true, IsRequired = true)]
        public long Total { get; set; }

        /// <summary>
        /// 分页总数(根据总数和页容量自动计算)
        /// </summary>
        [DataMember(EmitDefaultValue = true, IsRequired = true)]
        public long PageCount { get { return (Total - 1) / PageSize + 1; } set { } }

        public ApiListData() { List = default(ICollection); }
        public ApiListData(ICollection list = default(ICollection), long total = 0) { List = list; Total = total; }
    }
    #endregion

    /// <summary>
    /// 数据列表接口模型
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Serializable]
    [DataContract]
    public class ApiDataListModel<T> : ApiDataModel<ApiListData<T>>
    {
        public ApiDataListModel() : base() { Data = new ApiListData<T>(); }
        public ApiDataListModel(ICollection<T> list = default(ICollection<T>), long total = 0) : base(new ApiListData<T>() { List = list, Total = total }) { }
    }

    /// <summary>
    /// 数据列表接口模型
    /// </summary>
    [Serializable]
    [DataContract]
    public class ApiDataListModel : ApiDataModel<ApiListData>
    {
        public ApiDataListModel() : base() { Data = new ApiListData(); }
        public ApiDataListModel(ICollection list = default(ICollection), long total = 0) : base(new ApiListData() { List = list, Total = total }) { }
    }

}
