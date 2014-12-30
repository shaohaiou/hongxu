<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="inventorymg.aspx.cs" Inherits="Hx.JcbWeb.inventory.inventorymg" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>库存管理</title>
    <link href="../css/admin.css" rel="stylesheet" type="text/css" />
</head>
<body>
    <div class="nav">
        <ul style="margin-left:20px;">
            <li class="current"><a href="javascript:void(0);" class="splitr">销售中车源</a></li>
            <li><a href="javascript:void(0);">已售车源</a></li>
        </ul>
        <input type="button" value="添加车源" id="btnAddCar" name="btnAddCar" class="btn fr btn1" style="margin-top:3px;margin-right:20px;" />
    </div>
    <div class="content">
        <div class="cl">
        </div>
        <div class="cr">
        </div>
    </div>
</body>
</html>
