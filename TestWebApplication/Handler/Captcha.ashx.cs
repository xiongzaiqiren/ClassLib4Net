using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TestWebApplication.Handler
{
    /// <summary>
    /// Captcha 的摘要说明
    /// </summary>
    public class Captcha : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            string data = context.Request.Params["OrderNo"] ?? ClassLib4Net.RandomCode.createRandomCode(4);
            string filename = context.Request["filename"] ?? ("二维码" + DateTime.Now.ToString("yyyyMMdd-HHmmss"));
            
            System.Drawing.Image _CodeImage = ClassLib4Net.VerificationCode.Captcha.Generate(data, 0, 0);
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