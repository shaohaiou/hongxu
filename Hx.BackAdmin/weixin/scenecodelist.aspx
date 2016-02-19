<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="scenecodelist.aspx.cs"
    Inherits="Hx.BackAdmin.weixin.scenecodelist" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>场景管理</title>
    <link href=<%=ResourceServer%>/css/admin.css rel="stylesheet" type="text/css" />
    <script src=<%=ResourceServer%>/js/jquery-1.3.2.min.js type="text/javascript"></script>
    <script src=<%=ResourceServer%>/js/ajaxupload.js type="text/javascript"></script>
    <script language="javascript" type="text/javascript">
        $(function () {
            $(".btnDel").click(function () {
                DelRow(this);
            });

            $("#btnAdd").click(function () {

                $("#hdnAddCount").val(parseInt($("#hdnAddCount").val()) + 1);
                $("#tblData").append("<tr><td><input type=\"text\" id=\"txtSceneName" + $("#hdnAddCount").val() + "\" name=\"txtSceneName" + $("#hdnAddCount").val() + "\" class=\"srk4 w120\" /></td>"
                 + "<td></td>"
                 + "<td><input type=\"text\" id=\"txtRedirectAddress" + $("#hdnAddCount").val() + "\" name=\"txtRedirectAddress" + $("#hdnAddCount").val() + "\" class=\"srk4 w240\" /></td>"
                 + "<td></td>"
                 + "<td><a href=\"javascript:void(0);\" class=\"btnDel\" val=\"0\">删除</a> </td></tr>");
                $(".btnDel").unbind("click").click(function () {
                    DelRow(this);
                });
            });
        });

        function DelRow(obj) {
            if ($(obj).attr("val") > 0) {
                $("#hdnDelIds").val($("#hdnDelIds").val() + ($("#hdnDelIds").val() == "" ? "" : ",") + $(obj).attr("val"));
            }
            $(obj).parent().parent().remove();
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
                            <%#Eval("Name")%></a></li></ItemTemplate>
            </asp:Repeater>
        </ul>
        <div class="flqh">
            <%if (Hx.Components.Web.HXContext.Current.AdminUser.Administrator)
              { %>
            <span><a href="scenecodemg.aspx?sid=<%= GetInt("sid")%>">活动设置</a></span>
            <%} %>
            <span class="dj"><a href="scenecodelist.aspx?sid=<%= GetInt("sid")%>">场景管理</a></span>
            <span><a href="scenecodestat.aspx?sid=<%= GetInt("sid")%>">数据统计</a></span>
        </div>
        <table border="0" cellspacing="0" cellpadding="0" class="biaoge2" id="tblData">
            <asp:Repeater ID="rptData" runat="server" OnItemDataBound="rptData_ItemDataBound">
                <HeaderTemplate>
                </HeaderTemplate>
                <HeaderTemplate>
                    <tr class="bgbt">
                        <td class="w120">
                            场景名称
                        </td>
                        <td class="w300">
                            二维码地址
                        </td>
                        <td class="w240">
                            扫描跳转地址
                        </td>
                        <td class="w60">
                            扫描数量
                        </td>
                        <td>
                            操作
                        </td>
                    </tr>
                </HeaderTemplate>
                <ItemTemplate>
                    <tr>
                        <td>
                            <asp:TextBox runat="server" ID="txtSceneName" CssClass="srk4 w120"></asp:TextBox>
                            <input type="hidden" runat="server" id="hdnID" value='<%#Eval("ID") %>' />
                        </td>
                        <td>
                            <asp:TextBox runat="server" ID="txtCodeAddress" CssClass="srk4 w300"></asp:TextBox>
                        </td>
                        <td>
                            <asp:TextBox runat="server" ID="txtRedirectAddress" CssClass="srk4 w240"></asp:TextBox>
                        </td>
                        <td>
                            <%#Eval("ScanNum")%>
                        </td>
                        <td>
                            <a href="javascript:void(0);" class="btnDel pl10" val="<%#Eval("ID") %>">删除</a>
                        </td>
                    </tr>
                </ItemTemplate>
            </asp:Repeater>
        </table>
        <div class="lan5x" style="padding-top: 10px;">
            <input type="hidden" runat="server" id="hdnAddCount" value="0" />
            <input type="hidden" runat="server" id="hdnDelIds" value="" />
            <input class="an1" type="button" value="添加" id="btnAdd" />
            <asp:Button ID="btnSubmit" OnClick="btnSubmit_Click" CssClass="an1" runat="server"
                Text="保存" />
        </div>
    </div>
    </form>
</body>
</html>
