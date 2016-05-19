﻿<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="main_s.aspx.cs" Inherits="Hx.BackAdmin.user.main_s" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>无标题文档</title>
    <link href=<%=ResourceServer%>/css/admin.css rel="stylesheet" type="text/css" />
    <script src=<%=ResourceServer%>/js/jquery-1.3.2.min.js type="text/javascript"></script>
    <script type="text/javascript">
        function setdaohan(t, url) {
            $("#daohan_sp").html("<a href='" + url + "'>" + t + "</a>");
        }
    </script>
</head>
<body style="background: url(../images/xdd.gif) repeat-x;">
    <div class="right_nav">
        <a href="adminlist.aspx" target="ztk" id="adminlist" runat="server" class="current">管理员管理</a> <a
            href="userlist.aspx?r=1024" target="ztk" id="userlistzjl" runat="server">总经理</a><a
            href="userlist.aspx?r=1" target="ztk" id="userlistjl" runat="server">销售经理</a><a
            href="userlist.aspx?r=2" target="ztk" id="userlist1" runat="server">销售顾问</a><a
            href="userlist.aspx?r=512" target="ztk" id="userlist3" runat="server">财务收银</a>
    </div>
    <div class="r_sy" id="daohan">
        当前位置：用户管理 &gt;&gt; <span id="daohan_sp">
            <%if (Admin.Administrator)
              { %>管理员管理<%}
              else
              { %>销售顾问<%} %></span>
    </div>
</body>
<script language="javascript" type="text/javascript">
    $(function () {
        $(".right_nav a").each(function () {
            $(this).click(function () {
                $(".r_sy").html("当前位置：用户管理 &gt;&gt; " + $(this).html());
                $(".right_nav a").removeClass("current");
                $(this).addClass("current");
            });
        });
    });
</script>
</html>
