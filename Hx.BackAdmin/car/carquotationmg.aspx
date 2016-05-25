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
    <%if (Admin.UserRole == Hx.Components.Enumerations.UserRoleType.财务出纳)
      { %>
      <div style="height:40px;display:block;line-height:40px;margin-bottom:5px;background:#21363e; text-align:right;color:#fff;">
        您好，<asp:HyperLink ID="hyName" runat="server"  style="color:#fff;"></asp:HyperLink> <a href="/logout.aspx" target="_parent" style="color:#fff;">[退出]</a> <a href="/user/changewd.aspx" style="color:#fff;">[修改密码]</a> <a href="/user/adminedit.aspx" target="_blank" style="color:#fff;">[完善信息]</a>
      </div>
      <%} %>
    <div class="ht_main" style="max-width:1180px;margin:10px auto;">
        <table width="1180" border="0" cellspacing="0" cellpadding="0" class="biaoge4" style="background-color: #f4f8fc;">
            <tr>
                <td class="w40 bold" rowspan="2">
                    查询：
                </td>
                <td>
                    公司：<asp:DropDownList ID="ddlCorporationFilter" runat="server" CssClass="mr10">
                    </asp:DropDownList> 
                    提交人：<asp:TextBox ID="txtCreator" runat="server" CssClass="srk6 mr10"></asp:TextBox> 
                    提交类型：<asp:DropDownList ID="ddlCarQuotationType" runat="server">
                    </asp:DropDownList>
                </td>
            </tr>
            <tr>
                <td>
                    提交日期：<asp:TextBox runat="server" ID="txtDate" CssClass="srk5"></asp:TextBox>
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
        <table width="1180" border="0" cellspacing="0" cellpadding="0" class="biaoge2">
            <asp:Repeater ID="rptCarQuotation" runat="server">
                <HeaderTemplate>
                    <tr class="bgbt">
                        <td>
                            操作
                        </td>
                        <td class="w60">
                            客户姓名
                        </td>
                        <td class="w80">
                            客户手机
                        </td>
                        <td class="w300">
                            车型
                        </td>
                        <td class="w120">
                            所属公司
                        </td>
                        <td class="w60">
                            提交类型
                        </td>
                        <td class="w60">
                            提交人
                        </td>
                        <td class="w120">
                            提交时间
                        </td>
                        <td class="w80">
                            财务复核
                        </td>
                        <td class="w60">
                            复核人
                        </td>
                        <td class="w120">
                            复核时间
                        </td>
                    </tr>
                </HeaderTemplate>
                <ItemTemplate>
                    <tr>
                        <td class="lan5x">
                            <a class="btndel" href="quotationcheck.aspx?id=<%#Eval("ID") %>" target="_blank"><%# Eval("CheckStatus").ToString() == "0" ? "审核" : "查看"%></a>
                        </td>
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
                            <%# GetCorpName(Eval("CorporationID"))%>
                        </td>
                        <td>
                            <%# (Hx.Car.Enum.CarQuotationType)(int)Eval("CarQuotationType")%>
                        </td>
                        <td>
                            <%#Eval("Creator")%>
                        </td>
                        <td>
                            <%#Eval("CreateTime","{0:yyyy-MM-dd HH:mm:ss}")%>
                        </td>
                        <td>
                            <%# Eval("CheckStatus").ToString() == "1" ? "<span class=\"green\">已复核</span>" : "<span class=\"red\">未复核</span>"%>
                        </td>
                        <td>
                            <%# Eval("CheckUser")%>
                        </td>
                        <td>
                            <%#Eval("CheckTime")%>
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
