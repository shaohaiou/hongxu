<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="scanfile.aspx.cs" Inherits="Hx.BackAdmin.scan.scanfile" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div class="ht_main">
        <%if(Admin.Administrator){ %>
        <ul class="xnav">
            <li class="current"><a href="scanfile.aspx?tid=<%=GetInt("tid") %>">扫描文件</a></li>
            <li><a href="filesearch.aspx?tid=<%=GetInt("tid") %>">文件查询</a></li>
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
        <table width="600" border="0" cellspacing="0" cellpadding="0" class="biaoge2" id="tblbank">
            <asp:Repeater ID="rptData" runat="server" OnItemDataBound="rptData_ItemDataBound">
                <HeaderTemplate>
                    <tr class="bgbt">
                        <td class="w120">
                            银行名称
                        </td>
                        <td class="w120">
                            所属公司
                        </td>
                        <td>
                            利率(3年)
                        </td>
                        <td>
                            利率(2年)
                        </td>
                        <td>
                            利率(1年)
                        </td>
                        <td>
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
                            <asp:DropDownList runat="server" ID="ddlCorporation">
                            </asp:DropDownList>
                        </td>
                        <td>
                            <input type="text" runat="server" id="txtBankProfitMargin3y" class="srk1 w60" value='<%#Eval("BankProfitMargin3y")%>' />
                        </td>
                        <td>
                            <input type="text" runat="server" id="txtBankProfitMargin2y" class="srk1 w60" value='<%#Eval("BankProfitMargin2y")%>' />
                        </td>
                        <td>
                            <input type="text" runat="server" id="txtBankProfitMargin1y" class="srk1 w60" value='<%#Eval("BankProfitMargin1y")%>' />
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
