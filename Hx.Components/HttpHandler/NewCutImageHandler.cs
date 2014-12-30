using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Web;
using Hx.Tools.Web;
using Hx.Tools;

namespace Hx.Components.HttpHandler
{
    public class NewCutImageHandler : RemoteConnection
    {
        public override bool IsReusable
        {
            get { return false; }
        }

        private string url = string.Empty;

        public override void Process(HttpContext context)
        {
            string result = string.Empty;//需要返回的信息

            //如果通过验证
            string methodName = WebHelper.GetString("action");//获取请求类型
            url = "http://" + context.Request.Url.Host;
            switch (methodName)
            {
                case "upload":
                    result = UpLoadImage();
                    break;
                case "GenerateBitmap":
                    result = GenerateCutImage();
                    break;
                case "GenerateBitmaps":
                    result = GenerateCutImages();
                    break;
                case "ckeditorUpload":
                    result = CkeditorUpload();
                    break;
                case "weixinUpload":
                    result = WeixinUpload();
                    break;
                case "weixinjtUpload":
                    result = WeixinjtUpload();
                    break;
                case "jobUpload":
                    result = JobUpload();
                    break;
                default:
                    result = "{msg:'上传出错！没有参数类型'}";
                    break;
            }
            context.Response.Write(result);
        }

        /// <summary>
        /// 上传文件
        /// </summary>
        /// <returns></returns>
        private string UpLoadImage()
        {
            try
            {
                return UploadWebServices();
            }
            catch (Exception ex)
            {
                return "{msg:'" + ex.StackTrace + "'}";
            }
        }

        /// <summary>
        /// WebServices上传处理方式
        /// </summary>
        /// <returns></returns>
        private string UploadWebServices()
        {
            HttpContext context = HttpContext.Current;
            string strExtension = Path.GetExtension(context.Request.Files[0].FileName).ToLower();
            ///处理上载的文件流信息。
            byte[] b = new byte[context.Request.Files[0].ContentLength];
            using (Stream fs = context.Request.Files[0].InputStream)
            {
                fs.Read(b, 0, context.Request.Files[0].ContentLength);
            }
            object[] o = new object[2];
            o[0] = b;
            o[1] = context.Request.Files[0].FileName;
            string result = DynamicWebServices.InvokeWebService(url + "/webservice/UploadServices.asmx", "UploadImage", o).ToString();
            if (!string.IsNullOrEmpty(result))
            {
                return "{msg:'success',src:'" + result + "'}";
            }
            else
            {
                return "{msg:'error',errorcode:'2'}";
            }
        }

        /// <summary>
        /// 生成裁剪文件
        /// </summary>
        /// <param name="src">图片路径</param>
        /// <returns>返回裁剪是否成功的消息</returns>
        private string GenerateCutImage()
        {
            try
            {
                return GenerateCutImageWebServices();
            }
            catch (Exception ex)
            {
                return "{msg:'" + ex.Message + "'}";
            }
        }

        /// <summary>
        /// 生成裁剪文件（大图）
        /// </summary>
        /// <param name="src">图片路径</param>
        /// <returns>返回裁剪是否成功的消息</returns>
        private string GenerateCutImages()
        {
            try
            {
                return GenerateCutImageWebServices("CutImages");
            }
            catch (Exception ex)
            {
                return "{msg:'" + ex.Message + "'}";
            }
        }

        /// <summary>
        /// 生成裁剪文件
        /// </summary>
        /// <param name="src">图片路径</param>
        /// <returns>返回裁剪是否成功的消息</returns>
        private string GenerateCutImageWebServices(string fun = "CutImage")
        {
            HttpContext context = HttpContext.Current;
            string src = context.Request["src"];
            object[] o = new object[6];
            o[0] = src;
            o[1] = context.Request["zoom"];
            o[2] = context.Request["x"];
            o[3] = context.Request["y"];
            o[4] = context.Request["width"];
            o[5] = context.Request["height"];
            string meg = DynamicWebServices.InvokeWebService(url + "/webservice/UploadServices.asmx", fun, o).ToString();
            return meg;
        }

        /// <summary>
        /// ckeditor上传文件
        /// </summary>
        /// <returns></returns>
        private string CkeditorUpload()
        {
            HttpContext context = HttpContext.Current;
            string strExtension = Path.GetExtension(context.Request.Files[0].FileName).ToLower();
            ///处理上载的文件流信息。
            byte[] b = new byte[context.Request.Files[0].ContentLength];
            using (Stream fs = context.Request.Files[0].InputStream)
            {
                fs.Read(b, 0, context.Request.Files[0].ContentLength);
            }
            object[] o = new object[2];
            o[0] = b;
            o[1] = context.Request.Files[0].FileName;
            string result = DynamicWebServices.InvokeWebService(url + "/webservice/UploadServices.asmx", "CkeditorUpload", o).ToString();
            if (!string.IsNullOrEmpty(result))
            {
                string callback = context.Request["CKEditorFuncNum"];
                return "<script type=\"text/javascript\">window.parent.CKEDITOR.tools.callFunction(" + callback + ",'" + result + "','')</script>";
            }
            else
            {
                return "<font color=\"red\"size=\"2\">*文件格式不正确（必须为.jpg/.gif/.bmp/.png文件）</font>";
            }
        }

        /// <summary>
        /// 微信项目上传
        /// </summary>
        /// <returns></returns>
        private string WeixinUpload()
        {
            HttpContext context = HttpContext.Current;
            string strExtension = Path.GetExtension(context.Request.Files[0].FileName).ToLower();
            ///处理上载的文件流信息。
            byte[] b = new byte[context.Request.Files[0].ContentLength];
            using (Stream fs = context.Request.Files[0].InputStream)
            {
                fs.Read(b, 0, context.Request.Files[0].ContentLength);
            }
            object[] o = new object[2];
            o[0] = b;
            o[1] = context.Request.Files[0].FileName;
            string result = DynamicWebServices.InvokeWebService(url + "/webservice/UploadServices.asmx", "WeixinUploadImage", o).ToString();
            if (!string.IsNullOrEmpty(result))
            {
                return "{msg:'success',src:'" + result + "'}";
            }
            else
            {
                return "{msg:'error',errorcode:'2'}";
            }
        }

        /// <summary>
        /// 集团投票活动上传
        /// </summary>
        /// <returns></returns>
        private string WeixinjtUpload()
        {
            HttpContext context = HttpContext.Current;
            string strExtension = Path.GetExtension(context.Request.Files[0].FileName).ToLower();
            ///处理上载的文件流信息。
            byte[] b = new byte[context.Request.Files[0].ContentLength];
            using (Stream fs = context.Request.Files[0].InputStream)
            {
                fs.Read(b, 0, context.Request.Files[0].ContentLength);
            }
            object[] o = new object[2];
            o[0] = b;
            o[1] = context.Request.Files[0].FileName;
            string result = DynamicWebServices.InvokeWebService(url + "/webservice/UploadServices.asmx", "WeixinjtUploadImage", o).ToString();
            if (!string.IsNullOrEmpty(result))
            {
                return "{msg:'success',src:'" + result + "'}";
            }
            else
            {
                return "{msg:'error',errorcode:'2'}";
            }
        }

        /// <summary>
        /// 招聘活动上传
        /// </summary>
        /// <returns></returns>
        private string JobUpload()
        {
            HttpContext context = HttpContext.Current;
            string strExtension = Path.GetExtension(context.Request.Files[0].FileName).ToLower();
            ///处理上载的文件流信息。
            byte[] b = new byte[context.Request.Files[0].ContentLength];
            using (Stream fs = context.Request.Files[0].InputStream)
            {
                fs.Read(b, 0, context.Request.Files[0].ContentLength);
            }
            object[] o = new object[2];
            o[0] = b;
            o[1] = context.Request.Files[0].FileName;
            string result = DynamicWebServices.InvokeWebService(url + "/webservice/UploadServices.asmx", "JobUploadImage", o).ToString();
            if (!string.IsNullOrEmpty(result))
            {
                return "{msg:'success',src:'" + result + "'}";
            }
            else
            {
                return "{msg:'error',errorcode:'2'}";
            }
        }
    }
}
