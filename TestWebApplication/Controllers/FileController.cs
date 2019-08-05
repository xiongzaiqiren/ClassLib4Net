using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TestWebApplication.Controllers
{
    public class FileController : Controller
    {
        // GET: File
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult GetDirectoryLength()
        {
            string dirPath = @"D:\wwwroot\MRCW.Web";
            var l = ClassLib4Net.FileHelper.GetDirectoryLength(dirPath);

            string str_HardDiskName = "D";
            var l2 = ClassLib4Net.FileHelper.GetHardDiskSpace(str_HardDiskName);
            var l3 = ClassLib4Net.FileHelper.GetHardDiskFreeSpace(str_HardDiskName);
            var PerHardDisk = ClassLib4Net.FileHelper.GetHardDiskFreeSpacePer(str_HardDiskName).ToString("0.##") + "%";

            return Content("路径【" + dirPath + "】占用大小：" + l + "，约" + ClassLib4Net.FileHelper.ConvertBytes(l)
                + "<br />"
                + "驱动器【" + str_HardDiskName + "】总空间：" + ClassLib4Net.FileHelper.ConvertBytes(l2) + "，占用大小：" + ClassLib4Net.FileHelper.ConvertBytes(l3) + "，约" + PerHardDisk
                );
        }


    }
}