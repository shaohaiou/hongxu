<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="dayreportmodulemg.aspx.cs"
    Inherits="Hx.BackAdmin.dayreport.dayreportmodulemg" %>

<%@ Register Assembly="AspNetPager" Namespace="Wuqi.Webdiyer" TagPrefix="webdiyer" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>日报栏目管理</title>
    <link href="../css/admin.css" rel="stylesheet" type="text/css" />
    <script src="../js/jquery-1.3.2.min.js" type="text/javascript"></script>
    <script language="javascript" type="text/javascript">
        $(function () {
            $(".btnDel").click(function () {
                DelRow(this);
            });

            $("#btnAdd").click(function () {
                var maxsort = 0;
                $(".sort").each(function () {
                    if (parseInt($(this).val()) > maxsort)
                        maxsort = parseInt($(this).val());
                });

                $("#hdnAddCount").val(parseInt($("#hdnAddCount").val()) + 1);
                $("#tblmodule").append("<tr><td></td>"
                 + "<td><input type=\"text\" id=\"txtSort" + $("#hdnAddCount").val() + "\" name=\"txtSort" + $("#hdnAddCount").val() + "\" class=\"srk4 sort\" value=\"" + (maxsort + 1) + "\" oldval=\"" + (maxsort + 1) + "\"\"/></td>"
                 + "<td><select type=\"text\" id=\"ddlDepartment" + $("#hdnAddCount").val() + "\" name=\"ddlDepartment" + $("#hdnAddCount").val() + "\">" + $("#ddlDepartment").html() + "</select></td>"
                 + "<td><input type=\"text\" id=\"txtName" + $("#hdnAddCount").val() + "\" name=\"txtName" + $("#hdnAddCount").val() + "\" class=\"srk1 w120\" value=\"\" /></td>"
                 + "<td><input type=\"text\" id=\"txtDescription" + $("#hdnAddCount").val() + "\" name=\"txtDescription" + $("#hdnAddCount").val() + "\" class=\"srk1 w240\" value=\"\" /></td>"
                 + "<td><input type=\"checkbox\" id=\"cbxIsmonthlytarget" + $("#hdnAddCount").val() + "\" name=\"cbxIsmonthlytarget" + $("#hdnAddCount").val() + "\" checked=\"checked\" class=\"cbxIsmonthlytarget\" /><input type=\"hidden\" id=\"hdnIsmonthlytarget" + $("#hdnAddCount").val() + "\" name=\"hdnIsmonthlytarget" + $("#hdnAddCount").val() + "\" value=\"1\" /></td>"
                 + "<td><input type=\"checkbox\" id=\"cbxMustinput" + $("#hdnAddCount").val() + "\" name=\"cbxMustinput" + $("#hdnAddCount").val() + "\" checked=\"checked\" class=\"cbxMustinput\" /><input type=\"hidden\" id=\"hdnMustinput" + $("#hdnAddCount").val() + "\" name=\"hdnMustinput" + $("#hdnAddCount").val() + "\" value=\"1\" /></td>"
                 + "<td><input type=\"checkbox\" id=\"cbxIscount" + $("#hdnAddCount").val() + "\" name=\"cbxIscount" + $("#hdnAddCount").val() + "\" checked=\"checked\" class=\"cbxIscount\" /><input type=\"hidden\" id=\"hdnIscount" + $("#hdnAddCount").val() + "\" name=\"hdnIscount" + $("#hdnAddCount").val() + "\" value=\"1\" /></td>"
                 + "<td><a href=\"javascript:void(0);\" class=\"btnDel\" val=\"0\">删除</a> </td></tr>");
                $(".btnDel").unbind("click").click(function () {
                    DelRow(this);
                });

                BindSortChange();
            });

            BindSortChange();
        });

        function DelRow(obj) {
            if ($(obj).attr("val") > 0) {
                $("#hdnDelIds").val($("#hdnDelIds").val() + ($("#hdnDelIds").val() == "" ? "" : ",") + $(obj).attr("val"));
            }
            $(obj).parent().parent().remove();
        }

        function BindSortChange() {
            $(".sort").unbind("change");
            $(".cbxIsmonthlytarget").unbind("click");
            $(".cbxMustinput").unbind("click");

            $(".sort").change(function () {
                var oldval = parseInt($(this).attr("oldval"));
                var newval = parseInt($(this).val());
                $(".sort").not(this).each(function () {
                    if (newval > oldval && oldval > 0 && parseInt($(this).val()) > oldval && parseInt($(this).val()) <= newval) {
                        $(this).val(parseInt($(this).val()) - 1);
                        $(this).attr("oldval", $(this).val());
                    }
                    else if (newval < oldval && oldval > 0 && parseInt($(this).val()) < oldval && parseInt($(this).val()) >= newval) {
                        $(this).val(parseInt($(this).val()) + 1);
                        $(this).attr("oldval", $(this).val());
                    }
                });
                $(this).attr("oldval", $(this).val());
            });

            $(".cbxIsmonthlytarget").click(function () {
                $(this).next().val($(this).attr("checked") ? "1" : "0");
            });
            $(".cbxMustinput").click(function () {
                $(this).next().val($(this).attr("checked") ? "1" : "0");
            });
            $(".cbxIscount").click(function () {
                $(this).next().val($(this).attr("checked") ? "1" : "0");
            });
        }
    </script>
</head>
<body>
    <form id="form1" runat="server">
    <div class="ht_main">
        <table width="500" border="0" cellspacing="0" cellpadding="0" class="biaoge4" style="background-color: #f4f8fc;">
            <tr>
                <td class="w40 bold">
                    查询：
                </td>
                <td>
                    部门：<asp:DropDownList ID="ddlDepartmentFilter" runat="server">
                    </asp:DropDownList>
                </td>
            </tr>
            <tr>
                <td>
                </td>
                <td>
                    <asp:Button runat="server" ID="btnFilter" CssClass="an1" Text="确定" OnClick="btnFilter_Click" />
                </td>
            </tr>
        </table>
        <table width="820" border="0" cellspacing="0" cellpadding="0" class="biaoge2" id="tblmodule">
            <asp:Repeater ID="rptmodule" runat="server" OnItemDataBound="rptmodule_ItemDataBound">
                <HeaderTemplate>
                </HeaderTemplate>
                <HeaderTemplate>
                    <tr class="bgbt">
                        <td class="bg21 w40">
                            ID
                        </td>
                        <td class="w40">
                            排序
                        </td>
                        <td class="w100">
                            所属部门
                        </td>
                        <td class="w160">
                            栏目名称
                        </td>
                        <td class="w240">
                            描述信息
                        </td>
                        <td class="w60">
                            月度目标
                        </td>
                        <td class="w40">
                            必填
                        </td>
                        <td class="w40">
                            合计
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
                            <asp:TextBox runat="server" ID="txtSort" CssClass="srk4 sort"></asp:TextBox>
                        </td>
                        <td>
                            <asp:DropDownList runat="server" ID="ddlDepartment">
                            </asp:DropDownList>
                        </td>
                        <td>
                            <input type="text" runat="server" id="txtName" class="srk1 w120" value='<%#Eval("Name") %>' />
                        </td>
                        <td>
                            <input type="text" runat="server" id="txtDescription" class="srk1 w240" value='<%#Eval("Description") %>' />
                        </td>
                        <td>
                            <input type="checkbox" runat="server" id="cbxIsmonthlytarget" checked='<%# Hx.Tools.DataConvert.SafeBool(Eval("Ismonthlytarget")) ? true : false %>' />
                        </td>
                        <td>
                            <input type="checkbox" runat="server" id="cbxMustinput" checked='<%# Hx.Tools.DataConvert.SafeBool(Eval("Mustinput")) ? true : false %>' />
                        </td>
                        <td>
                            <input type="checkbox" runat="server" id="cbxIscount" checked='<%# Hx.Tools.DataConvert.SafeBool(Eval("Iscount")) ? true : false %>' />
                        </td>
                        <td>
                            <a href="javascript:void(0);" class="btnDel pl10" val="<%#Eval("ID") %>">删除</a>
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
        </div>
    </div>
    </form>
</body>
</html>
