<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="useredit.aspx.cs" Inherits="Hx.BackAdmin.user.useredit" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>添加/编辑用户</title>
    <link href=<%=ResourceServer%>/css/admin.css rel="stylesheet" type="text/css" />
    <script src=<%=ResourceServer%>/js/jquery-1.3.2.min.js type="text/javascript"></script>
    <script type="text/javascript">
        var username = '';
        $(function () {
            username = $("#txtUserName").val();
            $("#cbIsAdmin").click(function () { setrole(); });
        });
        function ValidationName(source, arguments) {
            var v = false;
            if (username == $("#txtUserName").val()) {
                arguments.IsValid = true;
                return;
            }
            $.ajax({
                url: 'checkadmin.axd?d=' + new Date(),
                async: false,
                dataType: "json",
                data: { name: $("#txtUserName").val() },
                error: function (msg) {
                    alert("发生错误！");
                },
                success: function (data) {
                    if (data.result == 'success') {
                        v = true;
                    }
                    else {
                        v = false;
                    }
                }
            });
            arguments.IsValid = v;
        }
    </script>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <table width="100%" border="0" cellspacing="0" cellpadding="0" class="biaoge3">
            <caption class="bt2">
                添加/编辑用户</caption>
            <tbody>
                <tr>
                    <td class="bg1">
                        用户账户名：
                    </td>
                    <td class="bg2">
                        <asp:TextBox ID="txtUserName" runat="server"></asp:TextBox><asp:RequiredFieldValidator
                            ID="rfvname" runat="server" CssClass="red" ErrorMessage="账户名必须填写" ControlToValidate="txtUserName"
                            Display="Dynamic"></asp:RequiredFieldValidator>
                        <asp:CustomValidator ID="cvUserName" runat="server" ClientValidationFunction="ValidationName"
                            CssClass="red" ErrorMessage="该账户名已经被使用" Text="该账户名已经被使用" SetFocusOnError="True"
                            ControlToValidate="txtUserName" EnableClientScript="true" Display="Dynamic"></asp:CustomValidator>
                    </td>
                </tr>
                <tr>
                    <td class="bg1">
                        密码：
                    </td>
                    <td class="bg2">
                        <asp:TextBox ID="txtPassword" runat="server" TextMode="Password"></asp:TextBox><asp:RequiredFieldValidator
                            ID="rePassword" runat="server" ErrorMessage="密码必须填写" CssClass="red" ControlToValidate="txtPassword"></asp:RequiredFieldValidator>
                    </td>
                </tr>
                <tr>
                    <td class="bg1">
                        确认密码：
                    </td>
                    <td class="bg2">
                        <asp:TextBox ID="txtTruePassword" runat="server" TextMode="Password"></asp:TextBox><asp:RequiredFieldValidator
                            ID="reTruePassword" runat="server" CssClass="red" ErrorMessage="确认密码必须填写" ControlToValidate="txtTruePassword"></asp:RequiredFieldValidator>
                        <asp:CompareValidator runat="server" ID="cvPassword" ControlToCompare="txtPassword"
                            ControlToValidate="txtTruePassword" ErrorMessage="两次密码输入不一致" CssClass="red"></asp:CompareValidator>
                    </td>
                </tr>
                <tr>
                    <td class="bg1">
                        联系手机：
                    </td>
                    <td class="bg2">
                        <asp:TextBox ID="txtMobile" runat="server"></asp:TextBox>
                    </td>
                </tr>
                <tr id="trCorp" runat="server">
                    <td class="bg1">
                        所属分店：
                    </td>
                    <td class="bg2">
                        <asp:DropDownList ID="ddlCorporation" runat="server">
                        </asp:DropDownList>
                    </td>
                </tr>
                <tr>
                    <td colspan="2" style="text-align: center">
                        <div style="text-align: center">
                            <asp:CustomValidator ID="cvmess" Display="Dynamic" runat="server" ErrorMessage=""></asp:CustomValidator></div>
                        <asp:Button ID="btSave" runat="server" Text="保存" OnClick="btSave_Click" class="an1" />
                    </td>
                </tr>
            </tbody>
        </table>
        <asp:HiddenField ID="hdid" runat="server" />
        <asp:HiddenField ID="HfRid" runat="server" />
    </div>
    </form>
</body>
</html>
