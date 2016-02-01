<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="voterecordcachelist.aspx.cs" Inherits="Hx.BackAdmin.weixin.voterecordcachelist" %>


<%@ Register Assembly="AspNetPager" Namespace="Wuqi.Webdiyer" TagPrefix="webdiyer" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>投票记录</title>
    <link href="../css/admin.css" rel="stylesheet" type="text/css" />
    <script src="../js/jquery-1.3.2.min.js" type="text/javascript"></script>
    <script language="javascript" type="text/javascript">
        $(function () {
            $("#btnFilter").click(function () {
                var query = new Array();
                if ($.trim($("#txtAthleteName").val()) != "")
                    query[query.length] = "aname=" + $.trim($("#txtAthleteName").val());
                if ($.trim($("#txtSerialNumber").val()) != "-1")
                    query[query.length] = "snum=" + $.trim($("#txtSerialNumber").val());
                location = "?sid=" + <%=GetInt("sid") %> + "&" + (query.length > 0 ? $(query).map(function () {
                    return this;
                }).get().join("&") : "");

                return false;
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
                选手管理</a></span> <span><a href="voterecordlist.aspx?sid=<%= GetInt("sid")%>">
                    投票记录</a></span><span><a href="votecommentmg.aspx?sid=<%= GetInt("sid")%>">评论管理</a></span><%if(Admin.Administrator){ %><span class="dj"><a
                    href="voterecordcachelist.aspx?sid=<%= GetInt("sid")%>">投票队列</a></span><%} %>
        </div>
        <table border="0" cellspacing="0" cellpadding="0" class="biaoge2">
            <asp:Repeater ID="rptdata" runat="server">
                <HeaderTemplate>
                    <tr class="bgbt">
                        <td class="w120 tc">
                            选手姓名
                        </td>
                        <td class="w60 tc">
                            选手序号
                        </td>
                        <td class="w60 tc">
                            投票人
                        </td>
                        <td class="w40 tc">
                            性别
                        </td>
                        <td class="w160">
                            地区
                        </td>
                        <td class="w160 tc">
                            投票时间
                        </td>
                        <td class="w160 tc">
                            投票人标识
                        </td>
                    </tr>
                </HeaderTemplate>
                <ItemTemplate>
                    <tr>
                        <td>
                            <%#Eval("AthleteName") %>
                        </td>
                        <td>
                            <%#Eval("SerialNumber")%>
                        </td>
                        <td>
                            <%#Eval("Voter") %>
                        </td>
                        <td>
                            <%# Eval("Sex").ToString() == "0" ? "未知" : (Eval("Sex").ToString() == "1" ? "男" : "女")%>
                        </td>
                        <td>
                            <%# Eval("Country")%>
                            -
                            <%# Eval("Province")%>
                            -
                            <%# Eval("City")%>
                        </td>
                        <td>
                            <%#Eval("AddTime","{0:yyyy-MM-dd HH:mm:ss}") %>
                        </td>
                        <td>
                            <%#Eval("Openid")%>
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
