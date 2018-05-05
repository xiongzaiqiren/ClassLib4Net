using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TestWebApplication.Controllers
{
    /// <summary>
    /// 列表
    /// </summary>
    public class ListController : Controller
    {
        // GET: List
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// 分页控件样例
        /// </summary>
        /// <returns></returns>
        public ActionResult SampleForPagering(int PageIndex = 1, int pageSize = 10)
        {
            var model = new ClassLib4Net.Api.ApiListModel();
            model.PageIndex = PageIndex;
            model.PageSize = pageSize;
            model.Total = 500;

            return View(model);
        }

    }
}