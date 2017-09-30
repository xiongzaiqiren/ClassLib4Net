using System;
using System.Data;
using System.Collections;

using System.Web;

namespace ClassLib4Net
{
    /// <summary>
    /// 数据导出为Excel或XML文件
    /// </summary>
    public class ExportDataHelper
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public ExportDataHelper()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        /// <summary>
        /// 把DataSet导出到文件
        /// </summary>
        /// <param name="_caption">表头标题文字，以 | 分隔</param>
        /// <param name="ds">源数据DataSet</param>
        /// <param name="ef">导出格式</param>
        /// <param name="FileName">导出的文件名</param>
        public static void ExportDataSetToFile(string _caption, DataSet ds, ExportFormat ef, string FileName)
        {
            string[] s = _caption.Trim().Trim('|').Split('|');
            DataTable dt = ds.Tables[0];
            for (int i = 0; i < s.Length; i++)
            {
                if (i < dt.Columns.Count)
                    dt.Columns[i].Caption = s[i];
                else
                    break;
            }
            ExportDataSetToFile(ds, ef, FileName);
        }
        /// <summary>
        /// 把DataSet导出到文件
        /// </summary>
        /// <param name="_caption">表头标题文字，以 | 分隔</param>
        /// <param name="ds">源数据DataSet</param>
        /// <param name="cols">输出列</param>        
        /// <param name="ef">导出格式</param>
        /// <param name="FileName">导出的文件名</param>
        public static void ExportDataSetToFile(string _caption, DataSet ds, string[] cols, ExportFormat ef, string FileName)
        {
            string[] s = _caption.Trim().Trim('|').Split('|');
            DataTable dt = ds.Tables[0];
            for (int i = 0; i < s.Length; i++)
            {
                if (i < dt.Columns.Count)
                    dt.Columns[i].Caption = s[i];
                else
                    break;
            }
            ExportDataSetToFile(ds, cols, ef, FileName);
        }
        /// <summary>
        /// 把DataSet导出到文件
        /// </summary>
        /// <param name="_caption">表头标题文字</param>
        /// <param name="ds">源数据DataSet</param>
        /// <param name="ef">导出格式</param>
        /// <param name="FileName">导出的文件名</param>
        public static void ExportDataSetToFile(string[] _caption, DataSet ds, ExportFormat ef, string FileName)
        {
            DataTable dt = ds.Tables[0];
            for (int i = 0; i < _caption.Length; i++)
            {
                if (i < dt.Columns.Count)
                    dt.Columns[i].Caption = _caption[i];
                else
                    break;
            }
            ExportDataSetToFile(ds, ef, FileName);
        }
        /// <summary>
        /// 把DataSet导出到文件
        /// </summary>
        /// <param name="_caption">表头标题文字，以 | 分隔</param>        
        /// <param name="ds">源数据DataSet</param>
        /// <param name="cols">输出列</param>
        /// <param name="ef">导出格式</param>
        /// <param name="FileName">导出的文件名</param>
        public static void ExportDataSetToFile(string[] _caption, DataSet ds, string[] cols, ExportFormat ef, string FileName)
        {
            //DataTable dt = ds.Tables[0];
            //for (int i = 0; i < _caption.Length; i++)
            //{
            //    if (i < dt.Columns.Count)
            //        dt.Columns[i].Caption = _caption[i];
            //    else
            //        break;
            //}
            //ExportDataSetToFile(ds, cols, ef, FileName);
            HttpResponse resp;
            resp = HttpContext.Current.Response;
            resp.ContentEncoding = System.Text.Encoding.GetEncoding("GB2312");
            resp.AppendHeader("Content-Disposition", "attachment;filename=" + FileName);
            resp.Charset = "UTF-8";

            string colHeaders = "", ls_item = "";
            int i = 0;

            //定义表对象与行对像，同时用DataSet对其值进行初始化
            DataTable dt = ds.Tables[0];
            DataRow[] myRow = dt.Select("");
            // typeid=="1"时导出为EXCEL格式文件；typeid=="2"时导出为XML格式文件
            if (ef == ExportFormat.ExcelFormat)
            {
                resp.ContentType = "application/ms-excel";//image/JPEG;text/HTML;image/GIF;vnd.ms-excel/msword
                //取得数据表各列标题，各标题之间以\t分割，最后一个列标题后加回车符
                //for (i = 0; i < dt.Columns.Count - 1; i++)
                //colHeaders += dt.Columns[i].Caption.ToString() + "\t";
                for (i = 0; i < _caption.Length - 1; i++)
                    colHeaders += _caption[i] + "\t";

                colHeaders += _caption[i] + "\n";
                //向HTTP输出流中写入取得的数据信息
                resp.Write(colHeaders);
                //逐行处理数据  
                foreach (DataRow row in myRow)
                {
                    //在当前行中，逐列获得数据，数据之间以\t分割，结束时加回车符\n 
                    for (i = 0; i < cols.Length - 1; i++)
                    {
                        if (dt.Columns.Contains(cols[i]))
                            ls_item += row[cols[i]].ToString().Replace("\t", "").Replace("\n", "").Replace("\r", "") + "\t";
                    }

                    ls_item += row[cols[i]].ToString().Replace("\t", "").Replace("\n", "").Replace("\r", "") + "\n";

                    //当前行数据写入HTTP输出流，并且置空ls_item以便下行数据    
                    resp.Write(ls_item);
                    ls_item = "";
                }
            }
            else
            {
                if (ef == ExportFormat.XMLFormat)
                {
                    //从DataSet中直接导出XML数据并且写到HTTP输出流中
                    resp.Write(ds.GetXml());
                }
            }
            //写缓冲区中的数据到HTTP头文件中
            resp.End();
        }
        /// <summary>
        /// 把DataSet导出到文件
        /// </summary>
        /// <param name="ds">源数据DataSet</param>
        /// <param name="ef">导出格式</param>
        /// <param name="FileName">导出的文件名</param>
        public static void ExportDataSetToFile(DataSet ds, ExportFormat ef, string FileName)
        {
            HttpResponse resp;
            resp = HttpContext.Current.Response;
            resp.ContentEncoding = System.Text.Encoding.GetEncoding("GB2312");
            resp.AppendHeader("Content-Disposition", "attachment;filename=" + FileName);
            resp.Charset = "UTF-8";

            string colHeaders = "", ls_item = "";
            int i = 0;

            //定义表对象与行对像，同时用DataSet对其值进行初始化
            DataTable dt = ds.Tables[0];
            DataRow[] myRow = dt.Select("");
            // typeid=="1"时导出为EXCEL格式文件；typeid=="2"时导出为XML格式文件
            if (ef == ExportFormat.ExcelFormat)
            {
                resp.ContentType = "application/ms-excel";//image/JPEG;text/HTML;image/GIF;vnd.ms-excel/msword
                //取得数据表各列标题，各标题之间以\t分割，最后一个列标题后加回车符
                for (i = 0; i < dt.Columns.Count - 1; i++)
                    colHeaders += dt.Columns[i].Caption.ToString() + "\t";
                colHeaders += dt.Columns[i].Caption.ToString() + "\n";
                //向HTTP输出流中写入取得的数据信息
                resp.Write(colHeaders);
                //逐行处理数据  
                foreach (DataRow row in myRow)
                {
                    //在当前行中，逐列获得数据，数据之间以\t分割，结束时加回车符\n
                    for (i = 0; i < dt.Columns.Count - 1; i++)
                        ls_item += row[i].ToString().Replace("\t", "").Replace("\n", "").Replace("\r", "") + "\t";
                    ls_item += row[i].ToString().Replace("\t", "").Replace("\n", "").Replace("\r", "") + "\n";
                    //当前行数据写入HTTP输出流，并且置空ls_item以便下行数据    
                    resp.Write(ls_item);
                    ls_item = "";
                }
            }
            else
            {
                if (ef == ExportFormat.XMLFormat)
                {
                    //从DataSet中直接导出XML数据并且写到HTTP输出流中
                    resp.Write(ds.GetXml());
                }
            }
            //写缓冲区中的数据到HTTP头文件中
            resp.End();
        }
        /// <summary>
        /// 把DataSet导出到文件
        /// </summary>
        /// <param name="ds">源数据DataSet</param>
        /// <param name="cols">导出的列</param>
        /// <param name="ef">导出格式</param>
        /// <param name="FileName">导出的文件名</param>
        public static void ExportDataSetToFile(DataSet ds, string[] cols, ExportFormat ef, string FileName)
        {
            HttpResponse resp;
            resp = HttpContext.Current.Response;
            resp.ContentEncoding = System.Text.Encoding.GetEncoding("GB2312");
            resp.AppendHeader("Content-Disposition", "attachment;filename=" + FileName);
            resp.Charset = "UTF-8";

            string colHeaders = "", ls_item = "";
            int i = 0;

            //定义表对象与行对像，同时用DataSet对其值进行初始化
            DataTable dt = ds.Tables[0];
            DataRow[] myRow = dt.Select("");
            // typeid=="1"时导出为EXCEL格式文件；typeid=="2"时导出为XML格式文件
            if (ef == ExportFormat.ExcelFormat)
            {
                resp.ContentType = "application/ms-excel";//image/JPEG;text/HTML;image/GIF;vnd.ms-excel/msword
                //取得数据表各列标题，各标题之间以\t分割，最后一个列标题后加回车符
                //for (i = 0; i < dt.Columns.Count - 1; i++)
                //colHeaders += dt.Columns[i].Caption.ToString() + "\t";
                for (i = 0; i < cols.Length - 1; i++)
                    colHeaders += dt.Columns[cols[i]].Caption.ToString() + "\t";

                colHeaders += dt.Columns[cols[i]].Caption.ToString() + "\n";
                //向HTTP输出流中写入取得的数据信息
                resp.Write(colHeaders);
                //逐行处理数据  
                foreach (DataRow row in myRow)
                {
                    //在当前行中，逐列获得数据，数据之间以\t分割，结束时加回车符\n 
                    for (i = 0; i < cols.Length - 1; i++)
                    {
                        if (dt.Columns.Contains(cols[i]))
                            ls_item += row[cols[i]].ToString().Replace("\t", "").Replace("\n", "").Replace("\r", "") + "\t";
                    }

                    ls_item += row[cols[i]].ToString().Replace("\t", "").Replace("\n", "").Replace("\r", "") + "\n";

                    //当前行数据写入HTTP输出流，并且置空ls_item以便下行数据    
                    resp.Write(ls_item);
                    ls_item = "";
                }
            }
            else
            {
                if (ef == ExportFormat.XMLFormat)
                {
                    //从DataSet中直接导出XML数据并且写到HTTP输出流中
                    resp.Write(ds.GetXml());
                }
            }
            //写缓冲区中的数据到HTTP头文件中
            resp.End();
        }
        /// <summary>
        /// 把DataSet导出到Excel文件,并带表头
        /// </summary>
        /// <param name="title">表头</param>
        /// <param name="_caption">标题</param>
        /// <param name="ds">源数据DataSet</param>
        /// <param name="cols">导出的列</param>
        /// <param name="FileName">导出的文件名</param>
        public static void ExportDataSetToExcelWithTitle(string title, string[] _caption, DataSet ds, string[] cols, string FileName)
        {
            HttpResponse resp;
            resp = HttpContext.Current.Response;
            resp.ContentEncoding = System.Text.Encoding.GetEncoding("GB2312");
            resp.AppendHeader("Content-Disposition", "attachment;filename=" + FileName);
            resp.Charset = "UTF-8";

            string colHeaders = "", ls_item = "";
            int i = 0;

            //定义表对象与行对像，同时用DataSet对其值进行初始化
            DataTable dt = ds.Tables[0];
            DataRow[] myRow = dt.Select("");
            // typeid=="1"时导出为EXCEL格式文件；typeid=="2"时导出为XML格式文件 

            resp.ContentType = "application/ms-excel";//image/JPEG;text/HTML;image/GIF;vnd.ms-excel/msword

            //输出表头
            resp.Write(title + "\n");

            //取得数据表各列标题，各标题之间以\t分割，最后一个列标题后加回车符 
            for (i = 0; i < _caption.Length - 1; i++)
                colHeaders += _caption[i] + "\t";

            colHeaders += _caption[i] + "\n";
            //向HTTP输出流中写入取得的数据信息
            resp.Write(colHeaders);
            //逐行处理数据  
            foreach (DataRow row in myRow)
            {
                //在当前行中，逐列获得数据，数据之间以\t分割，结束时加回车符\n 
                for (i = 0; i < cols.Length - 1; i++)
                {
                    if (dt.Columns.Contains(cols[i]))
                        ls_item += row[cols[i]].ToString().Replace("\t", "").Replace("\n", "").Replace("\r", "") + "\t";
                }

                ls_item += row[cols[i]].ToString().Replace("\t", "").Replace("\n", "").Replace("\r", "") + "\n";

                //当前行数据写入HTTP输出流，并且置空ls_item以便下行数据    
                resp.Write(ls_item);
                ls_item = "";
            }


            //写缓冲区中的数据到HTTP头文件中
            resp.End();
        }
    }

    /// <summary>
    /// 导出文件的类型
    /// </summary>
    public enum ExportFormat
    {
        /// <summary>
        /// 
        /// </summary>
        ExcelFormat = 0,			//Excel格式
        /// <summary>
        /// 
        /// </summary>
        XMLFormat					//XML格式
    }
}