<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="cardpullrecordlist.aspx.cs"
    Inherits="Hx.BackAdmin.weixin.cardpullrecordlist" %>

<%@ Register Assembly="AspNetPager" Namespace="Wuqi.Webdiyer" TagPrefix="webdiyer" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>抽奖记录</title>
    <link href=<%=ResourceServer%>/css/admin.css rel="stylesheet" type="text/css" />
    <script src=<%=ResourceServer%>/js/jquery-1.3.2.min.js type="text/javascript"></script>
    <script language="javascript" type="text/javascript">
        $(function () {

            $("#btnClear").click(function () {
                return confirm("确定要清空抽奖记录吗？");
            });
            $(".btndel").click(function () {
                return confirm("确定要删除此记录吗？");
            });
        });
    </script>
</head>
<body>
    <form id="form1" runat="server">
    <div class="ht_main">
        <ul class="xnav">
            <%if (Hx.Components.Web.HXContext.Current.AdminUser.Administrator)
              { %><li><a href="cardsettinglist.aspx">卡券活动管理</a></li><%} %>
            <asp:Repeater ID="rpcg" runat="server">
                <ItemTemplate>
                    <li <%# SetCardSettingStatus(Eval("ID").ToString()) %> <%#Eval("ID").ToString() == GetString("sid") ? "class=\"current\"" : string.Empty %>>
                        <a href="cardmg.aspx?sid=<%#Eval("ID")%>">
                            <%#Eval("Name")%></a></li></ItemTemplate>
            </asp:Repeater>
        </ul>
        <div class="flqh">
            <span><a href="cardmg.aspx?sid=<%= GetInt("sid")%>">活动设置</a></span> 
            <span><a href="cardlist.aspx?sid=<%= GetInt("sid")%>">卡券管理</a></span> 
            <span class="dj"><a href="cardpullrecordlist.aspx?sid=<%= GetInt("sid")%>">抽奖记录</a></span>
        </div>
        <table width="500" border="0" cellspacing="0" cellpadding="0" class="biaoge4" style="background-color: #f4f8fc;">
            <tr>
                <td>
                    <asp:Button runat="server" ID="btnClear" CssClass="an1" Text="清空抽奖记录" OnClick="btnClear_Click" />
                </td>
            </tr>
        </table>
        <table border="0" cellspacing="0" cellpadding="0" class="biaoge2">
            <asp:Repeater ID="rptdata" runat="server">
                <HeaderTemplate>
                    <tr class="bgbt">
                        <td class="w80 tc">
                            用户
                        </td>
                        <td class="w80 tc">
                            抽奖结果
                        </td>
                        <td class="w200 tc">
                            奖品
                        </td>
                        <td class="w120 tc">
                            抽奖时间
                        </td>
                        <td class="w80 tc">
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
                            <%#Eval("PullResult").ToString() == "0" ? "未中奖" : (Eval("PullResult").ToString() == "1" ? "未领奖" : "已领奖")%>
                        </td>
                        <td>
                            <%#Eval("Cardtitle")%>
                        </td>
                        <td>
                            <%#Eval("AddTime","{0:yyyy-MM-dd HH:mm:ss}")%>
                        </td>
                        <td>
                            <a class="btndel" href="?sid=<%=GetInt("sid") %>&id=<%#Eval("ID") %>&action=del&from=<%=CurrentUrl %>">
                                删除</a>
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
