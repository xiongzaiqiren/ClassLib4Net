using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace ClassLib4Net.Api
{
    #region interface
    public interface IPaging
    {
        int PageSize { get; set; }
        int PageIndex { get; set; }
    }
    public interface ITotal
    {
        int Total { get; set; }
    }
    public interface IBase
    {
        int Status { get; set; }
        string Message { get; set; }
    }
    #endregion

    [Serializable]
    [DataContract]
    public class ApiModel : IBase
    {
        [DataMember(EmitDefaultValue = true, IsRequired = true)]
        public int Status { get; set; }

        [DataMember(EmitDefaultValue = true, IsRequired = true)]
        public string Message { get; set; }

        public ApiModel(int status = 0, string message = "")
        {
            Status = status;
            Message = message;
        }
    }

    [Serializable]
    [DataContract]
    public class ApiDataModel<T> : ApiModel
    {
        [DataMember(EmitDefaultValue = true, IsRequired = true)]
        public T Data { get; set; }

        public ApiDataModel(T data = default(T), int status = 0, string message = "") : base(status, message)
        {
            Data = data;
        }
    }

    [Serializable]
    [DataContract]
    public class ApiDataModel : ApiDataModel<object>
    {
        public ApiDataModel(object data = null, int status = 0, string message = "") : base(data, status, message) { }
    }

    #region internal

    [Serializable]
    [DataContract]
    public class ApiListData<T> : IPaging, ITotal
    {
        [DataMember(EmitDefaultValue = true, IsRequired = true)]
        public IList<T> List { get; set; }

        [DataMember(EmitDefaultValue = true, IsRequired = true)]
        public int PageSize { get; set; }
        [DataMember(EmitDefaultValue = true, IsRequired = true)]
        public int PageIndex { get; set; }
        [DataMember(EmitDefaultValue = true, IsRequired = true)]
        public int Total { get; set; }

        [DataMember(EmitDefaultValue = true, IsRequired = true)]
        public int PageCount { get { return (Total - 1) / PageSize + 1; } }

        internal ApiListData() { }
    }

    [Serializable]
    [DataContract]
    public class ApiListData : IPaging, ITotal
    {
        [DataMember(EmitDefaultValue = true, IsRequired = true)]
        public IList List { get; set; }

        [DataMember(EmitDefaultValue = true, IsRequired = true)]
        public int PageSize { get; set; }
        [DataMember(EmitDefaultValue = true, IsRequired = true)]
        public int PageIndex { get; set; }
        [DataMember(EmitDefaultValue = true, IsRequired = true)]
        public int Total { get; set; }


        [DataMember(EmitDefaultValue = true, IsRequired = true)]
        public int PageCount { get { return (Total - 1) / PageSize + 1; } }

        internal ApiListData() { }
    }
    #endregion

    [Serializable]
    [DataContract]
    public class ApiDataListModel<T> : ApiDataModel<ApiListData<T>>
    {
        public ApiDataListModel(IList<T> list = null, int total = 0) : base(new ApiListData<T>() { List = list ?? new List<T>(), Total = total }) { }
    }

    [Serializable]
    [DataContract]
    public class ApiDataListModel : ApiDataModel<ApiListData>
    {
        public ApiDataListModel(IList list, int total = 0) : base(new ApiListData() { List = list, Total = total }) { }
    }

    [Serializable]
    [DataContract]
    public class ApiListModel<T> : ApiModel, IPaging, ITotal
    {
        [DataMember(EmitDefaultValue = true, IsRequired = true)]
        public IList<T> List { get; set; }

        [DataMember(EmitDefaultValue = true, IsRequired = true)]
        public int PageSize { get; set; }
        [DataMember(EmitDefaultValue = true, IsRequired = true)]
        public int PageIndex { get; set; }
        [DataMember(EmitDefaultValue = true, IsRequired = true)]
        public int Total { get; set; }


        [DataMember(EmitDefaultValue = true, IsRequired = true)]
        public int PageCount { get { return (Total - 1) / PageSize + 1; } }

        public ApiListModel(IList<T> list = null, int total = 0, int status = 0, string message = "") : base(status, message)
        {
            Total = total;
            List = list ?? new List<T>();
        }
    }

    [Serializable]
    [DataContract]
    public class ApiListModel : ApiListModel<object>
    {
        public ApiListModel(IList<object> list = null, int total = 0, int status = 0, string message = "") : base(list, total, status, message) { }
    }




}
