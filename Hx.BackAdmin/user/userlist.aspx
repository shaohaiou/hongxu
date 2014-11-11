<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="userlist.aspx.cs" Inherits="Hx.BackAdmin.user.userlist" %>

<%@ Register Assembly="AspNetPager" Namespace="Wuqi.Webdiyer" TagPrefix="webdiyer" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>普通用户管理</title>
    <link href="../css/admin.css" rel="stylesheet" type="text/css" />
    <script src="../js/jquery-1.3.2.min.js" type="text/javascript"></script>
    <script type="text/javascript">
        $(function () {
            $(".btndel").click(function () {
                return confirm("确定要删除此用户吗？");
            });
        });
    </script>
</head>
<body>
    <form id="form1" runat="server">
    <div class="ht_main">
        <table width="690" border="0" cellspacing="0" cellpadding="0" class="biaoge4" style="background-color: #f4f8fc;">
            <tr>
                <td class="w40 bold">
                    查询：
                </td>
                <td>
                    公司：<asp:DropDownList ID="ddlCorporationFilter" runat="server" CssClass="mr10">
                    </asp:DropDownList>
                    用户名：<asp:TextBox ID="txtUserName" runat="server" CssClass="srk6"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td>
                </td>
                <td>
                    <asp:Button runat="server" ID="btnFilter" CssClass="an1" Text="确定" OnClick="btnFilter_Click" />
                </td>
            </tr>
        </table>
        <table width="690" border="0" cellspacing="0" cellpadding="0" class="biaoge2">
            <asp:Repeater ID="rpadmin" runat="server">
                <HeaderTemplate>
                    <tr class="bgbt">
                        <td class="w100">
                            用户名
                        </td>
                        <td class="w230">
                            所属公司
                        </td>
                        <td class="w120">
                            最后登录时间
                        </td>
                        <td class="w120">
                            最后登录IP
                        </td>
                        <td>
                            操作
                        </td>
                    </tr>
                </HeaderTemplate>
                <ItemTemplate>
                    <tr>
                        <td>
                            <%#Eval("UserName")%>
                        </td>
                        <td>
                            <%# GetCorporationName(Eval("Corporation").ToString())%>
                        </td>
                        <td>
                            <%#Eval("LastLoginTime", "{0:yyyy-MM-dd HH:mm}")%>
                        </td>
                        <td>
                            <%#Eval("LastLoginIP")%>
                        </td>
                        <td class="lan5x">
                            <a class="btndel" href="userlist.aspx?id=<%#Eval("ID") %>&action=del&from=<%=CurrentUrl %>">
                                删除</a><a href="useredit.aspx?id=<%#Eval("ID") %>&action=update&from=<%=CurrentUrl %>">编辑</a>
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
        <div style="text-align: center; width: 600px;" class="lan5x">
            <a href="useredit.aspx?from=<%=CurrentUrl %>">添加用户</a></div>
    </div>
    </form>
</body>
</html>
