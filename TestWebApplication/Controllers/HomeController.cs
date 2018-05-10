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
            var s1 = StringHelper.SubString("黑龙江省党政机关_jim-361", 30);
            var s2 = StringHelper.SubStringNatural("黑龙江省党政机关_jim-361", 30);

            var s3 = StringHelper.SubStringByBytes("黑龙江省党政机关_jim-361", 30, System.Text.Encoding.UTF8, null);

            var s4 = StringHelper.SubStringByBytes("江西高速公路投资_淡如水", 30, System.Text.Encoding.UTF8, null);
            s4 = StringHelper.SubStringByBytes(StringHelper.ConvertToUTF8Encoding("江西高速公路投资_淡如水"), 30, System.Text.Encoding.UTF8, null);

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