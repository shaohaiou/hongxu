﻿<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="job.aspx.cs" Inherits="Hx.BackAdmin.biz.job" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>招聘信息</title>
    <link href=<%=ResourceServer%>/css/admin.css rel="stylesheet" type="text/css" />
    <style type="text/css">
        .content
        {
            max-width: 480px;
            height:160px;
            margin:0 auto;
            position:relative;
        }
        .cstatic
        {
            padding:5.5% 0 0 31%;
            color: White;
            font-weight: bold;
            font-size: 20px;
            position:absolute;
            width:69%;
            left:0px;
        }
        .jz
        {
            padding: 5px 0 0 30px;
            color: Black;
        }
        .nr
        {
            font-weight: normal;
            font-size: 12px;
            overflow: hidden;
            padding:1.2% 0 0 7%;
            height:50px;
            cursor:pointer;
            line-height:22px;
            word-spacing: 2px;
            word-spacing: 5px\9;
            *word-spacing: 5px;
            _word-spacing: 5px;
        }
        img
        {
            float: left;
            width:100%;
            max-height:160px;
        }
        p{margin:0;}
        a:hover{text-decoration:none; style="position:relative;display:block;"}
        .space{display: inline-block;width: 2.5%;}
    </style>
</head>
<body>
    <div class="content">
        <a href="jobview.aspx" class="fll" target="_blank" title="红旭集团所有4S店招聘信息">
            <img src="<%=ImgServer %><%=CurrentJobOffer.PicPath %>" />
        </a>
    </div>
</body>
</html>
