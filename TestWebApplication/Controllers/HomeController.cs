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