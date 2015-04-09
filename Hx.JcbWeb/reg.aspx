<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="reg.aspx.cs" Inherits="Hx.JcbWeb.reg" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=gb2312" />
    <title>�����Ŷ��ֳ�-������</title>
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
                            alert("��������");
                        },
                        success: function (data) {
                            if (data.result == 'success') {
                                $(".framtip", $("#txtUserName").parent()).remove();
                                $("#txtUserName").parent().append($("<span class=\"framtip\"><em></em><span></span></span>"));
                            }
                            else {
                                $(".framtip", $("#txtUserName").parent()).remove();
                                $("#txtUserName").parent().append($("<span class=\"framtip wrog\"><em></em><span>�õ�¼���ѱ�ע���</span></span>"));
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
                return "�������¼��";
            if ($("#txtPassword").val() == "")
                return "�������¼����";
            if ($("#txtPasswordConfirm").val() == "")
                return "������ȷ������";
            if ($("#txtPasswordConfirm").val() != $("#txtPassword").val())
                return "�������벻һ��";
            if ($("input[name='rblCompanyType']:checked").length == 0)
                return "��ѡ��˾����";
            if ($("#txtCompanyName").val() == "")
                return "�����빫˾����";
            if ($("#txtName").val() == "")
                return "��������ϵ��";
            if ($("#txtPhone").val() == "")
                return "�������ֻ���";
            if ($("#ddlProvince").val() == "-1")
                return "��ѡ��ʡ��";
            if ($("#ddlCity").val() == "-1")
                return "��ѡ�����";
            if ($("#txtAddress").val() == "")
                return "��������ϸ��ַ";
            return "";
        }
    </script>
</head>
<body>
    <div class="header">
        <div class="area">
            <div class="fl">
                <a href="http://jcb.hongxu.cn" target="_blank" class="hlink1">������ֳ�</a><a href="http://jcb.hongxu.cn"
                    target="_blank" class="hlink2">������ֳ�</a></div>
            <p class="fr">
                <span class="f14">�������ͷ��绰��400-808-8888&nbsp;ת&nbsp;8</span><br />
                <span class="f13">�ͷ�QQ��47947953</span><br />
            </p>
        </div>
    </div>
    <form runat="server" id="form1">
    <div class="area1">
        <h2>
            ��ӭ��ͨ�����Ŷ��ֳ�������</h2>
        <div class="tit">
            <h1>
                �û�ע��</h1>
        </div>
        <ul class="fbfram">
            <li><span class="framth"><span class="red">*</span>��¼����</span><div class="framr">
                <asp:TextBox runat="server" ID="txtUserName" CssClass="sel inpwid1"></asp:TextBox>
            </div>
            </li>
            <li><span class="framth"><span class="red">*</span>��¼���룺</span><div class="framr">
                <asp:TextBox runat="server" ID="txtPassword" CssClass="sel inpwid1" TextMode="Password"></asp:TextBox>
            </div>
            </li>
            <li><span class="framth"><span class="red">*</span>ȷ�����룺</span><div class="framr">
                <asp:TextBox runat="server" ID="txtPasswordConfirm" CssClass="sel inpwid1" TextMode="Password"></asp:TextBox>
            </div>
            </li>
            <li><span class="framth"><span class="red">*</span>��˾���ͣ�</span><div class="framr">
                <asp:RadioButtonList runat="server" ID="rblCompanyType" RepeatDirection="Horizontal">
                </asp:RadioButtonList>
            </div>
            </li>
            <li><span class="framth"><span class="red">*</span>��˾���ƣ�</span><div class="framr">
                <asp:TextBox runat="server" ID="txtCompanyName" CssClass="sel inpwid2" Width="330"></asp:TextBox></div>
            </li>
            <li><span class="framth"><span class="red">*</span>��ϵ�ˣ�</span><div class="framr">
                <asp:TextBox runat="server" ID="txtName" CssClass="sel"></asp:TextBox></div>
            </li>
            <li><span class="framth"><span class="red">*</span>�ֻ��ţ�</span><div class="framr">
                <asp:TextBox runat="server" ID="txtPhone" CssClass="sel inpwid1"></asp:TextBox></div>
            </li>
            <li><span class="framth">QQ�ţ�</span><div class="framr">
                <asp:TextBox runat="server" ID="txtQQ" CssClass="sel inpwid1"></asp:TextBox></div>
            </li>
            <li><span class="framth"><span class="red">*</span>��˾��ַ��</span><div class="framr">
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
                    <a class="frabtna" href="javascript:void(0)" id="btnSubmit">�ύ</a><asp:Label runat="server"
                        ID="lblMsg" CssClass="red" style="position:absolute;"></asp:Label></div>
            </li>
        </ul>
    </div>
    </form>
    <div id="foot" class="area">
        Copyright <span class="fontArial">&copy;</span> 2015 hongxu.cn Inc. All Rights Reserved.
        ������ ��Ȩ����
    </div>
</body>
</html>
