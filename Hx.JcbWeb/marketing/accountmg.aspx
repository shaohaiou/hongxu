<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="accountmg.aspx.cs" Inherits="Hx.JcbWeb.marketing.accountmg" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>营销网站帐号管理</title>
    <link href="../css/admin.css" rel="stylesheet" type="text/css" />
    <script src="../js/jquery-1.3.2.min.js" type="text/javascript"></script>
    <script type="text/javascript" language="javascript">
        $(function () {
            $(".btnConfig").click(function () {
                window.external.AccountConfig($(this).attr("val"));
            });
        });
    </script>
</head>
<body>
    <form id="form1" runat="server">
    <div class="nav">
        <ul style="margin-left: 20px;">
            <li><a href="javascript:void(0);" class="splitr">营销网站车源管理</a></li>
            <li><a href="javascript:void(0);" class="splitr">营销记录</a></li>
            <li class="current"><a href="javascript:void(0);">营销网站帐号管理</a></li>
        </ul>
    </div>
    <table width="100%" class="mt10" id="content">
        <tbody>
            <tr>
                <td class="bold pl10">
                    全国性二手车网站
                </td>
            </tr>
            <tr>
                <td>
                    <ul>
                        <asp:Repeater runat="server" ID="rptData" OnItemDataBound="rptData_ItemDataBound">
                            <ItemTemplate>
                                <li class="w200 pl10 blockinline">
                                    <img runat="server" id="imgStatus" src="../images/noconfig.png" class="fl" />
                                    <a href="javascript:void(0);" target="_blank" runat="server" id="btnLink" class="ml10 fl" style="*line-height: 16px;">
                                        <%# Eval("Text") %></a>
                                    <input type="button" runat="server" id="btnConfig" class="btnConfig fr" value="配置" /></li>
                            </ItemTemplate>
                        </asp:Repeater>
                    </ul>
                </td>
            </tr>
            <tr>
                <td class="bold pl10">
                    分类信息网站
                </td>
            </tr>
            <tr>
                <td>
                    <ul>
                        <asp:Repeater runat="server" ID="rptData1" OnItemDataBound="rptData_ItemDataBound">
                            <ItemTemplate>
                                    <li class="w200 pl10 blockinline">
                                        <img runat="server" id="imgStatus" src="../images/noconfig.png" class="fl" />
                                        <a href="javascript:void(0);" target="_blank" runat="server" id="btnLink" class="ml10 fl" style="*line-height: 16px;">
                                            <%# Eval("Text") %></a>
                                        <input type="button" runat="server" id="btnConfig" class="btnConfig fr" value="配置" /></li>
                            </ItemTemplate>
                        </asp:Repeater>
                    </ul>
                </td>
            </tr>
            <tr>
                <td>
                    &nbsp;
                </td>
            </tr>
        </tbody>
    </table>
    </form>
</body>
</html>
