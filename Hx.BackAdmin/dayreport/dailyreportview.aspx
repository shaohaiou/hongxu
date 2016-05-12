<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="dailyreportview.aspx.cs"
    EnableViewState="true" Inherits="Hx.BackAdmin.dayreport.dailyreportview" %>

<%@ Import Namespace="Hx.Components.Enumerations" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>月报查询</title>
    <link href=<%=ResourceServer%>/css/admin.css rel="stylesheet" type="text/css" />
    <script src=<%=ResourceServer%>/js/jquery-1.3.2.min.js type="text/javascript"></script>
    <script src=<%=ResourceServer%>/js/comm.js type="text/javascript"></script>
    <script src="/js/WdatePicker.js" type="text/javascript"></script>
    <script type="text/javascript">
        var timer_bindevent;
        $(function () {
            $("#txtDate").click(function () {
                WdatePicker({ 'readOnly': 'true',<%if(CurrentDep==DayReportDep.销售部){ %> minDate: '2015-01-01',<%} %>maxDate:'<%=DateTime.Now.ToString("yyyy-MM-dd") %>', dateFmt: 'yyyy-MM', isShowToday: false, isShowClear: false });
            });
            $(".btnSubmit").click(function () {
                if ($("#ddlCorp").val() == "-1") {
                    $("#spMsg").text("请选择公司");
                    setTimeout(function () {
                        $("#spMsg").text("");
                    }, 2000);
                    return false;
                }
            });
            $("#btnSubmit").click(function () {
                return CheckForm();
            });
            $("#btnExportExcel").click(function () {
                return CheckForm();
            });

            $(window).scroll(function () {
                if ($(window).scrollLeft() > 20) {
                    $("#flay").css({ left: $(window).scrollLeft() });
                } else {
                    $("#flay").css({ left: "20px" });
                }
            });

            $("#tbData tr").hover(function () {
                var index = $("#tbData tr").index(this);
                var rowcount = $("#tbData tr").length / 2;
                if (index > 0 && index != rowcount) {
                    $("#tbData tr").eq(index).css("background-color", "#FFF68F");
                    $("#tbData tr").eq(index < rowcount ? (index + rowcount) : (index - rowcount)).css("background-color", "#FFF68F");
                }
            }, function () {
                var index = $("#tbData tr").index(this);
                var rowcount = $("#tbData tr").length / 2;
                if (index > 0 && index != rowcount) {
                    $("#tbData tr").eq(index).css("background-color", "");
                    $("#tbData tr").eq(index < rowcount ? (index + rowcount) : (index - rowcount)).css("background-color", "");
                }
            });
        });

        function CheckForm() {
            var pass = true;

            if ($("#ddlCorp").val() == "-1") {
                prompts($("#ddlCorp"), "请选择公司");
                pass = false;
            }

            return pass;
        }
    </script>
    <style type="text/css">
        .datatable
        {
            border-top: 1px solid #000;
            border-left: 1px solid #000;
        }
        .datatable tr
        {
            height: 18px;
        }
        .datatable td
        {
            border-right: 1px solid #000;
            border-bottom: 1px solid #000;
        }
        .bggray
        {
            background-color: Orange;
        }
        #flay
        {
            position: absolute;
            width: <%=GetFlayWidth()%>;
            overflow: hidden;
            background-color: white;
        }
    </style>
</head>
<body>
    <div class="ht_main" style="padding-top: 35px;">
        <ul class="xnav fixed">
            <li <%if(CurrentDep==DayReportDep.销售部){ %>class="current" <%} %> <%= GetDepHide(DayReportDep.销售部) %>>
                <a href="?<%= CurrentQuery%>&dep=<%= (int)DayReportDep.销售部%><%= NewQuery %>">销售部</a></li>
            <li <%if(CurrentDep==DayReportDep.售后部){ %>class="current" <%} %> <%= GetDepHide(DayReportDep.售后部) %>>
                <a href="?<%= CurrentQuery%>&dep=<%= (int)DayReportDep.售后部%><%= NewQuery %>">售后部</a></li>
            <li <%if(CurrentDep==DayReportDep.市场部){ %>class="current" <%} %> <%= GetDepHide(DayReportDep.市场部) %>>
                <a href="?<%= CurrentQuery%>&dep=<%= (int)DayReportDep.市场部%><%= NewQuery %>">市场部</a></li>
            <li <%if(CurrentDep==DayReportDep.财务部){ %>class="current" <%} %> <%= GetDepHide(DayReportDep.财务部) %>>
                <a href="?<%= CurrentQuery%>&dep=<%= (int)DayReportDep.财务部%><%= NewQuery %>">财务部</a></li>
            <li <%if(CurrentDep==DayReportDep.行政部){ %>class="current" <%} %> <%= GetDepHide(DayReportDep.行政部) %>>
                <a href="?<%= CurrentQuery%>&dep=<%= (int)DayReportDep.行政部%><%= NewQuery %>">行政部</a></li>
            <li <%if(CurrentDep==DayReportDep.精品部){ %>class="current" <%} %> <%= GetDepHide(DayReportDep.精品部) %>>
                <a href="?<%= CurrentQuery%>&dep=<%= (int)DayReportDep.精品部%><%= NewQuery %>">精品部</a></li>
            <li <%if(CurrentDep==DayReportDep.客服部){ %>class="current" <%} %> <%= GetDepHide(DayReportDep.客服部) %>>
                <a href="?<%= CurrentQuery%>&dep=<%= (int)DayReportDep.客服部%><%= NewQuery %>">客服部</a></li>
            <li <%if(CurrentDep==DayReportDep.二手车部){ %>class="current" <%} %> <%= GetDepHide(DayReportDep.二手车部) %>>
                <a href="?<%= CurrentQuery%>&dep=<%= (int)DayReportDep.二手车部%><%= NewQuery %>">二手车部</a></li>
            <li <%if(CurrentDep==DayReportDep.金融部){ %>class="current" <%} %> <%= GetDepHide(DayReportDep.金融部) %>>
                <a href="?<%= CurrentQuery%>&dep=<%= (int)DayReportDep.金融部%><%= NewQuery %>">金融部</a></li>
            <li <%if(CurrentDep==DayReportDep.DCC部){ %>class="current" <%} %> <%= GetDepHide(DayReportDep.DCC部) %>>
                <a href="?<%= CurrentQuery%>&dep=<%= (int)DayReportDep.DCC部%><%= NewQuery %>">DCC部</a></li>
            <li <%if(CurrentDep==DayReportDep.粘性产品){ %>class="current" <%} %> <%= GetDepHide(DayReportDep.粘性产品) %>>
                <a href="?<%= CurrentQuery%>&dep=<%= (int)DayReportDep.粘性产品%><%= NewQuery %>">粘性产品</a></li>
        </ul>
        <form id="form1" runat="server">
        <table width="100%" border="0" cellspacing="0" cellpadding="0" class="biaoge3">
            <caption class="bt2">
                月报查询</caption>
            <tbody>
                <tr>
                    <td class="bg1">
                        公司：
                    </td>
                    <td class="bg3">
                        <asp:DropDownList runat="server" ID="ddlCorp">
                        </asp:DropDownList>
                    </td>
                    <td class="bg1">
                        月份：
                    </td>
                    <td>
                        <asp:TextBox runat="server" ID="txtDate" CssClass="srk6"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td>
                    </td>
                    <td colspan="3">
                        <asp:Button runat="server" ID="btnSubmit" CssClass="an1" OnClick="btnSubmit_Click"
                            Text="生成月报" />
                        <asp:Button runat="server" ID="btnExportExcel" CssClass="an1" OnClick="btnExportExcel_Click"
                            Text="导出Excel" />
                        <%if(CurrentDep == DayReportDep.销售部 && CurrentUser.YearGather == "1"){ %>
                        <asp:Button runat="server" ID="btnXSYearAnalyze" CssClass="an1" OnClick="btnXSYearAnalyze_Click"
                            Text="年度销售数据分析" />
                        <%}else if (CurrentDep == DayReportDep.售后部 && CurrentUser.YearGather == "1"){ %>
                        <asp:Button runat="server" ID="btnSHYearAnalyze" CssClass="an1" OnClick="btnSHYearAnalyze_Click"
                            Text="年度售后数据分析" />
                        <%}else if(CurrentDep == DayReportDep.精品部 && CurrentUser.YearGather == "1") {%>
                        <asp:Button runat="server" ID="btnJPYearAnalyze" CssClass="an1" OnClick="btnJPYearAnalyze_Click"
                            Text="年度精品数据分析" />
                        <%} %>
                    </td>
                </tr>
                <tr>
                    <td>
                    </td>
                    <td colspan="3">
                        <span id="spMsg" class="red" runat="server"></span>
                    </td>
                </tr>
            </tbody>
        </table>
        </form>
        <table width="1700" border="0" cellspacing="0" cellpadding="0" id="tblView" runat="server"
            enableviewstate="false" style="display: none;">
            <tbody>
                <tr>
                    <td runat="server" id="tdKeyData" style="padding-bottom: 10px;">
                    </td>
                </tr>
                <tr>
                    <td>
                        <span class="gray">注：日期为红色的列表示当天数据是未审核的数据。</span>
                    </td>
                </tr>
                <tr class="bggray pt">
                    <td class="tc" style="border-top: 1px solid #000; border-right: 1px solid #000; border-left: 1px solid #000;
                        width: 900px;">
                        <span runat="server" id="spTitle" class="bold" style="font-size: 20px;"></span>
                    </td>
                </tr>
                <tr>
                    <td runat="server" id="tdData">
                    </td>
                </tr>
            </tbody>
        </table>
    </div>
    <div id="flay">
    </div>
    <script type="text/javascript">
        if ($("#tbData").length > 0) {
            $("#flay").html($("#tdData").html());
            $("#flay table").css("width", "1700px");
            $("#flay").css({ left: $("#tbData").offset().left, top: $("#tbData").offset().top });
        }
    </script>
</body>
</html>
