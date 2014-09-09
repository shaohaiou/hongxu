<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="corporationedit.aspx.cs"
    Inherits="Hx.BackAdmin.global.corporationedit" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>分店信息设置</title>
    <link href="../css/admin.css" rel="stylesheet" type="text/css" />
    <script src="../js/jquery-1.3.2.min.js" type="text/javascript"></script>
    <script language="javascript" type="text/javascript">
        $(function () {
            $("#cbxAll").click(function () {
                $(".cbxCarbrand").attr("checked", $(this).attr("checked"));
                $(".cbxSuball").attr("checked", $(this).attr("checked"));
                setcarbrandvalue();
            });
            $(".cbxCarbrand").click(function () {
                setcarbrandvalue();
            });

            $(".cbxSuball").click(function () {
                $(this).parent().parent().find(".cbxCarbrand").attr("checked", $(this).attr("checked"));
                setcarbrandvalue();
            });
            $(".cbxSuball").each(function () {
                var suball = $(this);
                $(this).parent().parent().find(".cbxCarbrand").each(function () {
                    if (!$(this).attr("checked"))
                        suball.removeAttr("checked");
                });
            });
        });

        function setcarbrandvalue() {
            var carbrand = $(".cbxCarbrand:checked").map(function () {
                return $(this).val();
            }).get().join('|');
            if (carbrand != '')
                carbrand = '|' + carbrand + '|'
            $("#hdnCarbrand").val(carbrand);
        }
    </script>
    <style type="text/css">
    .clpp ul{display:inline-block;width:160px;*display:inline;*zoom:1;vertical-align:top;margin-left:3px;}
    .nh{color:White;font-weight:bold;font-size:18px;background:#222;text-indent:5px;}
    </style>
</head>
<body>
    <form id="form1" runat="server">
    <div class="ht_main">
        <table width="100%" border="0" cellspacing="0" cellpadding="0" class="biaoge3">
            <caption class="bt2">
                分店信息设置</caption>
            <tbody>
                <tr>
                    <td class="bg1">
                        分店名称：
                    </td>
                    <td>
                        <asp:TextBox runat="server" ID="txtName" CssClass="srk1"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td class="bg1">
                        默认银行：
                    </td>
                    <td>
                        <asp:DropDownList ID="ddlBank" runat="server">
                        </asp:DropDownList>
                    </td>
                </tr>
                <tr>
                    <td class="bg1">
                        默认利率：
                    </td>
                    <td>
                        <asp:DropDownList runat="server" ID="ddlBankProfitMargin">
                        </asp:DropDownList>
                    </td>
                </tr>
                <tr>
                    <td class="vt tr">
                        相关车辆品牌：
                    </td>
                    <td>
                        <label class="block">
                            <input type="checkbox" id="cbxAll" class="fll"/>全选</label>
                    </td>
                </tr>
                <tr>
                    <td></td>
                    <td class="clpp">
                        <asp:Repeater runat="server" ID="rptCarbrand">
                            <ItemTemplate>
                                <%# GetNewCharStr(Eval("NameIndex").ToString())%>
                                <li>
                                    <label class="blockinline" style="line-height: 18px;">
                                        <input type="checkbox" class="cbxCarbrand fll" value="<%# Eval("Name") %>" <%#SetCarbrand(Eval("Name").ToString()) %> /><%# Eval("Name") %></label></li>
                            </ItemTemplate>
                            <FooterTemplate>
                                </ul>
                            </FooterTemplate>
                        </asp:Repeater>
                    </td>
                </tr>
                <tr>
                    <td>
                    </td>
                    <td>
                        <input type="hidden" runat="server" id="hdnCarbrand" />
                        <asp:Button runat="server" ID="btnSubmit" Text="保存" CssClass="an1" OnClick="btnSubmit_Click" />
                    </td>
                </tr>
            </tbody>
        </table>
    </div>
    </form>
</body>
</html>
