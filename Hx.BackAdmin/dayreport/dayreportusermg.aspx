<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="dayreportusermg.aspx.cs"
    Inherits="Hx.BackAdmin.dayreport.dayreportusermg" %>

<%@ Register Assembly="AspNetPager" Namespace="Wuqi.Webdiyer" TagPrefix="webdiyer" %>
<%@ Import Namespace="Hx.Tools" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>日报用户管理</title>
    <link href=<%=ResourceServer%>/css/admin.css rel="stylesheet" type="text/css" />
    <script src=<%=ResourceServer%>/js/jquery-1.3.2.min.js type="text/javascript"></script>
    <script language="javascript" type="text/javascript">
        $(function () {
            $(".btnDel").click(function () {
                DelRow(this);
            });

            $("#btnAdd").click(function () {
                $("#hdnAddCount").val(parseInt($("#hdnAddCount").val()) + 1);
                $("#tbldayreportuser").append("<tr><td></td>"
                 + "<td><input type=\"text\" id=\"txtUserName" + $("#hdnAddCount").val() + "\" name=\"txtUserName" + $("#hdnAddCount").val() + "\" class=\"srk1 w60\" value=\"\" /></td>"
                 + "<td><input type=\"text\" id=\"txtUserTag" + $("#hdnAddCount").val() + "\" name=\"txtUserTag" + $("#hdnAddCount").val() + "\" class=\"srk1 w60\" value=\"\" /></td>"
                 + "<td><select type=\"text\" id=\"ddlDepartment" + $("#hdnAddCount").val() + "\" name=\"ddlDepartment" + $("#hdnAddCount").val() + "\">" + $("#ddlDepartment").html() + "</select></td>"
                 + "<td><select type=\"text\" id=\"ddlCorporation" + $("#hdnAddCount").val() + "\" name=\"ddlCorporation" + $("#hdnAddCount").val() + "\">" + $("#ddlCorporation").html() + "</select></td>"
                 + "<td><a href=\"javascript:void(0);\" class=\"btnDel\" val=\"0\">删除</a> </td></tr>");
                $(".btnDel").unbind("click").click(function () {
                    DelRow(this);
                });
            });

            $("#btnFilter").click(function () {
                var query = new Array();
                if ($.trim($("#ddlCorporationFilter").val()) != "-1")
                    query[query.length] = "corp=" + $.trim($("#ddlCorporationFilter").val());
                if ($.trim($("#ddlDepartmentFilter").val()) != "-1")
                    query[query.length] = "dep=" + $.trim($("#ddlDepartmentFilter").val());
                if ($.trim($("#txtUserName").val()) != "")
                    query[query.length] = "username=" + $.trim($("#txtUserName").val());
                if ($.trim($("#txtUserTag").val()) != "")
                    query[query.length] = "usertag=" + $.trim($("#txtUserTag").val());
                location = "?" + (query.length > 0 ? $(query).map(function () {
                    return this;
                }).get().join("&") : "");

                return false;
            });
        });

        function DelRow(obj) {
            if ($(obj).attr("val") > 0) {
                $("#hdnDelIds").val($("#hdnDelIds").val() + ($("#hdnDelIds").val() == "" ? "" : ",") + $(obj).attr("val"));
            }
            $(obj).parent().parent().remove();
        }
    </script>
</head>
<body>
    <form id="form1" runat="server">
    <div class="ht_main">
        <table width="770" border="0" cellspacing="0" cellpadding="0" class="biaoge4" style="background-color: #f4f8fc;">
            <tr>
                <td class="w40 bold" rowspan="2">
                    查询：
                </td>
                <td>
                    公司：<asp:DropDownList ID="ddlCorporationFilter" runat="server" CssClass="mr10" EnableViewState="false">
                    </asp:DropDownList>
                    部门：<asp:DropDownList ID="ddlDepartmentFilter" runat="server" CssClass="mr10" EnableViewState="false">
                    </asp:DropDownList><br />
                </td>
            </tr>
            <tr>
                <td>
                    姓名：<asp:TextBox ID="txtUserName" runat="server" CssClass="srk6 mr10" EnableViewState="false"></asp:TextBox>
                    唯一标示：<asp:TextBox ID="txtUserTag" runat="server" CssClass="srk6" EnableViewState="false"></asp:TextBox>
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
        <table width="770" border="0" cellspacing="0" cellpadding="0" class="biaoge2" id="tbldayreportuser">
            <asp:Repeater ID="rptdayreportuser" runat="server" OnItemDataBound="rptdayreportuser_ItemDataBound">
                <HeaderTemplate>
                    <tr class="bgbt">
                        <td class="bg21">
                            ID
                        </td>
                        <td>
                            姓名
                        </td>
                        <td>
                            唯一标识
                        </td>
                        <td>
                            所属部门
                        </td>
                        <td>
                            所属公司
                        </td>
                        <td>
                            操作
                        </td>
                    </tr>
                </HeaderTemplate>
                <ItemTemplate>
                    <tr>
                        <td>
                            <%#Eval("ID") %><input type="hidden" runat="server" id="hdnID" value='<%#Eval("ID") %>' />
                        </td>
                        <td>
                            <input type="text" runat="server" id="txtUserName" class="srk1 w60" value='<%#Eval("UserName") %>' />
                        </td>
                        <td>
                            <input type="text" runat="server" id="txtUserTag" class="srk1 w60" value='<%#Eval("UserTag")%>' />
                        </td>
                        <td>
                            <asp:DropDownList runat="server" ID="ddlDepartment">
                            </asp:DropDownList>
                        </td>
                        <td>
                            <asp:DropDownList runat="server" ID="ddlCorporation">
                            </asp:DropDownList>
                        </td>
                        <td>
                            <a href="dayreportuseredit.aspx?usertag=<%#Eval("UserTag") %>">
                                设置权限</a> <a href="javascript:void(0);" class="btnDel pl10" val="<%#Eval("ID") %>">删除</a>
                            <a href="main_i.aspx?Nm=<%# Eval("UserName") %>&Id=<%#Eval("UserTag")%>&Mm=<%# (DateTime.Now.Year + DateTime.Now.Month + DateTime.Now.Day) * DataConvert.SafeInt(Eval("UserTag")) * 3 %>"
                                target="_blank" class="pl10">日报模块</a>
                        </td>
                    </tr>
                </ItemTemplate>
            </asp:Repeater>
        </table>
        <webdiyer:AspNetPager ID="search_fy" UrlPaging="true" NextPageText="下一页" PrevPageText="上一页"
            CurrentPageButtonClass="current" PageSize="50" runat="server" NumericButtonType="Text"
            MoreButtonType="Text" ShowFirstLast="false" HorizontalAlign="Left" AlwaysShow="false"
            ShowDisabledButtons="False" PagingButtonSpacing="">
        </webdiyer:AspNetPager>
        <div class="lan5x" style="padding-top: 10px;">
            <input type="hidden" runat="server" id="hdnAddCount" value="0" />
            <input type="hidden" runat="server" id="hdnDelIds" value="" />
            <input class="an1" type="button" value="添加" id="btnAdd" />
            <asp:Button ID="btnSubmit" OnClick="btnSubmit_Click" CssClass="an1" runat="server"
                Text="保存" />
        </div>
        <div class="hidden">
            <asp:DropDownList runat="server" ID="ddlDepartment">
            </asp:DropDownList>
            <asp:DropDownList runat="server" ID="ddlCorporation">
            </asp:DropDownList>
        </div>
    </div>
    </form>
</body>
</html>
