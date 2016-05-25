<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="cxmg.aspx.cs" Inherits="Hx.BackAdmin.car.cxmg" %>

<%@ Register Assembly="AspNetPager" Namespace="Wuqi.Webdiyer" TagPrefix="webdiyer" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>车型管理</title>
    <link href=<%=ResourceServer%>/css/admin.css rel="stylesheet" type="text/css" />
</head>
<body>
    <form id="form1" runat="server">
    <div class="ht_main">
        <table width="400" border="0" cellspacing="0" cellpadding="0" class="biaoge2">
            <asp:Repeater ID="rptData" runat="server">
                <HeaderTemplate>
                    <tr class="bgbt">
                        <td class="w60">
                            首字母
                        </td>
                        <td class="w240">
                            品牌
                        </td>
                        <td>
                            操作
                        </td>
                    </tr>
                </HeaderTemplate>
                <ItemTemplate>
                    <tr>
                        <td>
                            <input type="text" runat="server" id="txtNameIndex" class="srk1 w60" value='<%#Eval("NameIndex") %>' />
                            <input type="hidden" runat="server" id="hdnID" value='<%#Eval("ID") %>' />
                        </td>
                        <td>
                            <%#Eval("Name") %>
                        </td>
                        <td>
                            <a href="cxedit.aspx?id=<%#Eval("ID") %>&from=<%=CurrentUrl %>">
                                管理</a>
                            <a href="carmodeledit.aspx?bid=<%#Eval("ID") %>">
                                新增车型</a>
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
            <asp:Button ID="btnSubmit" OnClick="btnSubmit_Click" CssClass="an1" runat="server"
                Text="保存" />
        </div>
    </div>
    </form>
</body>
</html>