<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="quotationcheck.aspx.cs"
    Inherits="Hx.BackAdmin.car.quotationcheck" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>购车审核单</title>
    <link href=<%=ResourceServer%>/css/admin.css rel="stylesheet" type="text/css" />
    <script language="javascript">
        function printme() {
            var bodyhtml = document.body.innerHTML;
            document.body.innerHTML = document.getElementById('printframe').innerHTML;
            window.print();
            document.body.innerHTML = bodyhtml;
        } 
    </script>
    <style type="text/css">
        .bt{border-top:1px solid #000;}
        .br{border-right:1px solid #000;}
        .bb{border-bottom:1px solid #000;}
        .bl{border-left:1px solid #000;}
        .fs12{font-size:12px;}
        td{word-break:break-all;position: relative;}
        .paper{width:1000px;margin:10px auto;background:#fff; padding-top: 24px;}
        .shadow{ box-shadow: 0 0 3px #000;}
    </style>
    <style type="text/css" media="print">
        @media Print
        {
            .notprint
            {
                display: none;
            }
        }
    </style>
</head>
<body style="background:#ccc">
    <div class="paper shadow" id="printframe">
    
    <div style="width: 800px; margin: 0 auto;">
        <div style="text-align: center; font-size: 26px;padding-bottom:5px;font-weight:bolder;">
            整车销售审核单
        </div>
        <div style="width:100%;border:1px solid #000;font-size:18px;line-height:32px;">
            <table border="0" cellpadding="0" cellspacing="0" width="100%">
                <tr>
                    <td class="tc bold br bb w140">
                        用户单位/个人
                    </td>
                    <td class="br bb w200 <%if(CQ.cQcysName.Length > 12){ %>fs12<%} %>"><%=CQ.CustomerName%></td>
                    <td class="w100 tc bold br bb">销售日期</td>
                    <td class="w120 br bb" style="font-size:14px;"><%=CQ.SaleDay.HasValue ? CQ.SaleDay.Value.ToString("yyyy年MM月dd日") : "&nbsp;"%></td>
                    <td class="w100 tc bold br bb">订车日期</td>
                    <td class="bb" style="font-size:14px;"><%=CQ.PlaceDay.HasValue ? CQ.PlaceDay.Value.ToString("yyyy年MM月dd日") : "&nbsp;"%></td>
                </tr>
            </table>
            <table border="0" cellpadding="0" cellspacing="0" width="100%">
                <tr>
                    <td class="w80 tc bold br bb">颜色(外)</td>
                    <td class="w80 br bb <%if(CQ.cQcysName.Length > 4){ %>fs12<%} %>"><%=CQ.cQcysName%></td>
                    <td class="w40 tc bold br bb">车型</td>
                    <td class="br bb fs12" style="width:138px;"><div style="width:100%;word-break:break-all;position: absolute;top: 0;<%if(Hx.Tools.StrHelper.GetStringLength(CQ.cCxmc) > 23){ %>line-height: 16px;<%}%>"><%= CQ.cCxmc %></div></td>
                    <td class="w100 tc bold br bb">客户性质</td>
                    <td class="w120 br bb"><%=CQ.CarQuotationType == Hx.Car.Enum.CarQuotationType.全款购车 ? "现金" : "金融"%></td>
                    <td class="w100 tc bold br bb">车架号</td>
                    <td class="bb" style="font-size:14px;"><%= CQ.cCjh %></td>
                </tr>
            </table>
            <table border="0" cellpadding="0" cellspacing="0" width="100%">
                <tr>
                    <td class="bb pl5 w220"><span class="bold">标准车价：</span><span style="width: 90px;line-height:18px;" class="bb blockinline tr"><%=string.IsNullOrEmpty(CQ.fZdj) ? "&nbsp;" : CQ.fZdj%></span><span class="bold">元</span></td>
                    <td class="bb pl10" style="width:280px"><span class="bold">实际销售车价：</span><span style="width: 90px;line-height:18px;" class="bb blockinline tr"><%=string.IsNullOrEmpty(CQ.fCjj) ? "&nbsp;" : CQ.fCjj%></span><span class="bold">元</span></td>
                    <td class="bb"><span class="bold">差价审核（领导签字）：</span><span style="width: 74px;line-height:18px;" class="bb blockinline"><%if (CQ.ZJLCheckStatus == 1)
                                                                          { %><span style="line-height:12px;font-size:12px;padding:2px 6px;border:2px solid red;font-weight:bolder" class="red blockinline notprint"><%=CQ.ZJLCheckUser%></span><%}
                                                                          else
                                                                          { %>&nbsp;<%} %></span></td>
                </tr>
            </table>
            <table border="0" cellpadding="0" cellspacing="0" width="100%" style="line-height:28px;">
                <tr>
                    <td class="bb pl5 w240 br">
                        <table border="0" cellpadding="0" cellspacing="0" width="100%">
                            <tr>
                                <td colspan="2" class="bold" style="font-size:20px;">现金：</td>
                            </tr>
                            <tr style="height:28px;">
                                <td><span class="blockinline bold">车<span class="blockinline" style="width:30px;">&nbsp;</span>价：</span><span style="width: 120px;line-height:18px;" class="bb blockinline tr"><%=CQ.CarQuotationType == Hx.Car.Enum.CarQuotationType.金融购车 ? "&nbsp;" : (string.IsNullOrEmpty(CQ.fCjj) ? "&nbsp;" : CQ.fCjj)%></span></td>
                                <td class="bold" style="width:30px">元</td>
                            </tr>
                            <tr style="height:28px;">
                                <td><span class="blockinline bold">预收附加税：</span><span style="width: 96px;line-height:18px;" class="bb blockinline tr"><%=CQ.CarQuotationType == Hx.Car.Enum.CarQuotationType.金融购车 ? "&nbsp;" : (string.IsNullOrEmpty(CQ.cGzs) ? "&nbsp;" : CQ.cGzs)%></span></td>
                                <td class="bold" style="width:30px">元</td>
                            </tr>
                            <tr style="height:28px;">
                                <td><span class="blockinline bold">代收保险费：</span><span style="width: 96px;line-height:18px;" class="bb blockinline tr"><%=CQ.CarQuotationType == Hx.Car.Enum.CarQuotationType.金融购车 ? "&nbsp;" : (string.IsNullOrEmpty(CQ.Bxhj) ? "&nbsp;" : CQ.Bxhj)%></span></td>
                                <td class="bold" style="width:30px">元</td>
                            </tr>
                            <tr style="height:28px;">
                                <td><span class="blockinline bold">承保单位：</span><span style="width: 114px;line-height:18px;" class="bb blockinline tc"><%= CQ.CarQuotationType == Hx.Car.Enum.CarQuotationType.金融购车 ? "&nbsp;" : (string.IsNullOrEmpty(CQ.Bxgs) ? "&nbsp;" : CQ.Bxgs)%></span></td>
                                <td class="bold" style="width:30px">元</td>
                            </tr>
                            <tr style="height:28px;">
                                <td><span class="blockinline bold">上牌及手续费：</span><span style="width: 76px;line-height:18px;" class="bb blockinline tr"><%=CQ.CarQuotationType == Hx.Car.Enum.CarQuotationType.金融购车 ? "&nbsp;" : (string.IsNullOrEmpty(CQ.cSpf) ? "&nbsp;" : CQ.cSpf)%></span></td>
                                <td class="bold" style="width:30px">元</td>
                            </tr>
                            <tr style="height:28px;">
                                <td><span class="blockinline bold">汽车用品：</span><span style="width: 114px;line-height:18px;" class="bb blockinline tr"><%= CQ.CarQuotationType == Hx.Car.Enum.CarQuotationType.金融购车 ? "&nbsp;" : (string.IsNullOrEmpty(CQ.ChoicestGoods) ? "&nbsp;" : CQ.ChoicestGoodsPrice)%></span></td>
                                <td class="bold" style="width:30px">元</td>
                            </tr>
                            <tr style="height:28px;">
                                <td><span class="blockinline bold">无忧服务费：</span><span style="width: 96px;line-height:18px;" class="bb blockinline tr"><%= CQ.CarQuotationType == Hx.Car.Enum.CarQuotationType.金融购车 ? "&nbsp;" : (string.IsNullOrEmpty(CQ.Wyfw) ? "&nbsp;" : CQ.Wyfw)%></span></td>
                                <td class="bold" style="width:30px">元</td>
                            </tr>
                            <tr style="height:28px;">
                                <td><span class="blockinline bold">增值费：</span><span style="width: 134px;line-height:18px;" class="bb blockinline tr"><%=CQ.CarQuotationType == Hx.Car.Enum.CarQuotationType.金融购车 ? "&nbsp;" : (string.IsNullOrEmpty(CQ.Dbsplwf) ? "&nbsp;" : CQ.Dbsplwf)%></span></td>
                                <td class="bold" style="width:30px">元</td>
                            </tr>
                            <tr style="height:28px;">
                                <td><span class="blockinline bold">其<span class="blockinline" style="width:30px;">&nbsp;</span>他：</span><span style="width: 120px;line-height:18px;" class="bb blockinline tr"><%=CQ.CarQuotationType == Hx.Car.Enum.CarQuotationType.金融购车 ? "&nbsp;" : (string.IsNullOrEmpty(CQ.Qtfy) ? "&nbsp;" : CQ.Qtfy)%></span></td>
                                <td class="bold" style="width:30px">元</td>
                            </tr>
                            <tr style="height:28px;">
                                <td><span class="blockinline bold">总计金额：</span><span style="width: 114px;line-height:18px;" class="bb blockinline tr"><%= CQ.CarQuotationType == Hx.Car.Enum.CarQuotationType.金融购车 ? "&nbsp;" : (string.IsNullOrEmpty(CQ.TotalPrinces) ? "&nbsp;" : CQ.TotalPrinces.Replace("元", string.Empty).Trim())%></span></td>
                                <td class="bold" style="width:30px">元</td>
                            </tr>
                        </table>
                    </td>
                    <td class="bb pl5" style="width:280px">
                        <table border="0" cellpadding="0" cellspacing="0" width="100%">
                            <tr>
                                <td colspan="2" class="bold" style="font-size:20px;">按揭：</td>
                            </tr>
                            <tr style="height:28px;">
                                <td><span class="blockinline bold">车款首付：</span><span style="width: 154px;line-height:18px;" class="bb blockinline tr"><%= CQ.CarQuotationType == Hx.Car.Enum.CarQuotationType.全款购车 ? "&nbsp;" : (string.IsNullOrEmpty(CQ.TotalFirstPrinces) ? "&nbsp;" : CQ.TotalFirstPrinces.Replace("元", string.Empty).Trim())%></span></td>
                                <td class="bold" style="width:30px">元</td>
                            </tr>
                            <tr style="height:28px;">
                                <td><span class="blockinline bold">贷款年限：</span><span style="width: 154px;line-height:18px;" class="bb blockinline tr"><%= CQ.CarQuotationType == Hx.Car.Enum.CarQuotationType.全款购车 ? "&nbsp;" : (string.IsNullOrEmpty(CQ.LoanLength) ? "&nbsp;" : CQ.LoanLength)%></span></td>
                                <td class="bold" style="width:30px">年</td>
                            </tr>
                            <tr style="height:28px;">
                                <td><span class="blockinline bold">代收公证费：</span><span style="width: 136px;line-height:18px;" class="bb blockinline tr">&nbsp;</span></td>
                                <td class="bold" style="width:30px">元</td>
                            </tr>
                            <tr style="height:28px;">
                                <td><span class="blockinline bold">代收资信调查费：</span><span style="width: 98px;line-height:18px;" class="bb blockinline tr">&nbsp;</span></td>
                                <td class="bold" style="width:30px">元</td>
                            </tr>
                            <tr style="height:28px;">
                                <td><span class="blockinline bold">承保单位：</span><span style="width: 154px;line-height:18px;" class="bb blockinline tc"><%= CQ.CarQuotationType == Hx.Car.Enum.CarQuotationType.全款购车 ? "&nbsp;" : (string.IsNullOrEmpty(CQ.Bxgs) ? "&nbsp;" : CQ.Bxgs)%></span></td>
                                <td class="bold" style="width:30px">元</td>
                            </tr>
                            <tr style="height:28px;">
                                <td><span class="blockinline bold">无忧服务费：</span><span style="width: 136px;line-height:18px;" class="bb blockinline tr"><%= CQ.CarQuotationType == Hx.Car.Enum.CarQuotationType.全款购车 ? "&nbsp;" : (string.IsNullOrEmpty(CQ.Wyfw) ? "&nbsp;" : CQ.Wyfw)%></span></td>
                                <td class="bold" style="width:30px">元</td>
                            </tr>
                            <tr style="height:28px;">
                                <td><span class="blockinline bold">押<span class="blockinline" style="width:30px;">&nbsp;</span>金：</span><span style="width: 160px;line-height:18px;" class="bb blockinline tr"><%=CQ.CarQuotationType == Hx.Car.Enum.CarQuotationType.全款购车 ? "&nbsp;" : (string.IsNullOrEmpty(CQ.cXbyj) ? "&nbsp;" : CQ.cXbyj)%></span></td>
                                <td class="bold" style="width:30px">元</td>
                            </tr>
                            <tr style="height:28px;">
                                <td><span class="blockinline bold">汽车用品：</span><span style="width: 154px;line-height:18px;" class="bb blockinline tr"><%= CQ.CarQuotationType == Hx.Car.Enum.CarQuotationType.全款购车 ? "&nbsp;" : (string.IsNullOrEmpty(CQ.ChoicestGoods) ? "&nbsp;" : CQ.ChoicestGoodsPrice.Replace("元", string.Empty).Trim())%></span></td>
                                <td class="bold" style="width:30px">元</td>
                            </tr>
                            <tr style="height:28px;">
                                <td><span class="blockinline bold">其<span class="blockinline" style="width:30px;">&nbsp;</span>他：</span><span style="width: 160px;line-height:18px;" class="bb blockinline tr"><%=CQ.CarQuotationType == Hx.Car.Enum.CarQuotationType.全款购车 ? "&nbsp;" : (string.IsNullOrEmpty(CQ.Qtfy) ? "&nbsp;" : CQ.Qtfy)%></span></td>
                                <td class="bold" style="width:30px">元</td>
                            </tr>
                            <tr style="height:28px;">
                                <td><span class="blockinline bold">总计金额：</span><span style="width: 154px;line-height:18px;" class="bb blockinline tr"><%= CQ.CarQuotationType == Hx.Car.Enum.CarQuotationType.全款购车 ? "&nbsp;" : (string.IsNullOrEmpty(CQ.TotalPrinces) ? "&nbsp;" : CQ.TotalPrinces.Replace("元", string.Empty).Trim())%></span></td>
                                <td class="bold" style="width:30px">元</td>
                            </tr>
                        </table>
                    </td>
                    <td class="bb">
                        
                        <table border="0" cellpadding="0" cellspacing="0" width="100%">
                            <tr>
                                <td colspan="2" class="bold" style="font-size:20px;"><%if (CQ.JLCheckStatus == 1)
                                                                          { %><span style="line-height:18px;font-size:12px;" class="pl5 gray blockinline notprint"><%= CQ.ZJLCheckRemark%></span><%}
                                                                          else
                                                                          { %>&nbsp;<%} %></td>
                            </tr>
                            <tr style="height:28px;">
                                <td><span class="blockinline bold">贷款额：</span><span style="width: 160px;line-height:18px;" class="bb blockinline tr"><%= CQ.CarQuotationType == Hx.Car.Enum.CarQuotationType.全款购车 ? "&nbsp;" : (string.IsNullOrEmpty(CQ.LoanValue) ? "&nbsp;" : CQ.LoanValue)%></span></td>
                                <td class="bold" style="width:30px">元</td>
                            </tr>
                            <tr style="height:28px;">
                                <td><span class="blockinline bold">银<span class="blockinline" style="width:30px;">&nbsp;</span>行：</span><span style="width: 146px;line-height:18px;" class="bb blockinline tr"><%=(CQ.CarQuotationType == Hx.Car.Enum.CarQuotationType.全款购车  || CQ.BankingType == Hx.Car.Enum.BankingType.厂方金融) ? "&nbsp;" : (string.IsNullOrEmpty(CQ.BankName) ? "&nbsp;" : CQ.BankName)%></span></td>
                                <td class="bold" style="width:30px">&nbsp;</td>
                            </tr>
                            <tr style="height:28px;">
                                <td><span class="blockinline bold">预收附加税：</span><span style="width: 122px;line-height:18px;" class="bb blockinline tr"><%= CQ.CarQuotationType == Hx.Car.Enum.CarQuotationType.全款购车 ? "&nbsp;" : (string.IsNullOrEmpty(CQ.cGzs) ? "&nbsp;" : CQ.cGzs)%></span></td>
                                <td class="bold" style="width:30px">元</td>
                            </tr>
                            <tr style="height:28px;">
                                <td><span class="blockinline bold">代收上牌手续费：</span><span style="width: 84px;line-height:18px;" class="bb blockinline tr"><%=CQ.CarQuotationType == Hx.Car.Enum.CarQuotationType.全款购车 ? "&nbsp;" : (string.IsNullOrEmpty(CQ.cSpf) ? "&nbsp;" : CQ.cSpf)%></span></td>
                                <td class="bold" style="width:30px">元</td>
                            </tr>
                            <tr style="height:28px;">
                                <td><span class="blockinline bold">代收风险金：</span><span style="width: 122px;line-height:18px;" class="bb blockinline tr"><%=CQ.CarQuotationType == Hx.Car.Enum.CarQuotationType.全款购车 ? "&nbsp;" : (string.IsNullOrEmpty(CQ.Lyfxj) ? "&nbsp;" : CQ.Lyfxj)%></span></td>
                                <td class="bold" style="width:30px">元</td>
                            </tr>
                            <tr style="height:28px;">
                                <td><span class="blockinline bold">代收保险费：</span><span style="width: 124px;line-height:18px;" class="bb blockinline tr"><%= CQ.CarQuotationType == Hx.Car.Enum.CarQuotationType.全款购车 ? "&nbsp;" : (string.IsNullOrEmpty(CQ.Bxhj) ? "&nbsp;" : CQ.Bxhj)%></span></td>
                                <td class="bold" style="width:30px">元</td>
                            </tr>
                            <tr style="height:28px;">
                                <td><span class="blockinline bold">增值费：</span><span style="width: 160px;line-height:18px;" class="bb blockinline tr"><%= CQ.CarQuotationType == Hx.Car.Enum.CarQuotationType.全款购车 ? "&nbsp;" : (string.IsNullOrEmpty(CQ.Dbsplwf) ? "&nbsp;" : CQ.Dbsplwf)%></span></td>
                                <td class="bold" style="width:30px">元</td>
                            </tr>
                            <tr style="height:28px;">
                                <td>&nbsp;</td>
                                <td class="bold" style="width:30px">&nbsp;</td>
                            </tr>
                            <tr style="height:28px;">
                                <td>&nbsp;</td>
                                <td class="bold" style="width:30px">&nbsp;</td>
                            </tr>
                            <tr style="height:28px;">
                                <td>&nbsp;</td>
                                <td class="bold" style="width:30px">&nbsp</td>
                            </tr>
                        </table>
                    </td>
                </tr>
            </table>
            <table border="0" cellpadding="0" cellspacing="0" width="100%">
                <tr>
                    <td class="bb pl5 bold" style="width:40%;">销售员签字确认：</td>
                    <td class="bb bold">销售负责人签字确认：<%if(CQ.JLCheckStatus == 1){ %><span style="line-height:12px;font-size:12px;padding:2px 6px;border:2px solid red;" class="red blockinline notprint"><%=CQ.JLCheckUser %></span><span style="line-height:18px;font-size:12px;" class="pl5 gray blockinline notprint"><%= CQ.JLCheckRemark %></span><%} %></td>
                </tr>
            </table>
            <table border="0" cellpadding="0" cellspacing="0" width="100%" style="line-height:28px;">
                <tr style="height:28px;">
                    <td colspan="3" class="bold pl5">收银员复核：<%=CQ.CheckStatus == 0 ? "<span class=\"gray notprint\">未复核</span>" : "<span class=\"green notprint\">已复核</span>"%></td>
                </tr>
                <tr style="height:28px;">
                    <td class="pl5 w260"><span class="bold">应收金额：</span><span style="width: 90px;line-height:18px;" class="bb blockinline">&nbsp;</span><span class="bold">元</span></td>
                    <td class="pl10" style="width:280px"><span class="bold">实收金额：</span><span style="width: 90px;line-height:18px;" class="bb blockinline tr"><%= string.IsNullOrEmpty(CQ.CarQuotationType == Hx.Car.Enum.CarQuotationType.全款购车 ? CQ.TotalPrinces : CQ.TotalFirstPrinces) ? "&nbsp;" : (CQ.CarQuotationType == Hx.Car.Enum.CarQuotationType.全款购车 ? CQ.TotalPrinces : CQ.TotalFirstPrinces).Replace("元",string.Empty).Trim()%></span><span class="bold">元</span></td>
                    <td><span class="bold">确认签名：</span><span style="width: 90px;line-height:18px;" class="bb blockinline">&nbsp;</span></td>
                </tr>
            </table>
        </div>
        <form id="form1" runat="server" class="notprint mt10">
        <%if(CQ.CheckStatus == 0 && (((int)Admin.UserRole & (int)Hx.Components.Enumerations.UserRoleType.财务出纳) > 0 || Admin.Administrator)){ %>
        <asp:Button runat="server" ID="btnCheck" Text="审核通过" CssClass="an1" OnClick="btnCheck_Click" />
        <%} %>
        <%if(((int)Admin.UserRole & (int)Hx.Components.Enumerations.UserRoleType.销售员) > 0 || Admin.Administrator){ %>
        <asp:Button runat="server" ID="btnBack" Text="返回" CssClass="an1" OnClick="btnBack_Click" />
        <%} %>
        <%if (CQ.CheckStatus == 0 && CQ.JLCheckStatus == 0 && (((int)Admin.UserRole & (int)Hx.Components.Enumerations.UserRoleType.销售经理) > 0 || Admin.Administrator))
          { %>
        <asp:TextBox runat="server" ID="txtJLCheckRemark" CssClass="srk3"></asp:TextBox> <asp:Button runat="server" ID="btnJLCheck" Text="审核通过" CssClass="an1" OnClick="btnJLCheck_Click" />
        <%} %>
        <%if (CQ.CheckStatus == 0 && CQ.ZJLCheckStatus == 0 && (((int)Admin.UserRole & (int)Hx.Components.Enumerations.UserRoleType.总经理) > 0 || Admin.Administrator))
          { %>
        <asp:TextBox runat="server" ID="txtZJLCheckRemark" CssClass="srk3"></asp:TextBox> <asp:Button runat="server" ID="btnZJLCheck" Text="审核通过" CssClass="an1" OnClick="btnZJLCheck_Click" />
        <%} %>
        <input type="button" class="an1" value="打印" onclick="printme();" />
        </form>
    </div>
    </div>
</body>
</html>
