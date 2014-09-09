<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="top.aspx.cs" Inherits="Hx.BackAdmin.top" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
<meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
<title>页面头部</title>
<style type="text/css">
body{font-size:12px; margin:0; padding:0 8px 0 0; line-height:30px; text-align:right; color:#fff; background:#E7903F;}
a{color:#fff; text-decoration:none;}
a:hover{text-decoration:underline;}
</style>
</head>

<body>

您好，<%if (Admin.Administrator)
     {%>管理员<%} %> <asp:HyperLink ID="hyName" runat="server"></asp:HyperLink> <a href="logout.aspx" target="_parent">[退出]</a> <a href="user/changewd.aspx" target="mainFrame">[修改密码]</a> <a href="user/adminedit.aspx" target="mainFrame">[完善信息]</a> <a href="index.aspx" target="_parent">[返回首页]</a>

</body>
</html>
