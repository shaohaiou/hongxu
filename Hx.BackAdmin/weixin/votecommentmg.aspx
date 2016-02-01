<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="votecommentmg.aspx.cs"
    Inherits="Hx.BackAdmin.weixin.votecommentmg" %>

<%@ Register Assembly="AspNetPager" Namespace="Wuqi.Webdiyer" TagPrefix="webdiyer" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>评论管理</title>
    <link href="../css/admin.css" rel="stylesheet" type="text/css" />
    <script src="../js/jquery-1.3.2.min.js" type="text/javascript"></script>
    <script language="javascript" type="text/javascript">
        $(function () {
            $(".btnDel").click(function () {
                return confirm("确定要删除此记录吗？");
            });

            $("#cbxAll").click(function () {
                $(".cbxSub").attr("checked", $(this).attr("checked"));
            });

            $("#btnCheck").click(function () {
                var ids = $(".cbxSub:checked").map(function () {
                    return $(this).val() + "|" + $(this).attr("aid");
                }).get().join(",");
                if (ids == "")
                    alert("请至少选择一条记录");
                else
                    location.href = '?action=check&sid=<%= GetInt("sid")%>&ids=' + ids + '&from=<%= CurrentUrl%>';
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
            <span><a href="votemg.aspx?sid=<%= GetInt("sid")%>">活动设置</a></span> <span><a href="votepothunterlist.aspx?sid=<%= GetInt("sid")%>">
                选手管理</a></span> <span><a href="voterecordlist.aspx?sid=<%= GetInt("sid")%>">投票记录</a></span><span
                    class="dj"><a href="votecommentmg.aspx?sid=<%= GetInt("sid")%>">评论管理</a></span><%if(Admin.Administrator){ %><span><a
                    href="voterecordcachelist.aspx?sid=<%= GetInt("sid")%>">投票队列</a></span><%} %>
        </div>
        <table border="0" cellspacing="0" cellpadding="0" class="biaoge4" style="background-color: #f4f8fc;">
            <tr>
                <td class="w40 bold">
                    查询：
                </td>
                <td>
                    选手姓名：<asp:DropDownList ID="ddlPothunterName" runat="server" CssClass="mr10">
                    </asp:DropDownList>
                    审核状态：<asp:DropDownList ID="ddlCheckStatus" runat="server">
                        <asp:ListItem Value="">-审核状态-</asp:ListItem>
                        <asp:ListItem Value="0">未审核</asp:ListItem>
                        <asp:ListItem Value="1">审核通过</asp:ListItem>
                    </asp:DropDownList>
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
        <table border="0" cellspacing="0" cellpadding="0" class="biaoge2">
            <asp:Repeater ID="rptdata" runat="server">
                <HeaderTemplate>
                    <tr class="bgbt">
                        <td class="w60">
                            <label>
                                <input type="checkbox" id="cbxAll" class="fll" />选择</label>
                        </td>
                        <td class="w160 tc">
                            选手姓名
                        </td>
                        <td class="w80 tc">
                            评论人
                        </td>
                        <td class="w120 tc">
                            评论时间
                        </td>
                        <td class="w300 tc">
                            评论内容
                        </td>
                        <td class="w80 tc">
                            审核状态
                        </td>
                        <td class="w160">
                            操作
                        </td>
                    </tr>
                </HeaderTemplate>
                <ItemTemplate>
                    <tr>
                        <td>
                            <input type="checkbox" class="cbxSub" value="<%#Eval("ID") %>" aid="<%#Eval("AthleteID") %>" />
                        </td>
                        <td>
                            <%# GetPothunterName(Eval("AthleteID").ToString())%>
                        </td>
                        <td>
                            <%#Eval("Commenter")%>
                        </td>
                        <td>
                            <%#Eval("AddTime","{0:M月d日 HH:mm:ss}") %>
                        </td>
                        <td>
                            <%#Eval("Comment")%>
                        </td>
                        <td>
                            <%#Eval("CheckStatus").ToString() == "0" ? "<span class=\"red\">未审核</span>" : "<span class=\"green\">审核通过</span>"%>
                        </td>
                        <td>
                            <a href="?action=check&sid=<%=GetInt("sid") %>&ids=<%#Eval("ID") %>|<%#Eval("AthleteID") %>&from=<%= CurrentUrl%>" class="pl10">
                                审核通过</a> <a href="?action=del&sid=<%= GetInt("sid")%>&id=<%#Eval("ID") %>&aid=<%#Eval("AthleteID") %>&from=<%= CurrentUrl%>"
                                    class="btnDel pl10">删除</a>
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
            <input type="button" id="btnCheck" class="an1" value="审核通过" />
        </div>
    </div>
    </form>
</body>
</html>
