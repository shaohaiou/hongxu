﻿<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="main_s.aspx.cs" Inherits="Hx.BackAdmin.weixin.main_s" %>
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

<body style="background:url(../images/xdd.gif) repeat-x;">
<div class="right_nav">
     <a href="weixinactmg.aspx" target="ztk" id="weixinactmg" runat="server" class="current">测试活动</a>
     <a href="benzvotemg.aspx" target="ztk" id="benzvotemg" runat="server" class="hide">奔驰投票</a>
     <a href="jituanvotemg.aspx" target="ztk" id="jituanvotemg" runat="server" class="hide">集团投票</a>
     <a href="escpgmg.aspx" target="ztk" id="escpgmg" runat="server">爱车估价器</a>
     <a href="cardsettinglist.aspx" target="ztk" id="cardmg" runat="server">卡券活动</a>
     <a href="votesettinglist.aspx" target="ztk" id="votemg" runat="server">投票活动</a>
     <a href="scenecodesettinglist.aspx" target="ztk" id="scenecodemg" runat="server">场景二维码</a>
     <a href="gb61list.aspx" target="ztk" id="gb61" runat="server">广本61活动</a>
</div>

<div class="r_sy" id="daohan">
	当前位置：微信活动 &gt;&gt; <span id="daohan_sp">测试活动 </span>
</div>
</body>
<script language="javascript" type="text/javascript">
    $(function () {
        $(".right_nav a").each(function () {
            $(this).click(function () {
                $(".r_sy").html("当前位置：微信活动 &gt;&gt; " + $(this).html());
                $(".right_nav a").removeClass("current");
                $(this).addClass("current");
            });
        });
    });
</script>
</html>
