<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="monthlytarget.aspx.cs"
    Inherits="Hx.BackAdmin.dayreport.monthlytarget" %>

<%@ Import Namespace="Hx.Components.Enumerations" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>月度目标</title>
    <link href="../css/admin.css" rel="stylesheet" type="text/css" />
    <script src="../js/jquery-1.3.2.min.js" type="text/javascript"></script>
    <script src="../js/WdatePicker.js" type="text/javascript"></script>
    <script type="text/javascript">
        var timer_bindevent;
        $(function () {
            bindevent();

            $("#btnSubmit").click(function () {
                if ($("#ddlCorp").val() == "-1") {
                    $("#spMsg").text("请选择公司");
                    setTimeout(function () {
                        $("#spMsg").text("");
                    }, 2000);
                    return false;
                }
            });
        });

        function bindevent() {
            if ($("#hdnallowmodify").length > 0) {
                $("#btnSubmit").attr("disabled", $("#hdnallowmodify").val() == "1" ? "" : "disabled")
                .attr("class", $("#hdnallowmodify").val() == "1" ? "an1" : "an1dis");
            }
            $("#txtDate").unbind("click");
            $("#txtDate").click(function () {
                WdatePicker({ 'readOnly': 'true', dateFmt: 'yyyy-MM', isShowToday: false, isShowClear: false });
            });

            if (timer_bindevent)
                clearTimeout(timer_bindevent);
            timer_bindevent = setTimeout(function () {
                bindevent();
            }, 1000);
        }
    </script>
</head>
<body>
    <form id="form1" runat="server">
    <div class="ht_main">
        <ul class="xnav">
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
        <asp:ScriptManager runat="server" ID="sm">
        </asp:ScriptManager>
        <asp:UpdatePanel runat="server" ID="up1">
            <ContentTemplate>
                <table width="100%" border="0" cellspacing="0" cellpadding="0" class="biaoge3">
                    <caption class="bt2">
                        月度目标</caption>
                    <tbody>
                        <tr>
                            <td class="bg1">
                                公司：
                            </td>
                            <td colspan="3">
                                <asp:DropDownList runat="server" ID="ddlCorp" OnSelectedIndexChanged="ddlCorp_SelectedIndexChanged"
                                    AutoPostBack="true">
                                </asp:DropDownList>
                            </td>
                        </tr>
                        <tr>
                            <td class="bg1">
                                月份：
                            </td>
                            <td colspan="3">
                                <asp:TextBox runat="server" ID="txtDate" CssClass="srk6" OnTextChanged="txtDate_TextChanged"
                                    AutoPostBack="true"></asp:TextBox>
                                <asp:Label runat="server" ID="lblMsg" Text="" CssClass="red"></asp:Label>
                            </td>
                        </tr>
                        <tr>
                            <td class="gray bg1">温馨提示：</td>
                            <td colspan="3" class="gray">
                                请在每月7号之前（含每月7号）设置好月度目标，7号之后将不能设置、修改月度目标。
                            </td>
                        </tr>
                        <%if (CurrentDep == DayReportDep.销售部)
                          { %>
                        <tr>
                            <td colspan="4" style="background-color: #ccc; color: Black; font-weight: bold;">
                                关键指标
                            </td>
                        </tr>
                        <tr>
                            <td class="bg4 tr">
                                展厅留档率：
                            </td>
                            <td class="bg3">
                                <asp:TextBox runat="server" ID="txtXSztldl" CssClass="srk4 tr"></asp:TextBox>
                                %
                            </td>
                            <td class="bg4 tr">
                                展厅成交率：
                            </td>
                            <td>
                                <asp:TextBox runat="server" ID="txtXSztcjl" CssClass="srk4 tr"></asp:TextBox>
                                %
                            </td>
                        </tr>
                        <tr>
                            <td class="bg4 tr">
                                展厅占比：
                            </td>
                            <td class="bg3">
                                <asp:TextBox runat="server" ID="txtXSztzb" CssClass="srk4 tr"></asp:TextBox>
                                %
                            </td>
                            <td class="bg4 tr">
                                上牌率：
                            </td>
                            <td>
                                <asp:TextBox runat="server" ID="txtXSspl" CssClass="srk4 tr"></asp:TextBox>
                                %
                            </td>
                        </tr>
                        <tr>
                            <td class="bg4 tr">
                                上牌单台：
                            </td>
                            <td class="bg3">
                                <asp:TextBox runat="server" ID="txtXSspdt" CssClass="srk6"></asp:TextBox>
                            </td>
                            <td class="bg4 tr">
                                保险率：
                            </td>
                            <td>
                                <asp:TextBox runat="server" ID="txtXSbxl" CssClass="srk4 tr"></asp:TextBox>
                                %
                            </td>
                        </tr>
                        <tr>
                            <td class="bg4 tr">
                                保险单台：
                            </td>
                            <td class="bg3">
                                <asp:TextBox runat="server" ID="txtXSbxdt" CssClass="srk6"></asp:TextBox>
                            </td>
                            <td class="bg4 tr">
                                美容交车率：
                            </td>
                            <td>
                                <asp:TextBox runat="server" ID="txtXSmrjcl" CssClass="srk4 tr"></asp:TextBox>
                                %
                            </td>
                        </tr>
                        <tr>
                            <td class="bg4 tr">
                                美容单台：
                            </td>
                            <td class="bg3">
                                <asp:TextBox runat="server" ID="txtXSmrdt" CssClass="srk6"></asp:TextBox>
                            </td>
                            <td class="bg4 tr">
                                延保渗透率：
                            </td>
                            <td>
                                <asp:TextBox runat="server" ID="txtXSybstl" CssClass="srk4 tr"></asp:TextBox>
                                %
                            </td>
                        </tr>
                        <tr>
                            <td class="bg4 tr">
                                展厅精品前装率：
                            </td>
                            <td class="bg3">
                                <asp:TextBox runat="server" ID="txtXSztjpqzl" CssClass="srk4 tr"></asp:TextBox>
                                %
                            </td>
                            <td class="bg4 tr">
                                展厅精品单台：
                            </td>
                            <td>
                                <asp:TextBox runat="server" ID="txtXSztjpdt" CssClass="srk6"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td class="bg4 tr">
                                二网精品单台：
                            </td>
                            <td class="bg3">
                                <asp:TextBox runat="server" ID="txtXSewjpdt" CssClass="srk6"></asp:TextBox>
                            </td>
                            <td class="bg4 tr">
                                二手车评估率：
                            </td>
                            <td>
                                <asp:TextBox runat="server" ID="txtXSescpgl" CssClass="srk4 tr"></asp:TextBox>
                                %
                            </td>
                        </tr>
                        <tr>
                            <td class="bg4 tr">
                                按揭率：
                            </td>
                            <td class="bg3">
                                <asp:TextBox runat="server" ID="txtXSajl" CssClass="srk4 tr"></asp:TextBox>
                                %
                            </td>
                            <td class="bg4 tr">
                                按揭平均单台：
                            </td>
                            <td>
                                <asp:TextBox runat="server" ID="txtXSajpjdt" CssClass="srk6"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td class="bg4 tr">
                                免费保养渗透率：
                            </td>
                            <td class="bg3">
                                <asp:TextBox runat="server" ID="txtXSmfbystl" CssClass="srk4 tr"></asp:TextBox>
                                %
                            </td>
                            <td class="bg4 tr">
                                免费保养单台：
                            </td>
                            <td>
                                <asp:TextBox runat="server" ID="txtXSmfbypjdt" CssClass="srk6"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td colspan="4" style="background-color: #ccc; color: Black; font-weight: bold;">
                                各项目标值
                            </td>
                        </tr>
                        <%=GetTableStr()%>
                        <%}
                          else if (CurrentDep == DayReportDep.售后部)
                          {%>
                        <tr>
                            <td colspan="4" style="background-color: #ccc; color: Black; font-weight: bold;">
                                关键指标
                            </td>
                        </tr>
                        <tr>
                            <td class="bg4 tr">
                                来厂台次：
                            </td>
                            <td class="bg3">
                                <asp:TextBox runat="server" ID="txtSHlctc" CssClass="srk6"></asp:TextBox>
                            </td>
                            <td class="bg4 tr">
                                预约率：
                            </td>
                            <td>
                                <asp:TextBox runat="server" ID="txtSHyyl" CssClass="srk4 tr"></asp:TextBox>
                                %
                            </td>
                        </tr>
                        <tr>
                            <td class="bg4 tr">
                                产值达成：
                            </td>
                            <td class="bg3">
                                <asp:TextBox runat="server" ID="txtSHczdc" CssClass="srk6"></asp:TextBox>
                            </td>
                            <td class="bg4 tr">
                                事故产值占比：
                            </td>
                            <td>
                                <asp:TextBox runat="server" ID="txtSHsgczl" CssClass="srk4 tr"></asp:TextBox>
                                %
                            </td>
                        </tr>
                        <tr>
                            <td class="bg4 tr">
                                养护比例：
                            </td>
                            <td class="bg3">
                                <asp:TextBox runat="server" ID="txtSHyhbl" CssClass="srk4 tr"></asp:TextBox>
                                %
                            </td>
                            <td class="bg4 tr">
                                事故首次成功率：
                            </td>
                            <td>
                                <asp:TextBox runat="server" ID="txtSHsgsccgl" CssClass="srk4 tr"></asp:TextBox>
                                %
                            </td>
                        </tr>
                        <tr>
                            <td class="bg4 tr">
                                事故车短信成功率：
                            </td>
                            <td class="bg3">
                                <asp:TextBox runat="server" ID="txtSHsgcdxcgl" CssClass="srk4 tr"></asp:TextBox>
                                %
                            </td>
                            <td class="bg4 tr">
                                美容比例：
                            </td>
                            <td>
                                <asp:TextBox runat="server" ID="txtSHmrbl" CssClass="srk4 tr"></asp:TextBox>
                                %
                            </td>
                        </tr>
                        <tr>
                            <td class="bg4 tr">
                                索赔成功率：
                            </td>
                            <td class="bg3">
                                <asp:TextBox runat="server" ID="txtSHspcgl" CssClass="srk4 tr"></asp:TextBox>
                                %
                            </td>
                            <td class="bg4 tr">
                                内返率：
                            </td>
                            <td>
                                <asp:TextBox runat="server" ID="txtSHnfl" CssClass="srk4 tr"></asp:TextBox>
                                %
                            </td>
                        </tr>
                        <tr>
                            <td class="bg4 tr">
                                供货及时率：
                            </td>
                            <td class="bg3">
                                <asp:TextBox runat="server" ID="txtSHghjsl" CssClass="srk4 tr"></asp:TextBox>
                                %
                            </td>
                            <td class="bg4 tr">
                                事故再次成功率：
                            </td>
                            <td>
                                <asp:TextBox runat="server" ID="txtSHsgzccgl" CssClass="srk4 t"></asp:TextBox>
                                %
                            </td>
                        </tr>
                        <tr>
                            <td class="bg4 tr">
                                单台产值：
                            </td>
                            <td class="bg3">
                                <asp:TextBox runat="server" ID="txtSHdtcz" CssClass="srk6"></asp:TextBox>
                            </td>
                            <td class="bg4 tr">
                                保养单台产值：
                            </td>
                            <td>
                                <asp:TextBox runat="server" ID="txtSHbydtcz" CssClass="srk6"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td class="bg4 tr">
                                保养台次占比：
                            </td>
                            <td class="bg3">
                                <asp:TextBox runat="server" ID="txtSHbytczb" CssClass="srk4 tr"></asp:TextBox>
                                %
                            </td>
                            <td class="bg4 tr">
                                美容单台产值：
                            </td>
                            <td>
                                <asp:TextBox runat="server" ID="txtSHmrdtcz" CssClass="srk6"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td class="bg4 tr">
                                出厂台次：
                            </td>
                            <td class="bg3">
                                <asp:TextBox runat="server" ID="txtSHcctc" CssClass="srk6"></asp:TextBox>
                            </td>
                            <td class="bg4 tr">
                                延保达成：
                            </td>
                            <td>
                                <asp:TextBox runat="server" ID="txtSHybdcl" CssClass="srk6"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td colspan="4" style="background-color: #ccc; color: Black; font-weight: bold;">
                                各项目标值
                            </td>
                        </tr>
                        <%=GetTableStr()%>
                        <%}
                          else if (CurrentDep == DayReportDep.市场部)
                          {%>
                        <tr>
                            <td colspan="4" style="background-color: #ccc; color: Black; font-weight: bold;">
                                关键指标
                            </td>
                        </tr>
                        <tr>
                            <td class="bg4 tr">
                                总线索达成率：
                            </td>
                            <td class="bg2">
                                <asp:TextBox runat="server" ID="txtSCxsdcl" CssClass="srk4 tr"></asp:TextBox>
                                %<span class="pl10 gray">总线索实际达成数/总线索达成目标</span>
                            </td>
                            <td class="bg4 tr">
                                首次到访达成率：
                            </td>
                            <td>
                                <asp:TextBox runat="server" ID="txtSCscdfdcl" CssClass="srk4 tr"></asp:TextBox>
                                %<span class="pl10 gray">首次到访建档数/首次到访建档目标</span>
                            </td>
                        </tr>
                        <tr>
                            <td class="tr">
                                首次到访建档率：
                            </td>
                            <td>
                                <asp:TextBox runat="server" ID="txtSCsfjdl" CssClass="srk4 tr"></asp:TextBox>
                                %<span class="pl10 gray">首次到访建档数/首次到访线索数</span>
                            </td>
                            <td class="tr">
                                网络后台线索达成率：
                            </td>
                            <td>
                                <asp:TextBox runat="server" ID="txtSCwlhtxsdcl" CssClass="srk4 tr"></asp:TextBox>
                                %<span class="pl10 gray">网络后台建档数/网络后台建档目标</span>
                            </td>
                        </tr>
                        <tr>
                            <td class="tr">
                                网络线索建档率：
                            </td>
                            <td>
                                <asp:TextBox runat="server" ID="txtSCwlxsjdl" CssClass="srk4 tr"></asp:TextBox>
                                %<span class="pl10 gray">网络后台建档数/网络后台线索数</span>
                            </td>
                            <td class="tr">
                                呼入电话达成率：
                            </td>
                            <td>
                                <asp:TextBox runat="server" ID="txtSChrdhdcl" CssClass="srk4 tr"></asp:TextBox>
                                %<span class="pl10 gray">呼入电话（展厅+网络）建档数/呼入电话（展厅+网络）建档目标</span>
                            </td>
                        </tr>
                        <tr>
                            <td class="tr">
                                触点潜客达成率：
                            </td>
                            <td>
                                <asp:TextBox runat="server" ID="txtSCcdqkdcl" CssClass="srk4 tr"></asp:TextBox>
                                %<span class="pl10 gray">触点潜客建档数/触点潜客建档目标</span>
                            </td>
                            <td class="tr">
                                呼入电话建档率：
                            </td>
                            <td>
                                <asp:TextBox runat="server" ID="txtSChrdhjdl" CssClass="srk4 tr"></asp:TextBox>
                                %<span class="pl10 gray">呼入电话（展厅+网络）建档数/呼入电话（展厅+网络）线索数</span>
                            </td>
                        </tr>
                        <tr>
                            <td class="tr">
                                活动集客达成率：
                            </td>
                            <td colspan="3">
                                <asp:TextBox runat="server" ID="txtSChdjkdcl" CssClass="srk4 tr"></asp:TextBox>
                                %<span class="pl10 gray">活动集客建档数/活动集客建档目标</span>
                            </td>
                        </tr>
                        <tr>
                            <td colspan="4" style="background-color: #ccc; color: Black; font-weight: bold;">
                                各项目标值
                            </td>
                        </tr>
                        <tr class="hide">
                            <td class="bg4 tr">
                                新增有效线索量：
                            </td>
                            <td colspan="3">
                                <asp:TextBox runat="server" ID="txtSCxzyxxsl" CssClass="srk6"></asp:TextBox>
                            </td>
                        </tr>
                        <%=GetTableStr()%>
                        <%}
                          else if (CurrentDep == DayReportDep.财务部)
                          {%><%}
                          else if (CurrentDep == DayReportDep.行政部)
                          {%><%}
                          else if (CurrentDep == DayReportDep.精品部)
                          {%>
                        <%=GetTableStr()%>
                        <%}
                          else if (CurrentDep == DayReportDep.客服部)
                          {%>
                        <%=GetTableStr()%>
                        <%}
                          else if (CurrentDep == DayReportDep.二手车部)
                          {%>
                        <tr>
                            <td colspan="4" style="background-color: #ccc; color: Black; font-weight: bold;">
                                关键指标
                            </td>
                        </tr>
                        <tr>
                            <td class="bg4 tr">
                                销售有效推荐率：
                            </td>
                            <td class="bg3">
                                <asp:TextBox runat="server" ID="txtESCxsyxtjl" CssClass="srk4 tr"></asp:TextBox>
                                %
                            </td>
                            <td class="bg4 tr">
                                售后有效推荐率：
                            </td>
                            <td>
                                <asp:TextBox runat="server" ID="txtESCxhyxtjl" CssClass="srk4 tr"></asp:TextBox>
                                %
                            </td>
                        </tr>
                        <tr>
                            <td class="bg4 tr">
                                总评估成交率：
                            </td>
                            <td class="bg3">
                                <asp:TextBox runat="server" ID="txtESCpgcjl" CssClass="srk4 tr"></asp:TextBox>
                                %
                            </td>
                            <td class="bg4 tr">
                                总销售成交率：
                            </td>
                            <td>
                                <asp:TextBox runat="server" ID="txtESCxscjl" CssClass="srk4 tr"></asp:TextBox>
                                %
                            </td>
                        </tr>
                        <tr>
                            <td class="bg4 tr">
                                平均单台毛利：
                            </td>
                            <td class="bg3">
                                <asp:TextBox runat="server" ID="txtESCpjdtml" CssClass="srk6"></asp:TextBox>
                            </td>
                            <td class="bg4 tr">
                                总置换率：
                            </td>
                            <td>
                                <asp:TextBox runat="server" ID="txtESCzzhl" CssClass="srk4 tr"></asp:TextBox>
                                %
                            </td>
                        </tr>
                        <tr>
                            <td class="bg4 tr">
                                总有效评估量：
                            </td>
                            <td class="bg3">
                                <asp:TextBox runat="server" ID="txtESCzyxpgl" CssClass="srk6"></asp:TextBox>
                            </td>
                            <td class="bg4 tr">
                                总收购量：
                            </td>
                            <td>
                                <asp:TextBox runat="server" ID="txtESCzsgl" CssClass="srk6"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td class="bg4 tr">
                                总销售量：
                            </td>
                            <td class="bg3">
                                <asp:TextBox runat="server" ID="txtESCzxsl" CssClass="srk6"></asp:TextBox>
                            </td>
                            <td class="bg4 tr">
                                总毛利：
                            </td>
                            <td>
                                <asp:TextBox runat="server" ID="txtESCzml" CssClass="srk6"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td colspan="4" style="background-color: #ccc; color: Black; font-weight: bold;">
                                各项目标值
                            </td>
                        </tr>
                        <%=GetTableStr()%>
                        <%}
                          else if (CurrentDep == DayReportDep.金融部)
                          {%>
                          <%=GetTableStr()%>
                        <%}
                          else if (CurrentDep == DayReportDep.DCC部)
                          {%>
                        <tr>
                            <td colspan="4" style="background-color: #ccc; color: Black; font-weight: bold;">
                                关键指标
                            </td>
                        </tr>
                        <tr>
                            <td class="bg4 tr">
                                销量占展厅比：
                            </td>
                            <td class="bg3">
                                <asp:TextBox runat="server" ID="txtDCCztzb" CssClass="srk4 tr"></asp:TextBox>
                                %<span class="pl10 gray">DCC交车台次/展厅交车台次</span>
                            </td>
                            <td class="bg4 tr">
                                前台首电建档率：
                            </td>
                            <td>
                                <asp:TextBox runat="server" ID="txtDCCsdjdl" CssClass="srk4 tr"></asp:TextBox>
                                %<span class="pl10 gray">新增前台来电有效客户数/新增前台来电所有客户</span>
                            </td>
                        </tr>
                        <tr>
                            <td class="bg4 tr">
                                网络后台建档率：
                            </td>
                            <td class="bg3">
                                <asp:TextBox runat="server" ID="txtDCCwlhtjdl" CssClass="srk4 tr"></asp:TextBox>
                                %<span class="pl10 gray">网络后台建档数/（网络后台所有线索数）</span>
                            </td>
                            <td class="bg4 tr">
                                网络呼入建档率：
                            </td>
                            <td>
                                <asp:TextBox runat="server" ID="txtDCCwlhrjdl" CssClass="srk4 tr"></asp:TextBox>
                                %<span class="pl10 gray">网络呼入建档数/网络呼入所有线索数</span>
                            </td>
                        </tr>
                        <tr>
                            <td class="bg4 tr">
                                有效呼出率：
                            </td>
                            <td class="bg3">
                                <asp:TextBox runat="server" ID="txtDCCyxhcl" CssClass="srk4 tr"></asp:TextBox>
                                %<span class="pl10 gray">呼出有效数/呼出总量</span>
                            </td>
                            <td class="bg4 tr">
                                呼入呼出邀约到店率：
                            </td>
                            <td>
                                <asp:TextBox runat="server" ID="txtDCChrhcyyddl" CssClass="srk4 tr"></asp:TextBox>
                                %<span class="pl10 gray">首次邀约到店客户总数/新增线索建档总数</span>
                            </td>
                        </tr>
                        <tr>
                            <td class="bg4 tr">
                                再次邀约占比：
                            </td>
                            <td class="bg3">
                                <asp:TextBox runat="server" ID="txtDCCzcyyl" CssClass="srk4 tr"></asp:TextBox>
                                %<span class="pl10 gray">再邀约到店数/（首次邀约到店客户总数+再邀约到店数）</span>
                            </td>
                            <td class="bg4 tr">
                                成交率：
                            </td>
                            <td>
                                <asp:TextBox runat="server" ID="txtDCCcjl" CssClass="srk4 tr"></asp:TextBox>
                                %<span class="pl10 gray">DCC交车台次/首次邀约到店客户总数</span>
                            </td>
                        </tr>
                        <tr>
                            <td colspan="4" style="background-color: #ccc; color: Black; font-weight: bold;">
                                各项目标值
                            </td>
                        </tr>
                        <%=GetTableStr()%>
                        <%} %>
                    </tbody>
                </table>
            </ContentTemplate>
        </asp:UpdatePanel>
        <table width="100%" border="0" cellspacing="0" cellpadding="0" class="biaoge3">
            <tbody>
                <tr>
                    <td class="bg1">
                    </td>
                    <td>
                        <asp:Button runat="server" ID="btnSubmit" CssClass="an1" OnClick="btnSubmit_Click"
                            Text="提交" /><span id="spMsg" class="red" runat="server"></span>
                    </td>
                </tr>
            </tbody>
        </table>
    </div>
    </form>
</body>
</html>
