<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="job.aspx.cs" Inherits="Hx.BackAdmin.biz.job" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>招聘信息</title>
    <link href="../css/admin.css" rel="stylesheet" type="text/css" />
    <style type="text/css">
        .content
        {
            width: 480px;
            height: 105px;
            margin:0 auto;
            position:relative;
        }
        .cstatic
        {
            padding:26px 0 0 150px;
            color: White;
            font-weight: bold;
            font-size: 20px;
            position:absolute;
            width:330px;
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
            padding: 4px 0 0 22px;
            height:50px;
            cursor:pointer;
            line-height:23px;
            word-spacing: 2px;
            word-spacing: 5px\9;
            *word-spacing: 5px;
            _word-spacing: 5px;
        }
        img
        {
            float: left;
            width:474px;
            height:99px;
            border:1px solid #d7d7d7;
            padding: 2px;
        }
        p{margin:0;}
        a:hover{text-decoration:none; style="position:relative;display:block;"}
    </style>
</head>
<body>
    <div class="content">
        <a href="jobview.aspx" class="fll" target="_blank" title="红旭集团所有4S店招聘信息">
            <img src="../images/zhaopin.jpg?t=<%=DateTime.Now.ToString("yyyyMMddHHmmss") %>" />
            <div class="cstatic">
                <div class="nr">
                    <%= CurrentJobOffer == null ? string.Empty : CurrentJobOffer.Title.Replace("\r","<br>").Replace(" ","&nbsp;")%>
                </div>
            </div>
        </a>
    </div>
</body>
</html>
