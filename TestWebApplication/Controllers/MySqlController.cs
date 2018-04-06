using ClassLib4Net.Sample.Data.ORM.MySql.BLL;
using ClassLib4Net.Sample.Data.ORM.MySql.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TestWebApplication.Controllers
{
    public class MySqlController : Controller
    {
        // GET: MySql
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult get(string epGuid, string epName)
        {
            var entity = BLLRepository.epinfoBLL.LoadByName(epGuid, epName);
            return Json(entity, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult list()
        {
            var list = BLLRepository.epinfoBLL.LoadAll();

            return Json(list, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult insert()
        {
            var entity = new epinfo()
            {
                epGuid = Guid.NewGuid().ToString(),
                epName = DateTime.Now.ToString("yyyyMMddHHmmss"),
                epDescription = ClassLib4Net.RandomCode.createRandomCode(6, false),
                epShortName = "简称"
            };

            var num = BLLRepository.epinfoBLL.Insert(entity);

            return Json(new { num = num, entity = entity }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult edit()
        {
            return View();
        }
        public ActionResult delete()
        {
            return View();
        }


    }
}