using ClassLib4Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TestWebApplication.Controllers
{
    public class HomeController : Controller
    {
        // GET: Home
        public ActionResult Index()
        {
            var s1 = ClassLib4Net.StringHelper.SubString("黑龙江省党政机关_jim-361", 30);
            var s2 = ClassLib4Net.StringHelper.SubStringNatural("黑龙江省党政机关_jim-361", 30);

            var s3 = ClassLib4Net.StringHelper.SubStringByBytes("黑龙江省党政机关_jim-361", 30, System.Text.Encoding.UTF8, null);

            return View();
        }

        public ActionResult Cookie()
        {
            if(CookieHelper.CookieIsEnable())
            {
                string name = "myCookie";
                if(!string.IsNullOrWhiteSpace(CookieHelper.GetCookie(name)))
                {
                    CookieHelper.Remove(name);
                }
                else
                {
                    CookieHelper.SetCookie("Xiong", name);
                }
            }
            else
            {

            }

            return View();
        }
    }
}