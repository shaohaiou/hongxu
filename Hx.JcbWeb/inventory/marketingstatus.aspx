<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="marketingstatus.aspx.cs"
    Inherits="Hx.JcbWeb.inventory.marketingstatus" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>快速营销状态</title>
    <link href="../css/admin.css" rel="stylesheet" type="text/css" />
    <script src="../js/jquery-1.3.2.min.js" type="text/javascript"></script>
</head>
<body>
    <div class="nav">
        <ul style="margin-left: 20px;">
            <li class="current"><a href="javascript:void(0);" class="splitr">进行中</a></li>
            <li><a href="javascript:void(0);" class="splitr">等待中</a></li>
            <li><a href="javascript:void(0);" class="splitr">已失败</a></li>
            <li><a href="javascript:void(0);">已成功</a></li>
        </ul>
    </div>
    <div class="market">
        <ul>
            <asp:Repeater runat="server" ID="rptData" OnItemDataBound="rptData_ItemDataBound">
                <ItemTemplate>
                    <li>
                        <img id="picFirst" src="../images/fm.jpg" runat="server" />
                        <asp:Label runat="server" ID="lblTTL" class="ttl"></asp:Label>
                        <span id="txtMsg<%#Eval("ID") %>" class="red" ></span>
                        <a href="#@" target="_blank" id="linkView<%#Eval("ID") %>"></a></li>
                </ItemTemplate>
            </asp:Repeater>
        </ul>
    </div>
</body>
</html>
