﻿<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="sybxmg.aspx.cs" Inherits="Hx.BackAdmin.global.sybxmg" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>保险公司管理</title>
    <link href=<%=ResourceServer%>/css/admin.css rel="stylesheet" type="text/css" />
    <script src=<%=ResourceServer%>/js/jquery-1.3.2.min.js type="text/javascript"></script>
    <script language="javascript" type="text/javascript">
        $(function () {
            $(".btnDel").click(function () {
                DelRow(this);
            });

            $("#btnAdd").click(function () {
                $("#hdnAddCount").val(parseInt($("#hdnAddCount").val()) + 1);
                $("#tblsybx").append("<tr><td></td><td><input type=\"text\" id=\"txtName" + $("#hdnAddCount").val() + "\" name=\"txtName" + $("#hdnAddCount").val() + "\" class=\"srk1 w120\" value=\"\" /></td><td><a href=\"javascript:void(0);\" class=\"btnDel\" val=\"0\">删除</a> </td></tr>");
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
    <div class="ht_main">
        <ul class="xnav">
            <li class="current"><a href="sybxmg.aspx">商业保险</a></li>
            <li><a href="bankingmg.aspx">金融方案</a></li>
            <li><a href="choicestgoodsmg.aspx">汽车用品</a></li>
            <li><a href="giftmg.aspx">礼品</a></li>
            <li><a href="bankmg.aspx">银行</a></li>
            <li><a href="corporationmg.aspx">公司管理</a></li>
        </ul>
        <table width="300" border="0" cellspacing="0" cellpadding="0" class="biaoge2" id="tblsybx">
            <asp:Repeater ID="rptsybx" runat="server">
                <HeaderTemplate>
                    <tr class="bgbt">
                        <td class="bg21">
                            ID
                        </td>
                        <td>
                            保险公司
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
                            <input type="text" runat="server" id="txtName" class="srk1 w120" value='<%#Eval("Name") %>' />
                        </td>
                        <td>
                            <a href="sybxdetail.aspx?id=<%#Eval("ID") %>&from=<%= CurrentUrl%>" class="pl10">详细设置</a>
                            <a href="javascript:void(0);" class="btnDel pl10" val="<%#Eval("ID") %>">删除</a>
                        </td>
                    </tr>
                </ItemTemplate>
            </asp:Repeater>
        </table>
        <div class="lan5x" style="padding-top: 10px;">
            <input type="hidden" runat="server" id="hdnAddCount" value="0" />
            <input type="hidden" runat="server" id="hdnDelIds" value="" />
            <input class="an1" type="button" value="添加" id="btnAdd" />
            <asp:Button ID="btnSubmit" OnClick="btnSubmit_Click" CssClass="an1" runat="server"
                Text="保存" />
        </div>
    </div>
    </form>
</body>
</html>
