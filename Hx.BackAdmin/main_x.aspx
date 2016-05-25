<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="main_x.aspx.cs" Inherits="Hx.BackAdmin.main_x" %>


<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>无标题文档</title>
    <link href=<%=ResourceServer%>/css/admin.css rel="stylesheet" type="text/css" />
</head>
<body>
    <div class="ht_main">
        <div class="bt0">
            管理中心首页</div>
        <div class="bt2">
            您的资料</div>
        <div class="bt2x lan5">
            用户名：<%= AdminName %></div>
        <div class="bt2">
            快捷访问</div>
        <div class="bt2x lan5">
            <%if (Admin.Administrator
                  || ((int)Admin.UserRole & (int)Hx.Components.Enumerations.UserRoleType.销售经理) > 0
                  || ((int)Admin.UserRole & (int)Hx.Components.Enumerations.UserRoleType.总经理) > 0
                  || ((int)Admin.UserRole & (int)Hx.Components.Enumerations.UserRoleType.销售员) > 0)
              {%>
            <a href="/car/carquotation.aspx" target="_blank">车辆报价</a>
            <a href="/global/choicestgoodsmg.aspx">精品管理</a>
            <a href="/global/bankmg.aspx">银行管理</a>
            <a href="/car/automotivetypemg.aspx">有效车型设置</a>
            <%} %>
            <%if(Admin.UserRole == Hx.Components.Enumerations.UserRoleType.总经理 && !string.IsNullOrEmpty(Admin.OAID)){ %>
            <a target="_top" href="/dayreport/main_i.aspx?Nm=<%=Admin.Name %>&Id=<%=Admin.OAID %>&Mm=<%=(System.DateTime.Now.Year + System.DateTime.Now.Month + System.DateTime.Now.Day) * Hx.Tools.DataConvert.SafeInt(Admin.OAID) * 3%>">日报表</a>
            <a href="/car/carquotationmg.aspx">整车审核</a>
            <%} %>
            <%if (Admin.Administrator
                  || ((int)Admin.UserRole & (int)Hx.Components.Enumerations.UserRoleType.车型管理员) > 0)
              {%>
            <a href="/car/cxmg.aspx">车型管理</a>
            <a href="/global/corporationmg.aspx">公司管理</a>
            <%} %>
        </div>
    </div>
</body>
</html>
