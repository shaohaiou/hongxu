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
    <%if (Admin.UserRole == Hx.Components.Enumerations.UserRoleType.销售员)
      { %>
      <div style="height:40px;display:block;line-height:40px;margin-bottom:5px;background:#21363e; text-align:right;color:#fff;">
        您好，<asp:HyperLink ID="hyName" runat="server"  style="color:#fff;"></asp:HyperLink> <a href="/logout.aspx" target="_parent" style="color:#fff;">[退出]</a> <a href="/user/changewd.aspx" style="color:#fff;">[修改密码]</a> <a href="/user/adminedit.aspx" target="_blank" style="color:#fff;">[完善信息]</a>
      </div>
      <%} %>
    <div>
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