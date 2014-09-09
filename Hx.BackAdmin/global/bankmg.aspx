<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="bankmg.aspx.cs" Inherits="Hx.BackAdmin.global.bankmg" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>银行管理</title>
    <link href="../css/admin.css" rel="stylesheet" type="text/css" />
    <script src="../js/jquery-1.3.2.min.js" type="text/javascript"></script>
    <script language="javascript" type="text/javascript">
        $(function () {
            $(".btnDel").click(function () {
                DelRow(this);
            });

            $("#btnAdd").click(function () {
                $("#hdnAddCount").val(parseInt($("#hdnAddCount").val()) + 1);
                $("#tblbank").append("<tr><td></td>"
                 + "<td><input type=\"text\" id=\"txtName" + $("#hdnAddCount").val() + "\" name=\"txtName" + $("#hdnAddCount").val() + "\" class=\"srk1 w120\" value=\"\" /></td>"
                 + "<td><input type=\"text\" id=\"txtBankProfitMargin3y" + $("#hdnAddCount").val() + "\" name=\"txtBankProfitMargin3y" + $("#hdnAddCount").val() + "\" class=\"srk1 w60\" value=\"\" /></td>"
                 + "<td><input type=\"text\" id=\"txtBankProfitMargin2y" + $("#hdnAddCount").val() + "\" name=\"txtBankProfitMargin2y" + $("#hdnAddCount").val() + "\" class=\"srk1 w60\" value=\"\" /></td>"
                 + "<td><input type=\"text\" id=\"txtBankProfitMargin1y" + $("#hdnAddCount").val() + "\" name=\"txtBankProfitMargin1y" + $("#hdnAddCount").val() + "\" class=\"srk1 w60\" value=\"\" /></td>"
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
            <li><a href="sybxmg.aspx">商业保险</a></li>
            <li><a href="bankingmg.aspx">金融方案</a></li>
            <li><a href="choicestgoodsmg.aspx">汽车用品</a></li>
            <li><a href="giftmg.aspx">礼品</a></li>
            <li class="current"><a href="bankmg.aspx">银行</a></li>
            <li><a href="corporationmg.aspx">公司管理</a></li>
        </ul>
        <table width="600" border="0" cellspacing="0" cellpadding="0" class="biaoge2" id="tblbank">
            <asp:Repeater ID="rptbank" runat="server">
                <HeaderTemplate>
                    <tr class="bgbt">
                        <td class="bg21">
                            ID
                        </td>
                        <td>
                            银行名称
                        </td>
                        <td>
                            利率(3年)
                        </td>
                        <td>
                            利率(2年)
                        </td>
                        <td>
                            利率(1年)
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
                            <input type="text" runat="server" id="txtBankProfitMargin3y" class="srk1 w60" value='<%#Eval("BankProfitMargin3y")%>' />
                        </td>
                        <td>
                            <input type="text" runat="server" id="txtBankProfitMargin2y" class="srk1 w60" value='<%#Eval("BankProfitMargin2y")%>' />
                        </td>
                        <td>
                            <input type="text" runat="server" id="txtBankProfitMargin1y" class="srk1 w60" value='<%#Eval("BankProfitMargin1y")%>' />
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
