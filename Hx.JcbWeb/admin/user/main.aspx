<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="main.aspx.cs" Inherits="Hx.JcbWeb.admin.user.main" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head>
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>无标题文档</title>
</head>
<frameset rows="58,*" frameborder="no" border="0" framespacing="0">
  <frame src="main_s.aspx" name="sk" scrolling="No" noresize="noresize" id="sk" title="sk" />
  <frame <%if(Admin.Administrator) {%> src="adminlist.aspx"<%}else{ %> src="userlist.aspx"<%} %> name="ztk" id="ztk" title="ztk"/>
</frameset>
<noframes>
    <body>
    </body>
</noframes>
