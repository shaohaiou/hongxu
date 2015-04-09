<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="escpgmg.aspx.cs" Inherits="Hx.BackAdmin.weixin.escpgmg" %>

<%@ Register Assembly="AspNetPager" Namespace="Wuqi.Webdiyer" TagPrefix="webdiyer" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>客源管理</title>
    <link href="../css/admin.css" rel="stylesheet" type="text/css" />
</head>
<body>
    <form id="form1" runat="server">
    <div class="ht_main">
        <ul class="xnav">
            <li class="current"><a href="escpgmg.aspx">客源管理</a></li>
        </ul>
        <table border="0" cellspacing="0" cellpadding="0" class="biaoge2">
            <asp:Repeater ID="rptdata" runat="server">
                <HeaderTemplate>
                    <tr class="bgbt">
                        <td class="w120 tc">
                            手机号码
                        </td>
                        <td class="w100 tc">
                            品牌
                        </td>
                        <td class="w100 tc">
                            车系
                        </td>
                        <td class="w60 tc">
                            年份
                        </td>
                        <td class="w200 tc">
                            款式
                        </td>
                        <td class="w60 tc">
                            里程
                        </td>
                        <td class="w120 tc">
                            添加时间
                        </td>
                        <td class="w60 tc">
                            处理状态
                        </td>
                        <td>
                            操作
                        </td>
                    </tr>
                </HeaderTemplate>
                <ItemTemplate>
                    <tr>
                        <td>
                            <%#Eval("Phone")%>
                        </td>
                        <td>
                            <%#Eval("Brand")%>
                        </td>
                        <td>
                            <%#Eval("Chexi")%>
                        </td>
                        <td>
                            <%#Eval("Nianfen")%>
                        </td>
                        <td>
                            <%#Eval("Kuanshi")%>
                        </td>
                        <td>
                            <%#Eval("Licheng")%>
                        </td>
                        <td>
                            <%#Eval("AddTime")%>
                        </td>
                        <td>
                            <%# (bool)Eval("Restore") ? "<span class=\"green\">已处理</span>" : "<span class=\"red\">未处理</span>" %>
                        </td>
                        <td>
                            <a href="?id=<%#Eval("ID") %>&action=deel&from=<%= CurrentUrl%>" class="pl10 <%# (bool)Eval("Restore") ? "hide" : "" %>">处理</a>
                        </td>
                    </tr>
                </ItemTemplate>
            </asp:Repeater>
        </table>
        <webdiyer:AspNetPager ID="search_fy" UrlPaging="true" NextPageText="下一页" PrevPageText="上一页"
            CurrentPageButtonClass="current" PageSize="20" runat="server" NumericButtonType="Text"
            MoreButtonType="Text" ShowFirstLast="false" HorizontalAlign="Left" AlwaysShow="false"
            ShowDisabledButtons="False" PagingButtonSpacing="">
        </webdiyer:AspNetPager>
    </div>
    </form>
</body>
</html>

