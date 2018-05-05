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
            //System.Drawing.Image _CodeImage = ClassLib4Net.QRCodeHelper.Create("https://hbdd.taobao.com/", "Byte", 5, 0, "H", false, "D://360安全浏览器下载//16pic_1112753_b.jpg");
            //System.Drawing.Image _CodeImage = ClassLib4Net.QRCodeHelper.Create("https://hbdd.taobao.com/", "D://360安全浏览器下载//16pic_1112753_b.jpg", 7);
            System.Drawing.Image _CodeImage = ClassLib4Net.QRCodeHelper.Create("https://hbdd.taobao.com/", AppDomain.CurrentDomain.BaseDirectory + "Content//logo.jpg", 7);
            System.IO.MemoryStream _Stream = new System.IO.MemoryStream();
            _CodeImage.Save(_Stream, System.Drawing.Imaging.ImageFormat.Jpeg);
            //_CodeImage.Save(Server.MapPath("/1.jpeg"));  
            //_CodeImage.Save(Server.MapPath("/1.BMP"));  
            //_CodeImage.Save(Server.MapPath("/1.GIF"));  

            context.Response.ContentType = "image/tiff";
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