<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="carmodeledit.aspx.cs" Inherits="Hx.BackAdmin.car.carmodeledit" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>信息采集</title>
    <link href=<%=ResourceServer%>/css/admin.css rel="stylesheet" type="text/css" />
    <script src=<%=ResourceServer%>/js/jquery-1.3.2.min.js type="text/javascript"></script>
<style type="text/css">
.colortd label em{width: 16px;height: 16px;line-height: 0;overflow: hidden;*zoom: 1;position:absolute;}
.colortd label em.em-top{width: 16px;height: 8px;z-index: 10;}
.colortd label em.em-buttom{width: 16px;height: 8px;top: 9px;left: 1px;z-index: 10;}
.colortd label span{display: inline-block;width: 16px;height: 16px;border: 1px solid #cecece;*display: inline;*zoom: 1;position:relative;}
</style>
</head>
<body>
    <form id="form1" runat="server">
    <div class="ht_main">
        <asp:ScriptManager runat="server" ID="sm">
        </asp:ScriptManager>
        <asp:UpdatePanel runat="server" ID="up1">
            <ContentTemplate>
                <table width="100%" border="0" cellspacing="0" cellpadding="0" class="biaoge3">
                    <tbody>
                        <tr>
                            <td class="w160 tr">
                                品牌名称：
                            </td>
                            <td>
                                <asp:TextBox runat="server" ID="txtcChangs" CssClass="srk1"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td class="w160 tr">
                                车型名称：
                            </td>
                            <td>
                                <asp:TextBox runat="server" ID="txtcCxmc" CssClass="srk1"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td class="w160 tr">
                                厂家指导价(万元)：
                            </td>
                            <td>
                                <asp:TextBox runat="server" ID="txtfZdj" CssClass="srk1"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td class="w160 tr">
                                车身颜色：
                            </td>
                            <td>
                                <asp:Repeater runat="server" ID="rptcQcys">
                                    <ItemTemplate>
                                        <label class="blockinline">
                                            <label class="fll" style="line-height: 18px; _line-height: 22px;">
                                                <%#Eval("Name")%></label><span style="background-color: <%# Eval("Color")%>; display: inline-block;
                                                    *display: inline; *zoom: 1;"><img alt="<%#Eval("Name")%>" src="../images/color_mb.png"
                                                        style="width: 25px; height: 15px; border: 1px solid #d7d7d7; padding: 0px; margin: 0px;" /></span></label>
                                    </ItemTemplate>
                                </asp:Repeater>
                                <input type="hidden" runat="server" id="hdncQcys" />
                            </td>
                        </tr>
                        <tr>
                            <td class="w160 tr">
                                内饰颜色：
                            </td>
                            <td class="colortd">
                                <input type="hidden" runat="server" id="hdnInnerColor" />
                                <asp:Repeater runat="server" ID="rptcNsys">
                                    <ItemTemplate>
                                        <label class="blockinline">
                                            <label class="fll" style="line-height: 18px; _line-height: 22px;">
                                                <%#Eval("Name")%></label>
                                                    <%# GetInnerColor(Eval("Color").ToString()) %></label>
                                        <%#Container.ItemIndex > 0 && (Container.ItemIndex + 1) % 8 == 0 ? "<br />" : string.Empty%>
                                    </ItemTemplate>
                                </asp:Repeater>
                            </td>
                        </tr>
                    </tbody>
                </table>
            </ContentTemplate>
        </asp:UpdatePanel>
        <table width="100%" border="0" cellspacing="0" cellpadding="0" class="biaoge3">
            <tbody>
                <tr>
                    <td class="w160 tr">
                    </td>
                    <td>
                        <asp:Button runat="server" ID="btnSubmit" Text="保存" OnClick="btnSubmit_Click" CssClass="an1" />
                    </td>
                </tr>
            </tbody>
        </table>
    </div>
    </form>
</body>
</html>
