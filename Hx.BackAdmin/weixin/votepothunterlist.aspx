<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="votepothunterlist.aspx.cs"
    Inherits="Hx.BackAdmin.weixin.votepothunterlist" %>

<%@ Register Assembly="AspNetPager" Namespace="Wuqi.Webdiyer" TagPrefix="webdiyer" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>选手管理</title>
    <link href="../css/admin.css" rel="stylesheet" type="text/css" />
    <script src="../js/jquery-1.3.2.min.js" type="text/javascript"></script>
    <script language="javascript" type="text/javascript">
        $(function () {
            $(".btnDel").click(function () {
                return confirm("确定要删除此记录吗？");
            });
        })
    </script>
</head>
<body>
    <form id="form1" runat="server">
    <div class="ht_main">
        <ul class="xnav">
            <%if (Hx.Components.Web.HXContext.Current.AdminUser.Administrator)
              { %><li><a href="votesettinglist.aspx">投票活动管理</a></li><%} %>
            <asp:Repeater ID="rpcg" runat="server">
                <ItemTemplate>
                    <li <%# SetVoteSettingStatus(Eval("ID").ToString()) %> <%#Eval("ID").ToString() == GetString("sid") ? "class=\"current\"" : string.Empty %>>
                        <a href="votemg.aspx?sid=<%#Eval("ID")%>">
                            <%#Eval("Name")%></a></li></ItemTemplate>
            </asp:Repeater>
        </ul>
        <div class="flqh">
            <span><a href="votemg.aspx?sid=<%= GetInt("sid")%>">活动设置</a></span> <span class="dj">
                <a href="votepothunterlist.aspx?sid=<%= GetInt("sid")%>">选手管理</a></span> <span><a
                    href="voterecordlist.aspx?sid=<%= GetInt("sid")%>">投票记录</a></span> <span><a href="votecommentmg.aspx?sid=<%= GetInt("sid")%>">
                        评论管理</a></span>
        </div>
        <table border="0" cellspacing="0" cellpadding="0" class="biaoge2">
            <asp:Repeater ID="rptdata" runat="server">
                <HeaderTemplate>
                    <tr class="bgbt">
                        <td class="w120 tc">
                            照片
                        </td>
                        <td class="w60 tc">
                            姓名
                        </td>
                        <td class="w40 tc">
                            序号
                        </td>
                        <td class="w60 tc">
                            票数
                        </td>
                        <td class="w40 tc">
                            排名
                        </td>
                        <td class="w300">
                            个人介绍
                        </td>
                        <td class="w300">
                            参赛宣言
                        </td>
                        <td>
                            操作
                        </td>
                    </tr>
                </HeaderTemplate>
                <ItemTemplate>
                    <tr>
                        <td>
                            <img src='<%# Eval("PicPath") %>' alt="<%#Eval("Name") %>" style="width: 110px; height: 110px;" />
                        </td>
                        <td>
                            <%#Eval("Name") %>
                        </td>
                        <td>
                            <%#Eval("SerialNumber") %>
                        </td>
                        <td>
                            <%#Eval("Ballot") %>
                        </td>
                        <td>
                            <%#Eval("Order") %>
                        </td>
                        <td>
                            <textarea rows="4" style="width: 300px; border: 0px; resize: none;" onfocus="this.blur();"
                                readonly><%# Eval("Introduce")%></textarea>
                        </td>
                        <td>
                            <textarea rows="4" style="width: 300px; border: 0px; resize: none;" onfocus="this.blur();"
                                readonly><%#Eval("Declare")%></textarea>
                        </td>
                        <td>
                            <a href="votepothunteredit.aspx?sid=<%=GetString("sid") %>&id=<%#Eval("ID") %>&from=<%= CurrentUrl%>"
                                class="pl10">编辑</a> <a href="?action=del&sid=<%= GetInt("sid")%>&id=<%#Eval("ID") %>&from=<%= CurrentUrl%>"
                                    class="btnDel pl10">删除</a><br /><br />
                            <a class="pl10" href="votecommentmg.aspx?sid=<%=GetString("sid") %>&aid=<%#Eval("ID") %>">管理评论</a>
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
            <a href="votepothunteredit.aspx?sid=<%=GetString("sid") %>&from=<%=CurrentUrl %>"
                class="an4">新 增</a>
        </div>
    </div>
    </form>
</body>
</html>
