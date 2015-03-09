<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="main_s.aspx.cs" Inherits="Hx.BackAdmin.biz.main_s" %>
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
     <a href="jobedit.aspx" target="ztk" id="jobedit" runat="server" class="current">招聘管理</a>
     <a href="job.aspx" target="ztk" id="jobpreview" runat="server" >招聘预览</a>
     <a href="corpmien.aspx" target="ztk" id="corpmien" runat="server">集团荣誉</a>
     <a href="corpmiendetail.aspx" target="ztk" id="corpmiendetail" runat="server">荣誉预览</a>
</div>

<div class="r_sy" id="daohan">
	当前位置：招聘管理 &gt;&gt; <span id="daohan_sp">招聘管理 </span>
</div>
</body>
<script language="javascript" type="text/javascript">
    $(function () {
        $(".right_nav a").each(function () {
            $(this).click(function () {
                $(".r_sy").html("当前位置：招聘管理 &gt;&gt; " + $(this).html());
                $(".right_nav a").removeClass("current");
                $(this).addClass("current");
            });
        });
    });
</script>
</html>
