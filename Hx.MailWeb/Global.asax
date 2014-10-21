<%@ Application Language="C#" %>

<script runat="server">

    void Application_Start(object sender, EventArgs e) 
    {
        //在应用程序启动时运行的代码

    }
    
    void Application_End(object sender, EventArgs e) 
    {
        //在应用程序关闭时运行的代码

    }

    protected void Application_AuthorizeRequest(object sender, EventArgs e)
    {
        HttpApplication app = (HttpApplication)sender;
        Rewrite(app.Request.Path, app);
    }
        
    void Application_Error(object sender, EventArgs e) 
    { 
        //在出现未处理的错误时运行的代码

    }

    void Session_Start(object sender, EventArgs e) 
    {
        //在新会话启动时运行的代码

    }

    void Session_End(object sender, EventArgs e) 
    {
        //在会话结束时运行的代码。 
        // 注意: 只有在 Web.config 文件中的 sessionstate 模式设置为
        // InProc 时，才会引发 Session_End 事件。如果会话模式 
        //设置为 StateServer 或 SQLServer，则不会引发该事件。

    }

    private static void Rewrite(string requestedPath, System.Web.HttpApplication app)
    {
        string sendtourl = app.Context.Request.Url.ToString().Replace("http://mail.hongxu.cn/","http://mail.hongxu.cn:3000/");
        RewriteUrl(app.Context, sendtourl);
        return;
    }

    #region RewriteUrl
    /// <summary>
    /// Rewrite's a URL using <b>HttpContext.RewriteUrl()</b>.
    /// </summary>
    /// <param name="context">The HttpContext object to rewrite the URL to.</param>
    /// <param name="sendToUrl">The URL to rewrite to.</param>
    internal static void RewriteUrl(HttpContext context, string sendToUrl)
    {
        string x, y;
        RewriteUrl(context, sendToUrl, out x, out y);
    }

    /// <summary>
    /// Rewrite's a URL using <b>HttpContext.RewriteUrl()</b>.
    /// </summary>
    /// <param name="context">The HttpContext object to rewrite the URL to.</param>
    /// <param name="sendToUrl">The URL to rewrite to.</param>
    /// <param name="sendToUrlLessQString">Returns the value of sendToUrl stripped of the querystring.</param>
    /// <param name="filePath">Returns the physical file path to the requested page.</param>
    internal static void RewriteUrl(HttpContext context, string sendToUrl, out string sendToUrlLessQString, out string filePath)
    {
        // see if we need to add any extra querystring information
        if (context.Request.QueryString.Count > 0)
        {
            if (sendToUrl.IndexOf('?') != -1)
                sendToUrl += "&" + context.Request.QueryString.ToString();
            else
                sendToUrl += "?" + context.Request.QueryString.ToString();
        }

        // first strip the querystring, if any
        string queryString = String.Empty;
        sendToUrlLessQString = sendToUrl;
        if (sendToUrl.IndexOf('?') > 0)
        {
            sendToUrlLessQString = sendToUrl.Substring(0, sendToUrl.IndexOf('?'));
            queryString = sendToUrl.Substring(sendToUrl.IndexOf('?') + 1);
        }

        // grab the file's physical path
        filePath = string.Empty;
        //filePath = context.Server.MapPath(sendToUrlLessQString);
                    
        // rewrite the path...
        context.Response.Redirect(sendToUrlLessQString + queryString);


        // NOTE!  The above RewritePath() overload is only supported in the .NET Framework 1.1
        // If you are using .NET Framework 1.0, use the below form instead:
        // context.RewritePath(sendToUrl);
    }
        #endregion
       
</script>
