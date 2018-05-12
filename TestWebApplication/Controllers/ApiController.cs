using ClassLib4Net;
using ClassLib4Net.MVCControl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TestWebApplication.Controllers
{
    public class ApiController : Controller
    {
        // GET: Api

        public JsonResult Get()
        {
            var result = new JsonResult() { JsonRequestBehavior = JsonRequestBehavior.AllowGet, ContentEncoding = System.Text.Encoding.UTF8 };
            result.Data = new ClassLib4Net.Api.ApiModel()
            {
                Status = 200,
                Message = "ok"
            };

            return result;
        }

        public class PeopleModel
        {
            public string Name { get; set; }
            public string Nationality { get; set; }
            public string Province { get; set; }
            public int Age { get; set; }
        }

        public JsonResult GetData()
        {
            var result = new JsonResult() { JsonRequestBehavior = JsonRequestBehavior.AllowGet, ContentEncoding = System.Text.Encoding.UTF8 };
            result.Data = new ClassLib4Net.Api.ApiDataModel()
            {
                Data = new PeopleModel() { Name = "熊仔其人", Nationality = "中国", Province = "北京", Age = 25 },
                Status = 200,
                Message = "ok"
            };

            return result;
        }
        public JsonResult GetList()
        {
            var result = new JsonResult() { JsonRequestBehavior = JsonRequestBehavior.AllowGet, ContentEncoding = System.Text.Encoding.UTF8 };
            result.Data = new ClassLib4Net.Api.ApiListModel<PeopleModel>
            {
                List = new List<PeopleModel>() {
                     new PeopleModel(){ Name = "熊仔其人", Nationality = "中国", Province = "北京", Age = 25 },
                     new PeopleModel(){ Name = "xiongzaiqiren", Nationality = "China", Province = "Beijing", Age = 25 },
                     new PeopleModel(){ Name = "熊学浩", Nationality = "中国", Province = "河南" , Age = 25}
                 },
                PageIndex = 1,
                PageSize = 2,
                Total = 3,
                Status = 200,
                Message = "ok"
            };

            return result;
        }
        public JsonResult GetDataList()
        {
            var result = new JsonResult() { JsonRequestBehavior = JsonRequestBehavior.AllowGet, ContentEncoding = System.Text.Encoding.UTF8 };

            var entity = new ClassLib4Net.Api.ApiDataListModel();
            entity.Data.List = new List<PeopleModel>(){
                         new PeopleModel(){ Name = "熊仔其人", Nationality = "中国", Province = "北京" },
                         new PeopleModel(){ Name = "xiongzaiqiren", Nationality = "China", Province = "Beijing" },
                         new PeopleModel(){ Name = "熊学浩", Nationality = "中国", Province = "河南" }
                     };
            entity.Data.PageIndex = 1;
            entity.Data.PageSize = 2;
            entity.Data.Total = 3;

            entity.Status = 200;
            entity.Message = "ok";

            result.Data = entity;
            return result;
        }

        /// <summary>
        /// 解析基础接口模型
        /// </summary>
        public void Read()
        {
            string s = ClassLib4Net.Http.HttpHelper.Request(Request.Url.Scheme + "://" + Request.Url.Host + "/Api/Get");
            var json1 = s.ToObject<ClassLib4Net.Api.ApiModel>();
            var json2 = JsonHelper.DeSerialize<ClassLib4Net.Api.ApiModel>(s);
        }
        /// <summary>
        /// 解析数据接口模型
        /// </summary>
        public void ReadData()
        {
            string s = ClassLib4Net.Http.HttpHelper.Request(Request.Url.Scheme + "://" + Request.Url.Host + "/Api/GetData");
            var json1 = s.ToObject<ClassLib4Net.Api.ApiDataModel<PeopleModel>>();
            var json2 = JsonHelper.DeSerialize<ClassLib4Net.Api.ApiDataModel<PeopleModel>>(s);
        }
        /// <summary>
        /// 解析列表数据接口模型
        /// </summary>
        public void ReadList()
        {
            string s = ClassLib4Net.Http.HttpHelper.Request(Request.Url.Scheme + "://" + Request.Url.Host + "/Api/GetList");
            var json1 = s.ToObject<ClassLib4Net.Api.ApiListModel<PeopleModel>>();
            var json2 = JsonHelper.DeSerialize<ClassLib4Net.Api.ApiListModel<PeopleModel>>(s);
        }
        /// <summary>
        /// 解析数据列表接口模型
        /// </summary>
        public void ReadDataList()
        {
            string s = ClassLib4Net.Http.HttpHelper.Request(Request.Url.Scheme + "://" + Request.Url.Host + "/Api/GetDataList");
            var json1 = s.ToObject<ClassLib4Net.Api.ApiDataListModel<PeopleModel>>();
            var json2 = JsonHelper.DeSerialize<ClassLib4Net.Api.ApiDataListModel<PeopleModel>>(s);
        }

        #region Jsonp
        /// <summary>
        /// 处理Jsonp请求
        /// </summary>
        /// <param name="createrid"></param>
        /// <param name="creater"></param>
        /// <returns></returns>
        //[HttpGet]
        [ActionName("TestJsonp")]
        public ActionResult TestJsonp(string createrid, string creater)
        {
            var result = new ClassLib4Net.Api.ApiDataModel();
            result.Data = new { describe = "Jsonp by ClassLib4Net", createrid = createrid, creater = creater };
            result.Status = 200;
            result.Message = "ok";

            return this.Jsonp(result);
        }

        /// <summary>
        /// 处理Jsonp请求
        /// </summary>
        /// <param name="createrid"></param>
        /// <param name="creater"></param>
        /// <returns></returns>
        //[HttpGet]
        [ActionName("TestListJsonp")]
        public ActionResult TestListJsonp(string createrid, string creater)
        {
            var result = new ClassLib4Net.Api.ApiListModel();
            var list = new List<PeopleModel>() {
                new PeopleModel() { Name="AAA", Age=25, Nationality="abc", Province="北京"  },
                new PeopleModel() { Name="BBB", Age=28, Nationality="def", Province="天津"  },
                new PeopleModel() { Name="CCC", Age=30, Nationality="hig", Province="郑州"  },
            };

            result.List = list;
            result.Total = list.Count;
            result.PageIndex = 1;
            result.PageSize = 10;

            result.Status = 200;
            result.Message = "ok";

            return this.Jsonp(result);
        }


        /// <summary>
        /// 处理Jsonp请求
        /// </summary>
        /// <param name="createrid"></param>
        /// <param name="creater"></param>
        /// <returns></returns>
        //[HttpGet]
        [ActionName("TestDataListJsonp")]
        public ActionResult TestDataListJsonp(string createrid, string creater)
        {
            var result = new ClassLib4Net.Api.ApiDataListModel();
            var list = new List<PeopleModel>() {
                new PeopleModel() { Name="AAA", Age=25, Nationality="abc", Province="北京"  },
                new PeopleModel() { Name="BBB", Age=28, Nationality="def", Province="天津"  },
                new PeopleModel() { Name="CCC", Age=30, Nationality="hig", Province="郑州"  },
            };

            result.Data.List = list;
            result.Data.Total = list.Count;
            result.Data.PageIndex = 1;
            result.Data.PageSize = 10;

            result.Status = 200;
            result.Message = "ok";

            return this.Jsonp(result);
        }

        #endregion

    }
}