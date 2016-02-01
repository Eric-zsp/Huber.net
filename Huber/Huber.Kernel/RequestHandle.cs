using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Huber.Kernel.Entity;

namespace Huber.Kernel
{
    /// <summary>响应工具类
    /// </summary>
    public class RequestHandle
    {
        private static bool IsAjax(HttpRequest request)
        {
            return request.Headers["X-Requested-With"] != null;
        }

        /// <summary>将reques请求的参数封装到CorRefEntity对象中
        /// </summary>
        /// <param name="para"></param>
        /// <param name="request"></param>
        public static void FillCorRefEntity(RefRequestEntity para, HttpRequest request)
        {
            foreach (var key in request.Params.AllKeys)
            {
                para.Request.Add(key, request.Params[key]);

            }
        }
        /// <summary>URL404
        /// </summary>
        /// <param name="request"></param>
        /// <param name="respond"></param>
        public static void ResponseNotfound(HttpRequest request, HttpResponse respond)
        {
            if (IsAjax(request))
            {
                respond.Write(ResponseCodeEntity.CODE404);
                respond.End();
            }
            else
            {
                respond.Redirect(ResponseCodeEntity.ULR404);
                respond.End();
            }
        }
        /// <summary>NoLogin
        /// </summary>
        /// <param name="request"></param>
        /// <param name="respond"></param>
        public static void ResponseNoLogin(HttpRequest request, HttpResponse respond)
        {
            if (IsAjax(request))
            {
                respond.Write(ResponseCodeEntity.NoLogin);
                respond.End();
            }
            else
            {
                respond.Redirect(ResponseCodeEntity.LoginURL);//需要改成非调转形式
                respond.End();
            }
        }
        /// <summary>NoRight
        /// </summary>
        /// <param name="request"></param>
        /// <param name="respond"></param>
        public static void ResponseNoRight(HttpRequest request, HttpResponse respond)
        {
            if (IsAjax(request))
            {
                respond.Write(ResponseCodeEntity.NoRight);
                respond.End();
            }
            else
            {
                respond.Redirect(ResponseCodeEntity.NoRightURL);//需要改成非调转形式
                respond.End();
            }
        }

        public static void ResposeResult(HttpResponse respond, object result)
        {
            if (typeof(RefRespondEntity) == result.GetType())
            {
                RefRespondEntity temp_result = (RefRespondEntity)result;
                if (temp_result.ResultType == RespondType._Redirect)
                {
                    respond.Redirect((string)temp_result.ResultContext);
                    respond.End();
                }
                else if (temp_result.ResultType == RespondType._Stream)
                {
                    byte[] st = (byte[])temp_result.ResultStream;
                    respond.ContentType = "application/octet-stream";
                    respond.AddHeader("Content-Disposition", string.Format("attachment; filename={0}", (string)temp_result.ResultContext));
                    respond.OutputStream.Write(st, 0, st.Length);
                    respond.End();
                }
                else
                {
                    respond.Write(temp_result.ResultContext);
                    respond.End();
                }
            }
            else
            {
                respond.Write("Huber Module respose is not a RefRespondEntity");
            }
        }
    }
}
