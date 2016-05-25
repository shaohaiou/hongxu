<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="choicestgoodsmg.aspx.cs" Inherits="Hx.BackAdmin.global.choicestgoodsmg" %>

<%@ Register Assembly="AspNetPager" Namespace="Wuqi.Webdiyer" TagPrefix="webdiyer" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>汽车用品</title>
    <link href=<%=ResourceServer%>/css/admin.css rel="stylesheet" type="text/css" />
    <script src=<%=ResourceServer%>/js/jquery-1.3.2.min.js type="text/javascript"></script>
    <script language="javascript" type="text/javascript">
        $(function () {
            $(".btnDel").click(function () {
                DelRow(this);
            });

            $("#btnAdd").click(function () {
                $("#hdnAddCount").val(parseInt($("#hdnAddCount").val()) + 1);
                $("#tblData").append("<tr><td><input type=\"text\" id=\"txtName" + $("#hdnAddCount").val() + "\" name=\"txtName" + $("#hdnAddCount").val() + "\" class=\"srk1 w120\" value=\"\" /></td>"
                 + "<td><input type=\"text\" id=\"txtPrice" + $("#hdnAddCount").val() + "\" name=\"txtPrice" + $("#hdnAddCount").val() + "\" class=\"srk1 w60\" value=\"\" /></td>"
                 + "<td><input type=\"text\" id=\"txtProductType" + $("#hdnAddCount").val() + "\" name=\"txtProductType" + $("#hdnAddCount").val() + "\" class=\"srk1 w60\" value=\"\" /></td>"
                 + "<td><input type=\"text\" id=\"txtInpoint" + $("#hdnAddCount").val() + "\" name=\"txtInpoint" + $("#hdnAddCount").val() + "\" class=\"srk1 w60\" value=\"\" /></td>"
                 + "<td><select type=\"text\" id=\"ddlCorporation" + $("#hdnAddCount").val() + "\" name=\"ddlCorporation" + $("#hdnAddCount").val() + "\">" + $("#ddlCorporationFilter").html() + "</select></td>"
                 + "<td><input type=\"text\" id=\"txtRemark" + $("#hdnAddCount").val() + "\" name=\"txtRemark" + $("#hdnAddCount").val() + "\" class=\"srk1 w240\" value=\"\" /></td>"
                 + "<td><a href=\"javascript:void(0);\" class=\"btnDel\" val=\"0\">删除</a> </td></tr>");
                $(".btnDel").unbind("click").click(function () {
                    DelRow(this);
                });
            });
        });

        function DelRow(obj) {
            if ($(obj).attr("val") > 0) {
                $("#hdnDelIds").val($("#hdnDelIds").val() + ($("#hdnDelIds").val() == "" ? "" : ",") + $(obj).attr("val"));
            }
            $(obj).parent().parent().remove();
        }
    </script>
</head>
<body>
    <form id="form1" runat="server">
    <div class="ht_main">
        <%if(Admin.Administrator){ %>
        <ul class="xnav">
            <li><a href="sybxmg.aspx">商业保险</a></li>
            <li><a href="bankingmg.aspx">金融方案</a></li>
            <li class="current"><a href="choicestgoodsmg.aspx">汽车用品</a></li>
            <li><a href="giftmg.aspx">礼品</a></li>
            <li><a href="bankmg.aspx">银行</a></li>
            <li><a href="corporationmg.aspx">公司管理</a></li>
        </ul>
        <%} %>
        <table width="820" border="0" cellspacing="0" cellpadding="0" class="biaoge4" style="background-color: #f4f8fc;">
            <tr>
                <td class="w40 bold">
                    查询：
                </td>
                <td>
                    公司：<asp:DropDownList ID="ddlCorporationFilter" runat="server">
                    </asp:DropDownList>
                </td>
            </tr>
            <tr>
                <td>
                </td>
                <td>
                    <asp:Button runat="server" ID="btnFilter" CssClass="an1" Text="确定" OnClick="btnFilter_Click" />
                </td>
            </tr>
        </table>
        <table border="0" cellspacing="0" cellpadding="0" class="biaoge2" id="tblData">
            <asp:Repeater ID="rptData" runat="server" OnItemDataBound="rptData_ItemDataBound">
                <HeaderTemplate>
                    <tr class="bgbt">
                        <td class="w120">
                            产品名称
                        </td>
                        <td class="w60">
                            销售价
                        </td>
                        <td class="w60">
                            产品类型
                        </td>
                        <td class="w60">
                            适用车型
                        </td>
                        <td class="w120">
                            所属公司
                        </td>
                        <td class="w240">
                            备注
                        </td>
                        <td class="w60">
                            操作
                        </td>
                    </tr>
                </HeaderTemplate>
                <ItemTemplate>
                    <tr>
                        <td>
                            <input type="hidden" runat="server" id="hdnID" value='<%#Eval("ID") %>' />
                            <input type="text" runat="server" id="txtName" class="srk1 w120" value='<%#Eval("Name") %>' />
                        </td>
                        <td>
                            <input type="text" runat="server" id="txtPrice" class="srk1 w60" value='<%#Eval("Price")%>' />
                        </td>
                        <td>
                            <input type="text" runat="server" id="txtProductType" class="srk1 w60" value='<%#Eval("ProductType")%>' />
                        </td>
                        <td>
                            <input type="text" runat="server" id="txtInpoint" class="srk1 w60" value='<%#Eval("Inpoint")%>' />
                        </td>
                        <td>
                            <asp:DropDownList runat="server" ID="ddlCorporation">
                            </asp:DropDownList>
                        </td>
                        <td>
                            <input type="text" runat="server" id="txtRemark" class="srk1 w240" value='<%#Eval("Remark")%>' />
                        </td>
                        <td>
                            <a href="javascript:void(0);" class="btnDel pl10" val="<%#Eval("ID") %>">删除</a>
                        </td>
                    </tr>
                </ItemTemplate>
            </asp:Repeater>
        </table>
        <webdiyer:AspNetPager ID="search_fy" UrlPaging="true" NextPageText="下一页" PrevPageText="上一页"
            CurrentPageButtonClass="current" PageSize="10" runat="server" NumericButtonType="Text"
            MoreButtonType="Text" ShowFirstLast="false" HorizontalAlign="Left" AlwaysShow="false"
            ShowDisabledButtons="False" PagingButtonSpacing="">
        </webdiyer:AspNetPager>
        <div class="lan5x" style="padding-top: 10px;">
            <input type="hidden" runat="server" id="hdnAddCount" value="0" />
            <input type="hidden" runat="server" id="hdnDelIds" value="" />
            <input class="an1" type="button" value="添加" id="btnAdd" />
            <asp:Button ID="btnSubmit" OnClick="btnSubmit_Click" CssClass="an1" runat="server"
                Text="保存" />
        </div>
    </div>
    </form>
</body>
</html>
