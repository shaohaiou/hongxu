<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="weixinactmg.aspx.cs" Inherits="Hx.BackAdmin.weixin.weixinactmg" %>

<%@ Register Assembly="AspNetPager" Namespace="Wuqi.Webdiyer" TagPrefix="webdiyer" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>参与用户</title>
    <link href=<%=ResourceServer%>/css/admin.css rel="stylesheet" type="text/css" />
    <script src=<%=ResourceServer%>/js/jquery-1.3.2.min.js type="text/javascript"></script>
</head>
<body>
    <form id="form1" runat="server">
    <div class="ht_main">
        <table width="360" border="0" cellspacing="0" cellpadding="0" class="biaoge4" style="background-color: #f4f8fc;">
            <tr>
                <td class="w40 bold" rowspan="2">
                    查询：
                </td>
                <td>
                    姓名：<asp:TextBox runat="server" ID="txtNickname" CssClass="srk6 mr10"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td>
                    性别：<asp:DropDownList runat="server" ID="ddlSex">
                        <asp:ListItem Value="0" Text="-请选择-"></asp:ListItem>
                        <asp:ListItem Value="1" Text="男"></asp:ListItem>
                        <asp:ListItem Value="2" Text="女"></asp:ListItem>
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
        <table width="360" border="0" cellspacing="0" cellpadding="0" class="biaoge2">
            <asp:Repeater ID="rptweixinact" runat="server">
                <HeaderTemplate>
                </HeaderTemplate>
                <HeaderTemplate>
                    <tr class="bgbt">
                        <td class="w60">
                            姓名
                        </td>
                        <td class="w60">
                            性别
                        </td>
                        <td class="w60">
                            国家
                        </td>
                        <td class="w60">
                            省
                        </td>
                        <td class="w60">
                            市
                        </td>
                        <td class="w60">
                            集赞
                        </td>
                    </tr>
                </HeaderTemplate>
                <ItemTemplate>
                    <tr>
                        <td>
                            <%# Eval("Nickname")%>
                        </td>
                        <td>
                            <a style="color: Blue" href="?sex=<%# Eval("Sex")%>">
                                <%# Eval("Sex").ToString() == "0" ? "未知" : (Eval("Sex").ToString() == "1" ? "男" : "女")%></a>
                        </td>
                        <td>
                            <%# Eval("Country")%>
                        </td>
                        <td>
                            <%# Eval("Province")%>
                        </td>
                        <td>
                            <%# Eval("City")%>
                        </td>
                        <td>
                            <%# Eval("AtcValue")%>
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
    </div>
    </form>
</body>
</html>
