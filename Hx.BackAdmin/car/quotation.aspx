<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="quotation.aspx.cs" Inherits="Hx.BackAdmin.car.quotation" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>购车报价单</title>
    <link href=<%=ResourceServer%>/css/admin.css rel="stylesheet" type="text/css" />
    <style type="text/css">
        body
        {
            font-size: 14px;
        }
        tr
        {
            height: 40px;
        }
        .head
        {
            font-weight: bolder;
            font-size: 36px;
            display: block;
            text-align: center;
            margin-bottom: 20px;
        }
        .title
        {
            text-indent: 2em;
            font-weight: bolder;
            font-size: 17px;
            line-height: 28px;
            margin: 0px;
        }
        .text
        {
            border: 0px;
            border-bottom: 1px solid;
            height: 20px;
            line-height: 20px;
            display: inline-block;
            vertical-align: bottom;
        }
        @media Print
        {
            .notprint
            {
                display: none;
            }
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <table width="600" border="0" cellspacing="0" cellpadding="0" style="margin-left: 40px;">
            <tr>
                <td colspan="4">
                    <span class="head">购车清单（分期付款）</span>
                </td>
            </tr>
            <tr>
                <td colspan="4">
                    <p class="title">
                        您好，欢迎您来我公司购车，我公司办理汽车销售业务流程及所需要的费用如下：</p>
                </td>
            </tr>
            <tr>
                <td class="w70 tr">
                    <span class="fll">车</span><span class="flr">型：</span>
                </td>
                <td colspan="3">
                    <label class="text" id="txtcCxmc" style="font-size: 12px; width: 330px;">
                        <%= CQ.cChangs %>
                        <%= CQ.cCxmc %></label>
                    颜<span style="width: 14px;" class="blockinline">&nbsp;</span>色：
                    <label class="text" runat="server" id="txtcQcys" style="width: 130px;*width:126px; font-size: 12px;">
                        <% if (!string.IsNullOrEmpty(CQ.cQcys) && CQ.cQcys.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Length == 2)
                           { %><label class="blockinline fll" style="line-height: 18px;"><%=CQ.cQcys.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries)[0] %></label>
                        <span style="background-color: <%=CQ.cQcys.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries)[1] %>;
                            display: inline-block; *display: inline;_zoom:1;">
                            <img id="imgcQcys" src="../images/color_mb.png" style="width: 25px; height: 15px;
                                border: 1px solid #d7d7d7; padding: 0px; margin: 0px;" /></span><%} %></label>
                </td>
            </tr>
            <tr>
                <td class="tr">
                    <span class="fll">车</span><span class="flr">价：</span>
                </td>
                <td colspan="3">
                    <label class="text" id="txtfCjj" style="width: 100%;">
                        <%= CQ.fCjj%></label>
                </td>
            </tr>
            <tr>
                <td class="tr">
                    <span class="fll">贷</span><span class="flr">款：</span>
                </td>
                <td colspan="3">
                    <label class="text" runat="server" id="txtLoanValue" style="width: 420px;*width:400px;">
                        <%= CQ.LoanValue == "0" ? string.Empty : CQ.LoanValue%></label>&nbsp;期限<label class="text"
                            runat="server" id="txtLoanLength" style="width: 60px;"><%= CQ.LoanLength == "0" ? string.Empty : CQ.LoanLength.ToString()%></label>年
                </td>
            </tr>
            <tr>
                <td colspan="4">
                    首付现金：<label class="text" runat="server" id="txtFirstPayment" style="width: 530px;">
                        <%= CQ.TotalFirstPrinces == "0" ? string.Empty : CQ.TotalFirstPrinces%></label>
                </td>
            </tr>
            <tr>
                <td colspan="4">
                    附加税约：<label class="text" runat="server" id="txtfjsy" style="width: 270px;">
                        <%= Math.Round(Hx.Tools.DataConvert.SafeDouble(CQ.cGzs) + Hx.Tools.DataConvert.SafeDouble(CQ.cCcs),2).ToString()%></label>（多退少补，以国家规定标准发票为准）
                </td>
            </tr>
            <tr>
                <td colspan="4">
                    上牌费：<label class="text" runat="server" id="txtcSpf" style="width: 300px;"><%= CQ.cSpf%></label>（包括立户费、照相、牌照、喷沙等）
                </td>
            </tr>
            <tr>
                <td colspan="4">
                    保险费用：<label class="text" runat="server" id="txtbxfy" style="width: 160px;"><%= GetBxfy()%></label>（第三责任险<label
                        class="text" runat="server" id="txtcDszrx" style="width: 70px; text-indent: 2px;"><%= CQ.cDszrx%></label>车损<label
                            class="text" runat="server" id="txtcCsx" style="width: 70px; text-indent: 2px;"><%= CQ.cCsx == "0" ? string.Empty : CQ.cCsx%></label>盗抢<label
                                class="text" runat="server" id="txtcDqx" style="width: 70px; text-indent: 2px;"><%= CQ.cDqx == "0" ? string.Empty : CQ.cDqx%></label>
                </td>
            </tr>
            <tr>
                <td colspan="4">
                    司乘<label class="text" runat="server" id="txtcsc" style="width: 60px; text-indent: 2px;"><%= GetSc()%></label>不计免赔<label
                        class="text" runat="server" id="txtcBjmp" style="width: 55px; text-indent: 2px;"><%= CQ.cBjmp == "0" ? string.Empty : CQ.cBjmp%></label>玻璃<label
                            class="text" runat="server" id="txtcBlx" style="width: 55px; text-indent: 2px;"><%= (!CQ.IscBlx || CQ.cBlx == "0") ? string.Empty : CQ.cBlx%></label>自然<label
                                class="text" runat="server" id="txtcZrx" style="width: 55px; text-indent: 2px;"><%= (!CQ.IscZrx || CQ.cZrx == "0") ? string.Empty : CQ.cZrx%></label>指定维修<label
                                    class="text" runat="server" id="txtcZdwx" style="width: 55px; text-indent: 2px;"><%= (!CQ.IscZdwx || CQ.cZdwx == "0") ? string.Empty : CQ.cZdwx%></label>交强险<label
                                        class="text" runat="server" id="txtcJqs" style="width: 55px; text-indent: 2px;"><%= CQ.cJqs == "0" ? string.Empty : CQ.cJqs%></label>）
                </td>
            </tr>
            <tr>
                <td colspan="4">
                    履约风险金：<label class="text" runat="server" id="txtLyfxj" style="width: 210px;"><%= CQ.Lyfxj == "0" ? string.Empty : CQ.Lyfxj%></label>
                    续保押金：<label class="text" runat="server" id="txtcXbyj" style="width: 150px;">
                        <%= CQ.cXbyj == "0" ? string.Empty : CQ.cXbyj%></label>（可退还）
                </td>
            </tr>
            <tr>
                <td colspan="4">
                    交车费：<label class="text" runat="server" id="txtDbsplwf" style="width: 544px;"><%= CQ.Dbsplwf == "0" ? string.Empty : CQ.Dbsplwf%></label>
                </td>
            </tr>
            <tr>
                <td colspan="4">
                    代办分期付款手续费、劳务费：<label class="text" runat="server" id="txtDbfqlwf" style="width: 404px;"><%= CQ.Dbfqlwf == "0" ? string.Empty : CQ.Dbfqlwf%></label>
                </td>
            </tr>
            <tr>
                <td class="tr">
                    <span class="fll">合</span><span class="flr">计：</span>
                </td>
                <td colspan="3">
                    <label class="text" id="txtTotalPrinces" style="width: 100%;">
                        <%= CQ.TotalPrinces%></label>
                </td>
            </tr>
            <tr>
                <td colspan="4">
                    分期付款月还款额：<label class="text" runat="server" id="txtRepaymentPerMonth" style="width: 473px;"><%= CQ.RepaymentPerMonth == "0" ? string.Empty : CQ.RepaymentPerMonth%></label>
                </td>
            </tr>
            <tr>
                <td class="w70 tr">
                    <span class="fll">精</span><span class="flr">品：</span>
                </td>
                <td style="width:300px;*width:290px;">
                    <label class="text" runat="server" id="txtChoicestGoods" style="width: 100%;">
                        <%= CQ.ChoicestGoods%></label>
                </td>
                <td class="w70 tr">
                    精品金额：
                </td>
                <td class="w160">
                    <label class="text" runat="server" id="txtChoicestGoodsPrince" style="width: 100%;">
                        <%= CQ.ChoicestGoodsPrice == "0" ? string.Empty : CQ.ChoicestGoodsPrice%></label>
                </td>
            </tr>
            <tr>
                <td class="w70 tr">
                    <span class="fll">备</span><span class="flr">注：</span>
                </td>
                <td colspan="3">
                    <label class="text" runat="server" id="txtGift" style="width: 100%;">
                        <%= CQ.Gift%></label>
                </td>
            </tr>
            <tr>
                <td colspan="4">
                    此单为内部专用，不得外传，谢谢合作！
                </td>
            </tr>
            <tr>
                <td colspan="2">
                    客户确认以上费用签字：
                </td>
                <td colspan="2">
                    销售顾问：<%= CQ.Creator %>
                </td>
            </tr>
            <tr>
                <td colspan="2">
                    联系电话：<%= CQ.CustomerMobile %>
                </td>
                <td colspan="2">
                    联系电话：
                </td>
            </tr>
            <tr>
                <td colspan="2">
                    置换二手车客户签字：
                </td>
                <td class="tr" colspan="2">
                    <span class="blockinline tc w40">
                        <%= CQ.CreateTime.Value.Year%></span> <span class="bold blockinline">年</span>
                    <span class="blockinline tc w40">
                        <%= CQ.CreateTime.Value.Month%></span> <span class="bold blockinline">月</span>
                    <span class="blockinline tc w40">
                        <%= CQ.CreateTime.Value.Day%></span> <span class="bold blockinline">日</span>
                </td>
            </tr>
            <tr class="notprint">
                <td colspan="4">
                    <input type="button" id="btnPrint" value="打印" class="an1 mr10" onclick="javascript:window.print();" />
                    <asp:Button runat="server" ID="btnBack" Text="返回" CssClass="an1" OnClick="btnBack_Click" />
                </td>
            </tr>
        </table>
    </div>
    </form>
</body>
</html>
