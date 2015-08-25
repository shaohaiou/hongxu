<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="scenecodemg.aspx.cs" Inherits="Hx.BackAdmin.weixin.scenecodemg" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>活动设置</title>
    <link href="../css/admin.css" rel="stylesheet" type="text/css" />
    <script src="../js/jquery-1.9.1.js" type="text/javascript"></script>
    <script type="text/javascript">
        $(function () {
            $("#cbxAllPowerUser").click(function () {
                $(".cbxPowerUser").attr("checked", $(this).attr("checked"));
                setpoweruser();
            })
            $(".cbxPowerUser").click(function () {
                setpoweruser();
            });
        })

        function setpoweruser() {
            var poweruser = $(".cbxPowerUser:checked").map(function () {
                return $(this).val();
            }).get().join('|');
            if (poweruser != '')
                poweruser = '|' + poweruser + '|'
            $("#hdnPowerUser").val(poweruser);
        }
    </script>
</head>
<body>
    <form id="form1" runat="server">
    <div class="ht_main">
        <ul class="xnav">
            <%if (Hx.Components.Web.HXContext.Current.AdminUser.Administrator)
              { %><li><a href="scenecodesettinglist.aspx">场景二维码管理</a></li><%} %>
            <%if (Hx.Components.Web.HXContext.Current.AdminUser.Administrator || ((int)Hx.Components.Web.HXContext.Current.AdminUser.UserRole & (int)Hx.Components.Enumerations.UserRoleType.微信活动管理员) > 0)
              { %>
            <li><a href="scenecodestatall.aspx">数据统计</a></li>
            <%} %>
            <asp:Repeater ID="rpcg" runat="server">
                <ItemTemplate>
                    <li <%# SetScenecodeSettingStatus(Eval("ID").ToString()) %> <%#Eval("ID").ToString() == GetString("sid") ? "class=\"current\"" : string.Empty %>>
                        <a href="scenecodemg.aspx?sid=<%#Eval("ID")%>">
                            <%#Eval("Name").ToString()%></a></li></ItemTemplate>
            </asp:Repeater>
        </ul>
        <div class="flqh">
            <span class="dj"><a href="scenecodemg.aspx?sid=<%= GetInt("sid")%>">活动设置</a></span> <span>
                <a href="scenecodelist.aspx?sid=<%= GetInt("sid")%>">场景管理</a></span> <span><a href="scenecodestat.aspx?sid=<%= GetInt("sid")%>">
                    数据统计</a></span>
        </div>
        <table width="100%" border="0" cellspacing="0" cellpadding="0" class="biaoge3">
            <caption class="bt2">
                活动设置</caption>
            <tbody>
                <%if (Hx.Components.Web.HXContext.Current.AdminUser.Administrator)
                  { %>
                <tr>
                    <td class="bg1">
                        管理员：
                    </td>
                    <td class="clpp">
                        <ul>
                            <li class="nh">
                                <label>
                                    <input type="checkbox" id="cbxAllPowerUser" class="fll" />全选</label></li>
                        </ul>
                    </td>
                </tr>
                <tr>
                    <td>
                    </td>
                    <td>
                        <asp:Repeater runat="server" ID="rptPowerUser">
                            <HeaderTemplate>
                                <ul>
                            </HeaderTemplate>
                            <ItemTemplate>
                                <li class="blockinline" style="width: 230px;">
                                    <label>
                                        <input type="checkbox" class="cbxPowerUser fll" value="<%# Eval("ID") %>" <%#SetPowerUser(Eval("ID").ToString()) %> /><%# Eval("UserName")%></label></li>
                            </ItemTemplate>
                            <FooterTemplate>
                                </ul></FooterTemplate>
                        </asp:Repeater>
                    </td>
                </tr>
                <%} %>
                <tr>
                    <td colspan="2" style="text-align: center">
                        <asp:Button ID="btSave" runat="server" Text="保存" OnClick="btSave_Click" class="an1" />
                    </td>
                </tr>
            </tbody>
        </table>
    </div>
    <input type="hidden" runat="server" id="hdnPowerUser" />
    <input type="hidden" runat="server" id="hdnName" />
    </form>
</body>
</html>
