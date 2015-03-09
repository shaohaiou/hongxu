<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="left.aspx.cs" Inherits="Hx.BackAdmin.left" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
<meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
<title>红旭后台管理中心</title>
    <link href="css/admin.css" rel="stylesheet" type="text/css" />
    <script src="js/jquery-1.3.2.min.js" type="text/javascript"></script>
<style type="text/css">
body{background:url(images/ldi.gif) repeat-y;}
</style>
<script type="text/javascript">
    $(function () {
        $(".left_nav a").click(function () {
            $(".left_nav a").removeClass("current");
            $(this).addClass("current");
            return true;
        });
    });
</script>
</head>

<body style="text-align:center;">

<a href="index.aspx" target="_parent" style="display: inline-block;margin: 10px 0 0 30px;*display:inline;*zoom:1;"><img src="images/logo.png" /></a>

<div class="left_nav">
	<a href="main.aspx" target="mainFrame" class="current" runat="server" id="index_page">管理首页</a>
	<a href="car/main.aspx" target="mainFrame" runat="server" id="carquotationindex_page">车辆管理</a>
	<a href="global/main.aspx" target="mainFrame"  runat="server" id="globalindex_page">系统设置</a>
	<a href="user/main.aspx" target="mainFrame"  runat="server" id="userindex_page">用户管理</a>
	<a href="dayreport/main.aspx" target="mainFrame"  runat="server" id="dayreport_page">日报管理</a>
	<a href="weixin/main.aspx" target="mainFrame"  runat="server" id="weixin_page">微信活动</a>
	<a href="biz/main.aspx" target="mainFrame"  runat="server" id="zhaopin_page">oa行政</a>
</div>
</body>
</html>
