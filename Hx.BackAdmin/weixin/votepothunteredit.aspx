﻿<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="votepothunteredit.aspx.cs"
    Inherits="Hx.BackAdmin.weixin.votepothunteredit" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>添加/编辑选手信息</title>
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
                data: { action: 'weixinUpload' },
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

                        $("#imgpic").attr("src", "<%=ImgServer %>" + response.src);
                        $("#hdimage_pic").val(response.src);
                    } else {
                        alert(response.errorcode);
                        button.val('上传图片');
                    }
                    window.clearInterval(interval);
                    this.enable();
                }
            });

            $(".uploadbtpics").each(function () {
                var imgpath_pics;
                var button1 = $(this), interval1;
                new AjaxUpload(button1, {
                    action: '/cutimage.axd',
                    name: 'picfile',
                    responseType: 'json',
                    data: { action: 'weixinUpload' },
                    onSubmit: function (file, ext) {
                        if (!(ext && /^(jpg|png|jpeg|gif)$/i.test(ext))) {
                            alert('只能上传图片！');
                            return false;
                        }
                        button1.val('上传中');
                        this.disable();
                        interval1 = window.setInterval(function () {
                            var text = button1.val();
                            if (text.length < 13) {
                                button1.val(text + '.');
                            } else {
                                button1.val('上传中');
                            }
                        }, 200);
                    },
                    onComplete: function (file, response) {
                        if (response.msg == "success") {
                            button1.val('修改图片');

                            $("img", button1.parent()).attr("src", "<%=ImgServer %>" + response.src).attr("val", response.src);
                            var imgs = $(".imgpics").map(function () {
                                return $(this).attr("val");
                            }).get().join("|")
                            $("#hdimage_pics").val(imgs);
                        } else {
                            alert(response.errorcode);
                            button1.val('上传图片');
                        }
                        window.clearInterval(interval1);
                        this.enable();
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
            <%if (Hx.Components.Web.HXContext.Current.AdminUser.Administrator)
              { %><li><a href="votesettinglist.aspx">投票活动管理</a></li><%} %>
            <asp:Repeater ID="rpcg" runat="server">
                <ItemTemplate>
                    <li <%# SetVoteSettingStatus(Eval("ID").ToString()) %> <%#Eval("ID").ToString() == GetString("sid") ? "class=\"current\"" : string.Empty %>>
                        <a href="votemg.aspx?sid=<%#Eval("ID")%>">
                            <%#Eval("Name")%></a></li></ItemTemplate>
            </asp:Repeater>
        </ul>
        <div class="flqh">
            <span><a href="votemg.aspx?sid=<%= GetInt("sid")%>">活动设置</a></span> <span class="dj">
                <a href="votepothunterlist.aspx?sid=<%= GetInt("sid")%>">选手管理</a></span> <span><a
                    href="voterecordlist.aspx?sid=<%= GetInt("sid")%>">投票记录</a></span> <span><a href="votecommentmg.aspx?sid=<%= GetInt("sid")%>">
                        评论管理</a></span><%if (Admin.Administrator)
                                         { %><span><a href="voterecordcachelist.aspx?sid=<%= GetInt("sid")%>">投票队列</a></span><%} %>
        </div>
        <table width="100%" border="0" cellspacing="0" cellpadding="0" class="biaoge3">
            <caption class="bt2">
                添加/编辑选手信息</caption>
            <tbody>
                <tr>
                    <td class="bg1">
                        <span class="red">*</span>姓名：
                    </td>
                    <td>
                        <asp:TextBox ID="txtName" runat="server"></asp:TextBox><asp:RequiredFieldValidator
                            ID="rfvname" runat="server" CssClass="red" ErrorMessage="姓名必须填写" ControlToValidate="txtName"
                            Display="Dynamic"></asp:RequiredFieldValidator>
                    </td>
                </tr>
                <tr>
                    <td class="bg1">
                        排序：
                    </td>
                    <td>
                        <asp:TextBox ID="txtSerialNumber" runat="server"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td class="bg1">
                        序号描述：
                    </td>
                    <td>
                        <asp:TextBox ID="txtSerialNumberDetail" runat="server"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td class="bg1">
                        票数：
                    </td>
                    <td>
                        <asp:TextBox ID="txtBallot" runat="server"></asp:TextBox><asp:RegularExpressionValidator
                            ControlToValidate="txtBallot" ErrorMessage="请输入正确的票数" Text="请输入正确的票数" CssClass="red"
                            ID="revballot" runat="server" SetFocusOnError="True" Display="Dynamic" ValidationExpression="^\d+$"></asp:RegularExpressionValidator>
                    </td>
                </tr>
                <tr>
                    <td class="bg1">
                        个人介绍：
                    </td>
                    <td>
                        <asp:TextBox ID="txtIntroduce" runat="server" TextMode="MultiLine" Rows="4" Width="300"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td class="bg1">
                        参赛宣言：
                    </td>
                    <td>
                        <asp:TextBox ID="txtDeclare" runat="server" TextMode="MultiLine" Rows="4" Width="300"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td class="bg1">
                        头像：
                    </td>
                    <td>
                        <input type="button" name="uploadbtpic" id="uploadbtpic" value="上传图片" class="an3" /><br />
                        <img src="../images/fm.jpg" alt="头像" id="imgpic" style="width: 110px; height: 110px;"
                            runat="server" />
                    </td>
                </tr>
                <tr>
                    <td class="bg1 vt">
                        风采展示：
                    </td>
                    <td>
                        <ul>
                            <li style="padding-right: 10px;">
                                <input type="button" value="上传图片" class="an3 uploadbtpics" /><br />
                                <img src="../images/fm.jpg" alt="展示图片1" id="imgpics1" class="imgpics" style="width: 440px;
                                    height: auto;" val="" runat="server" />
                            </li>
                            <li style="padding-right: 10px;">
                                <input type="button" value="上传图片" class="an3 uploadbtpics" /><br />
                                <img src="../images/fm.jpg" alt="展示图片2" id="imgpics2" class="imgpics" style="width: 440px;
                                    height: auto;" val="" runat="server" />
                            </li>
                            <li style="padding-right: 10px;">
                                <input type="button" value="上传图片" class="an3 uploadbtpics" /><br />
                                <img src="../images/fm.jpg" alt="展示图片3" id="imgpics3" class="imgpics" style="width: 440px;
                                    height: auto;" val="" runat="server" />
                            </li>
                            <li style="padding-right: 10px;">
                                <input type="button" value="上传图片" class="an3 uploadbtpics" /><br />
                                <img src="../images/fm.jpg" alt="展示图片4" id="imgpics4" class="imgpics" style="width: 440px;
                                    height: auto;" val="" runat="server" />
                            </li>
                            <li style="padding-right: 10px;">
                                <input type="button" value="上传图片" class="an3 uploadbtpics" /><br />
                                <img src="../images/fm.jpg" alt="展示图片5" id="imgpics5" class="imgpics" style="width: 440px;
                                    height: auto;" val="" runat="server" />
                            </li>
                            <li style="padding-right: 10px;">
                                <input type="button" value="上传图片" class="an3 uploadbtpics" /><br />
                                <img src="../images/fm.jpg" alt="展示图片6" id="imgpics6" class="imgpics" style="width: 440px;
                                    height: auto;" val="" runat="server" />
                            </li>
                        </ul>
                    </td>
                </tr>
                <tr>
                    <td colspan="2" style="text-align: center">
                        <asp:Button ID="btSave" runat="server" Text="保存" OnClick="btSave_Click" class="an1" />
                        <asp:Label ID="lbMsg" runat="server" Text="Label" CssClass="hongzi" Visible="false"></asp:Label>
                    </td>
                </tr>
            </tbody>
        </table>
        <input id="hdimage_pic" runat="server" type="hidden" />
        <input id="hdimage_pics" runat="server" type="hidden" />
        <asp:HiddenField ID="hdid" runat="server" />
        <asp:HiddenField ID="HfRid" runat="server" />
    </div>
    </form>
</body>
</html>
