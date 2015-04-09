<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="changewd.aspx.cs" Inherits="Hx.JcbWeb.admin.user.changewd" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>修改密码</title>
    <link href="../../css/admin.css" rel="stylesheet" type="text/css" />
</head>
<body>
    <form id="form1" runat="server">
    <div style="padding-left:20px;padding-top:20px;">
        <ul>
            <li class="FormNav">
                <dl>
                    <dd>
                        <label>
                            &nbsp;&nbsp;旧密码:</label>
                        <asp:TextBox ID="TxtOldPassword" runat="server" TextMode="password"></asp:TextBox><asp:RequiredFieldValidator
                            ID="rfvUserName" ControlToValidate="TxtOldPassword" ErrorMessage="*" runat="server"></asp:RequiredFieldValidator></dt>
                        <dd>
                            <label>
                                &nbsp;&nbsp;新密码:</label>
                            <asp:TextBox ID="TxtUserPassword" runat="server" TextMode="password"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="ValrUserPassword" ControlToValidate="TxtUserPassword"
                                runat="server" ErrorMessage="*" Display="Dynamic"></asp:RequiredFieldValidator></dd>
                        <dd>
                            <label>
                                确认密码:</label>
                            <asp:TextBox ID="TxtNewUserPassword" runat="server" TextMode="password"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="rfvTxtCoe" ControlToValidate="TxtNewUserPassword"
                                runat="server" ErrorMessage="*" Display="Dynamic"></asp:RequiredFieldValidator><asp:CompareValidator
                                    ID="CValidator" ControlToValidate="TxtNewUserPassword" ControlToCompare="TxtUserPassword"
                                    runat="server" ErrorMessage="密码不一致"></asp:CompareValidator></dd>
                        <dd>
                            <asp:Button ID="btSave" runat="server" Text="保存" OnClick="btSave_Click" />
                        </dd>
                        <dd>
                            <asp:Literal ID="lerrorMes" runat="server"></asp:Literal></dd>
                </dl>
            </li>
        </ul>
    </div>
    </form>
</body>
</html>
