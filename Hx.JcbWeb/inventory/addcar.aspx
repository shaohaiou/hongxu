<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="addcar.aspx.cs" Inherits="Hx.JcbWeb.inventory.addcar" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>添加车源</title>
    <link href="../css/admin.css" rel="stylesheet" type="text/css" />
</head>
<body>
    <div class="header">
        <h1>
            添加车源</h1>
        <span class="fr white"><a href="#">我的反馈</a> | <a href="#">意见反馈</a><input type="button"
            id="btnClose" class="hclose" /></span>
    </div>
    <div class="main">
        <form id="form1" runat="server">
        <div class="gray">
            车源信息填写越全面在车源上传到各网站时信息完整度越高，排位也越靠前。
        </div>
        <div class="ves">
            <div class="vtitle">基本信息</div>
            <table style="width:100%;">
                <thead>
                    <tr>
                        <td>&nbsp;</td>
                        <td></td>
                    </tr>
                </thead>
                <tbody>
                    <tr>
                        <td class="tr w160">
                            车架号(VIN)：
                        </td>
                        <td>
                            <input type="text" value="" class="srk5" runat="server" id="txtVINCode" />
                        </td>
                    </tr>
                </tbody>
                <tfoot>
                    <tr>
                        <td>&nbsp;</td>
                        <td></td>
                    </tr>
                </tfoot>
            </table>
        </div>
        </form>
    </div>
</body>
</html>
