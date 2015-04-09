<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="index.aspx.cs" Inherits="Hx.JcbWeb.admin.index" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">


<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>集车宝后台管理中心</title>
</head>
<frameset rows="*" cols="196,*" frameborder="no" border="0" framespacing="0">
	<frame src="left.aspx" name="leftFrame" scrolling="No" noresize="noresize" id="leftFrame" title="leftk" />

	<frameset rows="36,*" frameborder="no" border="0" framespacing="0">
		<frame src="top.aspx" name="topFrame" scrolling="No" noresize="noresize" id="topFrame" title="topk" />
		<frame src="main.aspx" name="mainFrame" id="mainFrame" title="nrk" />
	</frameset>
	
</frameset>
<noframes><body>
</body>
</noframes></html>
