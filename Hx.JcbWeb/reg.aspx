<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="reg.aspx.cs" Inherits="Hx.JcbWeb.reg" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=gb2312" />
    <title>红旭集团二手车-集车宝</title>
    <meta name="description" content="" />
    <meta name="keywords" content="" />
    <link href="css/style.css" rel="stylesheet" type="text/css" />
    <link rel="shortcut icon" href="favicon.ico" type="image/icon" />
    <script type="text/javascript" src="js/jquery-1.3.2.min.js"></script>
    <script type="text/javascript" language="javascript">
        var cleartimer = null;
        $(function () {
            $("#btnSubmit").click(function () {
                var checkresult = CheckForm();
                if (checkresult != "") {
                    if (cleartimer) clearTimeout(cleartimer);
                    $("#lblMsg").text(checkresult);
                    return false;
                }
                form1.submit();
            });
            $("#txtUserName").change(function () {
                if ($(this).val() != "") {
                    $.ajax({
                        url: 'checkadmin.axd?d=' + new Date(),
                        async: false,
                        dataType: "json",
                        data: { name: $("#txtUserName").val() },
                        error: function (msg) {
                            alert("发生错误！");
                        },
                        success: function (data) {
                            if (data.result == 'success') {
                                $(".framtip", $("#txtUserName").parent()).remove();
                                $("#txtUserName").parent().append($("<span class=\"framtip\"><em></em><span></span></span>"));
                            }
                            else {
                                $(".framtip", $("#txtUserName").parent()).remove();
                                $("#txtUserName").parent().append($("<span class=\"framtip wrog\"><em></em><span>该登录名已被注册过</span></span>"));
                            }
                        }
                    });
                }
            });
            setInterval(function () {
                if ($("#lblMsg").text() != "") {
                    cleartimer = setTimeout(function () {
                        $("#lblMsg").text("");
                    }, 2000);
                }
            }, 1000);
        })

        function CheckForm() {
            if ($("#txtUserName").val() == "")
                return "请输入登录名";
            if ($("#txtPassword").val() == "")
                return "请输入登录密码";
            if ($("#txtPasswordConfirm").val() == "")
                return "请输入确认密码";
            if ($("#txtPasswordConfirm").val() != $("#txtPassword").val())
                return "两次密码不一致";
            if ($("input[name='rblCompanyType']:checked").length == 0)
                return "请选择公司类型";
            if ($("#txtCompanyName").val() == "")
                return "请输入公司名称";
            if ($("#txtName").val() == "")
                return "请输入联系人";
            if ($("#txtPhone").val() == "")
                return "请输入手机号";
            if ($("#ddlProvince").val() == "-1")
                return "请选择省份";
            if ($("#ddlCity").val() == "-1")
                return "请选择城市";
            if ($("#txtAddress").val() == "")
                return "请输入详细地址";
            return "";
        }
    </script>
</head>
<body>
    <div class="header">
        <div class="area">
            <div class="fl">
                <a href="http://jcb.hongxu.cn" target="_blank" class="hlink1">红旭二手车</a><a href="http://jcb.hongxu.cn"
                    target="_blank" class="hlink2">红旭二手车</a></div>
            <p class="fr">
                <span class="f14">集车宝客服电话：400-808-8888&nbsp;转&nbsp;8</span><br />
                <span class="f13">客服QQ：47947953</span><br />
            </p>
        </div>
    </div>
    <form runat="server" id="form1">
    <div class="area1">
        <h2>
            欢迎开通红旭集团二手车集车宝</h2>
        <div class="tit">
            <h1>
                用户注册</h1>
        </div>
        <ul class="fbfram">
            <li><span class="framth"><span class="red">*</span>登录名：</span><div class="framr">
                <asp:TextBox runat="server" ID="txtUserName" CssClass="sel inpwid1"></asp:TextBox>
            </div>
            </li>
            <li><span class="framth"><span class="red">*</span>登录密码：</span><div class="framr">
                <asp:TextBox runat="server" ID="txtPassword" CssClass="sel inpwid1" TextMode="Password"></asp:TextBox>
            </div>
            </li>
            <li><span class="framth"><span class="red">*</span>确认密码：</span><div class="framr">
                <asp:TextBox runat="server" ID="txtPasswordConfirm" CssClass="sel inpwid1" TextMode="Password"></asp:TextBox>
            </div>
            </li>
            <li><span class="framth"><span class="red">*</span>公司类型：</span><div class="framr">
                <asp:RadioButtonList runat="server" ID="rblCompanyType" RepeatDirection="Horizontal">
                </asp:RadioButtonList>
            </div>
            </li>
            <li><span class="framth"><span class="red">*</span>公司名称：</span><div class="framr">
                <asp:TextBox runat="server" ID="txtCompanyName" CssClass="sel inpwid2" Width="330"></asp:TextBox></div>
            </li>
            <li><span class="framth"><span class="red">*</span>联系人：</span><div class="framr">
                <asp:TextBox runat="server" ID="txtName" CssClass="sel"></asp:TextBox></div>
            </li>
            <li><span class="framth"><span class="red">*</span>手机号：</span><div class="framr">
                <asp:TextBox runat="server" ID="txtPhone" CssClass="sel inpwid1"></asp:TextBox></div>
            </li>
            <li><span class="framth">QQ号：</span><div class="framr">
                <asp:TextBox runat="server" ID="txtQQ" CssClass="sel inpwid1"></asp:TextBox></div>
            </li>
            <li><span class="framth"><span class="red">*</span>公司地址：</span><div class="framr">
                <asp:ScriptManager runat="server" ID="sm1">
                </asp:ScriptManager>
                <asp:UpdatePanel runat="server" ID="upl1">
                    <ContentTemplate>
                        <asp:DropDownList runat="server" ID="ddlProvince" CssClass="sel" AutoPostBack="true"
                            OnSelectedIndexChanged="ddlProvince_SelectedIndexChanged">
                        </asp:DropDownList>
                        <asp:DropDownList runat="server" ID="ddlCity" CssClass="sel">
                        </asp:DropDownList>
                        <asp:TextBox runat="server" ID="txtAddress" CssClass="sel inpwid1"></asp:TextBox>
                    </ContentTemplate>
                </asp:UpdatePanel>
            </div>
            </li>
            <li><span class="framth"></span>
                <div class="framr">
                    <a class="frabtna" href="javascript:void(0)" id="btnSubmit">提交</a><asp:Label runat="server"
                        ID="lblMsg" CssClass="red" style="position:absolute;"></asp:Label></div>
            </li>
        </ul>
    </div>
    </form>
    <div id="foot" class="area">
        Copyright <span class="fontArial">&copy;</span> 2015 hongxu.cn Inc. All Rights Reserved.
        红旭集团 版权所有
    </div>
</body>
</html>
