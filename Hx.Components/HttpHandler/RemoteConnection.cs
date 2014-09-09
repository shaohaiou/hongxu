using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using Hx.Tools.Web;

namespace Hx.Components.HttpHandler
{
    public abstract class RemoteConnection : IHttpHandler
    {
        public virtual bool IsReusable
        {
            get { return true; }
        }

        public void ProcessRequest(HttpContext context)
        {
            if (!string.IsNullOrEmpty(WebHelper.GetString("check")))
            {
                ///验证   暂时没什么用
                Check(context);
            }
            else
            {
                Process(context);
            }
        }

        public virtual void Check(HttpContext context)
        {
            context.Response.Write("success");
        }

        public virtual void Process(HttpContext context)
        {
        }



    }
}
