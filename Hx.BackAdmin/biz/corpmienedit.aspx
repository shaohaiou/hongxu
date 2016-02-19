<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="corpmienedit.aspx.cs" Inherits="Hx.BackAdmin.biz.corpmienedit"
    ValidateRequest="false" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>添加/编辑集团荣誉</title>
    <link href=<%=ResourceServer%>/css/admin.css rel="stylesheet" type="text/css" />
    <script src=<%=ResourceServer%>/js/jquery-1.3.2.min.js type="text/javascript"></script>
    <script src=<%=ResourceServer%>/js/ckeditor/ckeditor.js type="text/javascript"></script>
    <script src=<%=ResourceServer%>/js/ajaxupload.js type="text/javascript"></script>
    <script type="text/javascript">
        $(function () {
            CKEDITOR.replace('txtContent', { toolbar: 'Basic', height: 680, width: 980 });

            var imgpath_pic;
            var button = $('#uploadbtpic'), interval;
            new AjaxUpload(button, {
                action: '/cutimage.axd',
                name: 'picfile',
                responseType: 'json',
                data: { action: 'jobUpload' },
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
        });
    </script>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <table width="100%" border="0" cellspacing="0" cellpadding="0" class="biaoge3">
            <caption class="bt2">
                添加/编辑集团荣誉</caption>
            <tbody>
                <tr>
                    <td class="bg1">
                        轮播图片：
                    </td>
                    <td class="bg2">
                        <input type="button" name="uploadbtpic" id="uploadbtpic" value="上传图片" class="an3" /><br />
                        <img src="../images/fm.jpg" alt="图片" id="imgpic" style="width: 100px; height: 100px;
                            border: 0px; padding: 0px;" runat="server" />
                    </td>
                </tr>
                <tr>
                    <td class="bg1">
                        轮播简介：
                    </td>
                    <td class="bg2">
                        <asp:TextBox runat="server" ID="txtIntroduce" CssClass="srk1"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td class="bg1">
                        详细内容：
                    </td>
                    <td class="bg2">
                        <asp:TextBox ID="txtContent" runat="server" TextMode="MultiLine" CssClass="ckeditor"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td>
                    </td>
                    <td>
                        <input type="hidden" runat="server" id="hdnID" />
                        <asp:Button ID="btSave" runat="server" Text="保存" OnClick="btSave_Click" class="an1" />
                    </td>
                </tr>
            </tbody>
        </table>
        <input id="hdimage_pic" runat="server" type="hidden" />
    </div>
    </form>
</body>
</html>
