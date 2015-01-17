<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="yjyx.aspx.cs" Inherits="Hx.JcbWeb.inventory.yjyx" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>一键营销</title>
    <link href="../css/admin.css" rel="stylesheet" type="text/css" />
    <script src="../js/jquery-1.3.2.min.js" type="text/javascript"></script>
    <script type="text/javascript">
        $(function () {
            $("#cbxAll").click(function () {
                $(".cbxAccount").attr("checked", $(this).attr("checked"));
                setAccounts();
            });
            $(".cbxAccount").click(function () {
                setAccounts();
            });
            $(".ddlAccounts").change(function () {
                setAccounts();
            });
            $("#btnSubmit").click(function () {
                if ($(this).attr("accounts") == "") {
                    alert("请选择营销网站");
                }
            });

            setAccounts();
        })

        function setAccounts() {
            var accounts = $(".cbxAccount:checked").map(function () {
                return $(this).parent().parent().parent().find("select").val();
            }).get().join(",");
            $("#btnSubmit").attr("accounts",accounts);
        }
    </script>
</head>
<body>
    <form id="form1" runat="server">
    <div style="width: 338px; margin: 0 auto; padding: 20px 30px">
        <label style="*line-height: 20px;">
            <input type="checkbox" class="fl" id="cbxAll" checked="checked" />选择营销网站</label>
        <br />
        <span style="padding: 5px 0; display: block;">被选择的网站</span>
        <table>
            <tbody>
                <asp:Repeater runat="server" ID="rptData" OnItemDataBound="rptData_ItemDataBound">
                    <ItemTemplate>
                        <tr>
                            <td class="w120 vb">
                                <label style="*line-height: 20px;">
                                    <input type="checkbox" class="fl cbxAccount" runat="server" id="cbxAccount" checked="checked" /><%#Eval("Text") %></label>
                            </td>
                            <td class="vb">
                                帐号：<asp:DropDownList runat="server" ID="ddlAccounts" class="ddlAccounts">
                                </asp:DropDownList>
                                <asp:Label runat="server" ID="lblIsMarketing" CssClass="red"></asp:Label>
                            </td>
                        </tr>
                    </ItemTemplate>
                </asp:Repeater>
            </tbody>
        </table>
        <div class="tc psubmit">
            <asp:Button runat="server" ID="btnSubmit" Text="确定" CssClass="an1"/>
        </div>
    </div>
    </form>
</body>
</html>
