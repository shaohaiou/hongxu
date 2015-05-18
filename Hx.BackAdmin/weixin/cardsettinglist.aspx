<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="cardsettinglist.aspx.cs"
    Inherits="Hx.BackAdmin.weixin.cardsettinglist" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>卡券活动管理</title>
    <link href="../css/admin.css" rel="stylesheet" type="text/css" />
    <script src="../js/jquery-1.3.2.min.js" type="text/javascript"></script>
    <script language="javascript" type="text/javascript">
        $(function () {
            $(".btnDel").click(function () {
                DelRow(this);
            });

            $("#btnAdd").click(function () {

                $("#hdnAddCount").val(parseInt($("#hdnAddCount").val()) + 1);
                $("#tblData").append("<tr><td><input type=\"text\" id=\"txtName" + $("#hdnAddCount").val() + "\" name=\"txtName" + $("#hdnAddCount").val() + "\" class=\"srk4 w120\" /></td>"
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
        <ul class="xnav">
            <li class="current"><a href="cardsettinglist.aspx">卡券活动管理</a></li>
            <asp:Repeater ID="rpcg" runat="server">
                <ItemTemplate>
                    <li><a href="cardmg.aspx?sid=<%#Eval("ID")%>">
                        <%#Eval("Name").ToString()%></a></li></ItemTemplate>
            </asp:Repeater>
        </ul>
        <table border="0" cellspacing="0" cellpadding="0" class="biaoge2" id="tblData">
            <asp:Repeater ID="rptData" runat="server" OnItemDataBound="rptData_ItemDataBound">
                <HeaderTemplate>
                    <tr class="bgbt">
                        <td class="w120">
                            活动名称
                        </td>
                        <td class="w100">
                            管理操作
                        </td>
                    </tr>
                </HeaderTemplate>
                <ItemTemplate>
                    <tr>
                        <td>
                            <input type="hidden" runat="server" id="hdnID" value='<%#Eval("ID") %>' />
                            <asp:TextBox runat="server" ID="txtName" CssClass="srk4 w120"></asp:TextBox>
                        </td>
                        <td class="lan5x">
                            <a href="javascript:void(0);" class="btnDel pl10" val="<%#Eval("ID") %>">删除</a><a
                                class="btnDel pl10" href="cardmg.aspx?sid=<%#Eval("ID") %>&from=<%=CurrentUrl %>">管理</a>
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
    </div>
    </form>
</body>
</html>
