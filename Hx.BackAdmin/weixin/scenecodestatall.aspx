<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="scenecodestatall.aspx.cs"
    Inherits="Hx.BackAdmin.weixin.scenecodestatall" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>数据统计</title>
    <link href=<%=ResourceServer%>/css/admin.css rel="stylesheet" type="text/css" />
</head>
<body>
    <form id="form1" runat="server">
    <div class="ht_main">
        <ul class="xnav">
            <%if (Hx.Components.Web.HXContext.Current.AdminUser.Administrator)
              { %><li><a href="scenecodesettinglist.aspx">场景二维码管理</a></li><%} %>
            <%if (Hx.Components.Web.HXContext.Current.AdminUser.Administrator || ((int)Hx.Components.Web.HXContext.Current.AdminUser.UserRole & (int)Hx.Components.Enumerations.UserRoleType.微信活动管理员) > 0)
              { %>
            <li class="current"><a href="scenecodestatall.aspx">数据统计</a></li>
            <%} %>
            <asp:Repeater ID="rpcg" runat="server">
                <ItemTemplate>
                    <li <%# SetScenecodeSettingStatus(Eval("ID").ToString()) %>>
                        <a href="scenecodemg.aspx?sid=<%#Eval("ID")%>">
                            <%#Eval("Name")%></a></li></ItemTemplate>
            </asp:Repeater>
        </ul>
        <table width="100%" border="0" cellspacing="0" cellpadding="0" class="biaoge3">
            <caption class="bt2">
                数据统计</caption>
            <tbody>
                <tr>
                    <td class="w120">
                        总数：<%= Count %>
                    </td>
                    <td>
                        <asp:Button runat="server" ID="btnExport" OnClick="btnExport_Click" Text="导出Excel" CssClass="an1" />
                    </td>
                </tr>
            </tbody>
        </table>  
        <%= TblStr %>
    </div>
    </form>
</body>
</html>
