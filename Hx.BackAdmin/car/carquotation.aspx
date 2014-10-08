﻿<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="carquotation.aspx.cs" Inherits="Hx.BackAdmin.car.carquotation"
    EnableViewState="true" %>

<%@ Import Namespace="Hx.Car.Enum" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>全款购车</title>
    <link href="../css/admin.css" rel="stylesheet" type="text/css" />
    <link href="../css/calculator/calculator.css" rel="stylesheet" type="text/css" />
    <script src="../js/jquery-1.3.2.min.js" type="text/javascript"></script>
    <script src="../js/calculator.js" type="text/javascript"></script>
    <script language="javascript" type="text/javascript">
        var curnumberbox;
        var timer_bindevent;
        $(function () {
            $("#btnSubmit").click(function () {
                var msg = CheckForm();
                if (msg != "") {
                    $("#lblMsg").text(msg);
                    setTimeout(function () {
                        $("#lblMsg").text();
                    }, 2000);
                    return false;
                }
                return true;
            });

            bindnumberboxevent();
            timer_bindevent = setTimeout(function () {
                bindnumberboxevent();
            }, 1000);
        });

        function CheckForm() {
            var msg = "";
            if ($.trim($("#txtCustomerName").val()) == "") {
                msg = "请输入客户姓名";
            }
            else if ($.trim($("#txtCustomerMobile").val()) == "") {
                msg = "请输入客户联系电话";
            }
            else if ($.trim($("#txtCustomerQQ").val()) == "" && $.trim($("#txtCustomerEmail").val()) == "" && $.trim($("#txtCustomerMicroletter").val()) == "") {
                msg = "客户QQ、客户邮箱、客户微信号至少填写一项";
            }
            else if ($("#cbxIsSwap").attr("checked") && $("#txtSwapDetail").val() == "车型：\r颜色：\r上牌年份：\r公里数：\r是否有重大事故：") {
                msg = "请完善二手车描述";
            }

            return msg;
        }

        function bindnumberboxevent() {
            $(".number").unbind("focus");
            $(".cbxcBjmp").unbind("click");
            $("#cbxcSj").unbind("click");
            $("#cbxcCk").unbind("click");
            $("#ddlSztb").unbind("change");
            $("#pnlcal").unbind("blur");
            $("#cbxIsSwap").unbind("click")
            $(".number").focus(function () {
                var t = this;
                setTimeout(function () { t.select() }, 0);
            });
            $("#pnlcal").blur(function () {
                $("#pnlcal").hide();
            });

            //计算总价
            var total = 0;
            $(".mustcount").each(function () {
                total += parseFloat($(this).val());
            });
            $(".needcount").each(function () {
                if ($(this).prev().find("input:checkbox").attr("checked")) {
                    total += parseFloat($(this).val());
                }
            });
            $("#lblTotalPrinces").val(Math.round(total));

            //计算首付合计
            var firsttotal = 0;
            firsttotal = total;
            <%if(CQType == CarQuotationType.贷款购车){ %>
                $(".firstcount").each(function(){
                    firsttotal += parseFloat($(this).val());
                });
                firsttotal -=  parseFloat($("#txtfCjj").val());
                firsttotal -=  parseFloat($("#txtInterest").val());
            <%} %>
            $("#lblTotalFirstPrinces").val(Math.round(firsttotal));

            //计算保险合计
            var bxtotal = 0;
            bxtotal = parseFloat($("#txtcJqs").val());
            $(".trbaoxian .mustcount").each(function () {
                bxtotal += parseFloat($(this).val());
            });
            $(".trbaoxian .needcount").each(function () {
                if($("#" + $(this).attr("id").replace("txt","cbx")).attr("checked")){
                    bxtotal += parseFloat($(this).val());
                }
//                if ($(this).prev().find("input:checkbox").attr("checked")) {
//                    bxtotal += parseFloat($(this).val());
//                }
            });
            $("#lblBxhj").text(Math.round(bxtotal * 100) / 100);

            //绑定汽车颜色
            if ($("#hdnColor").val() != "") {
                $("input[name='rdocQcys']").each(function () {
                    if ($(this).val() == $("#hdnColor").val()) {
                        $(this).attr("checked", true);
                    }
                });
                $("#hdnColor").val("");
            }

            $(".cbxcBjmp").click(function () {
                CountBjmp();
            });

            $("#cbxcSj").click(function () {
                CountBjmp();
            });

            $("#cbxcCk").click(function () {
                CountBjmp();
            });

            $("#ddlSztb").change(function () {
                CountBjmp();
            });

            $("#cbxIsSwap").click(function () {
                $("#trSwap").attr("style", $(this).attr("checked") ? "" : "display:none;");
            });

            if (timer_bindevent)
                clearTimeout(timer_bindevent);
            timer_bindevent = setTimeout(function () {
                bindnumberboxevent();
            }, 1000);
        }

        function CountBjmp() {
            var bjmp = 0;
            var zkxs = parseFloat($("#ddlZkxs").val());
            var gcj = parseFloat($("#txtfCjj").val());
            var zws = parseInt($("#hdnZws").val());
            $(".cbxcBjmp").each(function () {
                if ($(this).attr("checked")) {
                    if ($(this).attr("title") == "车损")
                        bjmp += (566 + (gcj * 1.35 / 100)) * zkxs * 0.15;
                    if ($(this).attr("title") == "三者")
                        bjmp += $("#ddlSztb option:selected").attr("num") * zkxs * 0.15;
                    if ($(this).attr("title") == "人员") {
                        if ($("#cbxcSj").attr("checked"))
                            bjmp += parseFloat($("#txtcSjtb").val()) * 10000 * 0.41 / 100 * zkxs * 0.15;
                        if ($("#cbxcCk").attr("checked"))
                            bjmp += parseFloat($("#txtcCktb").val()) * 10000 * 0.26 / 100 * zkxs * (zws - 1) * 0.15;
                    }
                    if ($(this).attr("title") == "盗抢")
                        bjmp += (120 + (gcj * 0.41 / 100)) * zkxs * 0.2;
                }
            });
            $("#txtcBjmp").val(Math.round(bjmp * 100) / 100);
            $("#hdncBjmptb").val("");
            var cbjmptb = $(".cbxcBjmp:checked").map(function () {
                return $(this).attr("title");
            }).get().join('|');
            if (cbjmptb != "") $("#hdncBjmptb").val("|" + cbjmptb + "|");
        }
    </script>
</head>
<body>
    <form id="form1" runat="server">
    <%if (Admin.UserRole == Hx.Components.Enumerations.UserRoleType.销售员)
      { %>
      <div style="height:40px;display:block;line-height:40px;margin-bottom:5px;background:#21363e; text-align:right;color:#fff;">
        您好，<asp:HyperLink ID="hyName" runat="server"  style="color:#fff;"></asp:HyperLink> <a href="/logout.aspx" target="_parent" style="color:#fff;">[退出]</a> <a href="/user/changewd.aspx" style="color:#fff;">[修改密码]</a> <a href="/user/adminedit.aspx" target="_blank" style="color:#fff;">[完善信息]</a>
      </div>
      <%} %>
    <div class="ht_main">
        <ul class="xnav">
            <li<% if(CQType== Hx.Car.Enum.CarQuotationType.全款购车) {%> class="current" <%} %>><a href="carquotation.aspx?t=0">全款购车</a></li>
            <li<% if(CQType== Hx.Car.Enum.CarQuotationType.贷款购车) {%> class="current"<%} %>><a href="carquotation.aspx?t=1">贷款购车</a>
            </li>
        </ul>
        <asp:ScriptManager runat="server" ID="sm">
        </asp:ScriptManager>
        <asp:UpdatePanel runat="server" ID="up1">
            <ContentTemplate>
                <table width="100%" border="0" cellspacing="0" cellpadding="0" class="biaoge3">
                    <tbody>
                        <tr>
                            <td class="bg1">
                                <span class="red">*</span>客户姓名：
                            </td>
                            <td class="bg3">
                                <asp:TextBox runat="server" ID="txtCustomerName" CssClass="srk5"></asp:TextBox>
                            </td>
                            <td class="bg1">
                                <span class="red">*</span>客户联系电话：
                            </td>
                            <td>
                                <asp:TextBox runat="server" ID="txtCustomerMobile" CssClass="srk5 number" AutoPostBack="true"
                                    OnTextChanged="txtCustomerMobile_TextChanged"></asp:TextBox>
                                <asp:DropDownList runat="server" ID="ddlQuotationHistory" AutoPostBack="true" OnSelectedIndexChanged="ddlQuotationHistory_SelectedIndexChanged">
                                    <asp:ListItem Text="-报价记录-" Value="-1"></asp:ListItem>
                                </asp:DropDownList>
                            </td>
                        </tr>
                        <tr>
                            <td class="bg1">
                                客户QQ：
                            </td>
                            <td class="bg3">
                                <asp:TextBox runat="server" ID="txtCustomerQQ" CssClass="srk5"></asp:TextBox>
                            </td>
                            <td class="bg1">
                                客户邮箱：
                            </td>
                            <td>
                                <asp:TextBox runat="server" ID="txtCustomerEmail" CssClass="srk1"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td class="bg1">
                                客户微信号：
                            </td>
                            <td colspan="3">
                                <asp:TextBox runat="server" ID="txtCustomerMicroletter" CssClass="srk5"></asp:TextBox>
                                <span class="gray">(QQ、邮箱、微信号至少填一项)</span>
                            </td>
                        </tr>
                        <tr>
                            <td class="bg1 bold">
                                总金额：
                            </td>
                            <td colspan="3">
                                <asp:TextBox runat="server" ID="lblTotalPrinces" Style="border: 0px;"></asp:TextBox>
                            </td>
                        </tr>
                        <tr <%= CQType == CarQuotationType.全款购车 ? "style=\"display:none;\"" : "" %>>
                            <td class="bg1 bold">
                                首付现金：
                            </td>
                            <td colspan="3">
                                <asp:TextBox runat="server" ID="lblTotalFirstPrinces" Style="border: 0px;"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td colspan="4" style="background-color: #ccc; color: Black; font-weight: bold;">
                                基本信息
                            </td>
                        </tr>
                        <tr>
                            <td class="bg1">
                                车型：
                            </td>
                            <td colspan="3">
                                <asp:DropDownList runat="server" ID="ddlcChangs" CssClass="fll" AutoPostBack="true"
                                    OnSelectedIndexChanged="ddlcChangs_SelectedIndexChanged">
                                </asp:DropDownList>
                                <asp:DropDownList runat="server" ID="ddlcCxmc" AutoPostBack="true" OnSelectedIndexChanged="ddlcCxmc_SelectedIndexChanged">
                                </asp:DropDownList>
                            </td>
                        </tr>
                        <tr>
                            <td class="bg1">
                                颜色：
                            </td>
                            <td colspan="3">
                                <input type="hidden" runat="server" id="hdnColor" />
                                <asp:Repeater runat="server" ID="rptcQcys">
                                    <ItemTemplate>
                                        <label class="blockinline">
                                            <label class="fll" style="line-height: 18px; _line-height: 22px;">
                                                <input type="radio" name="rdocQcys" id="rdocQcys" class="fll" value="<%#Eval("Name").ToString() + "," + Eval("Color").ToString()%>"
                                                    <%# Container.ItemIndex == 0 ? "checked=\"checked\"" : string.Empty %> /><%#Eval("Name")%></label><span
                                                        style="background-color: <%# Eval("Color")%>; display: inline-block; *display: inline;
                                                        *zoom: 1;"><img alt="<%#Eval("Name")%>" src="../images/color_mb.png" style="width: 25px;
                                                            height: 15px; border: 1px solid #d7d7d7; padding: 0px; margin: 0px;" /></span></label>
                                    </ItemTemplate>
                                </asp:Repeater>
                            </td>
                        </tr>
                        <tr>
                            <td class="bg1">
                                厂商指导价：
                            </td>
                            <td class="bg3">
                                <asp:TextBox runat="server" ID="txtfZdj" CssClass="srk5 tr money" Enabled="false"
                                    Text="0"></asp:TextBox>
                                元
                            </td>
                            <td class="bg1">
                                购置价：
                            </td>
                            <td>
                                <asp:TextBox runat="server" ID="txtfCjj" CssClass="srk5 tr number money mustcount"
                                    Text="0" AutoPostBack="true" OnTextChanged="txtfCjj_TextChanged"></asp:TextBox>
                                元
                            </td>
                        </tr>
                        <tr>
                            <td colspan="4" style="background-color: #ccc; color: Black; font-weight: bold;">
                                必要花费
                            </td>
                        </tr>
                        <tr>
                            <td class="bg1">
                                购置税：
                            </td>
                            <td class="bg3">
                                <asp:TextBox runat="server" ID="txtcGzs" CssClass="srk5 tr number money mustcount"
                                    Text="0"></asp:TextBox>
                                元
                            </td>
                            <td class="bg1">
                                上牌费：
                            </td>
                            <td>
                                <asp:TextBox runat="server" ID="txtcSpf" CssClass="srk5 tr number money mustcount"
                                    Text="0"></asp:TextBox>
                                元
                            </td>
                        </tr>
                        <tr>
                            <td class="bg1">
                                车船税：
                            </td>
                            <td class="bg3">
                                <asp:TextBox runat="server" ID="txtcCcs" CssClass="srk5 tr number money mustcount"
                                    Text="0"></asp:TextBox>
                                元
                            </td>
                            <td class="bg1">
                                交强险：
                            </td>
                            <td>
                                <asp:TextBox runat="server" ID="txtcJqs" CssClass="srk5 tr number money mustcount"
                                    Text="0"></asp:TextBox>
                                元
                            </td>
                        </tr>
                        <tr>
                            <td colspan="4" style="background-color: #ccc; color: Black; font-weight: bold;">
                                商业保险
                            </td>
                        </tr>
                        <tr class="trbaoxian">
                            <td class="bg1">
                                折扣系数：
                            </td>
                            <td class="bg3">
                                <asp:DropDownList runat="server" ID="ddlZkxs" Enabled="false" AutoPostBack="true"
                                    OnSelectedIndexChanged="ddlZkxs_SelectedIndexChanged">
                                    <asp:ListItem Text="100%" Value="1"></asp:ListItem>
                                    <asp:ListItem Text="95%" Value="0.95" Selected="True"></asp:ListItem>
                                    <asp:ListItem Text="90%" Value="0.9"></asp:ListItem>
                                    <asp:ListItem Text="85.5%" Value="0.855"></asp:ListItem>
                                    <asp:ListItem Text="81%" Value="0.81"></asp:ListItem>
                                    <asp:ListItem Text="76.95%" Value="0.7695"></asp:ListItem>
                                    <asp:ListItem Text="72.90%" Value="0.729"></asp:ListItem>
                                    <asp:ListItem Text="70%" Value="0.7"></asp:ListItem>
                                </asp:DropDownList>
                            </td>
                            <td class="bg1">
                                玻璃产地：
                            </td>
                            <td>
                                <asp:DropDownList runat="server" ID="ddlBlcd" AutoPostBack="true" OnSelectedIndexChanged="ddlBlcd_SelectedIndexChanged">
                                    <asp:ListItem Text="国产" Value="0" Selected="True"></asp:ListItem>
                                    <asp:ListItem Text="进口" Value="1"></asp:ListItem>
                                </asp:DropDownList>
                            </td>
                        </tr>
                        <tr class="trbaoxian">
                            <td class="bg1">
                                车损险：
                            </td>
                            <td class="bg3">
                                <asp:TextBox runat="server" ID="txtcCsx" CssClass="srk5 tr number money mustcount"
                                    Text="0"></asp:TextBox>
                                元
                            </td>
                            <td class="bg1">
                                第三者责任险：
                            </td>
                            <td>
                                投保
                                <asp:DropDownList runat="server" ID="ddlSztb" AutoPostBack="true" OnSelectedIndexChanged="ddlSztb_SelectedIndexChanged">
                                    <asp:ListItem Text="100" Value="100" num="2124"></asp:ListItem>
                                    <asp:ListItem Text="50" Value="50" num="1631"></asp:ListItem>
                                    <asp:ListItem Text="30" Value="30" num="1359" Selected="True"></asp:ListItem>
                                    <asp:ListItem Text="20" Value="20" num="1204"></asp:ListItem>
                                    <asp:ListItem Text="15" Value="15" num="1108"></asp:ListItem>
                                    <asp:ListItem Text="10" Value="10" num="972"></asp:ListItem>
                                    <asp:ListItem Text="5" Value="5" num="673"></asp:ListItem>
                                </asp:DropDownList>
                                万
                                <asp:TextBox runat="server" ID="txtcDszrx" CssClass="srk5 tr number money mustcount"
                                    Text="0"></asp:TextBox>
                                元
                            </td>
                        </tr>
                        <tr class="trbaoxian">
                            <td class="bg1">
                                盗抢险：
                            </td>
                            <td class="bg3">
                                <asp:TextBox runat="server" ID="txtcDqx" CssClass="srk5 tr number money mustcount"
                                    Text="0"></asp:TextBox>
                                元
                            </td>
                            <td class="bg1">
                                人员险：
                            </td>
                            <td>
                                <input type="hidden" runat="server" id="hdnZws" value="2" />
                                <label class="blockinline" style="line-height: 18px; margin-right: 10px; _line-height: 22px;">
                                    <asp:CheckBox runat="server" ID="cbxcSj" Checked="true" CssClass="fll" />司机</label> 投保<asp:TextBox
                                        runat="server" ID="txtcSjtb" Text="1" CssClass="srk9"></asp:TextBox>万 保费：<asp:TextBox
                                            runat="server" ID="txtcSj" CssClass="w60 srk5 tr number money needcount" Text="0"></asp:TextBox>
                                元
                                <label class="blockinline" style="line-height: 18px; margin-right: 10px; _line-height: 22px;">
                                    <asp:CheckBox runat="server" ID="cbxcCk" Checked="true" CssClass="fll" />乘客</label> 投保<asp:TextBox
                                        runat="server" ID="txtcCktb" Text="1" CssClass="srk9"></asp:TextBox>万 保费：<asp:TextBox
                                            runat="server" ID="txtcCk" CssClass="w60 srk5 tr number money needcount" Text="0"></asp:TextBox>
                                元
                            </td>
                        </tr>
                        <tr class="trbaoxian">
                            <td class="bg1">
                                指定维修：
                            </td>
                            <td class="bg3">
                                <label class="blockinline" style="line-height: 18px; margin-right: 10px; _line-height: 22px;">
                                    <asp:CheckBox runat="server" ID="cbxcZdwx" Checked="false" CssClass="fll" />指定专修厂</label>
                                <asp:TextBox runat="server" ID="txtcZdwx" CssClass="srk7 tr number money needcount"
                                    Text="0"></asp:TextBox>
                                元
                            </td>
                            <td class="bg1">
                                附加险：
                            </td>
                            <td>
                                <label class="blockinline" style="line-height: 18px; margin-right: 10px; _line-height: 22px;">
                                    <asp:CheckBox runat="server" ID="cbxcZrx" Checked="true" CssClass="fll" />自然险</label><asp:TextBox
                                        runat="server" ID="txtcZrx" CssClass="w60 srk5 tr number money needcount" Text="0"></asp:TextBox>
                                元
                                <label class="blockinline" style="line-height: 18px; margin-right: 10px; _line-height: 22px;">
                                    <asp:CheckBox runat="server" ID="cbxcBlx" Checked="true" CssClass="fll" />玻璃险</label><asp:TextBox
                                        runat="server" ID="txtcBlx" CssClass="w60 srk5 tr number money needcount" Text="0"></asp:TextBox>
                                元
                                <div style="display: none;">
                                    <label class="blockinline" style="line-height: 18px; margin-right: 10px; _line-height: 22px;">
                                        <asp:CheckBox runat="server" ID="cbxcHhx" Checked="false" CssClass="fll" />划痕险</label><asp:TextBox
                                            runat="server" ID="txtcHhx" CssClass="w60 srk5 tr number money needcount" Text="0"></asp:TextBox>
                                    元
                                    <label class="blockinline" style="line-height: 18px; margin-right: 10px; _line-height: 22px;">
                                        <asp:CheckBox runat="server" ID="cbxcSsx" Checked="false" CssClass="fll" />涉水险</label><asp:TextBox
                                            runat="server" ID="txtcSsx" CssClass="w60 srk5 tr number money needcount" Text="0"></asp:TextBox>
                                    元
                                </div>
                            </td>
                        </tr>
                        <tr class="trbaoxian">
                            <td class="bg1">
                                不计免赔：
                            </td>
                            <td colspan="3">
                                <label class="blockinline" style="line-height: 18px; margin-right: 10px; _line-height: 22px;">
                                    <input type="checkbox" runat="server" id="cbxcBjmpcs" name="cbxcBjmpcs" class="cbxcBjmp"
                                        checked="checked" value="0.15" title="车损" />车损
                                </label>
                                <label class="blockinline" style="line-height: 18px; margin-right: 10px; _line-height: 22px;">
                                    <input type="checkbox" id="cbxcBjmpsz" runat="server" name="cbxcBjmpsz" class="cbxcBjmp"
                                        checked="checked" value="0.15" title="三者" />三者
                                </label>
                                <label class="blockinline" style="line-height: 18px; margin-right: 10px; _line-height: 22px;">
                                    <input type="checkbox" id="cbxcBjmpry" runat="server" name="cbxcBjmpry" class="cbxcBjmp"
                                        checked="checked" value="0.15" title="人员" />人员
                                </label>
                                <label class="blockinline" style="line-height: 18px; margin-right: 10px; _line-height: 22px;">
                                    <input type="checkbox" id="cbxcBjmpdq" runat="server" name="cbxcBjmpdq" class="cbxcBjmp"
                                        checked="checked" value="0.2" title="盗抢" />盗抢
                                </label>
                                <asp:TextBox runat="server" ID="txtcBjmp" CssClass="srk5 tr number money mustcount"
                                    Text="0"></asp:TextBox>
                                元
                                <input type="hidden" id="hdncBjmptb" value="|车损|三者|人员|盗抢|" runat="server" />
                            </td>
                        </tr>
                        <tr>
                            <td class="bg1">
                                保险合计：
                            </td>
                            <td>
                                <asp:Label runat="server" ID="lblBxhj"></asp:Label>
                            </td>
                        </tr>
                        <% if (CQType == Hx.Car.Enum.CarQuotationType.贷款购车)
                           { %>
                        <tr>
                            <td colspan="4" style="background-color: #ccc; color: Black; font-weight: bold;">
                                金融方案
                            </td>
                        </tr>
                        <tr class="hidden">
                            <td class="bg1">
                                银行：
                            </td>
                            <td class="bg3">
                            </td>
                            <td class="bg1">
                                贷款方案：
                            </td>
                            <td>
                                <asp:DropDownList runat="server" ID="ddlLoanType">
                                </asp:DropDownList>
                            </td>
                        </tr>
                        <tr>
                            <td class="bg1">
                                利率：
                            </td>
                            <td class="bg3">
                                <asp:DropDownList runat="server" ID="ddlBankingType" AutoPostBack="true" OnSelectedIndexChanged="ddlBankingType_SelectedIndexChanged">
                                </asp:DropDownList>
                                <asp:DropDownList runat="server" ID="ddlBank" CssClass="hidden" AutoPostBack="true"
                                    OnSelectedIndexChanged="ddlBank_SelectedIndexChanged">
                                </asp:DropDownList>
                                <asp:TextBox runat="server" ID="txtProfitMargin" CssClass="srk7" Text="306.7" AutoPostBack="true"
                                    OnTextChanged="txtProfitMargin_TextChanged"></asp:TextBox>
                            </td>
                            <td class="bg1">
                                月还款额：
                            </td>
                            <td>
                                <asp:TextBox runat="server" ID="txtRepaymentPerMonth" CssClass="srk5 tr number money firstcount"
                                    Text="0"></asp:TextBox>
                                元
                            </td>
                        </tr>
                        <tr>
                            <td class="bg1">
                                首付款：
                            </td>
                            <td class="bg3">
                                <asp:DropDownList runat="server" ID="ddlSfbl" AutoPostBack="true" OnSelectedIndexChanged="ddlSfbl_SelectedIndexChanged">
                                    <asp:ListItem Text="30%" Value="0.3" Selected="True"></asp:ListItem>
                                    <asp:ListItem Text="50%" Value="0.5"></asp:ListItem>
                                </asp:DropDownList>
                                <asp:TextBox runat="server" ID="txtFirstPayment" ReadOnly="true" CssClass="srk5 tr number money firstcount"
                                    Text="0"></asp:TextBox>
                                元
                            </td>
                            <td class="bg1">
                                贷款额：
                            </td>
                            <td>
                                <asp:TextBox runat="server" ID="txtLoanValue" CssClass="srk5 tr number money" Text="0"></asp:TextBox>
                                元
                            </td>
                        </tr>
                        <tr>
                            <td class="bg1">
                                贷款期：
                            </td>
                            <td class="bg3">
                                <asp:DropDownList runat="server" ID="ddlLoanLength" AutoPostBack="true" OnSelectedIndexChanged="ddlLoanLength_SelectedIndexChanged">
                                    <asp:ListItem Text="36" Value="3" Selected="True"></asp:ListItem>
                                    <asp:ListItem Text="24" Value="2"></asp:ListItem>
                                    <asp:ListItem Text="12" Value="1"></asp:ListItem>
                                </asp:DropDownList>
                                月
                            </td>
                            <td class="bg1">
                                利息：
                            </td>
                            <td>
                                <asp:TextBox runat="server" ID="txtInterest" CssClass="srk5 tr number money mustcount"
                                    Text="0"></asp:TextBox>
                                元
                            </td>
                        </tr>
                        <tr>
                            <td class="bg1">
                                续保押金：
                            </td>
                            <td class="bg3">
                                <asp:TextBox runat="server" ID="txtcXbyj" CssClass="srk5 tr number money mustcount"
                                    Text="0"></asp:TextBox>
                                元
                            </td>
                            <td class="bg1">
                                履约风险金：
                            </td>
                            <td>
                                <asp:TextBox runat="server" ID="txtLyfxj" CssClass="srk5 tr number money mustcount"
                                    Text="0"></asp:TextBox>
                                元
                            </td>
                        </tr>
                        <td class="bg1">
                            代办分期付款手续费、劳务费：
                        </td>
                        <td colspan="3">
                            <asp:TextBox runat="server" ID="txtDbfqlwf" CssClass="srk5 tr number money mustcount"
                                Text="0"></asp:TextBox>
                            元，其中 资信费：<asp:TextBox runat="server" ID="txtZxf" CssClass="srk5 tr number money"
                                Text="0"></asp:TextBox>
                            调查费：<asp:TextBox runat="server" ID="txtDcf" CssClass="srk5 tr number money" Text="0"></asp:TextBox>
                        </td>
                        <%} %>
                        <tr>
                            <td colspan="4" style="background-color: #ccc; color: Black; font-weight: bold;">
                                汽车用品
                            </td>
                        </tr>
                        <tr>
                            <td class="bg1">
                                精品说明：
                            </td>
                            <td class="bg3">
                                <asp:TextBox runat="server" ID="txtChoicestGoods" CssClass="srk1"></asp:TextBox>
                            </td>
                            <td class="bg1">
                                精品金额：
                            </td>
                            <td>
                                <asp:TextBox runat="server" ID="txtChoicestGoodsPrice" CssClass="srk5 tr number money mustcount"
                                    Text="0"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td colspan="4" style="background-color: #ccc; color: Black; font-weight: bold;">
                                其他
                            </td>
                        </tr>
                        <tr>
                            <td class="bg1">
                                交车费：
                            </td>
                            <td colspan="3">
                                <asp:TextBox runat="server" ID="txtDbsplwf" CssClass="srk5 tr number money mustcount"
                                    Text="0"></asp:TextBox>
                                元
                            </td>
                        </tr>
                        <tr>
                            <td class="bg1">
                            </td>
                            <td colspan="3">
                                <label class="blockinline" style="line-height: 18px; _line-height: 22px;">
                                    <asp:CheckBox runat="server" ID="cbxIsSwap" CssClass="fll" />是否置换二手车</label>
                            </td>
                        </tr>
                        <tr id="trSwap" style="display: none;" runat="server">
                            <td class="bg1">
                                <span class="red">*</span>二手车描述：
                            </td>
                            <td colspan="3">
                                <textarea runat="server" id="txtSwapDetail" class="srk2"></textarea>
                            </td>
                        </tr>
                        <tr>
                            <td class="bg1">
                                备注：
                            </td>
                            <td colspan="3">
                                <asp:TextBox runat="server" ID="txtGift" CssClass="srk1"></asp:TextBox>
                            </td>
                        </tr>
                    </tbody>
                </table>
                <asp:Label ID="lbljs" runat="server" CssClass="hidden"></asp:Label>
            </ContentTemplate>
        </asp:UpdatePanel>
        <table width="100%" border="0" cellspacing="0" cellpadding="0" class="biaoge3">
            <tbody>
                <tr>
                    <td class="bg1">
                    </td>
                    <td>
                        <asp:Button runat="server" ID="btnSubmit" Text="生成报价单" CssClass="an1" OnClick="btnSubmit_Click" />
                        <asp:Label ID="lblMsg" runat="server" CssClass="red"></asp:Label>
                    </td>
                </tr>
            </tbody>
        </table>
    </div>
    </form>
    <div id="pnlcal" style="display: none; position: absolute;">
        <div id="divcal">
        </div>
    </div>
</body>
</html>