<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="accountconfig.aspx.cs"
    Inherits="Hx.JcbWeb.marketing.accountconfig" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>营销网站帐号管理</title>
    <link href="../css/admin.css" rel="stylesheet" type="text/css" />
    <script src="../js/jquery-1.3.2.min.js" type="text/javascript"></script>
    <script language="javascript" type="text/javascript">
        $(function () {
            $(".btnDel").click(function () {
                DelRow(this);
            });

            $("#btnAdd").click(function () {
                $("#hdnAddCount").val(parseInt($("#hdnAddCount").val()) + 1);
                $("#dvData").append("<ul style=\"padding:10px 0;\">"
                 + "<li class=\"fl w200\"><input type=\"text\" id=\"txtAccountName" + $("#hdnAddCount").val() + "\" name=\"txtAccountName" + $("#hdnAddCount").val() + "\" class=\"srk1 w120\" /></li>"
                 + "<li class=\"fl w200\"><input type=\"password\" id=\"txtPassword" + $("#hdnAddCount").val() + "\" name=\"txtPassword" + $("#hdnAddCount").val() + "\" class=\"srk1 w120\" /></li>"
                 + "<li class=\"fl w200\"><select type=\"text\" id=\"ddlJcbAccountType" + $("#hdnAddCount").val() + "\" name=\"ddlJcbAccountType" + $("#hdnAddCount").val() + "\">" + $("#ddlJcbAccountType").html() + "</select></li>"
                 + "<li class=\"blockinline w120\"><a href=\"javascript:void(0);\" class=\"btnDel\" val=\"0\">删除</a> </li>"
                 + "</ul>");
                $(".btnDel").unbind("click").click(function () {
                    DelRow(this);
                });
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
    <div style="border: 1px solid #7c7c7c; padding: 10px; margin: 10px;max-width:750px;min-height:30px;" id="dvData">
        <asp:Repeater runat="server" ID="rptData" OnItemDataBound="rptData_ItemDataBound">
            <HeaderTemplate>
                <ul class="bold">
                    <li class="fl w200">帐号</li>
                    <li class="fl w200">密码</li>
                    <li class="fl w200">帐号类型</li>
                    <li class="w120 blockinline">操作</li>
                </ul>
            </HeaderTemplate>
            <ItemTemplate>
                <ul style="padding:10px 0;">
                    <li class="fl w200">
                        <input type="hidden" runat="server" id="hdnID" value='<%#Eval("ID") %>' />
                        <input type="text" runat="server" id="txtAccountName" class="srk1 w120" value='<%#Eval("AccountName") %>' /></li>
                    <li class="fl w200">
                        <input type="password" runat="server" id="txtPassword" class="srk1 w120" value='<%#Eval("Password") %>' /></li>
                    <li class="fl w200">
                        <asp:DropDownList runat="server" ID="ddlJcbAccountType">
                        </asp:DropDownList>
                    </li>
                    <li class="w120 blockinline"><a href="javascript:void(0);" class="btnDel pl10" val="<%#Eval("ID") %>">
                        删除</a></li>
                </ul>
            </ItemTemplate>
        </asp:Repeater>
    </div>
    <div class="lan5x pl10">
        <input type="hidden" runat="server" id="hdnAddCount" value="0" />
        <input type="hidden" runat="server" id="hdnDelIds" value="" />
        <input class="an1" type="button" value="添加" id="btnAdd" />
        <asp:Button ID="btnSubmit" OnClick="btnSubmit_Click" CssClass="an1" runat="server"
            Text="保存" />
    </div>
    <div class="hide">
        <asp:DropDownList runat="server" ID="ddlJcbAccountType">
        </asp:DropDownList>
    </div>
    </form>
</body>
</html>
