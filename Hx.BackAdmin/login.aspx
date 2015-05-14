<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="login.aspx.cs" Inherits="Hx.BackAdmin.login" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml" style="height: 100%;">
<head>
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>日报表</title>
    <link href="images/apple-touch-icon-114x114.png" sizes="114x114" rel="apple-touch-icon">
    <script src="js/jquery-1.3.2.min.js" type="text/javascript"></script>
    <script type="text/javascript">
        if (self.location != top.location) {
            top.location.href = self.location;
        }
        function fGetCode() {
            var gNow = new Date();
            $("#imgcode").attr("src", "checkcode.axd?x=" + gNow.getSeconds());
        }
        $(function () {
            fGetCode();
        });
    </script>
    <style type="text/css">
        body
        {
            padding: 0;
            font-family: Verdana, Geneva, sans-serif;
            font-size: 12px;
            color: #333;
        }
        img
        {
            border: 0;
        }
        a
        {
            color: #09f;
        }
        .main
        {
            width: 500px;
            margin: 0 auto;
        }
        .logo
        {
            width: 251px;
            float: left;
        }
        .logor
        {
            width: 240px;
            padding-top: 21px;
            float: left;
        }
        .logor ul
        {
            margin: 0;
            padding: 0;
            list-style: none;
        }
        .logor ul li
        {
            padding: 5px 0;
            list-style: none;
            margin: 0;
            text-align: left;
        }
        .an
        {
            border: 0px;
            width: 200px;
            font-weight: bold;
            background: #e8e8e8;
            padding: 4px 6px 2px 6px;
            font-size: 18px;
        }
        .ft
        {
            padding-top: 200px;
            text-align: center;
            overflow: hidden;
        }
        
        .srk1
        {
            width: 175px;
            height: 18px;
            border: 1px solid;
            border-color: #707070 #CECECE #CECECE #ABABAB;
            background: #F9F9F9 url(images/login/z1.gif) no-repeat;
            padding-left: 52px;
        }
        .srk1:hover, .srk1:focus
        {
            border-color: #6FB1DF;
            color: #333;
            background-color: #f5f9fd;
        }
        .srk2
        {
            width: 175px;
            height: 18px;
            border: 1px solid;
            border-color: #707070 #CECECE #CECECE #ABABAB;
            background: #F9F9F9 url(images/login/z2.gif) no-repeat;
            padding-left: 52px;
        }
        .srk2:hover, .srk2:focus
        {
            border-color: #6FB1DF;
            color: #333;
            background-color: #f5f9fd;
        }
        .srk3
        {
            width: 78px;
            height: 18px;
            border: 1px solid;
            border-color: #707070 #CECECE #CECECE #ABABAB;
            background: #F9F9F9 url(images/login/z3.gif) no-repeat;
            padding-left: 52px;
        }
        .srk3:hover, .srk3:focus
        {
            border-color: #6FB1DF;
            color: #333;
            background-color: #f5f9fd;
        }
    </style>
</head>
<body style="height: 100%;">
    <form id="form1" runat="server" style="height: 100%; text-align: center;">
    <div class="main" style="height: 90%; display: inline-block; *display: inline; *zoom: 1;">
        <div class="logo" style="padding-top: 130px;">
            <img src="images/login/logo.jpg" /></div>
        <div class="logor" style="padding-top: 151px;">
            <ul>
                <li>
                    <asp:TextBox ID="tbUserName" runat="server" CssClass="srk1"></asp:TextBox>
                </li>
                <li>
                    <asp:TextBox ID="tbUserPwd" runat="server" CssClass="srk2" TextMode="Password"></asp:TextBox>
                </li>
                <li style="height: 30px;display:none;">
                    <asp:TextBox ID="tbCode" runat="server" CssClass="srk3" Style="float: left;"></asp:TextBox>
                    <span style="display: inline-block; *display: inline; *zoom: 1;"><a href="javascript:fGetCode()">
                        <img src="checkcode.axd" alt="点击刷新" id="imgcode" width="90" /></a></span>
                </li>
                <li>
                    <asp:Button ID="btSave" runat="server" Text="登录" CssClass="an" OnClick="btSave_Click" />
                </li>
                <li>
                    <asp:Label ID="lbMsgUser" runat="server" Text="Label" CssClass="hongzi" Visible="false"></asp:Label></li>
            </ul>
        </div>
    </div>
    <div style="clear: both;">
    </div>
    <div style="text-align: center;">
        红旭网站后台管理系统 V1.0 Powered by <a href="http://www.hongxu.cn">www.hongxu.cn</a> 2013-2020
    </div>
    </form>
</body>
</html>
