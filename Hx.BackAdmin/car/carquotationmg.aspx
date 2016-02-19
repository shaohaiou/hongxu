<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="carquotationmg.aspx.cs"
    Inherits="Hx.BackAdmin.car.carquotationmg" %>

<%@ Register Assembly="AspNetPager" Namespace="Wuqi.Webdiyer" TagPrefix="webdiyer" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>报价记录管理</title>
    <link href=<%=ResourceServer%>/css/admin.css rel="stylesheet" type="text/css" />
    <script src=<%=ResourceServer%>/js/jquery-1.3.2.min.js type="text/javascript"></script>
    <script src="/js/WdatePicker.js" type="text/javascript"></script>
    <script type="text/javascript">
        var timer_bindevent;
        $(function () {
            $("#txtDate,#txtDateEnd").click(function () {
                WdatePicker({ 'readOnly': 'true', dateFmt: 'yyyy-MM-dd' });
            });
        });
    </script>
</head>
<body>
    <form id="form1" runat="server">
    <div class="ht_main">
        <table width="920" border="0" cellspacing="0" cellpadding="0" class="biaoge4" style="background-color: #f4f8fc;">
            <tr>
                <td class="w40 bold" rowspan="2">
                    查询：
                </td>
                <td>
                    公司：<asp:DropDownList ID="ddlCorporationFilter" runat="server" CssClass="mr10">
                    </asp:DropDownList> 
                    报价人：<asp:TextBox ID="txtCreator" runat="server" CssClass="srk6 mr10"></asp:TextBox> 
                    报价类型：<asp:DropDownList ID="ddlCarQuotationType" runat="server">
                    </asp:DropDownList>
                </td>
            </tr>
            <tr>
                <td>
                    报价日期：<asp:TextBox runat="server" ID="txtDate" CssClass="srk5"></asp:TextBox>
                    -
                    <asp:TextBox runat="server" ID="txtDateEnd" CssClass="srk5 mr10"></asp:TextBox>
                    客户姓名：<asp:TextBox runat="server" ID="txtCustomerName" CssClass="srk6"></asp:TextBox>
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
        <table width="920" border="0" cellspacing="0" cellpadding="0" class="biaoge2">
            <asp:Repeater ID="rptCarQuotation" runat="server">
                <HeaderTemplate>
                    <tr class="bgbt">
                        <td class="w60">
                            客户姓名
                        </td>
                        <td class="w80">
                            客户手机
                        </td>
                        <td class="w300">
                            车型
                        </td>
                        <td class="w60">
                            报价类型
                        </td>
                        <td class="w60">
                            报价人
                        </td>
                        <td class="w120">
                            所属公司
                        </td>
                        <td class="w120">
                            报价时间
                        </td>
                        <td>
                            操作
                        </td>
                    </tr>
                </HeaderTemplate>
                <ItemTemplate>
                    <tr>
                        <td>
                            <%#Eval("CustomerName")%>
                        </td>
                        <td>
                            <%#Eval("CustomerMobile")%>
                        </td>
                        <td>
                            <%#Eval("cCxmc")%>
                        </td>
                        <td>
                            <%# (Hx.Car.Enum.CarQuotationType)(int)Eval("CarQuotationType")%>
                        </td>
                        <td>
                            <%#Eval("Creator")%>
                        </td>
                        <td>
                            <%# GetCorpName(Eval("CorporationID"))%>
                        </td>
                        <td>
                            <%#Eval("CreateTime","{0:yyyy-MM-dd HH:mm:ss}")%>
                        </td>
                        <td class="lan5x">
                            <a class="btndel" href="quotation.aspx?id=<%#Eval("ID") %>" target="_blank">详细信息</a>
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
    </div>
    </form>
</body>
</html>
