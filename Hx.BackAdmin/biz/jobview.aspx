<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="jobview.aspx.cs" Inherits="Hx.BackAdmin.biz.jobview" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>招聘信息</title>
    <link type="text/css" rel="stylesheet" href="../js/ckeditor/contents.css">
    <style data-cke-temp="1">
        .cke_editable
        {
            cursor: text;
        }
        .cke_editable img, .cke_editable input, .cke_editable textarea
        {
            cursor: default;
        }
        .cke_contents_ltr a.cke_anchor, .cke_contents_ltr a.cke_anchor_empty, .cke_editable.cke_contents_ltr a[name], .cke_editable.cke_contents_ltr a[data-cke-saved-name]
        {
            background: url(../js/ckeditor/plugins/link/images/anchor.png) no-repeat left center;
            border: 1px dotted #00f;
            background-size: 16px;
            padding-left: 18px;
            cursor: auto;
        }
        .cke_contents_ltr img.cke_anchor
        {
            background: url(../js/ckeditor/plugins/link/images/anchor.png) no-repeat left center;
            border: 1px dotted #00f;
            background-size: 16px;
            width: 16px;
            min-height: 15px;
            height: 1.15em;
            vertical-align: text-bottom;
        }
        .cke_contents_rtl a.cke_anchor, .cke_contents_rtl a.cke_anchor_empty, .cke_editable.cke_contents_rtl a[name], .cke_editable.cke_contents_rtl a[data-cke-saved-name]
        {
            background: url(../js/ckeditor/plugins/link/images/anchor.png) no-repeat right center;
            border: 1px dotted #00f;
            background-size: 16px;
            padding-right: 18px;
            cursor: auto;
        }
        .cke_contents_rtl img.cke_anchor
        {
            background: url(../js/ckeditor/plugins/link/images/anchor.png) no-repeat right center;
            border: 1px dotted #00f;
            background-size: 16px;
            width: 16px;
            min-height: 15px;
            height: 1.15em;
            vertical-align: text-bottom;
        }
    </style>
</head>
<body class="cke_editable cke_editable_themed cke_contents_ltr">
    <div style="width:880px;margin:0 auto;">
    <%=CurrentJobOffer == null ? string.Empty : CurrentJobOffer.Content%>
    </div>
</body>
</html>
