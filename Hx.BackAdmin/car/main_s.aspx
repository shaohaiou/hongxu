<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="main_s.aspx.cs" Inherits="Hx.BackAdmin.car.main_s" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
<meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
<title>无标题文档</title>
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
     <a href="carcollect.aspx" target="ztk" id="carcollect" runat="server" class="current">信息采集</a>
     <a href="carquotation.aspx?t=0" target="_blank" id="carquotation" runat="server" class="nohover">车辆报价</a>
     <a href="automotivetypemg.aspx" target="ztk" id="automotivetypemg" runat="server">车型管理</a>
     <a href="carquotationmg.aspx" target="ztk" id="carquotationmg" runat="server">报价记录</a>
</div>

<div class="r_sy" id="daohan">
	当前位置：车辆管理 &gt;&gt; <span id="daohan_sp">信息采集</span>
</div>
</body>
<script language="javascript" type="text/javascript">
    $(function () {
        $(".right_nav a").not("#carquotation").each(function () {
            $(this).click(function () {
                $(".r_sy").html("当前位置：车辆管理 &gt;&gt; " + $(this).html());
                $(".right_nav a").removeClass("current");
                $(this).addClass("current");
            });
        });
    });
</script>
</html>
