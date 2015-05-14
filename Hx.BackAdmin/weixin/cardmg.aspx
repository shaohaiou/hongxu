<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="cardmg.aspx.cs" Inherits="Hx.BackAdmin.weixin.cardmg" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>活动设置</title>
    <link href="../css/admin.css" rel="stylesheet" type="text/css" />
    <script src="../js/jquery-1.3.2.min.js" type="text/javascript"></script>
    <script src="../js/ajaxupload.js" type="text/javascript"></script>
    <script type="text/javascript">
        $(function () {
            $(".uploadbtpic").each(function () {
                var imgpath_pic;
                var button = $(this), interval;
                new AjaxUpload(button, {
                    action: '/cutimage.axd',
                    name: 'picfile',
                    responseType: 'json',
                    data: { action: 'weixinjtUpload' },
                    onSubmit: function (file, ext) {
                        if (!(ext && /^(jpg|png|jpeg|gif)$/i.test(ext))) {
                            alert('只能上传图片！');
                            return false;
                        }
                        button.val('上传中');
                        this.disable();
                        interval = window.setInterval(function () {
                            var text = button.val();
                            if (text.length < 13) {
                                button.val(text + '.');
                            } else {
                                button.val('上传中');
                            }
                        }, 200);
                    },
                    onComplete: function (file, response) {
                        button.val('修改图片');
                        window.clearInterval(interval);
                        this.enable();

                        button.next().next().attr("src", response.src);
                        button.prev().val(response.src);
                    }
                });
            });
        })
    </script>
</head>
<body>
    <form id="form1" runat="server">
    <div class="ht_main">
        <ul class="xnav">
            <li class="current"><a href="cardmg.aspx">活动设置</a></li>
            <li><a href="cardlist.aspx">卡券管理</a></li>
            <li><a href="cardpullrecordlist.aspx">抽奖记录</a></li>
        </ul>
        <table width="100%" border="0" cellspacing="0" cellpadding="0" class="biaoge3">
            <caption class="bt2">
                卡券活动设置</caption>
            <tbody>
                <tr>
                    <td class="bg1">
                        活动总开关：
                    </td>
                    <td>
                        <asp:CheckBox runat="server" ID="cbxSwitch" Text="开启" />
                    </td>
                </tr>
                <tr>
                    <td class="bg1">
                        AppID：
                    </td>
                    <td>
                        <asp:TextBox runat="server" ID="txtAppID" CssClass="srk1"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td class="bg1">
                        AppSecret：
                    </td>
                    <td>
                        <asp:TextBox runat="server" ID="txtAppSecret" CssClass="srk1"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td class="bg1">
                        公众号名称：
                    </td>
                    <td>
                        <asp:TextBox runat="server" ID="txtAppName" CssClass="srk1"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td class="bg1">
                        公众号二维码图片：
                    </td>
                    <td>
                        <input id="hdnAppImg" runat="server" type="hidden" />
                        <input type="button" class="uploadbtpic an3" value="上传图片" /><br />
                        <img src="../images/fm.jpg" alt="图片" id="imgAppImg" style="width: 160px; height: 160px;"
                            runat="server" />
                    </td>
                </tr>
                <tr>
                    <td class="bg1">
                        活动规则：
                    </td>
                    <td>
                        <asp:TextBox ID="txtActRule" runat="server" TextMode="MultiLine" Rows="6" CssClass="w300"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td class="bg1">
                        奖项设置：
                    </td>
                    <td>
                        <asp:TextBox ID="txtAwards" runat="server" TextMode="MultiLine" Rows="6" CssClass="w300"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td class="bg1">
                        中奖率：
                    </td>
                    <td>
                        <asp:TextBox runat="server" ID="txtWinRate" CssClass="srk4"></asp:TextBox>%
                    </td>
                </tr>
                <tr style="background-color: #ccc; color: Black; font-weight: bold;">
                    <td colspan="2">
                        微信分享
                    </td>
                </tr>
                <tr>
                    <td class="bg1">
                        分享图片：
                    </td>
                    <td>
                        <input id="hdimage_pic" runat="server" type="hidden" />
                        <input type="button" class="uploadbtpic an3" value="上传图片" /><br />
                        <img src="../images/fm.jpg" alt="图片" id="imgpic" style="width: 110px; height: 110px;"
                            runat="server" />
                    </td>
                </tr>
                <tr>
                    <td class="bg1">
                        分享链接地址：
                    </td>
                    <td>
                        <asp:TextBox ID="txtShareLinkUrl" runat="server" CssClass="w300"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td class="bg1">
                        分享标题：
                    </td>
                    <td>
                        <asp:TextBox ID="txtShareTitle" runat="server" CssClass="w160"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td class="bg1">
                        分享描述：
                    </td>
                    <td>
                        <asp:TextBox ID="txtShareDesc" runat="server" TextMode="MultiLine" Rows="6" CssClass="w300"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td colspan="2" style="text-align: center">
                        <asp:Button ID="btSave" runat="server" Text="保存" OnClick="btSave_Click" class="an1" />
                    </td>
                </tr>
            </tbody>
        </table>
    </div>
    </form>
</body>
</html>
