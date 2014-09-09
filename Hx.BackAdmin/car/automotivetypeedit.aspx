<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="automotivetypeedit.aspx.cs"
    Inherits="Hx.BackAdmin.car.automotivetypeedit" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>有效车型设置</title>
    <link href="../css/admin.css" rel="stylesheet" type="text/css" />
    <script src="../js/jquery-1.3.2.min.js" type="text/javascript"></script>
    <script type="text/javascript" language="javascript">
        $(function () {
            $("#cbxAll").click(function () {
                $(".cbxAutomotivetype").attr("checked", $(this).attr("checked"));
                $(".cbxSuball").attr("checked", $(this).attr("checked"));
                setautomotivetypevalue();
            });

            $(".cbxAutomotivetype").click(function () {
                setautomotivetypevalue();
            });
            $(".cbxAutomotivetype").each(function () {
                if (!$(this).attr("checked"))
                    $("#cbxAll").removeAttr("checked");
            });

            $(".cbxSuball").click(function () {
                $(this).parent().parent().find(".cbxAutomotivetype").attr("checked", $(this).attr("checked"));
                setautomotivetypevalue();

            });
            $(".cbxSuball").each(function () {
                var suball = $(this);
                $(this).parent().parent().find(".cbxAutomotivetype").each(function () {
                    if (!$(this).attr("checked"))
                        suball.removeAttr("checked");
                });
            });
        });

        function setautomotivetypevalue() {
            $("#hdnAutomotivetype").val($(".cbxAutomotivetype:checked").map(function () {
                return $(this).val();
            }).get().join(','));
        }
    </script>
    <style type="text/css">
    .yxcx ul{display:inline-block;width:330px;*display:inline;*zoom:1;vertical-align:top;margin-left:3px;}
    .nh{color:White;font-weight:bold;font-size:18px;background:#222;text-indent:5px;}
    </style>
</head>
<body>
    <form id="form1" runat="server">
    <div class="ht_main">
        <table width="100%" border="0" cellspacing="0" cellpadding="0" class="biaoge3">
            <caption class="bt2">
                有效车型设置</caption>
            <tr>
                <td class="bg1">
                    品牌：
                </td>
                <td>
                    <asp:Label runat="server" ID="lblCarbrand"></asp:Label>
                </td>
            </tr>
            <tr>
                <td class="bg1 vt">
                    有效车型：
                </td>
                <td>
                    <label class="block">
                        <input type="checkbox" id="cbxAll" class="fll" checked="checked" />全选</label>
                </td>
            </tr>
            <tr>
                <td class="bg1">
                </td>
                <td class="yxcx">
                    <asp:Repeater runat="server" ID="rptAutomotivetype">
                        <ItemTemplate>
                            <%# GetNewAutomotivetypeStr(Eval("cCxmc").ToString())%>
                            <li class="blockinline" style="width: 320px; line-height: 18px;">
                                <label class="blockinline" style="line-height: 18px;">
                                    <input type="checkbox" id="cbxAutomotivetype" class="fll cbxAutomotivetype" value="<%# Eval("id") %>"
                                        <%# SetAutomotive(Eval("id").ToString()) %> />
                                    <%# Eval("cCxmc")%></label>
                            </li>
                        </ItemTemplate>
                        <FooterTemplate>
                            </ul>
                        </FooterTemplate>
                    </asp:Repeater>
                </td>
            </tr>
            <tr>
                <td class="bg1">
                </td>
                <td>
                    <input type="hidden" runat="server" id="hdnAutomotivetype" />
                    <asp:Button runat="server" ID="btnSubmit" Text="保存" CssClass="an1" OnClick="btnSubmit_Click" />
                </td>
            </tr>
            <tbody>
            </tbody>
        </table>
    </div>
    </form>
</body>
</html>
