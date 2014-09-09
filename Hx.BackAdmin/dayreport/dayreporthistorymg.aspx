<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="dayreporthistorymg.aspx.cs"
    EnableViewState="False" Inherits="Hx.BackAdmin.dayreport.dayreporthistorymg" %>

<%@ Import Namespace="Hx.Components.Enumerations" %>
<%@ Register Assembly="AspNetPager" Namespace="Wuqi.Webdiyer" TagPrefix="webdiyer" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>日报录入记录</title>
    <link href="../css/admin.css" rel="stylesheet" type="text/css" />
    <script src="../js/jquery-1.3.2.min.js" type="text/javascript"></script>
    <script src="../js/WdatePicker.js" type="text/javascript"></script>
    <script type="text/javascript">
        var showdetailid = "0";
        $(function () {
            $("#txtDate").click(function () {
                WdatePicker({ 'readOnly': 'true', dateFmt: 'yyyy-MM-dd' });
            });

            $(".btnDetail").click(function () {
                if ($(this).attr("val") == showdetailid) {
                    showdetailid = "0";
                    $("#showdetail").hide();
                }
                else {
                    showdetailid = $(this).attr("val");
                    showDetail(this);
                }
            });

            $("#btnFilter").click(function () {
                var query = new Array();
                if ($.trim($("#txtDate").val()) != "")
                    query[query.length] = "date=" + $.trim($("#txtDate").val());
                if ($.trim($("#txtOperator").val()) != "")
                    query[query.length] = "oper=" + $.trim($("#txtOperator").val());
                if ($.trim($("#ddlReportCorporation").val()) != "-1")
                    query[query.length] = "rcorp=" + $.trim($("#ddlReportCorporation").val());
                if ($.trim($("#ddlCorporation").val()) != "-1")
                    query[query.length] = "corp=" + $.trim($("#ddlCorporation").val());
                if ($.trim($("#ddlDepartment").val()) != "-1")
                    query[query.length] = "dep=" + $.trim($("#ddlDepartment").val());
                if ($.trim($("#ddlReportDepartment").val()) != "-1")
                    query[query.length] = "rdep=" + $.trim($("#ddlReportDepartment").val());
                location = "?" + (query.length > 0 ? $(query).map(function () {
                    return this;
                }).get().join("&") : "");

                return false;
            });
        });

        function showDetail(obj) {
            $("#showdetail").show();
            $("#showdetail").html($(obj).parent().find(".pndetail").html());

            var left = $(obj).offset().left + 40;
            var top = $(obj).offset().top;
            var wheight = $(window).height();
            var height = $("#showdetail").height();
            if (height > wheight)
                top = 0;
            $("#showdetail").css({ left: left, top: top });
        }
    </script>
</head>
<body>
    <div class="ht_main">
        <form runat="server" id="form1">
        <table width="950" border="0" cellspacing="0" cellpadding="0" class="biaoge4" style="background-color: #f4f8fc;">
            <tr>
                <td class="w40 bold" rowspan="2">
                    查询：
                </td>
                <td>
                    日报日期：<asp:TextBox runat="server" ID="txtDate" CssClass="srk6 mr10"></asp:TextBox>
                    日报部门：<asp:DropDownList runat="server" ID="ddlReportDepartment">
                    </asp:DropDownList>
                    日报公司：<asp:DropDownList runat="server" ID="ddlReportCorporation">
                    </asp:DropDownList>
                </td>
            </tr>
            <tr>
                <td>
                    操作人公司：<asp:DropDownList runat="server" ID="ddlCorporation">
                    </asp:DropDownList>
                    操作人部门：<asp:DropDownList runat="server" ID="ddlDepartment">
                    </asp:DropDownList>
                    操作人：<asp:TextBox runat="server" ID="txtOperator" CssClass="srk6"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td>
                </td>
                <td>
                    <input type="submit" id="btnFilter" class="an1" value="确定" />
                </td>
            </tr>
        </table>
        </form>
        <table width="950" border="0" cellspacing="0" cellpadding="0" class="biaoge2">
            <asp:Repeater ID="rpthistory" runat="server">
                <HeaderTemplate>
                </HeaderTemplate>
                <HeaderTemplate>
                    <tr class="bgbt">
                        <td class="w60">
                            日报日期
                        </td>
                        <td class="w60">
                            日报部门
                        </td>
                        <td class="w230">
                            日报公司
                        </td>
                        <td class="w230">
                            操作人所属公司
                        </td>
                        <td class="w80">
                            操作人部门
                        </td>
                        <td class="w60">
                            操作人
                        </td>
                        <td class="w120">
                            操作时间
                        </td>
                        <td>
                            详细
                        </td>
                    </tr>
                </HeaderTemplate>
                <ItemTemplate>
                    <tr>
                        <td>
                            <%# Eval("DayUnique")%>
                        </td>
                        <td>
                            <a style="color: Blue" href="?rdep=<%# (int)(DayReportDep)Eval("ReportDepartment")%>">
                                <%# Eval("ReportDepartment").ToString()%></a>
                        </td>
                        <td>
                            <a style="color: Blue" href="?rcorp=<%# Eval("ReportCorporationID")%>">
                                <%# GetCorpname(Eval("ReportCorporationID").ToString())%></a>
                        </td>
                        <td>
                            <a style="color: Blue" href="?corp=<%# Eval("CreatorCorporationID")%>">
                                <%# Eval("CreatorCorporationName")%></a>
                        </td>
                        <td>
                            <a style="color: Blue" href="?dep=<%# (int)(DayReportDep)Eval("CreatorDepartment")%>">
                                <%# Eval("CreatorDepartment").ToString()%></a>
                        </td>
                        <td>
                            <a style="color: Blue" href="?oper=<%# Eval("Creator")%>">
                                <%# Eval("Creator")%></a>
                        </td>
                        <td>
                            <%# Eval("CreateTime","{0:yyyy-MM-dd HH:mm:ss}")%>
                        </td>
                        <td>
                            <a href="javascript:void(0);" class="btnDetail pl10" val="<%#Eval("ID") %>">查看</a>
                            <div class="pndetail hide">
                                <%# GetDetail(Eval("ReportCorporationID").ToString(), Eval("ReportDepartment").ToString(), Eval("Detail").ToString())%></div>
                        </td>
                    </tr>
                </ItemTemplate>
            </asp:Repeater>
        </table>
        <webdiyer:AspNetPager ID="search_fy" UrlPaging="true" NextPageText="下一页" PrevPageText="上一页"
            CurrentPageButtonClass="current" PageSize="10" runat="server" NumericButtonType="Text"
            MoreButtonType="Text" ShowFirstLast="false" HorizontalAlign="Left" AlwaysShow="false"
            ShowDisabledButtons="False" PagingButtonSpacing="">
        </webdiyer:AspNetPager>
        <div id="showdetail" style="display: none; position: absolute; width: 330px; background: #fff;
            border: 3px solid #E7903F; padding: 5px;">
            <div id="detailcontent">
            </div>
        </div>
    </div>
</body>
</html>
