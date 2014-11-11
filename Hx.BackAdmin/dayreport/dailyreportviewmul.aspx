<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="dailyreportviewmul.aspx.cs"
    ValidateRequest="false" Inherits="Hx.BackAdmin.dayreport.dailyreportviewmul" %>

<%@ Import Namespace="Hx.Components.Enumerations" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>月报汇总</title>
    <link href="../css/admin.css" rel="stylesheet" type="text/css" />
    <script src="../js/jquery-1.3.2.min.js" type="text/javascript"></script>
    <script src="../js/comm.js" type="text/javascript"></script>
    <script src="../js/WdatePicker.js" type="text/javascript"></script>
    <script type="text/javascript">
        var timer_bindevent;
        $(function () {
            $("#txtDate").click(function () {
                WdatePicker({ 'readOnly': 'true', dateFmt: 'yyyy-MM-dd', maxDate: '#F{$dp.$D(\'txtDate2\')}' });
            });

            $("#txtDate2").click(function () {
                WdatePicker({ 'readOnly': 'true', dateFmt: 'yyyy-MM-dd', minDate: '#F{$dp.$D(\'txtDate\')}', maxDate: '%y-%M-%d' });
            });

            $(window).scroll(function () {
                if ($(window).scrollLeft() > 20) {
                    $("#flay").css({ left: $(window).scrollLeft() });
                } else {
                    $("#flay").css({ left: "20px" });
                }
            });

            $("#btnSubmit").click(function () {
                $(".overlay").css({ 'display': 'block', 'opacity': '0.8' });
                $(".showbox").stop(true).animate({ 'margin-top': '200px', 'opacity': '1' }, 200);
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
        
        .overlay
        {
            position: fixed;
            top: 0;
            right: 0;
            bottom: 0;
            left: 0;
            z-index: 998;
            width: 100%;
            height: 100%;
            _padding: 0 20px 0 0;
            background: #f6f4f5;
            display: none;
        }
        .showbox
        {
            position: fixed;
            top: 0;
            left: 50%;
            z-index: 9999;
            opacity: 0;
            filter: alpha(opacity=0);
            margin-left: -80px;
        }
        *html, *html body
        {
            background-image: url(about:blank);
            background-attachment: fixed;
        }
        *html .showbox, *html .overlay
        {
            position: absolute;
            top: expression(eval(document.documentElement.scrollTop));
        }
        #AjaxLoading
        {
            border: 1px solid #8CBEDA;
            color: #37a;
            font-size: 12px;
            font-weight: bold;
        }
        #AjaxLoading div.loadingWord
        {
            width: 240px;
            height: 50px;
            line-height: 50px;
            border: 2px solid #D6E7F2;
            background: #fff;
        }
        #AjaxLoading img
        {
            margin: 10px 15px;
            float: left;
            display: inline;
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
                        <%if (CurrentDep == DayReportDep.DCC部)
                          { %>
                        <asp:Button runat="server" ID="btnDCCZhhz" CssClass="an1" OnClick="btnKeyTarget_Click"
                            Text="综合汇总" />
                        <asp:Button runat="server" ID="btnDCCQdzhl" CssClass="an1" OnClick="btnKeyTarget_Click"
                            Text="渠道汇总" />
                        <asp:Button runat="server" ID="btnDCCCjqdzb" CssClass="an1" OnClick="btnKeyTarget_Click"
                            Text="网络汇总" />
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
    <div class="overlay">
    </div>
    <div id="AjaxLoading" class="showbox">
        <div class="loadingWord">
            <img src="/images/waiting.gif">正在生成数据，请稍候...</div>
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
