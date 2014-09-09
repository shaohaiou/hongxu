<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="carcollect.aspx.cs" Inherits="Hx.BackAdmin.car.carcollect" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>信息采集</title>
    <link href="../css/admin.css" rel="stylesheet" type="text/css" />
    <script src="../js/jquery-1.3.2.min.js" type="text/javascript"></script>
    <script language="javascript" type="text/javascript">

        var timer_bindevent;
        $(function () {
            timer_bindevent = setTimeout(function () {
                bindevent();
            }, 500);
        });

        function bindevent() {
            $("#btnCollectInfo").unbind("click");

            $("#btnCollectInfo").click(function () {
                if ($("#txtUrl").val() == "") {
                    $("#lblMsg").text("请输入采集地址");
                    setTimeout(function () {
                        $("#lblMsg").text("");
                    }, 500);
                    return false;
                }
                $("#lblMsg").text("正在采集...");
                setTimeout(function () {
                    $("#btnCollectInfo").attr("disabled", "true");
                }, 100);
            });

            if (timer_bindevent)
                clearTimeout(timer_bindevent);
            timer_bindevent = setTimeout(function () {
                bindevent();
            }, 500);
        }

    </script>
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
                                采集地址：
                            </td>
                            <td>
                                <asp:TextBox runat="server" ID="txtUrl" CssClass="srk1"></asp:TextBox>
                                <asp:Button runat="server" ID="btnCollectInfo" CssClass="an2" Text="采集" OnClick="btnCollectInfo_Click" />
                                <asp:Label runat="server" ID="lblMsg" CssClass="red"></asp:Label>
                            </td>
                        </tr>
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
                                汽车级别：
                            </td>
                            <td>
                                <asp:TextBox runat="server" ID="txtcJibie" CssClass="srk1"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td class="w160 tr">
                                发动机：
                            </td>
                            <td>
                                <asp:TextBox runat="server" ID="txtcFdj" CssClass="srk1"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td class="w160 tr">
                                变速箱：
                            </td>
                            <td>
                                <asp:TextBox runat="server" ID="txtcBsx" CssClass="srk1"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td class="w160 tr">
                                长×宽×高(mm)：
                            </td>
                            <td>
                                <asp:TextBox runat="server" ID="txtcCkg" CssClass="srk1"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td class="w160 tr">
                                车身结构：
                            </td>
                            <td>
                                <asp:TextBox runat="server" ID="txtcCsjg" CssClass="srk1"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td class="w160 tr">
                                最高车速(km/h)：
                            </td>
                            <td>
                                <asp:TextBox runat="server" ID="txtcZgcs" CssClass="srk1"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td class="w160 tr">
                                官方0-100加速(s)：
                            </td>
                            <td>
                                <asp:TextBox runat="server" ID="txtcGfjs" CssClass="srk1"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td class="w160 tr">
                                实测0-100加速(s)：
                            </td>
                            <td>
                                <asp:TextBox runat="server" ID="txtcScjs" CssClass="srk1"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td class="w160 tr">
                                实测100-0制动(m)：
                            </td>
                            <td>
                                <asp:TextBox runat="server" ID="txtcSczd" CssClass="srk1"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td class="w160 tr">
                                实测油耗(L)：
                            </td>
                            <td>
                                <asp:TextBox runat="server" ID="txtcScyh" CssClass="srk1"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td class="w160 tr">
                                工信部综合油耗(L)：
                            </td>
                            <td>
                                <asp:TextBox runat="server" ID="txtcGxbyh" CssClass="srk1"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td class="w160 tr">
                                整车质保：
                            </td>
                            <td>
                                <asp:TextBox runat="server" ID="txtcZczb" CssClass="srk1"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td colspan="2" style="background-color: #ccc; color: Black; font-weight: bold;">
                                车身：
                            </td>
                        </tr>
                        <tr>
                            <td class="w160 tr">
                                长度(mm)：
                            </td>
                            <td>
                                <asp:TextBox runat="server" ID="txtcChang" CssClass="srk1"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td class="w160 tr">
                                宽度(mm)：
                            </td>
                            <td>
                                <asp:TextBox runat="server" ID="txtcKuan" CssClass="srk1"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td class="w160 tr">
                                高度(mm)：
                            </td>
                            <td>
                                <asp:TextBox runat="server" ID="txtcGao" CssClass="srk1"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td class="w160 tr">
                                轴距(mm)：
                            </td>
                            <td>
                                <asp:TextBox runat="server" ID="txtcZhouju" CssClass="srk1"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td class="w160 tr">
                                前轮距(mm)：
                            </td>
                            <td>
                                <asp:TextBox runat="server" ID="txtcQnju" CssClass="srk1"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td class="w160 tr">
                                后轮距(mm)：
                            </td>
                            <td>
                                <asp:TextBox runat="server" ID="txtcHnju" CssClass="srk1"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td class="w160 tr">
                                最小离地间隙(mm)：
                            </td>
                            <td>
                                <asp:TextBox runat="server" ID="txtcLdjx" CssClass="srk1"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td class="w160 tr">
                                整备质量(Kg)：
                            </td>
                            <td>
                                <asp:TextBox runat="server" ID="txtcZbzl" CssClass="srk1"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td class="w160 tr">
                                车 型：
                            </td>
                            <td>
                                <asp:TextBox runat="server" ID="txtcChesjg" CssClass="srk1"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td class="w160 tr">
                                车门数(个)：
                            </td>
                            <td>
                                <asp:TextBox runat="server" ID="txtcCms" CssClass="srk1"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td class="w160 tr">
                                座位数(个)：
                            </td>
                            <td>
                                <asp:TextBox runat="server" ID="txtcZws" CssClass="srk1"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td class="w160 tr">
                                油箱容积(L)：
                            </td>
                            <td>
                                <asp:TextBox runat="server" ID="txtcYxrj" CssClass="srk1"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td class="w160 tr">
                                行李厢容积(L)：
                            </td>
                            <td>
                                <asp:TextBox runat="server" ID="txtcXlxrj" CssClass="srk1"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td colspan="2" style="background-color: #ccc; color: Black; font-weight: bold;">
                                发动机：
                            </td>
                        </tr>
                        <tr>
                            <td class="w160 tr">
                                发动机型号：
                            </td>
                            <td>
                                <asp:TextBox runat="server" ID="txtcFdjxh" CssClass="srk1"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td class="w160 tr">
                                排量(mL)：
                            </td>
                            <td>
                                <asp:TextBox runat="server" ID="txtfPail" CssClass="srk1"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td class="w160 tr">
                                进气形式：
                            </td>
                            <td>
                                <asp:TextBox runat="server" ID="txtcJqxs" CssClass="srk1"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td class="w160 tr">
                                气缸排列形式：
                            </td>
                            <td>
                                <asp:TextBox runat="server" ID="txtcQgpl" CssClass="srk1"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td class="w160 tr">
                                气缸数(个)：
                            </td>
                            <td>
                                <asp:TextBox runat="server" ID="txtfQgs" CssClass="srk1"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td class="w160 tr">
                                每缸气门数(个)：
                            </td>
                            <td>
                                <asp:TextBox runat="server" ID="txtcQms" CssClass="srk1"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td class="w160 tr">
                                压缩比：
                            </td>
                            <td>
                                <asp:TextBox runat="server" ID="txtcYsb" CssClass="srk1"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td class="w160 tr">
                                配气机构：
                            </td>
                            <td>
                                <asp:TextBox runat="server" ID="txtcPqjg" CssClass="srk1"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td class="w160 tr">
                                缸径：
                            </td>
                            <td>
                                <asp:TextBox runat="server" ID="txtcGangj" CssClass="srk1"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td class="w160 tr">
                                冲程：
                            </td>
                            <td>
                                <asp:TextBox runat="server" ID="txtcChongc" CssClass="srk1"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td class="w160 tr">
                                最大马力(Ps)：
                            </td>
                            <td>
                                <asp:TextBox runat="server" ID="txtcZdml" CssClass="srk1"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td class="w160 tr">
                                最大功率(kW)：
                            </td>
                            <td>
                                <asp:TextBox runat="server" ID="txtcZdgl" CssClass="srk1"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td class="w160 tr">
                                最大功率转速(rpm)：
                            </td>
                            <td>
                                <asp:TextBox runat="server" ID="txtcZhuans" CssClass="srk1"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td class="w160 tr">
                                最大扭矩(N·m)：
                            </td>
                            <td>
                                <asp:TextBox runat="server" ID="txtcZdlz" CssClass="srk1"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td class="w160 tr">
                                最大扭矩转速(rpm)：
                            </td>
                            <td>
                                <asp:TextBox runat="server" ID="txtcLzzs" CssClass="srk1"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td class="w160 tr">
                                发动机特有技术：
                            </td>
                            <td>
                                <asp:TextBox runat="server" ID="txtcTyjs" CssClass="srk1"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td class="w160 tr">
                                燃料形式：
                            </td>
                            <td>
                                <asp:TextBox runat="server" ID="txtcRlxs" CssClass="srk1"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td class="w160 tr">
                                燃油标号：
                            </td>
                            <td>
                                <asp:TextBox runat="server" ID="txtcRybh" CssClass="srk1"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td class="w160 tr">
                                供油方式：
                            </td>
                            <td>
                                <asp:TextBox runat="server" ID="txtcGyfs" CssClass="srk1"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td class="w160 tr">
                                缸盖材料：
                            </td>
                            <td>
                                <asp:TextBox runat="server" ID="txtcGgcl" CssClass="srk1"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td class="w160 tr">
                                缸体材料：
                            </td>
                            <td>
                                <asp:TextBox runat="server" ID="txtcGtcl" CssClass="srk1"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td class="w160 tr">
                                环保标准：
                            </td>
                            <td>
                                <asp:TextBox runat="server" ID="txtcHbbz" CssClass="srk1"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td colspan="2" style="background-color: #ccc; color: Black; font-weight: bold;">
                                变速箱：
                            </td>
                        </tr>
                        <tr>
                            <td class="w160 tr">
                                简称：
                            </td>
                            <td>
                                <asp:TextBox runat="server" ID="txtcJianc" CssClass="srk1"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td class="w160 tr">
                                挡位个数：
                            </td>
                            <td>
                                <asp:TextBox runat="server" ID="txtcDwgs" CssClass="srk1"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td class="w160 tr">
                                变速箱类型：
                            </td>
                            <td>
                                <asp:TextBox runat="server" ID="txtcBsxlx" CssClass="srk1"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td colspan="2" style="background-color: #ccc; color: Black; font-weight: bold;">
                                底盘转向：
                            </td>
                        </tr>
                        <tr>
                            <td class="w160 tr">
                                驱动方式：
                            </td>
                            <td>
                                <asp:TextBox runat="server" ID="txtcQdfs" CssClass="srk1"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td class="w160 tr">
                                前悬挂类型：
                            </td>
                            <td>
                                <asp:TextBox runat="server" ID="txtcQxglx" CssClass="srk1"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td class="w160 tr">
                                后悬挂类型：
                            </td>
                            <td>
                                <asp:TextBox runat="server" ID="txtcHxglx" CssClass="srk1"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td class="w160 tr">
                                助力类型：
                            </td>
                            <td>
                                <asp:TextBox runat="server" ID="txtcZllx" CssClass="srk1"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td class="w160 tr">
                                车体结构：
                            </td>
                            <td>
                                <asp:TextBox runat="server" ID="txtcCtjg" CssClass="srk1"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td colspan="2" style="background-color: #ccc; color: Black; font-weight: bold;">
                                车轮制动：
                            </td>
                        </tr>
                        <tr>
                            <td class="w160 tr">
                                前制动器类型：
                            </td>
                            <td>
                                <asp:TextBox runat="server" ID="txtcQzdq" CssClass="srk1"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td class="w160 tr">
                                后制动器类型：
                            </td>
                            <td>
                                <asp:TextBox runat="server" ID="txtcHzdq" CssClass="srk1"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td class="w160 tr">
                                驻车制动类型：
                            </td>
                            <td>
                                <asp:TextBox runat="server" ID="txtcZczd" CssClass="srk1"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td class="w160 tr">
                                前轮胎规格：
                            </td>
                            <td>
                                <asp:TextBox runat="server" ID="txtcQnt" CssClass="srk1"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td class="w160 tr">
                                后轮胎规格：
                            </td>
                            <td>
                                <asp:TextBox runat="server" ID="txtcHnt" CssClass="srk1"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td class="w160 tr">
                                备胎规格：
                            </td>
                            <td>
                                <asp:TextBox runat="server" ID="txtcBetai" CssClass="srk1"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td colspan="2" style="background-color: #ccc; color: Black; font-weight: bold;">
                                安全装备：
                            </td>
                        </tr>
                        <tr>
                            <td class="w160 tr">
                                驾驶座安全气囊：
                            </td>
                            <td>
                                <asp:TextBox runat="server" ID="txtcJszqls" CssClass="srk1"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td class="w160 tr">
                                副驾驶安全气囊：
                            </td>
                            <td>
                                <asp:TextBox runat="server" ID="txtcFjsqls" CssClass="srk1"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td class="w160 tr">
                                前排侧气囊：
                            </td>
                            <td>
                                <asp:TextBox runat="server" ID="txtcQpcql" CssClass="srk1"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td class="w160 tr">
                                后排侧气囊：
                            </td>
                            <td>
                                <asp:TextBox runat="server" ID="txtcHpcql" CssClass="srk1"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td class="w160 tr">
                                前排头部气囊(气帘)：
                            </td>
                            <td>
                                <asp:TextBox runat="server" ID="txtcQptb" CssClass="srk1"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td class="w160 tr">
                                后排头部气囊(气帘)：
                            </td>
                            <td>
                                <asp:TextBox runat="server" ID="txtcHptb" CssClass="srk1"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td class="w160 tr">
                                膝部气囊：
                            </td>
                            <td>
                                <asp:TextBox runat="server" ID="txtcQbql" CssClass="srk1"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td class="w160 tr">
                                胎压监测装置：
                            </td>
                            <td>
                                <asp:TextBox runat="server" ID="txtcTyjc" CssClass="srk1"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td class="w160 tr">
                                零胎压继续行驶：
                            </td>
                            <td>
                                <asp:TextBox runat="server" ID="txtcLty" CssClass="srk1"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td class="w160 tr">
                                安全带未系提示：
                            </td>
                            <td>
                                <asp:TextBox runat="server" ID="txtcHqdts" CssClass="srk1"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td class="w160 tr">
                                ISO FIX儿童座椅接口：
                            </td>
                            <td>
                                <asp:TextBox runat="server" ID="txtcIso" CssClass="srk1"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td class="w160 tr">
                                LATCH儿童座椅接口：
                            </td>
                            <td>
                                <asp:TextBox runat="server" ID="txtcLatch" CssClass="srk1"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td class="w160 tr">
                                发动机电子防盗：
                            </td>
                            <td>
                                <asp:TextBox runat="server" ID="txtcFdjfd" CssClass="srk1"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td class="w160 tr">
                                车内中控锁：
                            </td>
                            <td>
                                <asp:TextBox runat="server" ID="txtcCnzks" CssClass="srk1"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td class="w160 tr">
                                遥控钥匙：
                            </td>
                            <td>
                                <asp:TextBox runat="server" ID="txtcYkys" CssClass="srk1"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td class="w160 tr">
                                无钥匙启动系统：
                            </td>
                            <td>
                                <asp:TextBox runat="server" ID="txtcWysqd" CssClass="srk1"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td colspan="2" style="background-color: #ccc; color: Black; font-weight: bold;">
                                操控配置：
                            </td>
                        </tr>
                        <tr>
                            <td class="w160 tr">
                                ABS防抱死：
                            </td>
                            <td>
                                <asp:TextBox runat="server" ID="txtcAbs" CssClass="srk1"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td class="w160 tr">
                                制动力分配(EBD/CBC等)：
                            </td>
                            <td>
                                <asp:TextBox runat="server" ID="txtcCbc" CssClass="srk1"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td class="w160 tr">
                                刹车辅助(EBA/BAS/BA等)：
                            </td>
                            <td>
                                <asp:TextBox runat="server" ID="txtcBa" CssClass="srk1"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td class="w160 tr">
                                牵引力控制(ASR/TCS/TRC等)：
                            </td>
                            <td>
                                <asp:TextBox runat="server" ID="txtcTrc" CssClass="srk1"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td class="w160 tr">
                                车身稳定控制(ESP/DSC/VSC等)：
                            </td>
                            <td>
                                <asp:TextBox runat="server" ID="txtcVsc" CssClass="srk1"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td class="w160 tr">
                                自动驻车/上坡辅助：
                            </td>
                            <td>
                                <asp:TextBox runat="server" ID="txtcZdzc" CssClass="srk1"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td class="w160 tr">
                                陡坡缓降：
                            </td>
                            <td>
                                <asp:TextBox runat="server" ID="txtcDphj" CssClass="srk1"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td class="w160 tr">
                                可变悬挂：
                            </td>
                            <td>
                                <asp:TextBox runat="server" ID="txtcKbxg" CssClass="srk1"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td class="w160 tr">
                                空气悬挂：
                            </td>
                            <td>
                                <asp:TextBox runat="server" ID="txtcKqxg" CssClass="srk1"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td class="w160 tr">
                                可变转向比：
                            </td>
                            <td>
                                <asp:TextBox runat="server" ID="txtcKbzxb" CssClass="srk1"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td colspan="2" style="background-color: #ccc; color: Black; font-weight: bold;">
                                外部配置：
                            </td>
                        </tr>
                        <tr>
                            <td class="w160 tr">
                                电动天窗：
                            </td>
                            <td>
                                <asp:TextBox runat="server" ID="txtcZdtc" CssClass="srk1"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td class="w160 tr">
                                全景天窗：
                            </td>
                            <td>
                                <asp:TextBox runat="server" ID="txtcQjtc" CssClass="srk1"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td class="w160 tr">
                                运动外观套件：
                            </td>
                            <td>
                                <asp:TextBox runat="server" ID="txtcYdwg" CssClass="srk1"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td class="w160 tr">
                                铝合金轮毂：
                            </td>
                            <td>
                                <asp:TextBox runat="server" ID="txtcLhjn" CssClass="srk1"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td class="w160 tr">
                                电动吸合门：
                            </td>
                            <td>
                                <asp:TextBox runat="server" ID="txtcDdxhm" CssClass="srk1"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td colspan="2" style="background-color: #ccc; color: Black; font-weight: bold;">
                                内部配置：
                            </td>
                        </tr>
                        <tr>
                            <td class="w160 tr">
                                真皮方向盘：
                            </td>
                            <td>
                                <asp:TextBox runat="server" ID="txtcZpfxp" CssClass="srk1"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td class="w160 tr">
                                方向盘上下调节：
                            </td>
                            <td>
                                <asp:TextBox runat="server" ID="txtcSxtj" CssClass="srk1"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td class="w160 tr">
                                方向盘前后调节：
                            </td>
                            <td>
                                <asp:TextBox runat="server" ID="txtcQhtj" CssClass="srk1"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td class="w160 tr">
                                方向盘电动调节：
                            </td>
                            <td>
                                <asp:TextBox runat="server" ID="txtcDdtj" CssClass="srk1"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td class="w160 tr">
                                多功能方向盘：
                            </td>
                            <td>
                                <asp:TextBox runat="server" ID="txtcDgnfxp" CssClass="srk1"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td class="w160 tr">
                                方向盘换挡：
                            </td>
                            <td>
                                <asp:TextBox runat="server" ID="txtcFxphd" CssClass="srk1"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td class="w160 tr">
                                定速巡航：
                            </td>
                            <td>
                                <asp:TextBox runat="server" ID="txtcDsxh" CssClass="srk1"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td class="w160 tr">
                                泊车辅助：
                            </td>
                            <td>
                                <asp:TextBox runat="server" ID="txtcBcfz" CssClass="srk1"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td class="w160 tr">
                                倒车视频影像：
                            </td>
                            <td>
                                <asp:TextBox runat="server" ID="txtcDcsp" CssClass="srk1"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td class="w160 tr">
                                行车电脑显示屏：
                            </td>
                            <td>
                                <asp:TextBox runat="server" ID="txtcDnxsp" CssClass="srk1"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td class="w160 tr">
                                HUD抬头数字显示：
                            </td>
                            <td>
                                <asp:TextBox runat="server" ID="txtcHud" CssClass="srk1"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td colspan="2" style="background-color: #ccc; color: Black; font-weight: bold;">
                                座椅配置：
                            </td>
                        </tr>
                        <tr>
                            <td class="w160 tr">
                                真皮/仿皮座椅：
                            </td>
                            <td>
                                <asp:TextBox runat="server" ID="txtcZpzy" CssClass="srk1"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td class="w160 tr">
                                运动座椅：
                            </td>
                            <td>
                                <asp:TextBox runat="server" ID="txtcYdzy" CssClass="srk1"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td class="w160 tr">
                                座椅高低调节：
                            </td>
                            <td>
                                <asp:TextBox runat="server" ID="txtcZygd" CssClass="srk1"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td class="w160 tr">
                                腰部支撑调节：
                            </td>
                            <td>
                                <asp:TextBox runat="server" ID="txtcYbzc" CssClass="srk1"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td class="w160 tr">
                                肩部支撑调节：
                            </td>
                            <td>
                                <asp:TextBox runat="server" ID="txtcJbzc" CssClass="srk1"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td class="w160 tr">
                                前排座椅电动调节：
                            </td>
                            <td>
                                <asp:TextBox runat="server" ID="txtcQpddtj" CssClass="srk1"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td class="w160 tr">
                                第二排靠背角度调节：
                            </td>
                            <td>
                                <asp:TextBox runat="server" ID="txtcEpjdtj" CssClass="srk1"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td class="w160 tr">
                                第二排座椅移动：
                            </td>
                            <td>
                                <asp:TextBox runat="server" ID="txtcEpzyyd" CssClass="srk1"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td class="w160 tr">
                                后排座椅电动调节：
                            </td>
                            <td>
                                <asp:TextBox runat="server" ID="txtcHptj" CssClass="srk1"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td class="w160 tr">
                                电动座椅记忆：
                            </td>
                            <td>
                                <asp:TextBox runat="server" ID="txtcDdjy" CssClass="srk1"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td class="w160 tr">
                                前排座椅加热：
                            </td>
                            <td>
                                <asp:TextBox runat="server" ID="txtcQpzyjr" CssClass="srk1"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td class="w160 tr">
                                后排座椅加热：
                            </td>
                            <td>
                                <asp:TextBox runat="server" ID="txtcHpzyjr" CssClass="srk1"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td class="w160 tr">
                                座椅通风：
                            </td>
                            <td>
                                <asp:TextBox runat="server" ID="txtcZytf" CssClass="srk1"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td class="w160 tr">
                                座椅按摩：
                            </td>
                            <td>
                                <asp:TextBox runat="server" ID="txtcZyam" CssClass="srk1"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td class="w160 tr">
                                后排座椅整体放倒：
                            </td>
                            <td>
                                <asp:TextBox runat="server" ID="txtcHpztpf" CssClass="srk1"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td class="w160 tr">
                                后排座椅比例放倒：
                            </td>
                            <td>
                                <asp:TextBox runat="server" ID="txtcHpblpf" CssClass="srk1"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td class="w160 tr">
                                第三排座椅：
                            </td>
                            <td>
                                <asp:TextBox runat="server" ID="txtcSpzy" CssClass="srk1"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td class="w160 tr">
                                前座中央扶手：
                            </td>
                            <td>
                                <asp:TextBox runat="server" ID="txtcQzfs" CssClass="srk1"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td class="w160 tr">
                                后座中央扶手：
                            </td>
                            <td>
                                <asp:TextBox runat="server" ID="txtcHzfs" CssClass="srk1"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td class="w160 tr">
                                后排杯架：
                            </td>
                            <td>
                                <asp:TextBox runat="server" ID="txtcHphj" CssClass="srk1"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td class="w160 tr">
                                电动后备厢：
                            </td>
                            <td>
                                <asp:TextBox runat="server" ID="txtcDdhbx" CssClass="srk1"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td colspan="2" style="background-color: #ccc; color: Black; font-weight: bold;">
                                多媒体配置：
                            </td>
                        </tr>
                        <tr>
                            <td class="w160 tr">
                                GPS导航系统：
                            </td>
                            <td>
                                <asp:TextBox runat="server" ID="txtcGps" CssClass="srk1"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td class="w160 tr">
                                定位互动服务：
                            </td>
                            <td>
                                <asp:TextBox runat="server" ID="txtcDwfw" CssClass="srk1"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td class="w160 tr">
                                中控台彩色大屏：
                            </td>
                            <td>
                                <asp:TextBox runat="server" ID="txtcCsdp" CssClass="srk1"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td class="w160 tr">
                                人机交互系统：
                            </td>
                            <td>
                                <asp:TextBox runat="server" ID="txtcRjjh" CssClass="srk1"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td class="w160 tr">
                                内置硬盘：
                            </td>
                            <td>
                                <asp:TextBox runat="server" ID="txtcNzyp" CssClass="srk1"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td class="w160 tr">
                                蓝牙/车载电话：
                            </td>
                            <td>
                                <asp:TextBox runat="server" ID="txtcCzdh" CssClass="srk1"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td class="w160 tr">
                                车载电视：
                            </td>
                            <td>
                                <asp:TextBox runat="server" ID="txtcCzds" CssClass="srk1"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td class="w160 tr">
                                后排液晶屏：
                            </td>
                            <td>
                                <asp:TextBox runat="server" ID="txtcHpyjp" CssClass="srk1"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td class="w160 tr">
                                外接音源接口(AUX/USB/iPod等)：
                            </td>
                            <td>
                                <asp:TextBox runat="server" ID="txtcIpod" CssClass="srk1"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td class="w160 tr">
                                CD支持MP3/WMA：
                            </td>
                            <td>
                                <asp:TextBox runat="server" ID="txtcMp3" CssClass="srk1"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td class="w160 tr">
                                单碟CD：
                            </td>
                            <td>
                                <asp:TextBox runat="server" ID="txtcDdcd" CssClass="srk1"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td class="w160 tr">
                                虚拟多碟CD：
                            </td>
                            <td>
                                <asp:TextBox runat="server" ID="txtcXndd" CssClass="srk1"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td class="w160 tr">
                                多碟CD系统：
                            </td>
                            <td>
                                <asp:TextBox runat="server" ID="txtcDuodcd" CssClass="srk1"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td class="w160 tr">
                                单碟DVD：
                            </td>
                            <td>
                                <asp:TextBox runat="server" ID="txtcDddvd" CssClass="srk1"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td class="w160 tr">
                                多碟DVD系统：
                            </td>
                            <td>
                                <asp:TextBox runat="server" ID="txtcDuodvd" CssClass="srk1"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td class="w160 tr">
                                2-3喇叭扬声器系统：
                            </td>
                            <td>
                                <asp:TextBox runat="server" ID="txtc23lb" CssClass="srk1"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td class="w160 tr">
                                4-5喇叭扬声器系统：
                            </td>
                            <td>
                                <asp:TextBox runat="server" ID="txtc45lb" CssClass="srk1"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td class="w160 tr">
                                6-7喇叭扬声器系统：
                            </td>
                            <td>
                                <asp:TextBox runat="server" ID="txtc67lb" CssClass="srk1"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td class="w160 tr">
                                ≥8喇叭扬声器系统：
                            </td>
                            <td>
                                <asp:TextBox runat="server" ID="txtc8lb" CssClass="srk1"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td colspan="2" style="background-color: #ccc; color: Black; font-weight: bold;">
                                灯光配置：
                            </td>
                        </tr>
                        <tr>
                            <td class="w160 tr">
                                氙气大灯：
                            </td>
                            <td>
                                <asp:TextBox runat="server" ID="txtcXqdd" CssClass="srk1"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td class="w160 tr">
                                LED大灯：
                            </td>
                            <td>
                                <asp:TextBox runat="server" ID="txtcLed" CssClass="srk1"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td class="w160 tr">
                                日间行车灯：
                            </td>
                            <td>
                                <asp:TextBox runat="server" ID="txtcRjxcd" CssClass="srk1"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td class="w160 tr">
                                自动头灯：
                            </td>
                            <td>
                                <asp:TextBox runat="server" ID="txtcZdtd" CssClass="srk1"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td class="w160 tr">
                                转向头灯(辅助灯)：
                            </td>
                            <td>
                                <asp:TextBox runat="server" ID="txtcZxtd" CssClass="srk1"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td class="w160 tr">
                                前雾灯：
                            </td>
                            <td>
                                <asp:TextBox runat="server" ID="txtcQwd" CssClass="srk1"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td class="w160 tr">
                                大灯高度可调：
                            </td>
                            <td>
                                <asp:TextBox runat="server" ID="txtcGdkt" CssClass="srk1"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td class="w160 tr">
                                大灯清洗装置：
                            </td>
                            <td>
                                <asp:TextBox runat="server" ID="txtcQxzz" CssClass="srk1"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td class="w160 tr">
                                车内氛围灯：
                            </td>
                            <td>
                                <asp:TextBox runat="server" ID="txtcCnfwd" CssClass="srk1"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td colspan="2" style="background-color: #ccc; color: Black; font-weight: bold;">
                                玻璃/后视镜：
                            </td>
                        </tr>
                        <tr>
                            <td class="w160 tr">
                                前电动车窗：
                            </td>
                            <td>
                                <asp:TextBox runat="server" ID="txtcQddcc" CssClass="srk1"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td class="w160 tr">
                                后电动车窗：
                            </td>
                            <td>
                                <asp:TextBox runat="server" ID="txtcHddcc" CssClass="srk1"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td class="w160 tr">
                                车窗防夹手功能：
                            </td>
                            <td>
                                <asp:TextBox runat="server" ID="txtcFjs" CssClass="srk1"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td class="w160 tr">
                                防紫外线/隔热玻璃：
                            </td>
                            <td>
                                <asp:TextBox runat="server" ID="txtcFzwx" CssClass="srk1"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td class="w160 tr">
                                后视镜电动调节：
                            </td>
                            <td>
                                <asp:TextBox runat="server" ID="txtcHsjtj" CssClass="srk1"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td class="w160 tr">
                                后视镜加热：
                            </td>
                            <td>
                                <asp:TextBox runat="server" ID="txtcHsjjr" CssClass="srk1"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td class="w160 tr">
                                后视镜自动防眩目：
                            </td>
                            <td>
                                <asp:TextBox runat="server" ID="txtcFxm" CssClass="srk1"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td class="w160 tr">
                                后视镜电动折叠：
                            </td>
                            <td>
                                <asp:TextBox runat="server" ID="txtcZdzd" CssClass="srk1"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td class="w160 tr">
                                后视镜记忆：
                            </td>
                            <td>
                                <asp:TextBox runat="server" ID="txtcHsjjy" CssClass="srk1"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td class="w160 tr">
                                后排侧遮阳帘：
                            </td>
                            <td>
                                <asp:TextBox runat="server" ID="txtcHpcz" CssClass="srk1"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td class="w160 tr">
                                遮阳板化妆镜：
                            </td>
                            <td>
                                <asp:TextBox runat="server" ID="txtcHzj" CssClass="srk1"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td class="w160 tr">
                                后雨刷：
                            </td>
                            <td>
                                <asp:TextBox runat="server" ID="txtcHys" CssClass="srk1"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td class="w160 tr">
                                感应雨刷：
                            </td>
                            <td>
                                <asp:TextBox runat="server" ID="txtcGyys" CssClass="srk1"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td colspan="2" style="background-color: #ccc; color: Black; font-weight: bold;">
                                空调/冰箱：
                            </td>
                        </tr>
                        <tr>
                            <td class="w160 tr">
                                手动空调：
                            </td>
                            <td>
                                <asp:TextBox runat="server" ID="txtcSdkt" CssClass="srk1"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td class="w160 tr">
                                自动空调：
                            </td>
                            <td>
                                <asp:TextBox runat="server" ID="txtcZdkt" CssClass="srk1"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td class="w160 tr">
                                后排独立空调：
                            </td>
                            <td>
                                <asp:TextBox runat="server" ID="txtcDlkt" CssClass="srk1"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td class="w160 tr">
                                后座出风口：
                            </td>
                            <td>
                                <asp:TextBox runat="server" ID="txtcHzcfk" CssClass="srk1"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td class="w160 tr">
                                温度分区控制：
                            </td>
                            <td>
                                <asp:TextBox runat="server" ID="txtcWdkz" CssClass="srk1"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td class="w160 tr">
                                空气调节/花粉过滤：
                            </td>
                            <td>
                                <asp:TextBox runat="server" ID="txtcKqtj" CssClass="srk1"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td class="w160 tr">
                                车载冰箱：
                            </td>
                            <td>
                                <asp:TextBox runat="server" ID="txtcCzbx" CssClass="srk1"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td colspan="2" style="background-color: #ccc; color: Black; font-weight: bold;">
                                高科技配置：
                            </td>
                        </tr>
                        <tr>
                            <td class="w160 tr">
                                自动泊车入位：
                            </td>
                            <td>
                                <asp:TextBox runat="server" ID="txtcPcrw" CssClass="srk1"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td class="w160 tr">
                                并线辅助：
                            </td>
                            <td>
                                <asp:TextBox runat="server" ID="txtcBxfz" CssClass="srk1"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td class="w160 tr">
                                主动刹车/主动安全系统：
                            </td>
                            <td>
                                <asp:TextBox runat="server" ID="txtcZdsc" CssClass="srk1"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td class="w160 tr">
                                整体主动转向系统：
                            </td>
                            <td>
                                <asp:TextBox runat="server" ID="txtcZtzx" CssClass="srk1"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td class="w160 tr">
                                夜视系统：
                            </td>
                            <td>
                                <asp:TextBox runat="server" ID="txtcYsxt" CssClass="srk1"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td class="w160 tr">
                                中控液晶屏分屏显示：
                            </td>
                            <td>
                                <asp:TextBox runat="server" ID="txtcFpxs" CssClass="srk1"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td class="w160 tr">
                                自适应巡航：
                            </td>
                            <td>
                                <asp:TextBox runat="server" ID="txtcZsyxh" CssClass="srk1"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td class="w160 tr">
                                全景摄像头：
                            </td>
                            <td>
                                <asp:TextBox runat="server" ID="txtcQjsxt" CssClass="srk1"></asp:TextBox>
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
