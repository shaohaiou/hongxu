<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="scenecodestat.aspx.cs"
    Inherits="Hx.BackAdmin.weixin.scenecodestat" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>场景二维码统计</title>
    <link href=<%=ResourceServer%>/css/admin.css rel="stylesheet" type="text/css" />
    <script src=<%=ResourceServer%>/js/jquery-1.3.2.min.js type="text/javascript"></script>
    <script src=<%=ResourceServer%>/js/jqchart.js type="text/javascript"></script>
    <script type="text/javascript">
        $(function () {
            var chartSetting = {
                config: {
                    title: "<%=CurrentSetting.Name %>",
                    titleTop:20,
                    titleLeft: 70,
                    labelX: [<%=LabelX %>],
                    scaleY: { min: 0, max: <%=Max %>, gap: <%=Gap %> },
                    width: <%=Width %>,
                    height: 400,
                    paddingL: 50,
                    paddingT: 50,
                    type: 'bar'
                },
                data: [
			[<%=Data %>]
		]

            };
            $('#MyCanvas').jQchart(chartSetting);
        });
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
                            <%#Eval("Name")%></a></li></ItemTemplate>
            </asp:Repeater>
        </ul>
        <div class="flqh">
            <%if (Hx.Components.Web.HXContext.Current.AdminUser.Administrator)
              { %>
            <span><a href="scenecodemg.aspx?sid=<%= GetInt("sid")%>">活动设置</a></span>
            <%} %>
            <span><a href="scenecodelist.aspx?sid=<%= GetInt("sid")%>">场景管理</a></span> <span
                class="dj"><a href="scenecodestat.aspx?sid=<%= GetInt("sid")%>">数据统计</a></span>
        </div>
        <table width="100%" border="0" cellspacing="0" cellpadding="0" class="biaoge3">
            <caption class="bt2">
                数据统计</caption>
            <tbody>
                <tr>
                    <td>
                        <canvas id="MyCanvas" style="border-right: #000 1px solid; border-top: #000 1px solid;
                            margin: 10px auto; border-left: #000 1px solid; border-bottom: #000 1px solid"></canvas>
                    </td>
                </tr>
            </tbody>
        </table>
    </div>
    </form>
</body>
</html>
