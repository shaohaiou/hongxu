<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="carmodeledit.aspx.cs" Inherits="Hx.BackAdmin.car.carmodeledit" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>信息采集</title>
    <link href=<%=ResourceServer%>/css/admin.css rel="stylesheet" type="text/css" />
    <link href=<%=ResourceServer%>/css/spectrum.css rel="stylesheet" type="text/css" />
    <script src=<%=ResourceServer%>/js/jquery-1.9.1.js type="text/javascript"></script>
    <script src=<%=ResourceServer%>/js/spectrum.js type="text/javascript"></script>
    <script type="text/javascript">
        $(function () {
            $(".qcysdel").click(function () {
                $(this).parent().remove();
                CountQcys();
            });
            $("#txtColorQcys").spectrum({
                change: function (color) {
                    $("#txtColorQcys").val(color.toHexString());
                }
            });
            $("#btnQcysadd").click(function () {
                var name = $("#txtNameQcys").val();
                var color = $("#txtColorQcys").val();
                if ($.trim(name) == "") {
                    alert("请输入颜色名称");
                    return;
                }
                if ($(".liqcys[data-name=\"" + name + "\"]").length > 0) {
                    alert("颜色重名");
                    return;
                }
                $("#ulcQcys").append("<li class=\"blockinline fll liqcys\" data-name=\"" + name + "\" data-color=\"" + color + "\">"
                + "<span class=\"fll\">" + name + "</span><span style=\"background-color: " + color + "; display:inline-block; margin-left:5px;*display: inline; *zoom: 1;\"><img alt=\"" + name + "\" src=\"../images/color_mb.png\""
                + "style=\"width: 25px; height: 15px; border: 1px solid #d7d7d7; padding: 0px; margin: 0px;\" /></span>"
                + "<a onclick=\"javascript:void(0);\" class=\"qcysdel\"></a></li>");
                CountQcys();

                $(".qcysdel").unbind("click")
                $(".qcysdel").click(function () {
                    $(this).parent().remove();
                    CountQcys();
                });
            });
            $(".nsysdel").click(function () {
                $(this).parent().remove();
                CountNsys();
            });
            $("#txtColorNsys1").spectrum({
                change: function (color) {
                    $("#txtColorNsys1").val(color.toHexString());
                }
            });
            $("#txtColorNsys2").spectrum({
                change: function (color) {
                    $("#txtColorNsys2").val(color.toHexString());
                }
            });
            $("#btnNsysadd").click(function () {
                var name = $("#txtNameNsys").val();
                var color1 = $("#txtColorNsys1").val();
                var color2 = $("#txtColorNsys2").val();
                if ($.trim(name) == "") {
                    alert("请输入颜色名称");
                    return;
                }
                if (!$("#cbxColorNsys1")[0].checked && !$("#cbxColorNsys2")[0].checked) {
                    alert("请至少选择一种颜色");
                    return;
                }
                if ($(".linsys[data-name=\"" + name + "\"]").length > 0) {
                    alert("颜色重名");
                    return;
                }
                if ($("#cbxColorNsys1")[0].checked && $("#cbxColorNsys2")[0].checked) {
                    $("#ulNsys").append("<li class=\"blockinline fll linsys\" data-name=\"" + name + "\" data-color=\"" + color1 + "," + color2 + "\">"
                    + "<label class=\"blockinline\"><label class=\"fll\" style=\"line-height: 18px; _line-height: 22px;\">" + name + "</label>"
                    + "<span><em class=\"em-top\" style=\"background-color:" + color1 + ";\"></em><em class=\"em-bottom\" style=\"background-color:" + color2 + ";\"></em><em class=\"em-bg\"></em></span>"
                    + "</label><a onclick=\"javascript:void(0);\" class=\"nsysdel\"></a></li>");
                } else {
                    var color = $("#cbxColorNsys1")[0].checked ? color1 : color2;
                    $("#ulNsys").append("<li class=\"blockinline fll linsys\" data-name=\"" + name + "\" data-color=\"" + color + "\">"
                    + "<label class=\"blockinline\"><label class=\"fll\" style=\"line-height: 18px; _line-height: 22px;\">" + name + "</label>"
                    + "<span><em style=\"background-color:" + color + ";\"></em><em class=\"em-bg\"></em></span>"
                    + "</label><a onclick=\"javascript:void(0);\" class=\"nsysdel\"></a></li>");
                }
                CountNsys();
                $(".nsysdel").unbind("click");
                $(".nsysdel").click(function () {
                    $(this).parent().remove();
                    CountNsys();
                });
            });
        })

        function CountQcys() {
            $("#hdncQcys").val($(".liqcys").map(function () {
                return $(this).attr("data-name") + "," + $(this).attr("data-color");
            }).get().join('|'));
        }
        function CountNsys() {
            $("#hdnInnerColor").val($(".linsys").map(function () {
                return $(this).attr("data-name") + "," + $(this).attr("data-color");
            }).get().join('|'));
        }
    </script>
<style type="text/css">
.liqcys,.linsys{width:160px;line-height: 18px; _line-height: 22px;}
.qcysdel,.nsysdel{cursor:pointer;width:18px;height:18px;background:url(../images/del.png) no-repeat;position:absolute;margin-left:2px;}
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
                            <td class="tr">
                                车型名称：
                            </td>
                            <td>
                                <asp:TextBox runat="server" ID="txtcCxmc" CssClass="srk1"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td class="tr">
                                厂家指导价(万元)：
                            </td>
                            <td>
                                <asp:TextBox runat="server" ID="txtfZdj" CssClass="srk1"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td class="tr">
                                车身颜色：
                            </td>
                            <td>
                                <input type="text" class="srk5" id="txtNameQcys" placeholder="颜色名称" />
                                <asp:TextBox ID="txtColorQcys" runat="server" value="#fff"></asp:TextBox>
                                <input type="button" value="添加" id="btnQcysadd" />
                            </td>
                        </tr>
                        <tr>
                            <td></td>
                            <td>
                                <ul id="ulcQcys" style="width:800px">
                                <asp:Repeater runat="server" ID="rptcQcys">
                                    <ItemTemplate>
                                            <li class="blockinline fll liqcys" data-name="<%#Eval("Name")%>" data-color="<%# Eval("Color")%>">
                                                <span class="fll"><%#Eval("Name")%></span><span style="background-color: <%# Eval("Color")%>; display:inline-block; margin-left:5px;
                                                    *display: inline; *zoom: 1;"><img alt="<%#Eval("Name")%>" src="../images/color_mb.png"
                                                        style="width: 25px; height: 15px; border: 1px solid #d7d7d7; padding: 0px; margin: 0px;" /></span>
                                                        <a onclick="javascript:void(0);" class="qcysdel"></a>
                                            </li>
                                    </ItemTemplate>
                                </asp:Repeater>
                                </ul>
                                <input type="hidden" runat="server" id="hdncQcys" />                                
                            </td>
                        </tr>
                        <tr>
                            <td class="w160 tr">
                                内饰颜色：
                            </td>
                            <td>
                                <input type="text" class="srk5" id="txtNameNsys" placeholder="颜色名称" />
                                <input type="checkbox" id="cbxColorNsys1" />
                                <asp:TextBox ID="txtColorNsys1" runat="server" value="#fff"></asp:TextBox>
                                <input type="checkbox" id="cbxColorNsys2" />
                                <asp:TextBox ID="txtColorNsys2" runat="server" value="#fff"></asp:TextBox>
                                <input type="button" value="添加" id="btnNsysadd" />
                            </td>
                        </tr>
                        <tr>
                            <td></td>
                            <td class="colortd">
                                <input type="hidden" runat="server" id="hdnInnerColor" />
                                <ul id="ulNsys" style="width:800px">
                                <asp:Repeater runat="server" ID="rptcNsys">
                                    <ItemTemplate>
                                        <li class="blockinline fll linsys" data-name="<%#Eval("Name")%>" data-color="<%# Eval("Color")%>">
                                        <label class="blockinline">
                                            <label class="fll" style="line-height: 18px; _line-height: 22px;">
                                                <%#Eval("Name")%></label>
                                                    <%# GetInnerColor(Eval("Color").ToString()) %></label><a onclick="javascript:void(0);" class="nsysdel"></a>
                                        </li>
                                    </ItemTemplate>
                                </asp:Repeater>         
                                </ul>                       
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
                        <asp:Button runat="server" ID="btnBack" Text="返回" OnClick="btnBack_Click" CssClass="an1" />
                        <span id="txtMsg" class="red" runat="server"></span>
                    </td>
                </tr>
            </tbody>
        </table>
    </div>
    </form>
</body>
</html>
