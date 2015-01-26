<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="inventoryright.aspx.cs"
    Inherits="Hx.JcbWeb.inventory.inventoryright" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <link href="../css/admin.css" rel="stylesheet" type="text/css" />
    <script src="../js/jquery-1.3.2.min.js" type="text/javascript"></script>
    <script src="../js/WdatePicker.js" type="text/javascript"></script>
    <script type="text/javascript" language="javascript">
        $(function () {
            $("#btnSearchswith").click(function () {
                $("#psearch").slideToggle(200);
                if ($(this).html() == "展开查询条件")
                    $(this).html("收起查询条件");
                else
                    $(this).html("展开查询条件");
            });
            $("#txtDateLastUpdateTimeBegin").click(function () {
                WdatePicker({ 'readOnly': 'true', dateFmt: 'yyyy-MM-dd', maxDate: '<%=DateTime.Today.ToString("yyyy-MM-dd") %>' });
            });
            $("#txtDateLastUpdateTimeEnd").click(function () {
                WdatePicker({ 'readOnly': 'true', dateFmt: 'yyyy-MM-dd', maxDate: '<%=DateTime.Today.ToString("yyyy-MM-dd") %>' });
            });
            $(".btnYxtg").click(function () {
                window.external.Yxtg($(this).attr("val"));
            });
            $(".btnYjyx").click(function () {
                window.external.Yjyx($(this).attr("val"));
            });
            $(".btnXgxx").click(function () {
                window.external.Xgxx($(this).attr("val"));
            });
        });
    </script>
</head>
<body>
    <form id="form1" runat="server">
    <div class="search">
        <div class="hide" id="psearch">
            <table>
                <tbody>
                    <tr>
                        <td class="tr w100">
                            VIN码：
                        </td>
                        <td class="w220">
                            <asp:TextBox runat="server" ID="txtSearchVINCode" CssClass="txtsearch"></asp:TextBox>
                        </td>
                        <td class="w220 tr">
                            车牌：
                        </td>
                        <td>
                            <asp:TextBox runat="server" ID="txtSearchCph" CssClass="txtsearch"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="tr">
                            预售价格：
                        </td>
                        <td>
                            <asp:TextBox runat="server" ID="txtSearchYsjBegin" CssClass="txtsearch1"></asp:TextBox>至
                            <asp:TextBox runat="server" ID="txtSearchYsjEnd" CssClass="txtsearch1"></asp:TextBox>万元
                        </td>
                        <td class="tr">
                            更新时间：
                        </td>
                        <td>
                            <asp:TextBox runat="server" ID="txtDateLastUpdateTimeBegin" CssClass="srk5"></asp:TextBox>
                            至
                            <asp:TextBox runat="server" ID="txtDateLastUpdateTimeEnd" CssClass="srk5"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="tr">
                            出兑价格：
                        </td>
                        <td>
                            <asp:DropDownList runat="server" ID="ddlCdjg">
                                <asp:ListItem Text="-请选择-" Value="-1"></asp:ListItem>
                                <asp:ListItem Text="5万以内" Value="1"></asp:ListItem>
                                <asp:ListItem Text="5-10万" Value="2"></asp:ListItem>
                                <asp:ListItem Text="10-15万" Value="3"></asp:ListItem>
                                <asp:ListItem Text="15-20万" Value="4"></asp:ListItem>
                                <asp:ListItem Text="20-30万" Value="5"></asp:ListItem>
                                <asp:ListItem Text="30万以上" Value="6"></asp:ListItem>
                            </asp:DropDownList>
                        </td>
                        <td class="tr">
                            &nbsp;
                        </td>
                        <td>
                            <asp:Button runat="server" ID="btnSearch" Text="查询" CssClass="btn btn2" OnClick="btnSearch_Click" />
                        </td>
                    </tr>
                </tbody>
            </table>
        </div>
        <a href="javascript:void(0);" id="btnSearchswith">展开查询条件</a>
    </div>
    <div id="pData">
        <div class="pdtitle">
            <ul>
                <li class="bfb10"><span class="ml20">序号</span></li>
                <li class="bfb12">图片</li>
                <li class="bfb20">基本信息</li>
                <li class="bfb12">预售价</li>
                <li class="bfb12">出兑价</li>
                <li class="bfb12">更新时间</li>
                <li class="bfb10">营销状态</li>
                <li class="bfb10">操作</li>
            </ul>
        </div>
        <div class="pdlist">
            <asp:Repeater runat="server" ID="rptData">
                <ItemTemplate>
                    <ul>
                        <li class="bfb10"><span class="ml20">
                            <%#Eval("ID") %></span></li>
                        <li class="bfb12 tc vc">
                            <img src="<%#Eval("FirstPic") %>" /><br />
                            共计<%#Eval("PicCount") %>张图片 </li>
                        <li class="bfb20">
                            <%#Eval("cCxmc")%><br />
                            VIN:<%#Eval("VINCode")%><br />
                            车牌号：<%#string.IsNullOrEmpty(Eval("Cph").ToString()) ? "-" : Eval("Cph").ToString()%><br />
                            <%#Eval("Bxlc")%>万公里
                            <%#Eval("Wgys")%><br />
                            <a href="javascript:void(0)" class="btnXgxx" val="<%#Eval("ID") %>">
                                修改信息</a> </li>
                        <li class="bfb12"><span class="red strong">
                            <%#Eval("Ysj")%>万</span><br />
                            <a href="javascript:void(0)">改价格</a> </li>
                        <li class="bfb12"><strong>
                            <%#decimal.Parse(Eval("Cdjg").ToString()) == 0 ? "未出兑" : (Eval("Cdjg").ToString() + "万")%></strong>
                        </li>
                        <li class="bfb12">
                            <%#Eval("LastUpdateTime", "{0:yyyy-MM-dd}<br />{0:HH:mm}")%></li>
                        <li class="bfb10"><a href="javascript:void(0);"
                            val="<%#Eval("ID") %>" class="btnYxtg">营销推广</a><br />
                            <a href="javascript:void(0);" class="btnYjyx" val="<%#Eval("ID") %>">
                                一键营销</a> </li>
                        <li class="bfb10"><a href="javascript:void(0);">删除</a></li>
                    </ul>
                </ItemTemplate>
            </asp:Repeater>
        </div>
        <div class="pdpage">
        </div>
    </div>
    </form>
</body>
</html>
