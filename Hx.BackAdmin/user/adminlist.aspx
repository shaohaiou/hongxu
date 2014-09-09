<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="adminlist.aspx.cs" Inherits="Hx.BackAdmin.user.adminlist" %>

<%@ Register Assembly="AspNetPager" Namespace="Wuqi.Webdiyer" TagPrefix="webdiyer" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>管理员管理</title>
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
        <table width="790" border="0" cellspacing="0" cellpadding="0" class="biaoge2">
            <asp:Repeater ID="rpadmin" runat="server">
                <HeaderTemplate>
                    <tr class="bgbt">
                        <td class="w100">
                            用户名
                        </td>
                        <td class="w230">
                            所属分店
                        </td>
                        <td class="w120">
                            最后登录时间
                        </td>
                        <td class="w120">
                            最后登录IP
                        </td>
                        <td class="w80">
                            是否管理员
                        </td>
                        <td>
                            操作
                        </td>
                    </tr>
                </HeaderTemplate>
                <ItemTemplate>
                    <tr>
                        <td>
                            <%# Eval("UserName")%>
                        </td>
                        <td>
                            <%# GetCorporationName(Eval("Corporation").ToString())%>
                        </td>
                        <td>
                            <%# Eval("LastLoginTime","{0:yyyy-MM-dd HH:mm}")%>
                        </td>
                        <td>
                            <%# Eval("LastLoginIP")%>
                        </td>
                        <td>
                            <%# (bool)Eval("Administrator") ? "是" : "否"%>
                        </td>
                        <td class="lan5x">
                            <a class="btndel" href="adminlist.aspx?id=<%#Eval("ID") %>&action=del&from=<%=UrlEncode(CurrentUrl) %>">
                                删除</a><a href="admins.aspx?id=<%#Eval("ID") %>&action=update&from=<%=UrlEncode(CurrentUrl) %>">编辑</a>
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
        <div style="text-align: center; width: 700px;" class="lan5x">
            <a href="admins.aspx?from=<%=UrlEncode(CurrentUrl) %>">添加管理员</a></div>
    </div>
    </form>
</body>
</html>
