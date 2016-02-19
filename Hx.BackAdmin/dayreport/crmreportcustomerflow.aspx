<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="crmreportcustomerflow.aspx.cs"
    EnableViewState="true" Inherits="Hx.BackAdmin.dayreport.crmreportcustomerflow" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>展厅来店(电)客流量登记表</title>
    <link href=<%=ResourceServer%>/css/admin.css rel="stylesheet" type="text/css" />
    <script src=<%=ResourceServer%>/js/jquery-1.3.2.min.js type="text/javascript"></script>
    <script src=<%=ResourceServer%>/js/comm.js type="text/javascript"></script>
    <script src="/js/WdatePicker.js" type="text/javascript"></script>
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
                    + "<td><select id=\"ddlCFVisitway" + $("#hdnAddCount").val() + "\" name=\"ddlCFVisitway" + $("#hdnAddCount").val() + "\">" + $("#ddlCFVisitway").html() + "</select></td>"
                    + "<td><input type=\"text\" id=\"txtCFVisitTime" + $("#hdnAddCount").val() + "\" name=\"txtCFVisitTime" + $("#hdnAddCount").val() + "\" class=\"srk1 w40 time\" value=\"\" /></td>"
                    + "<td><input type=\"text\" id=\"txtCFLeaveTime" + $("#hdnAddCount").val() + "\" name=\"txtCFLeaveTime" + $("#hdnAddCount").val() + "\" class=\"srk1 w40 time\" value=\"\" /></td>"
                    + "<td><input type=\"text\" id=\"txtCFReceiver" + $("#hdnAddCount").val() + "\" name=\"txtCFReceiver" + $("#hdnAddCount").val() + "\" class=\"srk1 w40\" value=\"\" /></td>"
                    + "<td><input type=\"text\" id=\"txtCFCustomerName" + $("#hdnAddCount").val() + "\" name=\"txtCFCustomerName" + $("#hdnAddCount").val() + "\" class=\"srk1 w40\" value=\"\" /></td>"
                    + "<td><select id=\"ddlCFVisitNature" + $("#hdnAddCount").val() + "\" name=\"ddlCFVisitNature" + $("#hdnAddCount").val() + "\">" + $("#ddlCFVisitNature").html() + "</select></td>"
                    + "<td><input type=\"text\" id=\"txtCFPhoneNum" + $("#hdnAddCount").val() + "\" name=\"txtCFPhoneNum" + $("#hdnAddCount").val() + "\" class=\"srk1 w80\" value=\"\" /></td>"
                    + "<td><input type=\"text\" id=\"txtCFIntentionModel" + $("#hdnAddCount").val() + "\" name=\"txtCFIntentionModel" + $("#hdnAddCount").val() + "\" class=\"srk1 w60\" value=\"\" /></td>"
                    + "<td><select id=\"ddlCFVisitChannel" + $("#hdnAddCount").val() + "\" name=\"ddlCFVisitChannel" + $("#hdnAddCount").val() + "\">" + $("#ddlCFVisitChannel").html() + "</select></td>"
                    + "<td><select id=\"ddlCFBuyType" + $("#hdnAddCount").val() + "\" name=\"ddlCFBuyType" + $("#hdnAddCount").val() + "\">" + $("#ddlCFBuyType").html() + "</select></td>"
                    + "<td><input type=\"text\" id=\"txtCFModelInUse" + $("#hdnAddCount").val() + "\" name=\"txtCFModelInUse" + $("#hdnAddCount").val() + "\" class=\"srk1 w60\" value=\"\" /></td>"
                    + "<td><input type=\"checkbox\" id=\"cbxCFIsLoan" + $("#hdnAddCount").val() + "\" name=\"cbxCFIsLoan" + $("#hdnAddCount").val() + "\" class=\"cbxnew\" /><input type=\"hidden\" id=\"hdnCFIsLoan" + $("#hdnAddCount").val() + "\" name=\"hdnCFIsLoan" + $("#hdnAddCount").val() + "\" value=\"0\" /></td>"
                    + "<td><input type=\"text\" id=\"txtCFFromArea" + $("#hdnAddCount").val() + "\" name=\"txtCFFromArea" + $("#hdnAddCount").val() + "\" class=\"srk1 w60\" value=\"\" /></td>"
                    + "<td><select id=\"ddlCFLevel" + $("#hdnAddCount").val() + "\" name=\"ddlCFLevel" + $("#hdnAddCount").val() + "\">" + $("#ddlCFLevel").html() + "</select></td>"
                    + "<td><select id=\"ddlCFCardInfo" + $("#hdnAddCount").val() + "\" name=\"ddlCFCardInfo" + $("#hdnAddCount").val() + "\">" + $("#ddlCFCardInfo").html() + "</select></td>"
                    + "<td><input type=\"checkbox\" id=\"cbxCFIsRide" + $("#hdnAddCount").val() + "\" name=\"cbxCFIsRide" + $("#hdnAddCount").val() + "\" value=\"0\"  class=\"cbxnew\" /><input type=\"hidden\" id=\"hdnCFIsRide" + $("#hdnAddCount").val() + "\" name=\"hdnCFIsRide" + $("#hdnAddCount").val() + "\" value=\"0\" /></td>"
                    + "<td><input type=\"checkbox\" id=\"cbxCFIsOrder" + $("#hdnAddCount").val() + "\" name=\"cbxCFIsOrder" + $("#hdnAddCount").val() + "\" value=\"0\"  class=\"cbxnew\" /><input type=\"hidden\" id=\"hdnCFIsOrder" + $("#hdnAddCount").val() + "\" name=\"hdnCFIsOrder" + $("#hdnAddCount").val() + "\" value=\"0\" /></td>"
                    + "<td><input type=\"checkbox\" id=\"cbxCFIsInvoice" + $("#hdnAddCount").val() + "\" name=\"cbxCFIsInvoice" + $("#hdnAddCount").val() + "\" value=\"0\"  class=\"cbxnew\" /><input type=\"hidden\" id=\"hdnCFIsInvoice" + $("#hdnAddCount").val() + "\" name=\"hdnCFIsInvoice" + $("#hdnAddCount").val() + "\" value=\"0\" /></td>"
                    + "<td><input type=\"checkbox\" id=\"cbxCFIsTurnover" + $("#hdnAddCount").val() + "\" name=\"cbxCFIsTurnover" + $("#hdnAddCount").val() + "\" value=\"0\"  class=\"cbxnew\" /><input type=\"hidden\" id=\"hdnCFIsTurnover" + $("#hdnAddCount").val() + "\" name=\"hdnCFIsTurnover" + $("#hdnAddCount").val() + "\" value=\"0\" /></td>"
                    + "<td><a href=\"javascript:void(0);\" class=\"btnDel hide flayopt\" val=\"0\">删除</a></td></tr>");
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
            $(".time").unbind("click");
            $(".time").click(function () {
                WdatePicker({ 'readOnly': 'true', dateFmt: 'HH:mm' });
            });

            $(".trcontent").unbind("hover");
            $(".trcontent").hover(function () {
                $(this).find(".flayopt").css("right",-$("body").scrollLeft()).show();
            }, function () {
                $(this).find(".flayopt").hide();
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
                    <li <%# HasReportInputPower(Eval("Value")) ? string.Empty : "class=\"hide\"" %> <%# (Hx.Components.Enumerations.CRMReportType)int.Parse(Eval("Value").ToString()) == Hx.Components.Enumerations.CRMReportType.客流量登记表 ? "class=\"current\"" : string.Empty%>>
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
        <table border="0" cellspacing="0" cellpadding="0" class="biaoge2" id="tblData" width="1420">
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
                            到访方式
                        </td>
                        <td class="w60">
                            来访时间
                        </td>
                        <td class="w60">
                            离去时间
                        </td>
                        <td class="w60">
                            销售顾问
                        </td>
                        <td class="w60 " >
                            客户姓名
                        </td>
                        <td>
                            来访性质
                        </td>
                        <td>
                            联系电话
                        </td>
                        <td>
                            意向车型
                        </td>
                        <td>
                            来店渠道
                        </td>
                        <td>
                            购入类型
                        </td>
                        <td>
                            现用车型
                        </td>
                        <td class="w40">
                            贷款
                        </td>
                        <td>
                            来自区域
                        </td>
                        <td>
                            级别
                        </td>
                        <td>
                            建卡情况
                        </td>
                        <td class="w60">
                            试乘试驾
                        </td>
                        <td class="w40">
                            订单
                        </td>
                        <td class="w40">
                            开发票
                        </td>
                        <td class="w40">
                            交车
                        </td>
                        <td class="w40 block">
                            
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
                            <asp:DropDownList runat="server" ID="ddlCFVisitway">
                            </asp:DropDownList>
                        </td>
                        <td>
                            <input type="text" runat="server" id="txtCFVisitTime" class="srk1 w40 time" value='<%#Eval("CFVisitTime") %>' />
                        </td>
                        <td>
                            <input type="text" runat="server" id="txtCFLeaveTime" class="srk1 w40 time" value='<%#Eval("CFLeaveTime") %>' />
                        </td>
                        <td>
                            <input type="text" runat="server" id="txtCFReceiver" class="srk1 w40" value='<%#Eval("CFReceiver") %>' />
                        </td>
                        <td>
                            <input type="text" runat="server" id="txtCFCustomerName" class="srk1 w40" value='<%#Eval("CFCustomerName") %>' />
                        </td>
                        <td>
                            <asp:DropDownList runat="server" ID="ddlCFVisitNature">
                            </asp:DropDownList>
                        </td>
                        <td>
                            <input type="text" runat="server" id="txtCFPhoneNum" class="srk1 w80" value='<%#Eval("CFPhoneNum") %>' />
                        </td>
                        <td>
                            <input type="text" runat="server" id="txtCFIntentionModel" class="srk1 w60" value='<%#Eval("CFIntentionModel") %>' />
                        </td>
                        <td>
                            <asp:DropDownList runat="server" ID="ddlCFVisitChannel">
                            </asp:DropDownList>
                        </td>
                        <td>
                            <asp:DropDownList runat="server" ID="ddlCFBuyType">
                            </asp:DropDownList>
                        </td>
                        <td>
                            <input type="text" runat="server" id="txtCFModelInUse" class="srk1 w60" value='<%#Eval("CFModelInUse") %>' />
                        </td>
                        <td>
                            <input type="checkbox" runat="server" id="cbxCFIsLoan" checked='<%# Eval("CFIsLoan").ToString() == "1"%>' />
                        </td>
                        <td>
                            <input type="text" runat="server" id="txtCFFromArea" class="srk1 w60" value='<%#Eval("CFFromArea") %>' />
                        </td>
                        <td>
                            <asp:DropDownList runat="server" ID="ddlCFLevel">
                            </asp:DropDownList>
                        </td>
                        <td>
                            <asp:DropDownList runat="server" ID="ddlCFCardInfo">
                            </asp:DropDownList>
                        </td>
                        <td>
                            <input type="checkbox" runat="server" id="cbxCFIsRide" checked='<%# Eval("CFIsRide").ToString() == "1"%>' />
                        </td>
                        <td>
                            <input type="checkbox" runat="server" id="cbxCFIsOrder" checked='<%# Eval("CFIsOrder").ToString() == "1"%>' />
                        </td>
                        <td>
                            <input type="checkbox" runat="server" id="cbxCFIsInvoice" checked='<%# Eval("CFIsInvoice").ToString() == "1"%>' />
                        </td>
                        <td>
                            <input type="checkbox" runat="server" id="cbxCFIsTurnover" checked='<%# Eval("CFIsTurnover").ToString() == "1" %>' />
                        </td>
                        <td>
                            <a href="javascript:void(0);" class="btnDel hide flayopt" val="<%#Eval("ID") %>">删除</a>
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
            <asp:DropDownList runat="server" ID="ddlCFVisitway">
            </asp:DropDownList>
            <asp:DropDownList runat="server" ID="ddlCFVisitNature">
            </asp:DropDownList>
            <asp:DropDownList runat="server" ID="ddlCFVisitChannel">
            </asp:DropDownList>
            <asp:DropDownList runat="server" ID="ddlCFBuyType">
            </asp:DropDownList>
            <asp:DropDownList runat="server" ID="ddlCFLevel">
            </asp:DropDownList>
            <asp:DropDownList runat="server" ID="ddlCFCardInfo">
            </asp:DropDownList>
        </div>
        </form>
    </div>
</body>
</html>
