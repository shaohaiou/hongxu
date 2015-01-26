<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="yxtg.aspx.cs" Inherits="Hx.JcbWeb.inventory.yxtg" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>营销推广</title>
    <link href="../css/admin.css" rel="stylesheet" type="text/css" />
    <script src="../js/jquery-1.3.2.min.js" type="text/javascript"></script>
    <script type="text/javascript">
        $(function () {
            $(".btnAutoLogin").click(function () {
                window.external.AutoLogin($(this).attr("val"));
            });
        })
    </script>
</head>
<body>
    <form id="form1" runat="server">
    <div style="padding: 0 5px;">
        <div style="height: 80px; line-height: 24px;">
            <img alt="" src="<%= CurrentCar.FirstPic%>" class="fl block" style="height: 76px;
                width: 120px; margin-top: 2px;" />
            <span>
                <%=CurrentCar.cCxmc%></span><br />
            <span class="gray">
                <%=CurrentCar.Bxlc%>万公里
                <%=CurrentCar.Wgys%></span><br />
            <span class="red bold">预售价：<%=CurrentCar.Ysj%></span> <span></span>
        </div>
        <div>
            <div class="pdtitle">
                <ul>
                    <li class="w200 tc">网站名称</li>
                    <li class="w100 tc">营销状态</li>
                    <li class="w120 tl">上传时间</li>
                    <li class="w200 tl">操作</li>
                </ul>
            </div>
            <div class="pdlist">
                <asp:Repeater runat="server" ID="rptData" OnItemDataBound="rptData_ItemDataBound">
                    <ItemTemplate>
                        <ul style="height:22px;">
                            <li class="w200 tc">
                                <%# Eval("Text") %></li>
                            <li class="w100 tc">
                                <asp:Label ID="lblMarketingStatus" runat="server"></asp:Label>
                            </li>
                            <li class="w120 tl">
                                <asp:Label runat="server" ID="lblUploadDate"></asp:Label></li>
                            <li class="w200 tl">
                                <a class="btnAutoLogin" id="btnAutoLogin" runat="server" href="javascript:void(0);">进后台</a>
                                <a runat="server" class="dis" id="btnView" href="javascript:void(0);" target="_blank">查看</a></li>
                        </ul>
                    </ItemTemplate>
                </asp:Repeater>
            </div>
        </div>
        <div class="tc psubmit">
            <asp:Button runat="server" ID="btnClose" Text="关闭" CssClass="an1" />
        </div>
    </div>
    </form>
</body>
</html>
