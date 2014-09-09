﻿<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="main_s.aspx.cs" Inherits="Hx.BackAdmin.dayreport.main_s" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head>
<meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
<title>月报查询</title>
     <link href="../css/admin.css" rel="stylesheet" type="text/css" />
    <script src="../js/jquery-1.3.2.min.js" type="text/javascript"></script>
     <script type="text/javascript">
         function setdaohan(t, url) {
             $("#daohan_sp").html("<a href='" + url + "'>" + t + "</a>");
         }
     </script>
</head>

<body style="background:url(../images/xdd.gif) repeat-x;">
<div class="right_nav">
     <a href="dayreportusermg.aspx" target="ztk" id="usermg" runat="server" class="current">日报用户</a>
     <a href="dayreportmodulemg.aspx" target="ztk" id="modulemg" runat="server">日报栏目</a>
     <a href="dayreporthistorymg.aspx" target="ztk" id="historymg" runat="server">录入记录</a>
</div>

<div class="r_sy" id="daohan">
	当前位置：日报管理 &gt;&gt; <span id="daohan_sp">日报用户 </span>
</div>
</body>
<script language="javascript" type="text/javascript">
    $(function () {
        $(".right_nav a").each(function () {
            $(this).click(function () {
                $(".r_sy").html("当前位置：日报管理 &gt;&gt; " + $(this).html());
                $(".right_nav a").removeClass("current");
                $(this).addClass("current");
            });
        });
    });
</script>
</html>
