<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="dailyreportcheck.aspx.cs"
    Inherits="Hx.BackAdmin.dayreport.dailyreportcheck" %>

<%@ Import Namespace="Hx.Components.Enumerations" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>日报审核</title>
    <link href=<%=ResourceServer%>/css/admin.css rel="stylesheet" type="text/css" />
    <script src=<%=ResourceServer%>/js/jquery-1.3.2.min.js type="text/javascript"></script>
    <script src=<%=ResourceServer%>/js/comm.js type="text/javascript"></script>
    <script src="/js/WdatePicker.js" type="text/javascript"></script>
    <script type="text/javascript">
        var timer_bindevent;
        $(function () {
            $("#txtDate").click(function () {
                WdatePicker({ 'readOnly': 'true', dateFmt: 'yyyy-MM-dd', maxDate: '<%=DateTime.Today.ToString("yyyy-MM-dd") %>' });
            });
            $(".btnUnPass").click(function () {
                if ($.trim($("#txtCheckRemark").val()) == "") {
                    return confirm("未填写备注，请确认要审核不通过吗？");
                }
            });
        });
    </script>
</head>
<body>
    <div class="ht_main" style="padding-top: 35px;">
        <ul class="xnav fixed">
            <li <%if(CurrentDep==DayReportDep.销售部){ %>class="current" <%} %> <%= GetDepHide(DayReportDep.销售部) %>>
                <a href="?<%= CurrentQuery%>&dep=<%= (int)DayReportDep.销售部%>">销售部</a></li>
            <li <%if(CurrentDep==DayReportDep.售后部){ %>class="current" <%} %> <%= GetDepHide(DayReportDep.售后部) %>>
                <a href="?<%= CurrentQuery%>&dep=<%= (int)DayReportDep.售后部%>">售后部</a></li>
            <li <%if(CurrentDep==DayReportDep.市场部){ %>class="current" <%} %> <%= GetDepHide(DayReportDep.市场部) %>>
                <a href="?<%= CurrentQuery%>&dep=<%= (int)DayReportDep.市场部%>">市场部</a></li>
            <li <%if(CurrentDep==DayReportDep.财务部){ %>class="current" <%} %> <%= GetDepHide(DayReportDep.财务部) %>>
                <a href="?<%= CurrentQuery%>&dep=<%= (int)DayReportDep.财务部%>">财务部</a></li>
            <li <%if(CurrentDep==DayReportDep.行政部){ %>class="current" <%} %> <%= GetDepHide(DayReportDep.行政部) %>>
                <a href="?<%= CurrentQuery%>&dep=<%= (int)DayReportDep.行政部%>">行政部</a></li>
            <li <%if(CurrentDep==DayReportDep.精品部){ %>class="current" <%} %> <%= GetDepHide(DayReportDep.精品部) %>>
                <a href="?<%= CurrentQuery%>&dep=<%= (int)DayReportDep.精品部%>">精品部</a></li>
            <li <%if(CurrentDep==DayReportDep.客服部){ %>class="current" <%} %> <%= GetDepHide(DayReportDep.客服部) %>>
                <a href="?<%= CurrentQuery%>&dep=<%= (int)DayReportDep.客服部%>">客服部</a></li>
            <li <%if(CurrentDep==DayReportDep.二手车部){ %>class="current" <%} %> <%= GetDepHide(DayReportDep.二手车部) %>>
                <a href="?<%= CurrentQuery%>&dep=<%= (int)DayReportDep.二手车部%>">二手车部</a></li>
            <li <%if(CurrentDep==DayReportDep.金融部){ %>class="current" <%} %> <%= GetDepHide(DayReportDep.金融部) %>>
                <a href="?<%= CurrentQuery%>&dep=<%= (int)DayReportDep.金融部%>">金融部</a></li>
            <li <%if(CurrentDep==DayReportDep.DCC部){ %>class="current" <%} %> <%= GetDepHide(DayReportDep.DCC部) %>>
                <a href="?<%= CurrentQuery%>&dep=<%= (int)DayReportDep.DCC部%>">DCC部</a></li>
        </ul>
        <form id="form1" runat="server">
        <table width="100%" border="0" cellspacing="0" cellpadding="0" class="biaoge3">
            <caption class="bt2">
                日报审核</caption>
            <tbody>
                <tr>
                    <td class="bg4 tr">
                        公司：
                    </td>
                    <td>
                        <asp:DropDownList runat="server" ID="ddlCorp" AutoPostBack="true" OnSelectedIndexChanged="ddlCorp_SelectedIndexChanged">
                        </asp:DropDownList>
                    </td>
                </tr>
                <tr>
                    <td class="bg4 tr">
                        日期：
                    </td>
                    <td>
                        <asp:TextBox runat="server" ID="txtDate" CssClass="srk6" OnTextChanged="txtDate_TextChanged"
                            AutoPostBack="true"></asp:TextBox>
                        <span id="spRemind" class="red" runat="server"></span>
                    </td>
                </tr>
                <tr>
                    <td class="bg4 tr">
                        审核状态：
                    </td>
                    <td>
                        <asp:Label runat="server" ID="txtDailyReportCheckStatus"></asp:Label>
                    </td>
                </tr>
                <%= GetDetail()%>
                <tr>
                    <td class="bg4 tr">
                        审核备注：
                    </td>
                    <td>
                        <asp:TextBox runat="server" ID="txtCheckRemark" TextMode="MultiLine" Style="width: 300px;
                            height: 54px;"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td class="bg4 tr">
                    </td>
                    <td>
                        <asp:Button runat="server" ID="btnPass" CssClass="an1" OnClick="btnPass_Click" Text="审核通过" />
                        <asp:Button runat="server" ID="btnUnPass" CssClass="an1" OnClick="btnUnPass_Click"
                            Text="审核不通过" />
                    </td>
                </tr>
                <tr>
                    <td class="bg4 tr">
                    </td>
                    <td>
                        <span id="spMsg" class="red" runat="server"></span>
                    </td>
                </tr>
            </tbody>
        </table>
        </form>
    </div>
</body>
</html>
