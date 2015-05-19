﻿<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="cardmg.aspx.cs" Inherits="Hx.BackAdmin.weixin.cardmg" %>

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
            $("#cbxAllPowerUser").click(function () {
                $(".cbxPowerUser").attr("checked", $(this).attr("checked"));
                setpoweruser();
            })
            $(".cbxPowerUser").click(function () {
                setpoweruser();
            });
        })

        function setpoweruser() {
            var poweruser = $(".cbxPowerUser:checked").map(function () {
                return $(this).val();
            }).get().join('|');
            if (poweruser != '')
                poweruser = '|' + poweruser + '|'
            $("#hdnPowerUser").val(poweruser);
        }
    </script>
    <style type="text/css">
    .clpp ul{display:inline-block;width:160px;*display:inline;*zoom:1;vertical-align:top;margin-left:3px;}
    .nh{color:White;font-weight:bold;font-size:18px;background:#222;text-indent:5px;}
    label{line-height: 18px;display:inline-block;*display:inline;*zoom:1;}
    </style>
</head>
<body>
    <form id="form1" runat="server">
    <div class="ht_main">
        <ul class="xnav">
            <%if (Hx.Components.Web.HXContext.Current.AdminUser.Administrator)
              { %><li><a href="cardsettinglist.aspx">卡券活动管理</a></li><%} %>
            <asp:Repeater ID="rpcg" runat="server">
                <ItemTemplate>
                    <li <%# SetCardSettingStatus(Eval("ID").ToString()) %> <%#Eval("ID").ToString() == GetString("sid") ? "class=\"current\"" : string.Empty %>>
                        <a href="cardmg.aspx?sid=<%#Eval("ID")%>">
                            <%#Eval("Name").ToString()%></a></li></ItemTemplate>
            </asp:Repeater>
        </ul>
        <div class="flqh">
            <span class="dj"><a href="cardmg.aspx?sid=<%= GetInt("sid")%>">活动设置</a></span> <span>
                <a href="cardlist.aspx?sid=<%= GetInt("sid")%>">卡券管理</a></span> <span><a href="cardpullrecordlist.aspx?sid=<%= GetInt("sid")%>">
                    抽奖记录</a></span>
        </div>
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
                <%if (Hx.Components.Web.HXContext.Current.AdminUser.Administrator)
                  { %>
                <tr>
                    <td class="bg1">
                        管理员：
                    </td>
                    <td class="clpp">
                        <ul>
                            <li class="nh">
                                <label>
                                    <input type="checkbox" id="cbxAllPowerUser" class="fll" />全选</label></li>
                        </ul>
                    </td>
                </tr>
                <tr>
                    <td>
                    </td>
                    <td>
                        <asp:Repeater runat="server" ID="rptPowerUser">
                            <HeaderTemplate>
                                <ul>
                            </HeaderTemplate>
                            <ItemTemplate>
                                <li class="blockinline" style="width: 230px;">
                                    <label>
                                        <input type="checkbox" class="cbxPowerUser fll" value="<%# Eval("ID") %>" <%#SetPowerUser(Eval("ID").ToString()) %> /><%# Eval("UserName")%></label></li>
                            </ItemTemplate>
                            <FooterTemplate>
                                </ul></FooterTemplate>
                        </asp:Repeater>
                    </td>
                </tr>
                <%} %>
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
    <input type="hidden" runat="server" id="hdnPowerUser" />
    <input type="hidden" runat="server" id="hdnName" />
    </form>
</body>
</html>