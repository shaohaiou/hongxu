﻿<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="cardlist.aspx.cs" Inherits="Hx.BackAdmin.weixin.cardlist" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>卡券管理</title>
    <link href="../css/admin.css" rel="stylesheet" type="text/css" />
    <script src="../js/jquery-1.3.2.min.js" type="text/javascript"></script>
    <script language="javascript" type="text/javascript">
        $(function () {
            $(".btnDel").click(function () {
                DelRow(this);
            });

            $("#btnAdd").click(function () {

                $("#hdnAddCount").val(parseInt($("#hdnAddCount").val()) + 1);
                $("#tblmodule").append("<tr><td><input type=\"text\" id=\"txtCardid" + $("#hdnAddCount").val() + "\" name=\"txtCardid" + $("#hdnAddCount").val() + "\" class=\"srk4 w240\" /></td>"
                 + "<td><input type=\"text\" id=\"txtCardtitle" + $("#hdnAddCount").val() + "\" name=\"txtCardtitle" + $("#hdnAddCount").val() + "\" class=\"srk1 w100\" value=\"\" /></td>"
                 + "<td><input type=\"text\" id=\"txtAward" + $("#hdnAddCount").val() + "\" name=\"txtAward" + $("#hdnAddCount").val() + "\" class=\"srk1 w80\" value=\"\" /></td>"
                 + "<td><input type=\"text\" id=\"txtNum" + $("#hdnAddCount").val() + "\" name=\"txtNum" + $("#hdnAddCount").val() + "\" class=\"srk1 w60\" value=\"\" /></td>"
                 + "<td><a href=\"javascript:void(0);\" class=\"btnDel\" val=\"0\">删除</a> </td></tr>");
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
            <li><a href="cardmg.aspx">活动设置</a></li>
            <li class="current"><a href="cardlist.aspx">卡券管理</a></li>
            <li><a href="cardpullrecordlist.aspx">抽奖记录</a></li>
        </ul>
        <table width="560" border="0" cellspacing="0" cellpadding="0" class="biaoge2" id="tblmodule">
            <asp:Repeater ID="rptData" runat="server" OnItemDataBound="rptData_ItemDataBound">
                <HeaderTemplate>
                </HeaderTemplate>
                <HeaderTemplate>
                    <tr class="bgbt">
                        <td class="w200">
                            卡券ID
                        </td>
                        <td class="w100">
                            卡券标题
                        </td>
                        <td class="w80">
                            奖项名称
                        </td>
                        <td class="w60">
                            库存
                        </td>
                        <td>
                            操作
                        </td>
                    </tr>
                </HeaderTemplate>
                <ItemTemplate>
                    <tr>
                        <td>
                            <asp:TextBox runat="server" ID="txtCardid" CssClass="srk4 w240"></asp:TextBox>
                            <input type="hidden" runat="server" id="hdnID" value='<%#Eval("ID") %>' />
                        </td>
                        <td>
                            <asp:TextBox runat="server" ID="txtCardtitle" CssClass="srk4 w100"></asp:TextBox>
                        </td>
                        <td>
                            <asp:TextBox runat="server" ID="txtAward" CssClass="srk4 w80"></asp:TextBox>
                        </td>
                        <td>
                            <asp:TextBox runat="server" ID="txtNum" CssClass="srk4 w60"></asp:TextBox>
                        </td>
                        <td>
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
