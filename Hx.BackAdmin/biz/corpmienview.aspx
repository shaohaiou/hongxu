<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="corpmienview.aspx.cs" Inherits="Hx.BackAdmin.biz.corpmienview" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>集团荣誉展示</title>
    <script src="../js/jquery-1.3.2.min.js" type="text/javascript"></script>
    <script language="javascript" type="text/javascript">
        $(function () {
            $("#Big_Slide_Last").css("top", ($(window).height() - $("#Big_Slide_Last").height() - 128) / 2);
            $("#Big_Slide_Next").css("top", ($(window).height() - $("#Big_Slide_Next").height() - 128) / 2);
        })
    </script>
    <style type="text/css">
        body, html, p, h1
        {
            margin: 0;
        }
        body{background:#D9D9D9;}
        .main
        {
            width: 960px;
            margin: 0 auto;
        }
        .head
        {
            padding: 20px 0;
        }
        .head h1
        {
            font-size: 60px;
            text-align: center;
        }
        .content
        {
            position: relative;
        }
        .cl
        {
            position: absolute;
            left: 0;
            width: 120px;
        }
        .cm
        {
            width: 720px;
            margin: 0 auto;
        }
        .cm p
        {
            text-align: center;
        }
        .cr
        {
            position: absolute;
            right: 0;
            top: 0;
            width: 120px;
        }
        #Big_Slide_Last
        {
            left: 20px;
            background: url(../images/biz/lastIco.png) no-repeat center top;
        }
        #Big_Slide_Next
        {
            right: 20px;
            background: url(../images/biz/nextIco.png) no-repeat center top;
        }
        #Big_Slide_Last, #Big_Slide_Next
        {
            width: 46px;
            height: 131px;
            color: #333;
            font-size: 18px;
            position: absolute;
            z-index: 9999;
            cursor: pointer;
            opacity: 0.5;
        }
        #Big_Slide_Last:hover, #Big_Slide_Next:hover
        {
            opacity: 1;
        }
    </style>
</head>
<body>
    <div class="main">
        <div class="head">
            <h1>
                红旭集团荣誉展示</h1>
        </div>
        <div class="content">
            <div class="cl">
                <a id="Big_Slide_Last" href="<%=PrevUrl %>"></a>
            </div>
            <div class="cm">
                <%=CurrentCorpMien == null ? string.Empty : CurrentCorpMien.Content%>
            </div>
            <div class="cr">
                <a id="Big_Slide_Next" href="<%=NextUrl %>"></a>
            </div>
        </div>
    </div>
</body>
</html>
