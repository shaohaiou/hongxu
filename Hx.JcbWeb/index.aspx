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
</head>
<body>
    <div class="header">
        <div class="area">
            <div class="fl">
                <a href="http://jcb.hongxu.cn" target="_blank" class="hlink1">������ֳ�</a><a href="http://jcb.hongxu.cn"
                    target="_blank" class="hlink2">������ֳ�</a></div>
            <p class="fr">
                <span class="f14">�������ͷ��绰��400-808-8888&nbsp;ת&nbsp;8</span><br />
                <span class="f13">�ͷ�QQ��47947953</span<br />
            </p>
        </div>
    </div>
    <div class="dlare">
        <div class="area">
            <div class="dlcon">
                <div class="dltit">
                    <h2>
                        ��Ա��¼</h2>
                    <a href="reg.aspx">��û��Ϊ��Ա?���뿪ͨ</a></div>
                <div class="dlbox" id="user-login">
                    <p>
                        <label>
                            ��&nbsp;&nbsp;����</label><input id="passport" name="" type="text" class="input" /></p>
                    <p>
                        <label>
                            ��&nbsp;&nbsp;�룺</label><input id="passwd" name="" type="password" class="input" /></p>
                    <p class="padl">
                        <input id="isRemember" type="checkbox" value="1" />
                        ��ס��һ�� <a target="_blank" href="http://jcb.hongxu.cn/recover.jsp">��������</a></p>
                    <p class="padl">
                        <input onclick="login2SC()" type="button" class="dlbtn" value="��¼" /></p>
                    <p id="message">
                    </p>
                </div>
                <div class="dltip">
                    <span class="tel">��������</span>
                    <p>
                        400 - 808 - 8888</p>
                </div>
            </div>
        </div>
    </div>
    <div id="foot" class="area">
        Copyright <span class="fontArial">&copy;</span> 2015 hongxu.cn Inc. All Rights Reserved.
        ������ ��Ȩ����
    </div>
</body>
</html>
