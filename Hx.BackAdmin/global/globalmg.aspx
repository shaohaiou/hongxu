<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="globalmg.aspx.cs" Inherits="Hx.BackAdmin.global.globalmg" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>全局设置</title>
    <link href="../css/admin.css" rel="stylesheet" type="text/css" />
    <script src="../js/jquery-1.3.2.min.js" type="text/javascript"></script>
    <script language="javascript" type="text/javascript">
        $(function () {
            $("#btnRefreshCache").click(function () {
                if (confirm("确定要更新缓存吗？")) {
                    $(this).attr("disabled", "disabled");
                    $("#spmsggray").text("正在更新缓存,请勿作其他操作...");
                    $("#spmsggreen").text("");
                    $.ajax({
                        url: "backadminaction.axd",
                        data: { action: "refreshbackadmincache", d: new Date() },
                        type: 'GET',
                        dataType: "json",
                        error: function (msg) {
                            alert("操作失败");
                            $("#btnRefreshCache").removeAttr("disabled");
                            $("#spmsggray").text("");
                            $("#spmsggreen").text("");
                        },
                        success: function (data) {
                            if (data.Value == "success") {
                                $("#spmsggray").text("");
                                $("#spmsggreen").text("更新完成");
                            }
                            else {
                                alert(data.Msg);
                                $("#spmsggray").text("");
                                $("#spmsggreen").text("");
                            }
                            $("#btnRefreshCache").removeAttr("disabled");
                        }
                    });
                }
            });
        });
    </script>
</head>
<body>
    <form id="form1" runat="server">
    <div class="ht_main">
        <table width="100%" border="0" cellspacing="0" cellpadding="0" class="biaoge3">
            <tbody>
                <tr>
                    <td class="bg1">
                        后台缓存更新：
                    </td>
                    <td>
                        <input type="button" id="btnRefreshCache" name="btnRefreshCache" class="an2" value="更新" />
                        <span class="gray" id="spmsggray"></span><span class="green" id="spmsggreen"></span>
                    </td>
                </tr>
                <tr class="hidden">
                    <td class="bg1">
                        银行利率：
                    </td>
                    <td>
                        <asp:TextBox runat="server" ID="txtProfitMargin" CssClass="srk7"></asp:TextBox>
                    </td>
                </tr>
                <tr class="hidden">
                    <td class="bg1">
                    </td>
                    <td>
                        <asp:Button runat="server" ID="btnSubmit" CssClass="an1" Text="提交" OnClick="btnSubmit_Click" />
                    </td>
                </tr>
            </tbody>
        </table>
    </div>
    </form>
</body>
</html>
