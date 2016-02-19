<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="showmessage.aspx.cs" Inherits="Hx.BackAdmin.message.showmessage" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="head1" runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>系统提示</title>
    <link href=/css/admin.css rel="stylesheet" type="text/css" />
    <script src=/js/jquery-1.3.2.min.js type="text/javascript"></script>
    <script type="text/javascript">
        var id = 'num';
        var n = 3;
        var resurl = "<%=ResolveClientUrl(ReUrl) %>";
        function showNum() {
            var idt = setTimeout(showNum, 1000);
            if (n < 0) {
                clearTimeout(idt);
                window.location.href = resurl;
                return;
            }
            var objTemp = document.getElementById(id);
            objTemp.innerHTML = n;
            n = n - 1;
        }

        $(function () {
            showNum();
        });
    </script>
    <style type="text/css">
        .xtts
        {
            border-top: 5px solid #deeffa;
            border-bottom: 5px solid #deeffa;
            border-left: 0;
            border-right: 0;
            padding: 15px;
            margin: 5px 0;
            line-height: 30px;
            text-align: center;
        }
        .xtts span.lv_lj a
        {
            text-decoration: underline;
        }
        .dalv
        {
            font-size: 16px;
            font-weight: 700;
            color: #2e8c00;
            line-height: 50px;
        }
    </style>
</head>
<body>
    <div style="margin: 0 20px;">
        <div class="bt0">
            <asp:Literal ID="lTitle" runat="server"></asp:Literal></div>
        <div class="xtts">
            <span>
                <asp:Literal ID="lContent" runat="server"></asp:Literal></span><br />
            <span>页面将在<em id='num'>3</em>秒后跳转...</span> <span>如果页面没有自动跳转，请点击<asp:HyperLink ID="hyreturn"
                runat="server">这里</asp:HyperLink>
            </span>
        </div>
    </div>
</body>
</html>
