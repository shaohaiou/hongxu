<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="scantypesetting.aspx.cs" Inherits="Hx.BackAdmin.scan.scantypesetting" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>扫描类型设置</title>
    <link href=<%=ResourceServer%>/css/admin.css rel="stylesheet" type="text/css" />
    <script src=<%=ResourceServer%>/js/jquery-1.3.2.min.js type="text/javascript"></script>
    <script language="javascript" type="text/javascript">
        $(function () {
            $("#cbxAllCorpPower").click(function () {
                $(".cbxCorpPower").attr("checked", $(this).attr("checked"));
                setcorppower();
            })
            $(".cbxCorpPower").click(function () {
                setcorppower();
            });
        });

        function setcorppower() {
            var corppower = $(".cbxCorpPower:checked").map(function () {
                return $(this).val();
            }).get().join('|');
            if (corppower != '')
                corppower = '|' + corppower + '|'
            $("#hdnCorpPower").val(corppower);
        }
    </script>
</head>
<body>
    <form id="form1" runat="server">
    <div class="ht_main">
        <table width="100%" border="0" cellspacing="0" cellpadding="0" class="biaoge3">
            <caption class="bt2">
                扫描类型设置</caption>
            <tbody>
                <tr>
                    <td class="tr">
                        名称：
                    </td>
                    <td>
                        <asp:TextBox runat="server" ID="txtName" CssClass="srk3"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td class="tr">
                        识别区域A点：
                    </td>
                    <td>
                        X:<asp:TextBox runat="server" ID="txtValidAreaXTop" CssClass="srk4"></asp:TextBox>
                        Y:<asp:TextBox runat="server" ID="txtValidAreaYTop" CssClass="srk4"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td class="tr">
                        识别区域B点：
                    </td>
                    <td>
                        X:<asp:TextBox runat="server" ID="txtValidAreaXBottom" CssClass="srk4"></asp:TextBox>
                        Y:<asp:TextBox runat="server" ID="txtValidAreaYBottom" CssClass="srk4"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td class="bg4 tr vt">
                        适用公司：
                    </td>
                    <td class="clpp">
                        <ul>
                            <li class="nh">
                                <label>
                                    <input type="checkbox" id="cbxAllCorpPower" class="fll" />全选</label></li>
                        </ul>
                    </td>
                </tr>
                <tr>
                    <td>
                    </td>
                    <td>
                        <asp:Repeater runat="server" ID="rptCorpPower" EnableTheming="false">
                            <HeaderTemplate>
                                <ul>
                            </HeaderTemplate>
                            <ItemTemplate>
                                <li class="blockinline" style="width:230px;">
                                    <label>
                                        <input type="checkbox" class="cbxCorpPower fll" value="<%# Eval("ID") %>"
                                            <%#SetCorpPower(Eval("ID").ToString()) %> /><%# Eval("Name") %></label></li>
                            </ItemTemplate>
                            <FooterTemplate>
                                </ul></FooterTemplate>
                        </asp:Repeater>
                    </td>
                </tr>
                <tr>
                    <td>
                    </td>
                    <td>
                        <input type="hidden" runat="server" id="hdnCorpPower" />
                        <asp:Button runat="server" ID="btnSubmit" Text="保存" CssClass="an1" OnClick="btnSubmit_Click" />
                    </td>
                </tr>
            </tbody>
        </table>
    </div>
    </form>
</body>
</html>
