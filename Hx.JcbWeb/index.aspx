<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="index.aspx.cs" Inherits="Hx.JcbWeb.index" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
    <meta http-equiv="Content-Type" content="text/html; charset=gb2312" />
    <title></title>
    <meta name="description" content="" />
    <meta name="keywords" content="" />
    <meta http-equiv="pragma" content="no-cache">
    <meta http-equiv="cache-control" content="no-cache">
    <meta http-equiv="expires" content="0">
    <link href="css/style.css" rel="stylesheet" type="text/css" />
    <link rel="shortcut icon" href="favicon.ico" type="image/icon" />
    <style>
        .ppContent
        {
            padding: 0 40px;
        }
        .ppContent .ppinfo
        {
            padding: 30px 0 40px 0;
            line-height: 25px;
        }
        .ppContent .ppcenter
        {
            text-align: center;
        }
        .ppContent .ppcenter a
        {
            margin: 0 5px;
        }
        
        /* select下拉 */
        .ppselecter, .ppselecter *
        {
            font-size: 12px;
            font-family: "宋体" , Verdana, Arial, Helvetica, sans-serif;
            line-height: normal;
        }
        .ppselecter
        {
            border: 1px solid #77AAE3;
            width: 220px;
            top: 4px;
            position: absolute;
            overflow: hidden;
            left: 0;
            left: -1px\9;
        }
        .ppselecter_ssl
        {
            width: 175px;
        }
        .ppselecter .ppseltit
        {
            background-color: #EEF7FD;
            height: 15px;
            padding-top: 4px;
            text-indent: 6px;
            color: #666;
            border-bottom: 1px solid #77AAE3;
        }
        .ppselecter table td table td
        {
            padding: 3px;
        }
        .ppselecter .active
        {
            background: #D1E8FA;
            color: #333;
            font-weight: normal;
        }
        
        /* 登录中 */
        .passportLoading
        {
            height: 70px;
            background: url(loading_bg.png) center center no-repeat;
            text-align: center;
            margin-top: 42px;
        }
        .passportLoading img
        {
            margin-top: 19px;
        }
        .saa_pspt .box .loadingText
        {
            text-align: center;
            margin-top: 5px;
        }
        
        .ppselecter table td
        {
            vertical-align: middle;
            text-align: left;
            height: auto !important;
            padding-top: inherit;
            border-style: none;
        }
    </style>
</head>
<body>
    <div class="header">
        <div class="area">
            <div class="fl">
                <a href="http://jcb.hongxu.cn" target="_blank" class="hlink1">红旭二手车</a><a href="http://jcb.hongxu.cn"
                    target="_blank" class="hlink2">红旭二手车</a></div>
            <p class="fr">
                <span class="f14">集车宝客服电话：400-808-8888&nbsp;转&nbsp;8</span><br />
                <span class="f13">客服QQ：47947953</span<br />
            </p>
        </div>
    </div>
    <div class="dlare">
        <div class="area">
            <div class="dlcon">
                <div class="dltit">
                    <h2>
                        会员登录</h2>
                    <a href="reg.aspx">还没成为会员?申请开通</a></div>
                <div class="dlbox" id="user-login">
                    <p>
                        <label>
                            账&nbsp;&nbsp;户：</label><input id="passport" name="" type="text" class="input" /></p>
                    <p>
                        <label>
                            密&nbsp;&nbsp;码：</label><input id="passwd" name="" type="password" class="input" /></p>
                    <p class="padl">
                        <input id="isRemember" type="checkbox" value="1" />
                        记住我一周 <a target="_blank" href="http://jcb.hongxu.cn/recover.jsp">忘记密码</a></p>
                    <p class="padl">
                        <input onclick="login2SC()" type="button" class="dlbtn" value="登录" /></p>
                    <p id="message">
                    </p>
                </div>
                <div class="dltip">
                    <span class="tel">服务热线</span>
                    <p>
                        400 - 808 - 8888</p>
                </div>
            </div>
        </div>
    </div>
    <div id="foot" class="area">
        Copyright <span class="fontArial">&copy;</span> 2015 hongxu.cn Inc. All Rights Reserved.
        红旭集团 版权所有
    </div>
</body>
</html>
