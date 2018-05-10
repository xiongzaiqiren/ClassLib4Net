using System;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace ClassLib4Net.MVCControl
{
    /// <summary>
    /// 处理Jsonp返回结果请求
    /// </summary>
    public class JsonpResult : JsonResult
    {
        public static readonly string JsonpCallbackName = "callback";
        public static readonly string CallbackApplicationType = "application/json";

        public override void ExecuteResult(ControllerContext context)
        {
            if(context == null)
            {
                throw new ArgumentNullException("context");
            }
            if((JsonRequestBehavior == JsonRequestBehavior.DenyGet) && String.Equals(context.HttpContext.Request.HttpMethod, "GET", StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException();
            }
            var response = context.HttpContext.Response;
            if(!String.IsNullOrEmpty(ContentType))
                response.ContentType = ContentType;
            else
                response.ContentType = CallbackApplicationType;
            if(ContentEncoding != null)
                response.ContentEncoding = this.ContentEncoding;
            if(Data != null)
            {
                String buffer;
                var request = context.HttpContext.Request;
                var serializer = new JavaScriptSerializer();
                if(request[JsonpCallbackName] != null)
                    buffer = String.Format("{0}({1})", request[JsonpCallbackName], serializer.Serialize(Data));//首先根据callback获取获取函数名，然后传入json字符串作为函数参数
                else
                    buffer = serializer.Serialize(Data);
                response.Write(buffer);
            }
        }
    }

    /// <summary>
    /// MVC Controller扩展
    /// </summary>
    public static partial class ControllerExtension
    {
        /// <summary>
        /// 支持Jsonp
        /// </summary>
        /// <param name="controller"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public static JsonpResult Jsonp(this Controller controller, object data)
        {
            JsonpResult result = new JsonpResult()
            {
                Data = data,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
            return result;
        }
    }

}
