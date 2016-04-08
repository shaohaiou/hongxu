<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="personaldatamg.aspx.cs"
    Inherits="Hx.BackAdmin.biz.personaldatamg" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>现有资料管理</title>
    <link href=<%=ResourceServer%>/css/admin.css rel="stylesheet" type="text/css" />
    <script src=<%=ResourceServer%>/js/jquery-1.3.2.min.js type="text/javascript"></script>
    <script src=<%=ResourceServer%>/js/ajaxupload.js type="text/javascript"></script>
    <script type="text/javascript">
        $(function () {
            $(".btnDel").click(function () {
                DelRow(this);
            });

            $("#btnAdd").click(function () {
                $("#hdnAddCount").val(parseInt($("#hdnAddCount").val()) + 1);
                $("#tblData").append("<tr><td class=\"w100\"></td>"
                 + "<td class=\"w100\"><input type=\"text\" id=\"txtName" + $("#hdnAddCount").val() + "\" name=\"txtName" + $("#hdnAddCount").val() + "\" class=\"srk1 w80\" value=\"\" /></td>"
                 + "<td class=\"w100\"><a href=\"javascript:void(0);\" id=\"btnUploadImg" + $("#hdnAddCount").val() + "\" class=\"uploadbtpics\">上传文件</a><input type=\"hidden\" id=\"hdnFilepath" + $("#hdnAddCount").val() + "\" name=\"hdnFilepath" + $("#hdnAddCount").val() + "\" class=\"hdnFilepath\" value=\"\" /></td>"
                 + "<td class=\"w100\"><a href=\"javascript:void(0);\" class=\"btnDel\" val=\"0\">删除</a> </td></tr>");
                $(".btnDel").unbind("click").click(function () {
                    DelRow(this);
                });
                UploadImg($("#btnUploadImg" + $("#hdnAddCount").val())[0]);
            });

            $(".uploadbtpics").each(function () {
                UploadImg(this);
            });
        });

        function DelRow(obj) {
            if ($(obj).attr("val") > 0) {
                $("#hdnDelIds").val($("#hdnDelIds").val() + ($("#hdnDelIds").val() == "" ? "" : ",") + $(obj).attr("val"));
            }
            $(obj).parent().parent().remove();
        }

        function UploadImg(btn) {
            var imgpath_pics;
            var button1 = $(btn), interval1;
            new AjaxUpload(button1, {
                action: '/cutimage.axd',
                name: 'picfile',
                responseType: 'json',
                data: { action: 'jobUpload' },
                onSubmit: function (file, ext) {
                    button1.html("上传中");
                    this.disable();
                    interval1 = window.setInterval(function () {
                        var text = button1.html();
                        if (text.length < 13) {
                            button1.html(text + '.');
                        } else {
                            button1.html("上传中");
                        }
                    }, 200);
                },
                onComplete: function (file, response) {
                    if (response.msg == "success") {
                        button1.html('修改文件');
                        $(".hdnFilepath", button1.parent()).val(response.src);
                    } else {
                        alert(response.errorcode);
                        button1.html('上传文件');
                    }
                    window.clearInterval(interval1);
                    this.enable();
                }
            });
        }
    </script>
</head>
<body>
    <form runat="server" id="form1">
    <div style="margin: 0 15px; padding-top: 15px;">
        <div style="border-bottom: 2px solid #ccc; padding-bottom: 3px; font-weight: bold;">
            现有资料管理
        </div>
        <div style="font-weight: bold; color: #003366; background-color: #ccc; padding: 8px 2px;
            font-size: 9pt;">
            现有资料
        </div>
        <div>
            <table id="tblData">
                <asp:Repeater ID="rptData" runat="server">
                    <ItemTemplate>
                        <tr>
                            <td class="w100">
                                <input type="hidden" runat="server" id="hdnID" value='<%#Eval("ID") %>' />
                                <a href="<%=ImgServer %><%# Eval("Filepath") %>" target="_blank">
                                    <%#Eval("Name") %></a>
                            </td>
                            <td class="w100<%if(UID != 2450){ %> hide<%} %>">
                                <input type="text" runat="server" id="txtName" class="srk1 w80" value='<%#Eval("Name") %>' />
                            </td>
                            <td class="w100<%if(UID != 2450){ %> hide<%} %>">
                                <a href="javascript:void(0);" class="uploadbtpics"><%# !string.IsNullOrEmpty(Eval("Filepath").ToString()) ? "修改文件" : "上传文件"%></a>
                                <input type="hidden" value='<%# Eval("Filepath") %>' runat="server" id="hdnFilepath"
                                    class="hdnFilepath" />
                            </td>
                            <td class="w100<%if(UID != 2450){ %> hide<%} %>">
                                <a href="javascript:void(0);" class="btnDel pl10" val="<%#Eval("ID") %>">删除</a>
                            </td>
                        </tr>
                    </ItemTemplate>
                </asp:Repeater>
            </table>
        </div>
        <%if (UID == 2450)
          { %>
        <div class="lan5x" style="padding-top: 10px;">
            <input type="hidden" runat="server" id="hdnAddCount" value="0" />
            <input type="hidden" runat="server" id="hdnDelIds" value="" />
            <input type="button" id="btnAdd" value="新增资料" />
            <asp:Button ID="btnSubmit" OnClick="btnSubmit_Click" runat="server" Text="保存" />
        </div>
        <%} %>
    </div>
    </form>
</body>
</html>
