<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="corpmienmore.aspx.cs" Inherits="Hx.BackAdmin.biz.corpmienmore" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>集团荣誉-全部</title>
    <link href="../css/admin.css" rel="stylesheet" type="text/css" />
    <style type="text/css">
        .content
        {
            width: 960px;
            margin: 0 auto;
        }
        .content ul li
        {
            float: left;
            width: 120px;
            height:114px;
            padding: 23px 0;
            display: inline;
            margin:0 5px;
        }
    </style>
</head>
<body>
    <div class="content">
        <ul>
            <asp:Repeater runat="server" ID="rptData">
                <ItemTemplate>
                    <li><a href="corpmienview.aspx?id=<%#Eval("ID") %>" title="<%#Eval("Introduce") %>"
                        target="_blank" style="display: block; position: relative; padding-top: 100px;
                        width: 120px; text-align: center;">
                        <img src="<%#Eval("Pic")%>" alt="" style="width: 100px; height: 100px; position: absolute;
                            left: 10px; top: 0;" />
                        <%#Eval("Introduce") %></a></li>
                </ItemTemplate>
            </asp:Repeater>
        </ul>
    </div>
</body>
</html>
