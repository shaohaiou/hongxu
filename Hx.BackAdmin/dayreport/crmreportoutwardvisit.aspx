<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="crmreportoutwardvisit.aspx.cs" Inherits="Hx.BackAdmin.dayreport.crmreportoutwardvisit" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>活动或外出访问客户信息</title>
    <link href="../css/admin.css" rel="stylesheet" type="text/css" />
    <script src="../js/jquery-1.3.2.min.js" type="text/javascript"></script>
    <script src="../js/comm.js" type="text/javascript"></script>
    <script src="../js/WdatePicker.js" type="text/javascript"></script>
    <script type="text/javascript">
        var timer_bindevent;

        $(function () {
            bindevent();

            $("#txtDate").click(function () {
                WdatePicker({ 'readOnly': 'true', dateFmt: 'yyyy-MM', isShowToday: false, isShowClear: false });
            });

            $(".btnDel").click(function () {
                DelRow(this);
            });

            $(".btnAdd").click(function () {
                $("#hdnAddCount").val(parseInt($("#hdnAddCount").val()) + 1);
                $("#tblData").append("<tr class=\"trcontent\"><td></td>"
                    + "<td><input type=\"text\" id=\"txtDate" + $("#hdnAddCount").val() + "\" name=\"txtDate" + $("#hdnAddCount").val() + "\" class=\"srk1 w70 date\" value=\"" + '<%= DateTime.Today.ToString("yyyy-MM-dd") %>' + "\" /></td>"
                    + "<td><select id=\"ddlOVCustomerNature" + $("#hdnAddCount").val() + "\" name=\"ddlOVCustomerNature" + $("#hdnAddCount").val() + "\">" + $("#ddlOVCustomerNature").html() + "</select></td>"
                    + "<td><input type=\"text\" id=\"txtOVActiveName" + $("#hdnAddCount").val() + "\" name=\"txtOVActiveName" + $("#hdnAddCount").val() + "\" class=\"srk1 w80\" value=\"\" /></td>"
                    + "<td><input type=\"text\" id=\"txtOVCustomerName" + $("#hdnAddCount").val() + "\" name=\"txtOVCustomerName" + $("#hdnAddCount").val() + "\" class=\"srk1 w40\" value=\"\" /></td>"
                    + "<td><input type=\"text\" id=\"txtOVPhoneNum" + $("#hdnAddCount").val() + "\" name=\"txtOVPhoneNum" + $("#hdnAddCount").val() + "\" class=\"srk1 w80\" value=\"\" /></td>"
                    + "<td><input type=\"text\" id=\"txtOVReceiver" + $("#hdnAddCount").val() + "\" name=\"txtOVReceiver" + $("#hdnAddCount").val() + "\" class=\"srk1 w40\" value=\"\" /></td>"
                    + "<td><input type=\"text\" id=\"txtOVIntentionModel" + $("#hdnAddCount").val() + "\" name=\"txtOVIntentionModel" + $("#hdnAddCount").val() + "\" class=\"srk1 w60\" value=\"\" /></td>"
                    + "<td><select id=\"ddlOVVisitChannel" + $("#hdnAddCount").val() + "\" name=\"ddlOVVisitChannel" + $("#hdnAddCount").val() + "\">" + $("#ddlOVVisitChannel").html() + "</select></td>"
                    + "<td><select id=\"ddlOVLevel" + $("#hdnAddCount").val() + "\" name=\"ddlOVLevel" + $("#hdnAddCount").val() + "\">" + $("#ddlOVLevel").html() + "</select></td>"
                    + "<td><input type=\"checkbox\" id=\"cbxOVIsRide" + $("#hdnAddCount").val() + "\" name=\"cbxOVIsRide" + $("#hdnAddCount").val() + "\" value=\"0\"  class=\"cbxnew\" /><input type=\"hidden\" id=\"hdnOVIsRide" + $("#hdnAddCount").val() + "\" name=\"hdnOVIsRide" + $("#hdnAddCount").val() + "\" value=\"0\" /></td>"
                    + "<td><select id=\"ddlOVCardInfo" + $("#hdnAddCount").val() + "\" name=\"ddlOVCardInfo" + $("#hdnAddCount").val() + "\">" + $("#ddlOVCardInfo").html() + "</select></td>"
                    + "<td><input type=\"text\" id=\"txtOVVisitNexttime" + $("#hdnAddCount").val() + "\" name=\"txtOVVisitNexttime" + $("#hdnAddCount").val() + "\" class=\"srk1 w70 nextdate\" value=\"\" /></td>"
                    + "<td><a href=\"javascript:void(0);\" class=\"btnDel\" val=\"0\">删除</a></td></tr>");
                $(".btnDel").unbind("click").click(function () {
                    DelRow(this);
                });
                $(".cbxnew").unbind("click").click(function () {
                    $(this).next().val($(this).attr("checked") ? "1" : "0");
                });
            });
        });

        function DelRow(obj) {
            if ($(obj).attr("val") > 0) {
                $("#hdnDelIds").val($("#hdnDelIds").val() + ($("#hdnDelIds").val() == "" ? "" : ",") + $(obj).attr("val"));
            }
            $(obj).parent().parent().remove();
        }

        function bindevent() {
            $(".date").unbind("click");
            $(".date").click(function () {
                WdatePicker({ 'readOnly': 'true', dateFmt: 'yyyy-MM-dd', maxDate: '<%=DateTime.Today.ToString("yyyy-MM-dd") %>' });
            });
            $(".nextdate").unbind("click");
            $(".nextdate").click(function () {
                WdatePicker({ 'readOnly': 'true', dateFmt: 'yyyy-MM-dd' });
            });

            if (timer_bindevent)
                clearTimeout(timer_bindevent);
            timer_bindevent = setTimeout(function () {
                bindevent();
            }, 500);
        }
    </script>
</head>
<body>
    <div class="ht_main" style="padding-top: 35px;">
        <ul class="xnav fixed">
            <asp:Repeater runat="server" ID="rptNav">
                <HeaderTemplate>
                    <%if (CurrentUser.CRMReportExportPowerSetting == "1")
                      {%>
                    <li><a href="crmreportexport.aspx?Nm=<%=Nm %>&Id=<%=Id %>&Mm=<%=Mm %>">报表导出</a></li>
                    <%} %>
                </HeaderTemplate>
                <ItemTemplate>
                    <li <%# HasReportInputPower(Eval("Value")) ? string.Empty : "class=\"hide\"" %> <%# (Hx.Components.Enumerations.CRMReportType)int.Parse(Eval("Value").ToString()) == Hx.Components.Enumerations.CRMReportType.活动外出访客信息 ? "class=\"current\"" : string.Empty%>>
                        <a href="<%#GetNavUrl(Eval("Value")) %>">
                            <%# Eval("Text") %></a></li>
                </ItemTemplate>
            </asp:Repeater>
        </ul>
        <form id="form1" runat="server">
        <table width="100%" border="0" cellspacing="0" cellpadding="0" class="biaoge3">
            <tr>
                <td class="bg4 tr">
                    公司：
                </td>
                <td>
                    <asp:DropDownList runat="server" ID="ddlCorp" AutoPostBack="true" OnSelectedIndexChanged="ddlCorp_SelectedIndexChanged">
                    </asp:DropDownList>
                </td>
            </tr>
            <tr>
                <td class="bg4 tr">
                    月份：
                </td>
                <td>
                    <asp:TextBox runat="server" ID="txtDate" CssClass="srk6" OnTextChanged="txtDate_TextChanged"
                        AutoPostBack="true"></asp:TextBox>
                </td>
            </tr>
        </table>
        <div class="lan5x" style="padding-top: 10px;">
            <input class="an1 btnAdd" type="button" value="添加" />
            <asp:Button ID="btnSubmit1" OnClick="btnSubmit_Click" CssClass="an1" runat="server"
                Text="保存" />
        </div>
        <table border="0" cellspacing="0" cellpadding="0" class="biaoge2" id="tblData" width="1000">
            <asp:Repeater ID="rptData" runat="server" OnItemDataBound="rptData_ItemDataBound">
                <HeaderTemplate>
                    <tr class="bgbt">
                        <td>
                            序号
                        </td>
                        <td>
                            日期
                        </td>
                        <td class="w60">
                            客户性质
                        </td>
                        <td class="w60">
                            活动名称
                        </td>
                        <td class="w60 " >
                            客户姓名
                        </td>
                        <td>
                            客户电话
                        </td>
                        <td class="w60">
                            销售顾问
                        </td>
                        <td>
                            拟购车型
                        </td>
                        <td>
                            客户信息来源
                        </td>
                        <td>
                            意向级别
                        </td>
                        <td class="w80">
                            是否试乘试驾
                        </td>
                        <td>
                            建卡情况
                        </td>
                        <td class="w80">
                            下次跟踪时间
                        </td>
                        <td class="w40">
                            操作
                        </td>
                    </tr>
                </HeaderTemplate>
                <ItemTemplate>
                    <tr class="trcontent">
                        <td>
                            <%# Container.ItemIndex + 1 %><input type="hidden" runat="server" id="hdnID" value='<%#Eval("ID") %>' />
                        </td>
                        <td>
                            <input type="text" runat="server" id="txtDate" class="srk1 w70 date" value='<%# Eval("Date","{0:yyyy-MM-dd}") %>' />
                        </td>
                        <td>
                            <asp:DropDownList runat="server" ID="ddlOVCustomerNature">
                            </asp:DropDownList>
                        </td>
                        <td>
                            <input type="text" runat="server" id="txtOVActiveName" class="srk1 w80" value='<%#Eval("OVActiveName") %>' />
                        </td>
                        <td>
                            <input type="text" runat="server" id="txtOVCustomerName" class="srk1 w40" value='<%#Eval("OVCustomerName") %>' />
                        </td>
                        <td>
                            <input type="text" runat="server" id="txtOVPhoneNum" class="srk1 w80" value='<%#Eval("OVPhoneNum") %>' />
                        </td>
                        <td>
                            <input type="text" runat="server" id="txtOVReceiver" class="srk1 w40" value='<%#Eval("OVReceiver") %>' />
                        </td>
                        <td>
                            <input type="text" runat="server" id="txtOVIntentionModel" class="srk1 w60" value='<%#Eval("OVIntentionModel") %>' />
                        </td>
                        <td>
                            <asp:DropDownList runat="server" ID="ddlOVVisitChannel">
                            </asp:DropDownList>
                        </td>
                        <td>
                            <asp:DropDownList runat="server" ID="ddlOVLevel">
                            </asp:DropDownList>
                        </td>
                        <td>
                            <input type="checkbox" runat="server" id="cbxOVIsRide" checked='<%# Eval("OVIsRide").ToString() == "1"%>' />
                        </td>
                        <td>
                            <asp:DropDownList runat="server" ID="ddlOVCardInfo">
                            </asp:DropDownList>
                        </td>
                        <td>
                            <input type="text" runat="server" id="txtOVVisitNexttime" class="srk1 w70 nextdate" value='<%# Eval("OVVisitNexttime") %>' />
                        </td>
                        <td>
                            <a href="javascript:void(0);" class="btnDel" val="<%#Eval("ID") %>">删除</a>
                        </td>
                    </tr>
                </ItemTemplate>
            </asp:Repeater>
        </table>
        <div class="lan5x" style="padding-top: 10px;">
            <input type="hidden" runat="server" id="hdnAddCount" value="0" />
            <input type="hidden" runat="server" id="hdnDelIds" value="" />
            <input class="an1 btnAdd" type="button" value="添加" />
            <asp:Button ID="btnSubmit" OnClick="btnSubmit_Click" CssClass="an1" runat="server"
                Text="保存" />
        </div>
        <div class="hidden">
            <asp:DropDownList runat="server" ID="ddlOVCustomerNature">
            </asp:DropDownList>
            <asp:DropDownList runat="server" ID="ddlOVVisitChannel">
            </asp:DropDownList>
            <asp:DropDownList runat="server" ID="ddlOVLevel">
            </asp:DropDownList>
            <asp:DropDownList runat="server" ID="ddlOVCardInfo">
            </asp:DropDownList>
        </div>
        </form>
    </div>
</body>
</html>

