<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="jituanvotesetting.aspx.cs"
    Inherits="Hx.BackAdmin.weixin.jituanvotesetting" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>集团投票活动设置</title>
    <link href=<%=ResourceServer%>/css/admin.css rel="stylesheet" type="text/css" />
    <script src=<%=ResourceServer%>/js/jquery-1.3.2.min.js type="text/javascript"></script>
    <script src=<%=ResourceServer%>/js/ajaxupload.js type="text/javascript"></script>
    <script type="text/javascript">
        $(function () {
            var imgpath_pic;
            var button = $('#uploadbtpic'), interval;
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
                    if (response.msg == "success") {
                        button.val('修改图片');

                        $("#imgpic").attr("src", response.src);
                        $("#hdimage_pic").val(response.src);
                    } else {
                        alert(response.errorcode);
                        button1.val('上传图片');
                    }
                    window.clearInterval(interval);
                    this.enable();
                }
            });
        })
    </script>
</head>
<body>
    <form id="form1" runat="server">
    <div class="ht_main">
        <ul class="xnav">
            <li><a href="jituanvotemg.aspx">选手管理</a></li>
            <li><a href="jituanvotelist.aspx">投票记录</a></li>
            <li class="current"><a href="jituanvotesetting.aspx">活动设置</a></li>
        </ul>
        <table width="100%" border="0" cellspacing="0" cellpadding="0" class="biaoge3">
            <caption class="bt2">
                集团投票活动设置</caption>
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
                        活动须知：
                    </td>
                    <td>
                        <asp:TextBox ID="txtMustKnow" runat="server" TextMode="MultiLine" Rows="6" CssClass="w300"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td class="bg1">
                        投票期限：
                    </td>
                    <td>
                        <asp:TextBox ID="txtOverdueMinutes" runat="server" CssClass="w40"></asp:TextBox>
                        分钟 <span class="gray">(用户从参与投票开始后限定时间内可以投票，不填或填0表示不限制)</span>
                    </td>
                </tr>
                <tr>
                    <td class="bg1">
                        每日投票次数：
                    </td>
                    <td>
                        <asp:TextBox ID="txtVoteTimes" runat="server" CssClass="w40"></asp:TextBox>
                        次 <span class="gray">(不填写或填0表示不限制次数)</span>
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
                        <input type="button" name="uploadbtpic" id="uploadbtpic" value="上传图片" class="an3" /><br />
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
    <input id="hdimage_pic" runat="server" type="hidden" />
    </form>
</body>
</html>
