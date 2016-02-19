<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="main_x.aspx.cs" Inherits="Hx.BackAdmin.global.main_x" %>

<html xmlns="http://www.w3.org/1999/xhtml">
<head>
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>无标题文档</title>
    <link href=<%=ResourceServer%>/css/admin.css rel="stylesheet" type="text/css" />
    <script src=<%=ResourceServer%>/js/jquery-1.3.2.min.js type="text/javascript"></script>
    <script language="javascript" type="text/javascript">
        //    $(function () {
        //        parent.sk.setdaohan("sss");
        //    })
    </script>
</head>
<body>
    <div class="ht_main">
        <div class="bt0">
            站点信息</div>
        <div class="bt1">
            <dl>
                站点名称:<br />
                <dt>
                    <input type="text" name="textfield" class="srk1" /></dt>
                <dd>
                    站点名称，将显示在导航条和标题中</dd>
            </dl>
        </div>
    </div>
</body>
</html>
