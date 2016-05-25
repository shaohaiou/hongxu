<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="dailyreportviewmul.aspx.cs"
    ValidateRequest="false" Inherits="Hx.BackAdmin.dayreport.dailyreportviewmul" %>

<%@ Import Namespace="Hx.Components.Enumerations" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>月报汇总</title>
    <link href=<%=ResourceServer%>/css/admin.css rel="stylesheet" type="text/css" />
    <script src=<%=ResourceServer%>/js/jquery-1.3.2.min.js type="text/javascript"></script>
    <script src=<%=ResourceServer%>/js/comm.js type="text/javascript"></script>
    <script src="/js/WdatePicker.js" type="text/javascript"></script>
    <script type="text/javascript">
        var timer_bindevent;
        $(function () {
            $("#txtDate").click(function () {
                WdatePicker({ 'readOnly': 'true',<%if(CurrentDep==DayReportDep.销售部){ %> minDate: '2015-07-01',<%} %>  dateFmt: 'yyyy-MM-dd', maxDate: '#F{$dp.$D(\'txtDate2\')}' });
            });

            $("#txtDate2").click(function () {
                WdatePicker({ 'readOnly': 'true', <%if(CurrentDep==DayReportDep.销售部){ %> minDate: '2015-07-01',<%} %>  dateFmt: 'yyyy-MM-dd', minDate: '#F{$dp.$D(\'txtDate\')}', maxDate: '%y-%M-%d' });
            });

            $(window).scroll(function () {
                if ($(window).scrollLeft() > 20) {
                    $("#flay").css({ left: $(window).scrollLeft() });
                } else {
                    $("#flay").css({ left: "20px" });
                }
            });

            $("#btnSubmit").click(function () {
                showLoading("正在加载数据，请稍候...");
            });

            $("#cbxAllDayReportCorp").click(function () {
                $(".cbxDayReportCorp").attr("checked", $(this).attr("checked"));
                setdayreportcorp();
            })
            $(".cbxDayReportCorp").click(function () {
                setdayreportcorp();
            });

            $("#btnDCCZhhz").click(function () {
                $("#hdnKeyReportType").val("dcczhhz");
            });
            $("#btnDCCQdzhl").click(function () {
                $("#hdnKeyReportType").val("dccqdzhl");
            });
            $("#btnDCCCjqdzb").click(function () {
                $("#hdnKeyReportType").val("dccjqdzb");
            });
            $("#btnKeyTarget").click(function () {
                $("#hdnKeyReportType").val("");
            });
        });
        function exportExcel() {
            post("<%=UrlDecode(CurrentUrl) %>", { act: "tabletoexcel", html: $("#tdData").html(), fn: $("#spTitle").text() });
        }
        function post(URL, PARAMS) {
            var temp = document.createElement("form");
            temp.action = URL;
            temp.method = "post";
            temp.style.display = "none";
            for (var x in PARAMS) {
                var opt = document.createElement("textarea");
                opt.name = x;
                opt.id = x;
                opt.value = PARAMS[x];
                temp.appendChild(opt);
            }
            document.body.appendChild(temp);
            temp.submit();
            return temp;
        }

        function setdayreportcorp() {
            var targetcorp = $(".cbxDayReportCorp:checked").map(function () {
                return $(this).val();
            }).get().join('|');
            if (targetcorp != '')
                targetcorp = '|' + targetcorp + '|'
            $("#hdnDayReportCorp").val(targetcorp);
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
        #tbData tr:hover
        {
            background-color: #FFF68F;
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
            width: 122px;
            overflow: hidden;
            background-color: white;
        }
        a, img
        {
            border: 0;
        }
        .demo
        {
            margin: 100px auto 0 auto;
            width: 400px;
            text-align: center;
            font-size: 18px;
        }
        .demo .action
        {
            color: #3366cc;
            text-decoration: none;
            font-family: "微软雅黑" , "宋体";
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
            <li <%if(CurrentDep==DayReportDep.无忧产品){ %>class="current" <%} %> <%= GetDepHide(DayReportDep.无忧产品) %>>
                <a href="?<%= CurrentQuery%>&dep=<%= (int)DayReportDep.无忧产品%><%= NewQuery %>">无忧产品</a></li>
        </ul>
        <form id="form1" runat="server">
        <table width="100%" border="0" cellspacing="0" cellpadding="0" class="biaoge3">
            <caption class="bt2">
                月报汇总</caption>
            <tbody>
                <tr>
                    <td class="bg1">
                        时间段：
                    </td>
                    <td>
                        <asp:TextBox runat="server" ID="txtDate" CssClass="srk6"></asp:TextBox>
                        -
                        <asp:TextBox runat="server" ID="txtDate2" CssClass="srk6"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td class="bg1">
                        公司：
                    </td>
                    <td class="clpp">
                        <ul>
                            <li class="nh">
                                <label>
                                    <input type="checkbox" id="cbxAllDayReportCorp" runat="server" class="fll" />全选</label></li>
                        </ul>
                    </td>
                </tr>
                <tr>
                    <td>
                    </td>
                    <td>
                        <asp:Repeater runat="server" ID="rptDayReportCorp">
                            <HeaderTemplate>
                                <ul>
                            </HeaderTemplate>
                            <ItemTemplate>
                                <li class="blockinline" style="width: 230px;">
                                    <label>
                                        <input type="checkbox" class="cbxDayReportCorp fll" value="<%# Eval("ID") %>" <%#SetDayReportCorp(Eval("ID").ToString()) %> /><%# Eval("Name") %></label></li>
                            </ItemTemplate>
                            <FooterTemplate>
                                </ul></FooterTemplate>
                        </asp:Repeater>
                    </td>
                </tr>
                <tr>
                    <td>
                    </td>
                    <td colspan="3">
                        <asp:Button runat="server" ID="btnSubmit" CssClass="an1" OnClick="btnSubmit_Click"
                            Text="月报汇总" />
                        <asp:Button runat="server" ID="btnKeyTarget" CssClass="an1" OnClick="btnKeyTarget_Click"
                            Text="指标汇总" />
                        <asp:Button runat="server" ID="btnReportCount" CssClass="an1" OnClick="btnReportCount_Click"
                            Text="填报统计" />
                        <%if (CurrentDep == DayReportDep.DCC部)
                          { %>
                        <asp:Button runat="server" ID="btnDCCZhhz" CssClass="an1" OnClick="btnKeyTarget_Click"
                            Text="综合汇总" />
                        <asp:Button runat="server" ID="btnDCCQdzhl" CssClass="an1" OnClick="btnKeyTarget_Click"
                            Text="渠道汇总" />
                        <asp:Button runat="server" ID="btnDCCCjqdzb" CssClass="an1" OnClick="btnKeyTarget_Click"
                            Text="网络汇总" />
                        <asp:Button runat="server" ID="btnDCCCyb" CssClass="an1" OnClick="btnDCCMonthly_Click"
                            Text="DCC月报" />
                        <%}else if(CurrentDep == DayReportDep.市场部) {%>
                        <asp:Button runat="server" ID="btnSCFollow" CssClass="an1" OnClick="btnSCFollow_Click"
                            Text="客源、线索进度下载" />
                        <asp:Button runat="server" ID="btnSCdzpxfa" CssClass="an1" OnClick="btnSCdzpxfa_Click"
                            Text="总经理关键指标排名" />
                        <%}
                          else if (CurrentDep == DayReportDep.销售部)
                          {%>
                        <asp:Button runat="server" ID="btnXSDayGather" CssClass="an1" OnClick="btnXSDayGather_Click"
                            Text="销售日报汇总" />
                            <%if(CurrentUser.YearGather == "1"){ %>
                        <asp:Button runat="server" ID="btnXSYearGather" CssClass="an1" OnClick="btnXSYearGather_Click"
                            Text="销售数据分析汇总" />
                            <%} %>
                        <%}else if(CurrentDep == DayReportDep.售后部){ %>
                            <%if(CurrentUser.YearGather == "1") {%>
                        <asp:Button runat="server" ID="btnSHYearGather" CssClass="an1" OnClick="btnSHYearGather_Click"
                            Text="售后数据分析汇总" />
                            <%} %>
                        <%}else if (CurrentDep == DayReportDep.精品部){ %>
                            <%if(CurrentUser.YearGather == "1") {%>
                        <asp:Button runat="server" ID="btnJPYearGather" CssClass="an1" OnClick="btnJPYearGather_Click"
                            Text="精品数据分析汇总" />
                            <%} %>                            
                        <%}else if(CurrentDep == DayReportDep.无忧产品){ %>
                        <asp:Button runat="server" ID="btnNXCPDataGather" CssClass="an1" OnClick="btnNXCPDataGather_Click"
                            Text="无忧产品数据分析汇总" />
                        <%} %>
                        <input type="button" name="btnExportExcel" id="btnExportExcel" value="导出Excel" onclick="exportExcel()"
                            class="an1" />
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
        <input type="hidden" runat="server" id="hdnDayReportCorp" />
        <input type="hidden" runat="server" id="hdnKeyReportType" />
        </form>
        <asp:Label runat="server" ID="lblReportCount" CssClass="mb10 mt10 block"></asp:Label>
        <table border="0" cellspacing="0" cellpadding="0" id="tblView" runat="server" enableviewstate="false"
            style="display: none;">
            <tbody>
                <tr class="bggray pt">
                    <td class="tl" style="border-top: 1px solid #000; border-right: 1px solid #000; border-left: 1px solid #000;">
                        <span runat="server" id="spTitle" class="bold" style="font-size: 20px; padding-left: 500px;">
                        </span>
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
            $("#flay table").width($("#tblView").width());
            $("#flay").css({ left: $("#tbData").offset().left, top: $("#tbData").offset().top });
        }
    </script>
</body>
</html>
