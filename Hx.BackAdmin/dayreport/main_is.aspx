<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="main_is.aspx.cs" Inherits="Hx.BackAdmin.dayreport.main_is" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>月报查询</title>
    <link href=<%=ResourceServer%>/css/admin.css rel="stylesheet" type="text/css" />
    <script src=<%=ResourceServer%>/js/jquery-1.3.2.min.js type="text/javascript"></script>
    <script type="text/javascript">
        function setdaohan(t, url) {
            $("#daohan_sp").html("<a href='" + url + "'>" + t + "</a>");
        }
        $(function () {
            $(".right_nav a").each(function () {
                $(this).click(function () {
                    $(".right_nav a").removeClass("current");
                    $(this).addClass("current");
                });
            });
        });
    </script>
</head>
<body style="background: url(../images/xdd.gif) repeat-x;">
    <div class="right_nav">
        <a href="dailyreport.aspx" target="ztk" id="dailyreport" runat="server">日报录入</a> 
        <a href="monthlytarget.aspx" target="ztk" id="monthlytarget" runat="server">月度目标</a> 
        <a href="monthlytarget.aspx" target="ztk" id="monthlytargetpre" runat="server">预算目标</a> 
        <a href="dailyreportview.aspx" target="ztk" id="dailyreportview" runat="server">月报查询</a>
        <a href="dailyreportviewmul.aspx" target="ztk" id="dailyreportviewmul" runat="server">月报汇总</a>
        <a href="dailyreportcheck.aspx" target="ztk" id="dailyreportcheck" runat="server">日报审核</a>
        <a href="crmreportcustomerflow.aspx" target="ztk" id="crmreportcustomerflow" runat="server">CRM报表</a>
    </div>
</body>
</html>
