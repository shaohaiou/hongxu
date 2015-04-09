<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="accountsel.aspx.cs" Inherits="Hx.JcbWeb.inventory.accountsel" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>手动营销-帐号选择</title>
    <link href="../css/admin.css" rel="stylesheet" type="text/css" />
    <script src="../js/jquery-1.3.2.min.js" type="text/javascript"></script>
    <script type="text/javascript">
        $(function () {
            $("#btnSubmit").click(function () {
                if ($(".cbxAccount:checked").length == 0) {
                    alert("请选择一个帐号");
                } else {
                    var id = $(".cbxAccount:checked").val();
                    window.external.AutoLogin(id);
                }
            });
        })
    </script>
</head>
<body>
    <div style="width: 240px; margin: 20px auto;">
        <asp:Repeater runat="server" ID="rptData">
            <HeaderTemplate>
                <ul style="line-height: 30px; font-weight: bold;">
                    <li class="w160 fl tc" style="background-color: Orange; color: White;">帐号</li>
                    <li class="w80 fl" style="background-color: Orange; color: White;">帐号类型</li>
                </ul>
            </HeaderTemplate>
            <ItemTemplate>
                <ul class="lh20">
                    <li class="w160 fl">
                        <label style="line-height: 20px; display: block;">
                            <input type="radio" style="margin-top: 3px;" class="cbxAccount fl" name="cbxAccount"
                                value="<%#Eval("ID") %>" /><%#Eval("AccountName")%></label>
                    </li>
                    <li class="w80 fl">
                        <%# Eval("JcbAccountType").ToString()%>
                    </li>
                </ul>
            </ItemTemplate>
            <AlternatingItemTemplate>
                <ul class="lh20">
                    <li class="w160 fl">
                        <label style="line-height: 20px; display: block;">
                            <input type="radio" style="margin-top: 3px;" class="cbxAccount fl" name="cbxAccount"
                                value="<%#Eval("ID") %>" /><%#Eval("AccountName")%></label>
                    </li>
                    <li class="w80 fl">
                        <%# Eval("JcbAccountType").ToString()%>
                    </li>
                </ul>
            </AlternatingItemTemplate>
        </asp:Repeater>
        <div class="pl20">
            <input type="button" id="btnSubmit" value=" 确定 " class="an1 mt10" />
        </div>
    </div>
</body>
</html>
