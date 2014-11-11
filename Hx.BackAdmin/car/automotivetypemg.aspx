<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="automotivetypemg.aspx.cs"
    Inherits="Hx.BackAdmin.car.automotivetypemg" %>

<%@ Register Assembly="AspNetPager" Namespace="Wuqi.Webdiyer" TagPrefix="webdiyer" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>车型管理</title>
    <link href="../css/admin.css" rel="stylesheet" type="text/css" />
    <script src="../js/jquery-1.3.2.min.js" type="text/javascript"></script>
    <script language="javascript" type="text/javascript">
        $(function () {
            $(".viewautomotivetype").click(function () {
                $("#showhideautomotivetype").show();
                var selids = $(this).attr("data").split("|");
                $(".hdnautomotivetype").each(function () {
                    for (var i = 0; i < selids.length; i++) {
                        if ($(this).val() == selids[i]) {
                            $(this).parent().show();
                            break;
                        }
                    }
                });

                var left = $(this).offset().left + 40;
                var top = $(this).offset().top;
                var wheight = $(window).height();
                var height = $("#showhideautomotivetype").height();
                if (height > wheight)
                    top = 0;
                $("#showhideautomotivetype").css({ left: left, top: top });


            }).mouseleave(function () {
                $("#showhideautomotivetype").hide();
                $(".hdnautomotivetype").each(function () {
                    $(this).parent().hide();
                });
            });
        });
    </script>
</head>
<body>
    <form id="form1" runat="server">
    <div class="ht_main">
        <table width="400" border="0" cellspacing="0" cellpadding="0" class="biaoge2" id="tblbank">
            <asp:Repeater ID="rptCarbrand" runat="server">
                <HeaderTemplate>
                    <tr class="bgbt">
                        <td class="w240">
                            车辆品牌
                        </td>
                        <td class="w60">
                            有效车型
                        </td>
                        <td>
                            操作
                        </td>
                    </tr>
                </HeaderTemplate>
                <ItemTemplate>
                    <tr>
                        <td>
                            <%# Eval("Name")%>
                        </td>
                        <td>
                            <a href="javascript:void(0);" data="<%# GetAutomotivetype(Eval("Name").ToString())%>"
                                class="viewautomotivetype" style="padding: 0 10px;">查看</a>
                        </td>
                        <td>
                            <a href="automotivetypeedit.aspx?id=<%#Eval("ID") %>&from=<%=CurrentUrl %>">
                                编辑</a>
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
        <div id="showhideautomotivetype" style="display: none; position: absolute; width: 750px;
            background: #fff; border: 3px solid #ccc;">
            <div>
                <ul style="width: 700px; margin-left: 20px; padding: 4px 0 4px 8px; line-height: 26px;
                    clear: both;">
                    <asp:Repeater runat="server" ID="rptAutomotivetype">
                        <ItemTemplate>
                            <li style="width: 320px; margin-left: 20px; float: left; display: none;">
                                <input type="hidden" value="<%# Eval("id") %>" class="hdnautomotivetype" />
                                <%# Eval("cCxmc")%>
                            </li>
                        </ItemTemplate>
                    </asp:Repeater>
                </ul>
            </div>
        </div>
    </div>
    </form>
</body>
</html>
