<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="inventorymg.aspx.cs" Inherits="Hx.JcbWeb.inventory.inventorymg" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>库存管理</title>
    <link href="../css/admin.css" rel="stylesheet" type="text/css" />
    <script src="../js/jquery-1.3.2.min.js" type="text/javascript"></script>
    <script type="text/javascript" language="javascript">
        $(function () {
            $("#content").height($(window).height() - 47);
            $("iframe").each(function () {
                $(this).height($(this).parent().height());
            });
        });
    </script>
</head>
<body>
    <div class="nav">
        <ul style="margin-left: 20px;">
            <li class="current"><a href="javascript:void(0);" class="splitr">销售中车源</a></li>
            <li><a href="javascript:void(0);">已售车源</a></li>
        </ul>
        <input type="button" value="添加车源" id="btnAddCar" name="btnAddCar" class="btn fr btn1"
            style="margin-top: 3px; margin-right: 20px;" />
    </div>
    <table width="100%" class="mt10" id="content">
        <tbody>
            <tr>
                <td class="w200">
                    <iframe src="inventoryleft.aspx" style="height:100%;width:100%;" frameborder="no" border="0" id="frmLeft"></iframe>
                </td>
                <td>
                    <iframe src="inventoryright.aspx" style="height:100%;width:100%;" frameborder="no" border="0" id="frmRight"></iframe>
                </td>
            </tr>
        </tbody>
    </table>
</body>
</html>
