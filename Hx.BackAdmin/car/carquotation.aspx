<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="carquotation.aspx.cs" Inherits="Hx.BackAdmin.car.carquotation"
    EnableViewState="true" %>

<%@ Import Namespace="Hx.Car.Enum" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title><%= CQType %></title>
    <link href=<%=ResourceServer%>/css/admin.css rel="stylesheet" type="text/css" />
    <link href=<%=ResourceServer%>/css/calculator/calculator.css rel="stylesheet" type="text/css" />
    <script src=<%=ResourceServer%>/js/jquery-1.3.2.min.js type="text/javascript"></script>
    <script src=<%=ResourceServer%>/js/calculator.js type="text/javascript"></script>
    <script src="/js/WdatePicker.js" type="text/javascript"></script>
    <script language="javascript" type="text/javascript">
        var curnumberbox;
        var timer_bindevent;
        $(function () {
            $("#btnSubmit").click(function () {
                var msg = CheckForm();
                if (msg != "") {
                    $("#lblMsg").text(msg);
                    setTimeout(function () {
                        $("#lblMsg").text('');
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
            else if ($.trim($("#txtSaleDay").val()) == "" && $.trim($("#txtPlaceDay").val()) == "" ) {
                msg = "销售日期与订车日期请至少填写一项";
            }
            else if ($("#cbxIsSwap").attr("checked") && $("#txtSwapDetail").val() == "车型：\r颜色：\r上牌年份：\r公里数：\r是否有重大事故：") {
                msg = "请完善二手车描述";
            }

            return msg;
        }

        function bindnumberboxevent() {
            $(".number").unbind("focus");
            $(".number").unbind("keyup");
            $(".cbxcBjmp").unbind("click");
            $("#cbxcSj").unbind("click");
            $("#cbxcCk").unbind("click");
            $("#ddlSztb").unbind("change");
            $("#ddlBankingType").unbind("change");
            $("#pnlcal").unbind("blur");
            $("#cbxIsSwap").unbind("click");
            $("#cbxQcypjz").unbind("click");
            $("#cbxWyfw").unbind("click");
            $(".wyfw").unbind("click");
            $(".txtwyfw").unbind("change");
            $("#btnQcypadd").unbind("click");
            $(".ypdel").unbind("click");
            $(".Date").unbind("click");
//            $(".bxcheck").unbind("click");
//            $(".bxfy").unbind("change");
            $(".number").focus(function () {
                var t = this;
                setTimeout(function () { t.select() }, 0);
            });
            $(".number").keyup(function(){
                $(this).val($(this).val().replace(/[^\0-9\.]/g,''));
            });
            $("#pnlcal").blur(function () {
                $("#pnlcal").hide();
            });
            $(".Date").click(function () {
                WdatePicker({ 'readOnly': 'true', dateFmt: 'yyyy-MM-dd'});
            });

            //计算总价
            var total = 0;
            $(".mustcount").each(function () {
                total += parseFloat($.trim($(this).val()) == "" ? "0" : $.trim($(this).val()));
            });
            $(".needcount").each(function () {
            
                if($("#" + $(this).attr("id").replace("txt","cbx")).attr("checked")){
                    total += parseFloat($.trim($(this).val()) == "" ? "0" : $.trim($(this).val()));
                }
            });
            $("#lblTotalPrinces").val(Math.round(total) + " 元");

            //计算首付合计
            var firsttotal = 0;
            firsttotal = total;
            <%if(CQType == CarQuotationType.金融购车){ %>
                $(".firstcount").each(function(){
                    firsttotal += parseFloat($.trim($(this).val()) == "" ? "0" : $.trim($(this).val()));
                });
                firsttotal -=  parseFloat($.trim($("#txtfCjj").val()) == "" ? "0" : $.trim($("#txtfCjj").val()));
                firsttotal -=  parseFloat($.trim($("#txtInterest").val()) == "" ? "0" : $.trim($("#txtInterest").val()));
            <%} %>
            $("#lblTotalFirstPrinces").val(Math.round(firsttotal));

            //绑定汽车颜色
            if ($("#hdnColor").val() != "") {
                $("input[name='rdocQcys']").each(function () {
                    if ($(this).val() == $("#hdnColor").val()) {
                        $(this).attr("checked", true);
                    }
                });
                $("#hdnColor").val("");
            }
            //绑定汽车颜色
            if ($("#hdnInnerColor").val() != "") {
                $("input[name='rdocNsys']").each(function () {
                    if ($(this).val() == $("#hdnInnerColor").val()) {
                        $(this).attr("checked", true);
                    }
                });
                $("#hdnInnerColor").val("");
            }

            $(".cbxcBjmp").click(function () {
                CountBjmp();
            });

            $("#ddlBankingType").change(function(){
                if($("#ddlBankingType option:selected").html() == "<%= Hx.Car.Enum.BankingType.银行金融.ToString() %>"){
                    $("#ddlBank").attr("class","");
                }else{
                    $("#ddlBank").attr("class","hidden");
                }
            });

            $("#cbxIsSwap").click(function () {
                $("#trSwap").attr("style", $(this).attr("checked") ? "" : "display:none;");
            });

            $("#cbxQcypjz").click(function () {
                $("#trQcypjz").attr("style", $(this).attr("checked") ? "" : "display:none;");
                CountQcyp();
            });

            $("#cbxWyfw").click(function () {
                $("#trWyfw").attr("style", $(this).attr("checked") ? "" : "display:none;");
                CountWyfw();
            });

            $(".wyfw").click(function () {
                CountWyfw();
            });

            $(".txtwyfw").change(function () {
                CountWyfw();
            });

            $("#btnQcypadd").click(function(){
                var id = $("#ddlQcyp option:selected").val();
                var name = $("#ddlQcyp option:selected").html();
                var price = $("#ddlQcyp option:selected").attr("data-price");
                if($("#ulQcyp li[data-id=\"" + id + "\"]").length == 0)
                    $("#ulQcyp").append("<li class=\"blockinline fll\" data-id=\"" + id + "\" data-price=\"" + price + "\"  style=\"width:200px;line-height: 18px; _line-height: 22px;\">" + name + "<span style=\"color:Gray\">(￥" + price + ")</span><a onclick=\"javascript:void(0);\" class=\"ypdel\"></a></li>");

                CountQcyp();
            });

            $(".ypdel").click(function(){
                $(this).parent().remove();
                CountQcyp();
            });

            if (timer_bindevent)
                clearTimeout(timer_bindevent);
            timer_bindevent = setTimeout(function () {
                bindnumberboxevent();
            }, 1000);
        }

        function CountBjmp() {
            $("#hdncBjmptb").val("");
            var cbjmptb = $(".cbxcBjmp:checked").map(function () {
                return $(this).attr("title");
            }).get().join('|');
            if (cbjmptb != "") $("#hdncBjmptb").val("|" + cbjmptb + "|");
        }

        function CountQcyp(){
            var qcypjz = 0;
            $("#hdnQcyp").val($("#ulQcyp li").map(function(){
                qcypjz += parseFloat($(this).attr("data-price"));
                return $(this).attr("data-id");
            }).get().join('|'));
            $("#txtQcypjz").val(qcypjz);
        }

        function CountWyfw(){
            var wyfwjz = 0;
            $(".wyfw input:checked").each(function(){
                wyfwjz += parseFloat($(this).parent().parent().next().val());
            });

            $("#txtWyfw").val(wyfwjz);
        }
    </script>
<style type="text/css">
.ypdel{cursor:pointer;width:18px;height:18px;background:url(../images/del.png) no-repeat;position:absolute;margin-left:2px;}
.colortd label em{width: 16px;height: 16px;line-height: 0;overflow: hidden;*zoom: 1;position:absolute;}
.colortd label em.em-top{width: 16px;height: 8px;z-index: 10;}
.colortd label em.em-buttom{width: 16px;height: 8px;top: 9px;left: 1px;z-index: 10;}
.colortd label span{display: inline-block;width: 16px;height: 16px;border: 1px solid #cecece;*display: inline;*zoom: 1;position:relative;}
</style>
</head>
<body style="background:#ccc">
    <form id="form1" runat="server">
    <%if (Admin.UserRole == Hx.Components.Enumerations.UserRoleType.销售员)
      { %>
      <div style="height:40px;display:block;line-height:40px;margin-bottom:5px;background:#21363e; text-align:right;color:#fff;">
        您好，<asp:HyperLink ID="hyName" runat="server"  style="color:#fff;"></asp:HyperLink> <a href="/logout.aspx" target="_parent" style="color:#fff;">[退出]</a> <a href="/user/changewd.aspx" style="color:#fff;">[修改密码]</a> <a href="/user/adminedit.aspx" target="_blank" style="color:#fff;">[完善信息]</a>
      </div>
      <%} %>
<div style="width: 1000px; margin: 0 auto;width:1000px;background:#fff; padding-top: 5px;box-shadow: 0 0 3px #000;">
    <div style="margin:15px 15px 0 20px;">
        <ul class="xnav">
            <li<% if(CQType== Hx.Car.Enum.CarQuotationType.全款购车) {%> class="current" <%} %>><a href="carquotation.aspx?t=0">全款购车</a></li>
            <li<% if(CQType== Hx.Car.Enum.CarQuotationType.金融购车) {%> class="current"<%} %>><a href="carquotation.aspx?t=1">金融购车</a>
            </li>
        </ul>
        <asp:ScriptManager runat="server" ID="sm">
        </asp:ScriptManager>
        <asp:UpdatePanel runat="server" ID="up1">
            <ContentTemplate>
                <table width="100%" border="0" cellspacing="0" cellpadding="0" class="biaoge3">
                    <tbody>
                        <tr>
                            <td class="bg11">
                                <span class="red">*</span>用户单位/个人：
                            </td>
                            <td class="bg3">
                                <asp:TextBox runat="server" ID="txtCustomerName" CssClass="srk5"></asp:TextBox>
                            </td>
                            <td class="bg11">
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
                            <td class="bg11">
                                客户微信号：
                            </td>
                            <td class="bg3">
                                <asp:TextBox runat="server" ID="txtCustomerMicroletter" CssClass="srk5"></asp:TextBox>
                            </td>
                            <td class="bg11">
                                车架号：
                            </td>
                            <td>
                                <asp:TextBox runat="server" ID="txtcCjh" CssClass="srk3"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td class="bg11">
                                销售日期：
                            </td>
                            <td class="bg3">
                                <asp:TextBox runat="server" ID="txtSaleDay" CssClass="Date srk5"></asp:TextBox>
                            </td>
                            <td class="bg11">
                                订车日期：
                            </td>
                            <td>
                                <asp:TextBox runat="server" ID="txtPlaceDay" CssClass="Date srk5"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td class="bg11 bold">
                                总计金额：
                            </td>
                            <td>
                                <asp:TextBox runat="server" ID="lblTotalPrinces" Style="border: 0px;"></asp:TextBox>
                            </td>
                            <td class="bg11">
                                是否老客户转介绍：
                            </td>
                            <td>
                                <asp:CheckBox runat="server" ID="cbxIslkhzjs" Text="" />
                            </td>
                        </tr>
                        <tr <%= CQType == CarQuotationType.全款购车 ? "style=\"display:none;\"" : "" %>>
                            <td class="bg11 bold">
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
                            <td class="bg11">
                                车型：
                            </td>
                            <td colspan="3">
                                <asp:DropDownList runat="server" ID="ddlcChangs" CssClass="fll mr10" AutoPostBack="true"
                                    OnSelectedIndexChanged="ddlcChangs_SelectedIndexChanged">
                                </asp:DropDownList>
                                <asp:DropDownList runat="server" ID="ddlcCxmc" AutoPostBack="true" OnSelectedIndexChanged="ddlcCxmc_SelectedIndexChanged">
                                </asp:DropDownList>
                            </td>
                        </tr>
                        <tr>
                            <td class="bg11">
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
                                                        style="background-color: <%# Eval("Color")%>;" class="blockinline"><img alt="<%#Eval("Name")%>" src="../images/color_mb.png" style="width: 25px;
                                                            height: 15px; border: 1px solid #d7d7d7; padding: 0px; margin: 0px;" /></span></label>
                                        <%#Container.ItemIndex > 0 && (Container.ItemIndex + 1) % 8 == 0 ? "<br />" : string.Empty%>
                                    </ItemTemplate>
                                </asp:Repeater>
                            </td>
                        </tr>
                        <tr>
                            <td class="bg11">
                                内饰颜色：
                            </td>
                            <td colspan="3" class="colortd">
                                <input type="hidden" runat="server" id="hdnInnerColor" />
                                <asp:Repeater runat="server" ID="rptcNsys">
                                    <ItemTemplate>
                                        <label class="blockinline">
                                            <label class="fll" style="line-height: 18px; _line-height: 22px;">
                                                <input type="radio" name="rdocNsys" id="rdocNsys" class="fll" value="<%#Eval("Name").ToString() + "," + Eval("Color").ToString()%>"
                                                    <%# Container.ItemIndex == 0 ? "checked=\"checked\"" : string.Empty %> /><%#Eval("Name")%></label>
                                                    <%# GetInnerColor(Eval("Color").ToString()) %></label>
                                        <%#Container.ItemIndex > 0 && (Container.ItemIndex + 1) % 8 == 0 ? "<br />" : string.Empty%>
                                    </ItemTemplate>
                                </asp:Repeater>
                            </td>
                        </tr>
                        <tr>
                            <td class="bg11">
                                实际销售车价：
                            </td>
                            <td class="bg3">
                                <asp:TextBox runat="server" ID="txtfCjj" CssClass="srk5 tr number money mustcount"
                                    Text="0"></asp:TextBox>
                                元
                            </td>
                            <td class="bg11">
                                标准车价：
                            </td>
                            <td>
                                <asp:TextBox runat="server" ID="txtfZdj" CssClass="srk5 tr number money"
                                    Text="0"></asp:TextBox>
                                元
                            </td>
                        </tr>
                        <tr>
                            <td colspan="4" style="background-color: #ccc; color: Black; font-weight: bold;">
                                现金部分
                            </td>
                        </tr>
                        <tr>
                            <td class="bg11">
                                预收附加税：
                            </td>
                            <td colspan="3">
                                <asp:TextBox runat="server" ID="txtcGzs" CssClass="srk5 tr number money mustcount"
                                    Text="0"></asp:TextBox>
                                元
                            </td>
                        </tr>
                        <tr>
                            <td class="bg11">
                                上牌及手续费：
                            </td>
                            <td colspan="3">
                                <asp:TextBox runat="server" ID="txtcSpf" CssClass="srk5 tr number money mustcount"
                                    Text="0"></asp:TextBox>
                                元
                            </td>
                        </tr>
                        <tr>
                            <td class="bg11">
                                <label class="blockinline" style="line-height: 18px; _line-height: 22px;">
                                    <asp:CheckBox runat="server" ID="cbxQcypjz" CssClass="fll" />汽车用品加装：</label>
                            </td>
                            <td colspan="3">
                                <asp:TextBox runat="server" ID="txtQcypjz" CssClass="srk5 tr number money needcount"
                                    Text="0"></asp:TextBox> 
                                元<label class="pl10"><asp:DropDownList runat="server" ID="ddlQcyp"></asp:DropDownList><input type="button" id="btnQcypadd" class="ml10" value="添加" /></label>
                            </td>
                        </tr>
                        <tr id="trQcypjz" style="display:none;" runat="server">
                            <td class="bg11">
                                &nbsp;
                            </td>
                            <td colspan="3">
                                <input type="hidden" runat="server" id="hdnQcyp" value="" />
                                        <ul style="width:800px" id="ulQcyp">
                                <asp:Repeater runat="server" ID="rptQcyp">
                                    <ItemTemplate>
                                        <li class="blockinline fll" data-id="<%#Eval("ID") %>" data-price="<%# Eval("Price") %>" style="width:200px;line-height: 18px; _line-height: 22px;">
                                            <%# Eval("Name") %><span style="color:Gray">(￥<%# Eval("Price") %>)</span><a onclick="javascript:void(0);" class="ypdel"></a>
                                        </li>
                                    </ItemTemplate>
                                </asp:Repeater>
                                        </ul>
                            </td>
                        </tr>
                        <tr>
                            <td class="bg11">
                                代收保险费合计：
                            </td>
                            <td colspan="3">
                                <asp:TextBox runat="server" ID="txtBxhj" Text="0" CssClass="srk5 tr number money mustcount"></asp:TextBox>
                                元 
                            </td>
                        </tr>
                        <tr>
                            <td class="bg11">
                                <label class="blockinline" style="line-height: 18px; _line-height: 22px;">
                                    <asp:CheckBox runat="server" ID="cbxWyfw" CssClass="fll" />无忧服务：</label>
                            </td>
                            <td colspan="3">
                                <asp:TextBox runat="server" ID="txtWyfw" CssClass="srk5 tr number money needcount"
                                    Text="0"></asp:TextBox> 
                                元
                            </td>
                        </tr>
                        <tr id="trWyfw" style="display:none;" runat="server">
                            <td class="bg11">
                                &nbsp;
                            </td>
                            <td colspan="3">
                                <label class="blockinline" style="line-height: 18px; margin-right: 10px; _line-height: 22px;">
                                    <asp:CheckBox runat="server" ID="cbxWyfwjytc" Checked="false" CssClass="fll wyfw" />机油套餐：</label><asp:TextBox
                                            runat="server" ID="txtWyfwjytc" CssClass="w60 srk5 tr number money txtwyfw" Text="0"></asp:TextBox>
                                元
                                <label class="blockinline" style="line-height: 18px; margin-right: 10px; _line-height: 22px;">
                                    <asp:CheckBox runat="server" ID="cbxWyfwblwyfw" Checked="false" CssClass="fll wyfw" />玻璃无忧服务：</label><asp:TextBox
                                            runat="server" ID="txtWyfwblwyfw" CssClass="w60 srk5 tr number money txtwyfw" Text="0"></asp:TextBox>
                                元
                                <label class="blockinline" style="line-height: 18px; margin-right: 10px; _line-height: 22px;">
                                    <asp:CheckBox runat="server" ID="cbxWyfwhhwyfw" Checked="false" CssClass="fll wyfw" />划痕无忧服务：</label><asp:TextBox
                                            runat="server" ID="txtWyfwhhwyfw" CssClass="w60 srk5 tr number money txtwyfw" Text="0"></asp:TextBox>
                                元
                                <label class="blockinline" style="line-height: 18px; margin-right: 10px; _line-height: 22px;">
                                    <asp:CheckBox runat="server" ID="cbxWyfwybwyfw" Checked="false" CssClass="fll wyfw" />延保无忧车服务：</label><asp:TextBox
                                            runat="server" ID="txtWyfwybwyfw" CssClass="w60 srk5 tr number money txtwyfw" Text="0"></asp:TextBox>
                                元
                            </td>
                        </tr>
                        <tr>
                            <td class="bg11">
                                增值费：
                            </td>
                            <td colspan="3">
                                <asp:TextBox runat="server" ID="txtDbsplwf" CssClass="srk5 tr number money mustcount"
                                    Text="0"></asp:TextBox>
                                元
                            </td>
                        </tr>
                        <tr>
                            <td class="bg11">
                                其他费用：
                            </td>
                            <td colspan="3">
                                <asp:TextBox runat="server" ID="txtQtfy" CssClass="srk5 tr number money mustcount"
                                    Text="0"></asp:TextBox>
                                元<asp:TextBox runat="server" ID="txtQtfyms" placeholder="描述..." CssClass="ml10"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td colspan="4" style="background-color: #ccc; color: Black; font-weight: bold;">
                                代收保险费
                            </td>
                        </tr>
                        <tr class="trbaoxian">
                            <td class="bg11">
                                承保单位：
                            </td>
                            <td class="bg3">
                                <asp:DropDownList runat="server" ID="ddlSybx"></asp:DropDownList>
                                <%--<asp:DropDownList runat="server" ID="ddlZkxs" Enabled="false" AutoPostBack="true"
                                    OnSelectedIndexChanged="ddlZkxs_SelectedIndexChanged">
                                    <asp:ListItem Text="100%" Value="1"></asp:ListItem>
                                    <asp:ListItem Text="95%" Value="0.95" Selected="True"></asp:ListItem>
                                    <asp:ListItem Text="90%" Value="0.9"></asp:ListItem>
                                    <asp:ListItem Text="85.5%" Value="0.855"></asp:ListItem>
                                    <asp:ListItem Text="81%" Value="0.81"></asp:ListItem>
                                    <asp:ListItem Text="76.95%" Value="0.7695"></asp:ListItem>
                                    <asp:ListItem Text="72.90%" Value="0.729"></asp:ListItem>
                                    <asp:ListItem Text="70%" Value="0.7"></asp:ListItem>
                                </asp:DropDownList>--%>
                            </td>
                            <td class="bg11">
                                <label class="blockinline" style="line-height: 18px; _line-height: 22px;">
                                    <asp:CheckBox runat="server" ID="cbxcJqs" Checked="false" CssClass="fll bxcheck" />交强险</label>：
                            </td>
                            <td>
                                <asp:TextBox runat="server" ID="txtcJqs" CssClass="srk5 tr number money bxfy"
                                    Text="0"></asp:TextBox>
                                元
                            </td>
                            <%--<td class="bg11">
                                玻璃产地：
                            </td>
                            <td>
                                <asp:DropDownList runat="server" ID="ddlBlcd" AutoPostBack="true" OnSelectedIndexChanged="ddlBlcd_SelectedIndexChanged">
                                    <asp:ListItem Text="国产" Value="0" Selected="True"></asp:ListItem>
                                    <asp:ListItem Text="进口" Value="1"></asp:ListItem>
                                </asp:DropDownList>
                            </td>--%>
                        </tr>
                        <tr>
                            <td class="bg11">
                                <label class="blockinline" style="line-height: 18px; _line-height: 22px;">
                                    <asp:CheckBox runat="server" ID="cbxcCsx" Checked="false" CssClass="fll bxcheck" />车损险</label>：
                            </td>
                            <td class="bg3">
                                <asp:TextBox runat="server" ID="txtcCsx" CssClass="srk5 tr number money bxfy"
                                    Text="0"></asp:TextBox>
                                元
                            </td>
                            <td class="bg11">
                                <label class="blockinline" style="line-height: 18px; _line-height: 22px;">
                                    <asp:CheckBox runat="server" ID="cbxcDszrx" Checked="false" CssClass="fll bxcheck" />第三者责任险：</label>
                            </td>
                            <td>
                                投保
                                <asp:DropDownList runat="server" ID="ddlSztb">
                                    <asp:ListItem Text="100" Value="100" num="2124"></asp:ListItem>
                                    <asp:ListItem Text="50" Value="50" num="1631"></asp:ListItem>
                                    <asp:ListItem Text="30" Value="30" num="1359" Selected="True"></asp:ListItem>
                                    <asp:ListItem Text="20" Value="20" num="1204"></asp:ListItem>
                                    <asp:ListItem Text="15" Value="15" num="1108"></asp:ListItem>
                                    <asp:ListItem Text="10" Value="10" num="972"></asp:ListItem>
                                    <asp:ListItem Text="5" Value="5" num="673"></asp:ListItem>
                                </asp:DropDownList>
                                万
                                <asp:TextBox runat="server" ID="txtcDszrx" CssClass="srk5 tr number money bxfy"
                                    Text="0"></asp:TextBox>
                                元
                            </td>
                        </tr>
                        <tr class="trbaoxian">
                            <td class="bg11">
                                <label class="blockinline" style="line-height: 18px; _line-height: 22px;">
                                    <asp:CheckBox runat="server" ID="cbxcDqx" Checked="false" CssClass="fll bxcheck" />盗抢险</label>：
                            </td>
                            <td class="bg3">
                                <asp:TextBox runat="server" ID="txtcDqx" CssClass="srk5 tr number money bxfy"
                                    Text="0"></asp:TextBox>
                                元
                            </td>
                            <td class="bg11">
                                人员险：
                            </td>
                            <td>
                                <label class="blockinline" style="line-height: 18px; margin-right: 10px; _line-height: 22px;">
                                    <asp:CheckBox runat="server" ID="cbxcSj" Checked="false" CssClass="fll bxcheck" />司机</label> 投保<asp:TextBox
                                        runat="server" ID="txtcSjtb" Text="1" CssClass="srk9"></asp:TextBox>万 保费：<asp:TextBox
                                            runat="server" ID="txtcSj" CssClass="w60 srk5 tr number money bxfy" Text="0"></asp:TextBox>
                                元
                                <label class="blockinline" style="line-height: 18px; margin-right: 10px; _line-height: 22px;">
                                    <asp:CheckBox runat="server" ID="cbxcCk" Checked="false" CssClass="fll bxcheck" />乘客</label> 投保<asp:TextBox
                                        runat="server" ID="txtcCktb" Text="1" CssClass="srk9"></asp:TextBox>万 保费：<asp:TextBox
                                            runat="server" ID="txtcCk" CssClass="w60 srk5 tr number money bxfy" Text="0"></asp:TextBox>
                                元
                            </td>
                        </tr>
                        <tr class="trbaoxian">
                            <td class="bg11">
                                <label class="blockinline" style="line-height: 18px;  _line-height: 22px;">
                                    <asp:CheckBox runat="server" ID="cbxcZdwx" Checked="false" CssClass="fll bxcheck" />指定专修厂</label>：
                            </td>
                            <td class="bg3">
                                <asp:TextBox runat="server" ID="txtcZdwx" CssClass="srk7 tr number money bxfy"
                                    Text="0"></asp:TextBox>
                                元
                            </td>
                            <td class="bg11">
                                附加险：
                            </td>
                            <td>
                                <label class="blockinline" style="line-height: 18px; margin-right: 10px; _line-height: 22px;">
                                    <asp:CheckBox runat="server" ID="cbxcZrx" Checked="false" CssClass="fll bxcheck" />自然险</label><asp:TextBox
                                        runat="server" ID="txtcZrx" CssClass="w60 srk5 tr number money bxfy" Text="0"></asp:TextBox>
                                元
                                <label class="blockinline" style="line-height: 18px; margin-right: 10px; _line-height: 22px;">
                                    <asp:CheckBox runat="server" ID="cbxcBlx" Checked="false" CssClass="fll bxcheck" />玻璃险</label><asp:TextBox
                                        runat="server" ID="txtcBlx" CssClass="w60 srk5 tr number money bxfy" Text="0"></asp:TextBox>
                                元
                                <div style="display: none;">
                                    <label class="blockinline" style="line-height: 18px; margin-right: 10px; _line-height: 22px;">
                                        <asp:CheckBox runat="server" ID="cbxcHhx" Checked="false" CssClass="fll bxcheck" />划痕险</label><asp:TextBox
                                            runat="server" ID="txtcHhx" CssClass="w60 srk5 tr number money bxfy" Text="0"></asp:TextBox>
                                    元
                                    <label class="blockinline" style="line-height: 18px; margin-right: 10px; _line-height: 22px;">
                                        <asp:CheckBox runat="server" ID="cbxcSsx" Checked="false" CssClass="fll bxcheck" />涉水险</label><asp:TextBox
                                            runat="server" ID="txtcSsx" CssClass="w60 srk5 tr number money bxfy" Text="0"></asp:TextBox>
                                    元
                                </div>
                            </td>
                        </tr>
                        <tr class="trbaoxian">
                            <td class="bg11">
                                不计免赔：
                            </td>
                            <td colspan="3">
                                <label class="blockinline" style="line-height: 18px; margin-right: 10px; _line-height: 22px;">
                                    <input type="checkbox" runat="server" id="cbxcBjmpcs" name="cbxcBjmpcs" class="fll cbxcBjmp"
                                         value="0.15" title="车损" />车损
                                </label>
                                <label class="blockinline" style="line-height: 18px; margin-right: 10px; _line-height: 22px;">
                                    <input type="checkbox" id="cbxcBjmpsz" runat="server" name="cbxcBjmpsz" class="fll cbxcBjmp"
                                         value="0.15" title="三者" />三者
                                </label>
                                <label class="blockinline" style="line-height: 18px; margin-right: 10px; _line-height: 22px;">
                                    <input type="checkbox" id="cbxcBjmpry" runat="server" name="cbxcBjmpry" class="fll cbxcBjmp"
                                         value="0.15" title="人员" />人员
                                </label>
                                <label class="blockinline" style="line-height: 18px; margin-right: 10px; _line-height: 22px;">
                                    <input type="checkbox" id="cbxcBjmpdq" runat="server" name="cbxcBjmpdq" class="fll cbxcBjmp"
                                         value="0.2" title="盗抢" />盗抢
                                </label>
                                <asp:TextBox runat="server" ID="txtcBjmp" CssClass="srk5 tr number money bxfy"
                                    Text="0"></asp:TextBox>
                                元
                                <input type="hidden" id="hdncBjmptb" value="" runat="server" />
                            </td>
                        </tr>
                        <% if (CQType == Hx.Car.Enum.CarQuotationType.金融购车)
                           { %>
                        <tr>
                            <td colspan="4" style="background-color: #ccc; color: Black; font-weight: bold;">
                                金融方案
                            </td>
                        </tr>
                        <%--<tr class="hidden">
                            <td class="bg11">
                                银行：
                            </td>
                            <td class="bg3">
                            </td>
                            <td class="bg11">
                                贷款方案：
                            </td>
                            <td>
                                <asp:DropDownList runat="server" ID="ddlLoanType">
                                </asp:DropDownList>-
                            </td>
                        </tr>--%>
                        <tr>
                            <td class="bg11">
                                方案：
                            </td>
                            <td colspan="3">
                                <asp:DropDownList runat="server" ID="ddlBankingType">
                                </asp:DropDownList>
                                <asp:DropDownList runat="server" ID="ddlBank">
                                </asp:DropDownList>
                                <%--<asp:TextBox runat="server" ID="txtProfitMargin" CssClass="srk7" Text="306.7" AutoPostBack="true"
                                    OnTextChanged="txtProfitMargin_TextChanged"></asp:TextBox>--%>
                            </td>
                        </tr>
                        <tr>
                            <td class="bg11">
                                首付款：
                            </td>
                            <td class="bg3">
                                <asp:DropDownList runat="server" ID="ddlSfbl">
                                    <asp:ListItem Text="0%" Value="0"></asp:ListItem>
                                    <asp:ListItem Text="10%" Value="0.1"></asp:ListItem>
                                    <asp:ListItem Text="20%" Value="0.2"></asp:ListItem>
                                    <asp:ListItem Text="30%" Value="0.3" Selected="True"></asp:ListItem>
                                    <asp:ListItem Text="40%" Value="0.4"></asp:ListItem>
                                    <asp:ListItem Text="50%" Value="0.5"></asp:ListItem>
                                    <asp:ListItem Text="60%" Value="0.6"></asp:ListItem>
                                </asp:DropDownList>
                                <asp:TextBox runat="server" ID="txtFirstPayment" CssClass="srk5 tr number money firstcount"
                                    Text="0"></asp:TextBox>
                                元
                            </td>
                            <td class="bg11">
                                贷款额：
                            </td>
                            <td>
                                <asp:TextBox runat="server" ID="txtLoanValue" CssClass="srk5 tr number money" Text="0"></asp:TextBox>
                                元
                            </td>
                        </tr>
                        <tr>
                            <td class="bg11">
                                贷款期：
                            </td>
                            <td class="bg3">
                                <asp:DropDownList runat="server" ID="ddlLoanLength">
                                    <asp:ListItem Text="36" Value="3" Selected="True"></asp:ListItem>
                                    <asp:ListItem Text="24" Value="2"></asp:ListItem>
                                    <asp:ListItem Text="18" Value="1.5"></asp:ListItem>
                                    <asp:ListItem Text="12" Value="1"></asp:ListItem>
                                </asp:DropDownList>
                                月
                            </td>
                            <td class="bg11">
                                利息：
                            </td>
                            <td>
                                <asp:TextBox runat="server" ID="txtInterest" CssClass="srk5 tr number money mustcount"
                                    Text="0"></asp:TextBox>
                                元
                            </td>
                        </tr>
                        <tr>
                            <td class="bg11">
                                月还款额：
                            </td>
                            <td class="bg3">
                                <asp:TextBox runat="server" ID="txtRepaymentPerMonth" CssClass="srk5 tr number money firstcount"
                                    Text="0"></asp:TextBox>
                                元
                            </td>
                            <td class="bg11">
                                押金：
                            </td>
                            <td>
                                <asp:TextBox runat="server" ID="txtcXbyj" CssClass="srk5 tr number money mustcount"
                                    Text="0"></asp:TextBox>
                                元
                            </td>
                        </tr>
                        <tr>
                            <td class="bg11">
                                代收风险金：
                            </td>
                            <td class="bg3">
                                <asp:TextBox runat="server" ID="txtLyfxj" CssClass="srk5 tr number money mustcount"
                                    Text="0"></asp:TextBox>
                                元
                            </td>
                            <td class="bg11">
                                代收资信调查费：
                            </td>
                            <td>
                                <asp:TextBox runat="server" ID="txtDbfqlwf" CssClass="srk5 tr number money mustcount"
                                    Text="0"></asp:TextBox>
                                元
                            </td>
                        </tr>
                        <tr>
                            <td class="bg11">
                                代收公证费：
                            </td>
                            <td colspan="3">
                                <asp:TextBox runat="server" ID="txtGzf" CssClass="srk5 tr number money mustcount"
                                    Text="0"></asp:TextBox>
                                元
                            </td>
                        </tr>
                        <tr class="hide">
                        </tr>
                        <%} %>
                        <tr>
                            <td colspan="4" style="background-color: #ccc; color: Black; font-weight: bold;">
                                其他
                            </td>
                        </tr>
                        <tr>
                            <td class="bg11">
                            </td>
                            <td colspan="3">
                                <label class="blockinline" style="line-height: 18px; _line-height: 22px;">
                                    <asp:CheckBox runat="server" ID="cbxIsSwap" CssClass="fll" />是否置换二手车</label>
                            </td>
                        </tr>
                        <tr id="trSwap" style="display: none;" runat="server">
                            <td class="bg11">
                                <span class="red">*</span>二手车描述：
                            </td>
                            <td colspan="3">
                                <textarea runat="server" id="txtSwapDetail" class="srk2"></textarea>
                            </td>
                        </tr>
                        <%if(Corp.Name.IndexOf("奥迪") > 0){ %>
                        <tr>
                            <td class="bg11">
                            </td>
                            <td colspan="3">
                                <label class="blockinline" style="line-height: 18px; _line-height: 22px;">
                                    <asp:CheckBox runat="server" ID="cbxIsDkh" CssClass="fll" />是否大客户</label>
                            </td>
                        </tr>
                        <tr>
                            <td class="bg11">
                            </td>
                            <td colspan="3">
                                <label class="blockinline" style="line-height: 18px; _line-height: 22px;">
                                    <asp:CheckBox runat="server" ID="cbxIsZcyh" CssClass="fll" />是否忠诚用户</label>
                            </td>
                        </tr>
                        <%} %>
                        <tr>
                            <td class="bg11">
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
                    <td class="bg11">
                    </td>
                    <td>
                        <asp:Button runat="server" ID="btnSubmit" Text="生成整车审核单" CssClass="an1" OnClick="btnSubmit_Click" />
                        <asp:Label ID="lblMsg" runat="server" CssClass="red"></asp:Label>
                    </td>
                </tr>
            </tbody>
        </table>
    </div>
</div>
    </form>
    <div id="pnlcal" style="display: none; position: absolute;">
        <div id="divcal">
        </div>
    </div>
</body>
</html>
