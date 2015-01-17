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
                            <td class="gray bg1">
                                温馨提示：
                            </td>
                            <td colspan="3" class="gray">
                                请在每月7号之前（含每月7号）设置好月度目标，7号之后将不能设置、修改月度目标。
                            </td>
                        </tr>
                        <%if (CurrentDep == DayReportDep.市场部)
                          {%>
                        <tr>
                            <td colspan="4" style="background-color: #ccc; color: Black; font-weight: bold;">
                                关键指标
                            </td>
                        </tr>
                        <tr>
                            <td class="tr">
                                首次到访达成率：
                            </td>
                            <td class="dq1">
                                <asp:TextBox runat="server" ID="txtSCscdfdcl" CssClass="srk4 tr"></asp:TextBox>
                                %<span class="pl10 gray">首次到访建档数/首次到访建档目标</span>
                            </td>
                            <td class="tr">
                                上月粉丝量：
                            </td>
                            <td>
                                <asp:TextBox runat="server" ID="txtSCsyfsl" CssClass="srk6"></asp:TextBox>
                                <span class="pl10 gray">到上月底粉丝量总量</span>
                            </td>
                        </tr>
                        <tr>
                            <td colspan="4" style="background-color: #ccc; color: Black; font-weight: bold;">
                                各项目标值
                            </td>
                        </tr>
                        <%=GetTableStr()%>
                        <%}
                          else if (CurrentDep == DayReportDep.财务部)
                          {%><%}
                          else if (CurrentDep == DayReportDep.行政部)
                          {%><%}
                          else if (CurrentDep == DayReportDep.销售部)
                          {%>
                        <tr>
                            <td colspan="4" style="background-color: #ccc; color: Black; font-weight: bold;">
                                关键指标
                            </td>
                        </tr>
                        <tr>
                            <td class="tr">
                                总销售台次：
                            </td>
                            <td class="dq1">
                                <asp:TextBox runat="server" ID="txtXSzxstc" CssClass="srk6"></asp:TextBox>
                            </td>
                            <td class="tr w160">
                                展厅占比：
                            </td>
                            <td>
                                <asp:TextBox runat="server" ID="txtXSztzb" CssClass="srk4 tr"></asp:TextBox>
                                %
                            </td>
                        </tr>
                        <tr>
                            <td class="tr">
                                展厅留档率：
                            </td>
                            <td>
                                <asp:TextBox runat="server" ID="txtXSztldl" CssClass="srk4"></asp:TextBox>
                                %
                            </td>
                            <td class="tr">
                                展厅成交率：
                            </td>
                            <td>
                                <asp:TextBox runat="server" ID="txtXSztcjl" CssClass="srk4 tr"></asp:TextBox>
                                %
                            </td>
                        </tr>
                        <tr>
                            <td class="tr">
                                上牌率：
                            </td>
                            <td>
                                <asp:TextBox runat="server" ID="txtXSspl" CssClass="srk4"></asp:TextBox>
                                %
                            </td>
                            <td class="tr">
                                上牌单台：
                            </td>
                            <td>
                                <asp:TextBox runat="server" ID="txtXSspdt" CssClass="srk6"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td class="tr">
                                展厅保险率：
                            </td>
                            <td>
                                <asp:TextBox runat="server" ID="txtXSztbxl" CssClass="srk4 tr"></asp:TextBox>
                                %
                            </td>
                            <td class="tr">
                                展厅保险单台：
                            </td>
                            <td>
                                <asp:TextBox runat="server" ID="txtXSztbxdt" CssClass="srk6"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td class="tr">
                                美容交车率：
                            </td>
                            <td>
                                <asp:TextBox runat="server" ID="txtXSmrjcl" CssClass="srk4"></asp:TextBox>
                                %
                            </td>
                            <td class="tr">
                                美容单台：
                            </td>
                            <td>
                                <asp:TextBox runat="server" ID="txtXSmrdt" CssClass="srk6"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td class="tr">
                                延保渗透率：
                            </td>
                            <td>
                                <asp:TextBox runat="server" ID="txtXSybstl" CssClass="srk4 tr"></asp:TextBox>
                                %
                            </td>
                            <td class="tr">
                                展厅精品前装率：
                            </td>
                            <td>
                                <asp:TextBox runat="server" ID="txtXSztjpqzl" CssClass="srk4"></asp:TextBox>
                                %
                            </td>
                        </tr>
                        <tr>
                            <td class="tr">
                                展厅精品平均单台：
                            </td>
                            <td>
                                <asp:TextBox runat="server" ID="txtXSztjppjdt" CssClass="srk6"></asp:TextBox>
                            </td>
                            <td class="tr">
                                二网精品平均单台：
                            </td>
                            <td>
                                <asp:TextBox runat="server" ID="txtXSewjppjdt" CssClass="srk6"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td class="tr">
                                销售置换台次：
                            </td>
                            <td>
                                <asp:TextBox runat="server" ID="txtXSxszhtc" CssClass="srk6"></asp:TextBox>
                            </td>
                            <td class="tr">
                                按揭率：
                            </td>
                            <td>
                                <asp:TextBox runat="server" ID="txtXSajl" CssClass="srk4 tr"></asp:TextBox>
                                %
                            </td>
                        </tr>
                        <tr>
                            <td class="tr">
                                按揭平均单台：
                            </td>
                            <td>
                                <asp:TextBox runat="server" ID="txtXSajpjdt" CssClass="srk6"></asp:TextBox>
                            </td>
                            <td class="tr">
                                免费保养渗透率：
                            </td>
                            <td>
                                <asp:TextBox runat="server" ID="txtXSmfbystl" CssClass="srk4"></asp:TextBox>
                                %
                            </td>
                        </tr>
                        <tr>
                            <td class="tr">
                                免费保养单台：
                            </td>
                            <td>
                                <asp:TextBox runat="server" ID="txtXSmfbydt" CssClass="srk6"></asp:TextBox>
                            </td>
                            <%if (CurrentCorporation != null && CurrentCorporation.DailyreportTpp == 1)
                              { %>
                            <td class="tr">
                                他品牌销售台次：
                            </td>
                            <td>
                                <asp:TextBox runat="server" ID="txtXStppxstc" CssClass="srk6"></asp:TextBox>
                            </td>
                            <%}
                              else
                              { %>
                            <td class="tr">
                            </td>
                            <td>
                            </td>
                            <%} %>
                        </tr>
                        <%if (CurrentCorporation != null && CurrentCorporation.DailyreportTpp == 1)
                          { %>
                        <tr>
                            <td class="tr">
                                他品牌单车毛利：
                            </td>
                            <td>
                                <asp:TextBox runat="server" ID="txtXStppdcml" CssClass="srk6"></asp:TextBox>
                            </td>
                            <td class="tr">
                                他品牌综合毛利：
                            </td>
                            <td>
                                <asp:TextBox runat="server" ID="txtXStppzhml" CssClass="srk6"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td class="tr">
                                他品牌平均单台：
                            </td>
                            <td>
                                <asp:TextBox runat="server" ID="txtXStpppjdt" CssClass="srk6"></asp:TextBox>
                            </td>
                            <td class="tr">
                            </td>
                            <td>
                            </td>
                        </tr>
                        <%} %>
                        <tr>
                            <td colspan="4" style="background-color: #ccc; color: Black; font-weight: bold;">
                                各项目标值
                            </td>
                        </tr>
                        <%=GetTableStr()%>
                        <%}
                          else
                          {%>
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
