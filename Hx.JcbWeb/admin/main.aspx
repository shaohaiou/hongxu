<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="main.aspx.cs" Inherits="Hx.JcbWeb.admin.main" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">

<head>
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>无标题文档</title>
    <link href="../css/admin.css" rel="stylesheet" type="text/css" />
</head>
<body style="background: url(images/xdd.gif) repeat-x;">
    <div class="right_nav">
        <span><a href="#">管理首页</a></span>
    </div>
    <div class="r_sy">
        当前位置：管理首页
    </div>
    <div class="ht_main">
        <div class="bt0">
            管理中心首页</div>
        <div class="bt2">
            您的资料</div>
        <div class="bt2x lan5">
            用户名：<%= UserName%></div>
        <div class="bt2">
            快捷访问</div>
        <div class="bt2x lan5">
        </div>
    </div>
</body>
</html>
