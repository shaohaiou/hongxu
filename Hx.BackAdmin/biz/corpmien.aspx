<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="corpmien.aspx.cs" Inherits="Hx.BackAdmin.biz.corpmien" %>

<%@ Register Assembly="AspNetPager" Namespace="Wuqi.Webdiyer" TagPrefix="webdiyer" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>集团荣誉</title>
    <link href=<%=ResourceServer%>/css/admin.css rel="stylesheet" type="text/css" />
    <script src=<%=ResourceServer%>/js/jquery-1.3.2.min.js type="text/javascript"></script>
    <script type="text/javascript">
        $(function () {
            $(".btndel").click(function () {
                return confirm("确定要删除该记录吗？");
            });

            $(".btnmoveprev").click(function () {
                var orderindex = parseInt($(this).attr("orderindex"));
                if (orderindex == 1) return false;
            });
            $(".btnmovenext").click(function () {
                var orderindex = parseInt($(this).attr("orderindex"));
                if (orderindex == <%= RecordCount %>) return false;
            });
        });
    </script>
</head>
<body>
    <form id="form1" runat="server">
    <div class="ht_main">
        <div class="lan5x">
            <a href="corpmienedit.aspx?from=<%=CurrentUrl %>" class="an4">新 增</a></div>
        <table width="280" border="0" cellspacing="0" cellpadding="0" class="biaoge2">
            <asp:Repeater ID="rptData" runat="server">
                <HeaderTemplate>
                    <tr class="bgbt">
                        <td>
                            荣誉展示
                        </td>
                        <td class="w160">
                            操作
                        </td>
                    </tr>
                </HeaderTemplate>
                <ItemTemplate>
                    <tr>
                        <td>
                            <a href="corpmienview.aspx?id=<%#Eval("ID") %>" title="<%#Eval("Introduce") %>" target="_blank"
                                style="display: block; position: relative; padding-top: 100px; width: 120px;
                                text-align: center;">
                                <img src="<%= ImgServer%><%#Eval("Pic")%>" alt="" style="width: 100px; height: 100px; position: absolute;
                                    left: 10px; top: 0;" />
                                <%#Hx.Tools.StrHelper.GetFuzzyChar(Eval("Introduce").ToString(),8) %></a>
                        </td>
                        <td class="lan5x">
                            <a class="btndel" href="corpmien.aspx?id=<%#Eval("ID") %>&action=del&from=<%=CurrentUrl %>">
                                删除</a><a href="corpmienedit.aspx?id=<%#Eval("ID") %>&from=<%=CurrentUrl %>">编辑</a>
                            <a class="btnmoveprev <%# int.Parse(Eval("OrderIndex").ToString()) ==1 ? "gray" : "" %>" orderindex="<%#Eval("OrderIndex") %>" href="corpmien.aspx?id=<%#Eval("ID") %>&toindex=<%# int.Parse(Eval("OrderIndex").ToString()) - 1 %>&action=move&from=<%=CurrentUrl %>">
                                上移</a> <a class="btnmovenext <%# int.Parse(Eval("OrderIndex").ToString()) ==RecordCount ? "gray" : "" %>" orderindex="<%#Eval("OrderIndex") %>" href="corpmien.aspx?id=<%#Eval("ID") %>&toindex=<%# int.Parse(Eval("OrderIndex").ToString()) + 1 %>&action=move&from=<%=CurrentUrl %>">
                                    下移</a>
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
        <div class="lan5x">
            <a href="corpmienedit.aspx?from=<%=CurrentUrl %>" class="an4">新 增</a></div>
    </div>
    </form>
</body>
</html>
