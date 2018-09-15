using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;

using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TestWebApplication.Controllers
{

/*
关于NPOI
NPOI是POI项目的.NET版本，是由@Tony Qu(http://tonyqus.cnblogs.com/)等大侠基于POI开发的，可以从http://npoi.codeplex.com/下载到它的最新版本。它不使用Office COM组件(Microsoft.Office.Interop.XXX.dll)，不需要安装Microsoft Office，支持对Office 97-2003的文件格式，功能比较强大。更详细的说明请看作者的博客或官方网站。

它的以下一些特性让我相当喜欢：
    1，支持对标准的Excel读写；
    2，支持对流(Stream)的读写 (而Jet OLEDB和Office COM都只能针对文件)；
    3，支持大部分Office COM组件的常用功能；
    4，性能优异 (相对于前面的方法)；
    5，使用简单，易上手；

使用NPOI
本文使用的是它当前的最新版本1.2.4，此版本的程序集缩减至2个：NPOI.dll、Ionic.Zip.dll，直接引用到项目中即可。

对于我们开发者使用的对象主要位于NPOI.HSSF.UserModel空间下，主要有HSSFWorkbook、HSSFSheet、HSSFRow、HSSFCell，对应的接口为位于NPOI.SS.UserModel空间下的IWorkbook、ISheet、IRow、ICell，分别对应Excel文件、工作表、行、列。
*/
    public class NPOIController : Controller
    {
        // GET: NPOI
        public ActionResult Index()
        {
            return View();
        }
        #region 导出/导入Excel
        private DataTable PatientNursingMonitor_GetTable(string name)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add(new DataColumn("MonitorDate", typeof(DateTime)));
            dt.Columns.Add(new DataColumn("Score", typeof(string)));

            for(var i = 0; i < 30; i++)
            {
                DataRow row = dt.NewRow();
                row["MonitorDate"] = new DateTime(2018, 8, ClassLib4Net.RandomCode.random.Next(1, 31));
                row["Score"] = ClassLib4Net.RandomCode.random.Next(0, 100).ToString();
                dt.Rows.Add(row);
            }

            return dt;
        }
        /// <summary>
        /// 导出Excel
        /// </summary>
        /// <returns></returns>
        public ActionResult exportExcel()
        {
            DataTable dt = PatientNursingMonitor_GetTable("");
            if(dt != null && dt.Rows.Count > 0)
            {
                DataTable dtExcel = new DataTable();
                dtExcel.Columns.Add(new DataColumn("监控时间", typeof(DateTime)));
                dtExcel.Columns.Add(new DataColumn("评分", typeof(string)));
                foreach(DataRow dr in dt.Rows)
                {
                    DataRow row = dtExcel.NewRow();
                    row["监控时间"] = dr["MonitorDate"].ToString();
                    row["评分"] = dr["Score"].ToString();
                    dtExcel.Rows.Add(row);
                }

                HttpContextBase context = this.HttpContext;
                string excelName = DateTime.Now.ToString("yyyyMMddHHmmssffffff") + "-xxx.xls";

                //Models.NPOIExcelHelper.ExportDTtoExcel(dtExcel, null, "c:\\" + excelName);
                Models.NPOIExcelHelper.RenderToExcel(dtExcel, context, excelName, "sheet1");
            }
            else
            {
                throw new Exception("暂无记录");
            }

            return null;
        }

        /// <summary>
        /// 导出Excel
        /// </summary>
        /// <returns></returns>
        public FileContentResult exportFile()
        {
            DataTable dt = PatientNursingMonitor_GetTable("");

            byte[] bytes = Models.NPOIExcelHelper.RenderBytesToExcel(dt, "sheet1");

            string excelName = DateTime.Now.ToString("yyyyMMddHHmmssffffff") + "-xxx.xls";
            return File(bytes, "application/octet-stream", excelName);
        }

        class Drug : List<DrugInfo> { }
        class DrugInfo
        {
            public int ID { get; set; }
            public string DrugName { get; set; }
            public string Company { get; set; }
            public string DrugType { get; set; }
            public string Unit { get; set; }
        }
        /// <summary>
        /// 导入Excel
        /// </summary>
        /// <returns></returns>
        public ActionResult importExcel()
        {
            string path = AppDomain.CurrentDomain.BaseDirectory + @"files\YPM.xls"; //Server.MapPath("/files/YPM.xls");
            DataTable dt = Models.NPOIExcelHelper.ImportExceltoDt(path, 0, -1);
            Drug dal = new Drug();
            for(int i = 0; i < dt.Rows.Count; i++)
            {
                DrugInfo info = new DrugInfo();
                info.ID = int.Parse(dt.Rows[i][0].ToString());
                info.DrugName = dt.Rows[i][1].ToString();//药品名称
                info.Company = dt.Rows[i][2].ToString();//药品名称
                info.DrugType = dt.Rows[i][3].ToString();//药品类别
                info.Unit = dt.Rows[i][4].ToString();//单位
                dal.Insert(i, info);
            }

            return Json(dal, "application/json", JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region 基础
        private void CreateSheet()
        {
            IWorkbook workbook = new HSSFWorkbook();//创建Workbook对象
            ISheet sheet = workbook.CreateSheet("Sheet1");//创建工作表
            IRow row = sheet.CreateRow(0);//在工作表中添加一行
            ICell cell = row.CreateCell(0);//在行中添加一列
            cell.SetCellValue("test");//设置列的内容
        }
        private void GetSheet(Stream stream)
        {
            IWorkbook workbook = new HSSFWorkbook(stream);//从流内容创建Workbook对象
            ISheet sheet = workbook.GetSheetAt(0);//获取第一个工作表
            IRow row = sheet.GetRow(0);//获取工作表第一行
            ICell cell = row.GetCell(0);//获取行的第一列
            string value = cell.ToString();//获取列的值
        }


        #endregion

        #region output
        static void SaveToFile(MemoryStream ms, string fileName)
        {
            using(FileStream fs = new FileStream(fileName, FileMode.Create, FileAccess.Write))
            {
                byte[] data = ms.ToArray();

                fs.Write(data, 0, data.Length);
                fs.Flush();

                data = null;
            }
        }

        static void RenderToBrowser(MemoryStream ms, HttpContext context, string fileName)
        {
            if(context.Request.Browser.Browser == "IE")
                fileName = HttpUtility.UrlEncode(fileName);
            context.Response.AddHeader("Content-Disposition", "attachment;fileName=" + fileName);
            context.Response.BinaryWrite(ms.ToArray());
        }

        /// <summary>
        /// Web.Mvc下输出文件
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public FileContentResult RenderToBrowser(byte[] bytes, string fileName)
        {
            return File(bytes, "application/octet-stream", fileName);
        }
        #endregion


    }
}