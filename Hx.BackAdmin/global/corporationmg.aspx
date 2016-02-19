<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="corporationmg.aspx.cs"
    Inherits="Hx.BackAdmin.global.corporationmg" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>公司管理</title>
    <link href=<%=ResourceServer%>/css/admin.css rel="stylesheet" type="text/css" />
    <script src=<%=ResourceServer%>/js/jquery-1.3.2.min.js type="text/javascript"></script>
    <script language="javascript" type="text/javascript">
        $(function () {
            $(".btnDel").click(function () {
                DelRow(this);
            });

            $("#btnAdd").click(function () {
                var maxsort = 0;
                $(".sort").each(function () {
                    if (parseInt($(this).val()) > maxsort)
                        maxsort = parseInt($(this).val());
                });

                $("#hdnAddCount").val(parseInt($("#hdnAddCount").val()) + 1);
                $("#tblbank").append("<tr><td><input type=\"text\" id=\"txtSort" + $("#hdnAddCount").val() + "\" name=\"txtSort" + $("#hdnAddCount").val() + "\" class=\"srk4 sort\" value=\"" + (maxsort + 1) + "\" oldval=\"" + (maxsort + 1) + "\"\"/></td>"
                 + "<td><input type=\"text\" id=\"txtName" + $("#hdnAddCount").val() + "\" name=\"txtName" + $("#hdnAddCount").val() + "\" class=\"srk1 w160\" value=\"\" /></td>"
                 + "<td></td><td></td><td><a href=\"javascript:void(0);\" class=\"btnDel pl10\" val=\"0\">删除</a> </td></tr>");
                $(".btnDel").unbind("click").click(function () {
                    DelRow(this);
                });

                BindSortChange();
            });

            $(".viewcarbrand").click(function () {
                $("#showhidecarbrand").show();
                var selids = $(this).attr("data").split("|");
                $(".hdncarbrand").each(function () {
                    for (var i = 0; i < selids.length; i++) {
                        if ($(this).val() == selids[i]) {
                            $(this).parent().show();
                            break;
                        }
                    }
                });

                var left = $(this).offset().left + 40;
                var top = $(this).offset().top;
                var wheight = $(window).height();
                var height = $("#showhidecarbrand").height();
                if (height > wheight)
                    top = $(window).scrollTop() ;
                $("#showhidecarbrand").css({ left: left, top: top });
            }).mouseleave(function () {
                $("#showhidecarbrand").hide();
                $(".hdncarbrand").each(function () {
                    $(this).parent().hide();
                });
            });

            BindSortChange();
        });

        function DelRow(obj) {
            if ($(obj).attr("val") > 0) {
                $("#hdnDelIds").val($("#hdnDelIds").val() + ($("#hdnDelIds").val() == "" ? "" : ",") + $(obj).attr("val"));
            }
            $(obj).parent().parent().remove();
        }

        function BindSortChange() {
            $(".sort").unbind("change");

            $(".sort").change(function () {
                var oldval = parseInt($(this).attr("oldval"));
                var newval = parseInt($(this).val());
                $(".sort").not(this).each(function () {
                    if (newval > oldval && oldval > 0 && parseInt($(this).val()) > oldval && parseInt($(this).val()) <= newval) {
                        $(this).val(parseInt($(this).val()) - 1);
                        $(this).attr("oldval", $(this).val());
                    }
                    else if (newval < oldval && oldval > 0 && parseInt($(this).val()) < oldval && parseInt($(this).val()) >= newval) {
                        $(this).val(parseInt($(this).val()) + 1);
                        $(this).attr("oldval", $(this).val());
                    }
                });
                $(this).attr("oldval", $(this).val());
            });
        }
    </script>
</head>
<body>
    <form id="form1" runat="server">
    <div class="ht_main">
        <ul class="xnav">
            <li><a href="sybxmg.aspx">商业保险</a></li>
            <li><a href="bankingmg.aspx">金融方案</a></li>
            <li><a href="choicestgoodsmg.aspx">汽车用品</a></li>
            <li><a href="giftmg.aspx">礼品</a></li>
            <li><a href="bankmg.aspx">银行</a></li>
            <li class="current"><a href="corporationmg.aspx">公司管理</a></li>
        </ul>
        <table width="500" border="0" cellspacing="0" cellpadding="0" class="biaoge2" id="tblbank">
            <asp:Repeater ID="rptCorporation" runat="server" OnItemDataBound="rptCorporation_ItemDataBound">
                <HeaderTemplate>
                    <tr class="bgbt">
                        <td>
                            排序
                        </td>
                        <td>
                            公司名称
                        </td>
                        <td>
                            贷款默认设置
                        </td>
                        <td>
                            相关车辆品牌
                        </td>
                        <td>
                            操作
                        </td>
                    </tr>
                </HeaderTemplate>
                <ItemTemplate>
                    <tr>
                        <td>
                            <input type="hidden" runat="server" id="hdnID" value='<%#Eval("ID") %>' />
                            <asp:TextBox runat="server" ID="txtSort" CssClass="srk4 sort"></asp:TextBox>
                        </td>
                        <td>
                            <input type="text" runat="server" id="txtName" class="srk1 w160" value='<%#Eval("Name") %>' />
                        </td>
                        <td>
                            <%# Eval("Bank")%>
                            <%# Eval("BankProfitMargin")%>
                        </td>
                        <td>
                            <a href="javascript:void(0);" data="<%# Eval("CarBrand")%>" class="viewcarbrand"
                                style="padding: 0 10px;">查看</a>
                        </td>
                        <td>
                            <a href="javascript:void(0);" class="btnDel pl10" val="<%#Eval("ID") %>">删除</a>
                            <a href="corporationedit.aspx?id=<%#Eval("ID") %>&from=<%=CurrentUrl %>">
                                编辑</a>
                        </td>
                    </tr>
                </ItemTemplate>
            </asp:Repeater>
        </table>
        <div class="lan5x" style="padding-top: 10px;">
            <input type="hidden" runat="server" id="hdnAddCount" value="0" />
            <input type="hidden" runat="server" id="hdnDelIds" value="" />
            <input class="an1" type="button" value="添加" id="btnAdd" />
            <asp:Button ID="btnSubmit" OnClick="btnSubmit_Click" CssClass="an1" runat="server"
                Text="保存" />
        </div>
        <div id="showhidecarbrand" style="display: none; position: absolute; width: 200px;
            background: #fff; border: 3px solid #ccc;">
            <div>
                <ul class="gl_k_nr" style="display: inline-block; width: 200px; margin-left: 20px;">
                    <asp:Repeater runat="server" ID="rptcarbrand">
                        <ItemTemplate>
                            <li class="gl_k_nr_s" style="display: none;">
                                <input type="hidden" value="<%# Eval("Name") %>" class="hdncarbrand" />
                                <%# Eval("Name") %>
                            </li>
                        </ItemTemplate>
                    </asp:Repeater>
                </ul>
            </div>
        </div>
    </div>
    </form>
</body>
</html>
