<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="jobedit.aspx.cs" ValidateRequest="false"
    Inherits="Hx.BackAdmin.biz.jobedit" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>招聘信息设置</title>
    <link href="../css/admin.css" rel="stylesheet" type="text/css" />
    <script src="../js/jquery-1.3.2.min.js" type="text/javascript"></script>
    <script src="../js/ckeditor/ckeditor.js" type="text/javascript"></script>
    <script type="text/javascript">
        $(function () {
            CKEDITOR.replace('txtContent', { toolbar: 'Basic', height: 680, width: 980 });
        });
    </script>
    <style type="text/css">
        .content
        {
            width: 480px;
            height: 105px;
            position: relative;
        }
        .cstatic
        {
            padding: 26px 0 0 150px;
            color: White;
            font-weight: bold;
            font-size: 20px;
            position: absolute;
            width: 330px;
            left: 0px;
        }
        .jz
        {
            padding: 5px 0 0 30px;
            color: Black;
        }
        .nr
        {
            font-weight: normal;
            font-size: 12px;
            overflow: hidden;
            padding: 3px 0 0 20px;
        }
        img
        {
            float: left;
            width: 474px !important;
            height: 99px !important;
            z-index: 8;
        }
        a:hover
        {
            text-decoration: none;
        }
        p
        {
            margin: 0;
        }
        #txtTitle
        {
            width: 290px;
            border: 0;
            margin: 0;
            color: White;
            overflow: hidden;
            background: url(../images/zhaopin.jpg) no-repeat -170px -28px;
            line-height:23px;
            line-height:24px\9;
            *line-height:24px;
            _line-height:24px;
            height:50px;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
    <div class="ht_main">
        <table width="100%" border="0" cellspacing="0" cellpadding="0" class="biaoge3">
            <caption class="bt2">
                招聘信息设置</caption>
            <tbody>
                <tr>
                    <td class="tr">
                        引导框：
                    </td>
                    <td>
                        <div class="content">
                            <a href="javascript:void(0);" title="红旭集团所有4S店招聘信息" class="fll">
                                <img src="../images/zhaopin.jpg?t=<%=DateTime.Now.ToString("yyyyMMddHHmmss") %>" />
                                <div class="cstatic">
                                    <div class="nr">
                                        <asp:TextBox ID="txtTitle" runat="server" TextMode="MultiLine" Rows="2"></asp:TextBox>
                                    </div>
                                </div>
                            </a>
                        </div>
                    </td>
                </tr>
                <tr>
                    <td class="tr">
                        详细说明：
                    </td>
                    <td>
                        <asp:TextBox ID="txtContent" runat="server" TextMode="MultiLine" CssClass="ckeditor"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td>
                    </td>
                    <td>
                        <input type="hidden" runat="server" id="hdnID" />
                        <asp:Button runat="server" ID="btnSubmit" Text="保存" CssClass="an1" OnClick="btnSubmit_Click" />
                    </td>
                </tr>
            </tbody>
        </table>
    </div>
    </form>
</body>
</html>
