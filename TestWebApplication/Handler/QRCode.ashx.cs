using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TestWebApplication.Handler
{
    /// <summary>
    /// QRCode 的摘要说明
    /// </summary>
    public class QRCode : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            string data = context.Request["data"] ?? "https://hbdd.taobao.com/";
            string filename = context.Request["filename"] ?? ("二维码" + DateTime.Now.ToString("yyyyMMdd-HHmmss"));
            int scale = int.Parse(context.Request["scale"] ?? "7");

            /*
             * //   scale:
             * //     二维码规模(值越大生成的二维码图片像素越高，示例：值4图片尺寸133*133px，值7图片尺寸232*232px，值10图片尺寸331*331px，值20图片尺寸661*661px，值40图片尺寸1321*1321px……)
             */

            //System.Drawing.Image _CodeImage = ClassLib4Net.QRCodeHelper.Create("https://hbdd.taobao.com/", "Byte", 5, 0, "H", false, "D://360安全浏览器下载//16pic_1112753_b.jpg");
            //System.Drawing.Image _CodeImage = ClassLib4Net.QRCodeHelper.Create("https://hbdd.taobao.com/", "D://360安全浏览器下载//16pic_1112753_b.jpg", 7);
            System.Drawing.Image _CodeImage = ClassLib4Net.QRCodeHelper.Create(data, AppDomain.CurrentDomain.BaseDirectory + "Content//logo.jpg", scale);
            System.IO.MemoryStream _Stream = new System.IO.MemoryStream();
            _CodeImage.Save(_Stream, System.Drawing.Imaging.ImageFormat.Jpeg);
            //_CodeImage.Save(Server.MapPath("/1.jpeg"));  
            //_CodeImage.Save(Server.MapPath("/1.BMP"));  
            //_CodeImage.Save(Server.MapPath("/1.GIF"));  

            context.Response.ContentType = "image/tiff";
            context.Response.AddHeader("Content-disposition", "attachment; filename=" + filename + ".jpg");
            context.Response.Clear();
            context.Response.BufferOutput = true;
            context.Response.BinaryWrite(_Stream.GetBuffer());
            context.Response.Flush();
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}