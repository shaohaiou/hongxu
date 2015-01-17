<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="addcar.aspx.cs" Inherits="Hx.JcbWeb.inventory.addcar" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>
        <%= GetInt("id") > 0 ? "编辑" : "添加" %>车源</title>
    <link href="../css/admin.css" rel="stylesheet" type="text/css" />
    <script src="../js/jquery-1.3.2.min.js" type="text/javascript"></script>
    <script src="../js/ajaxupload.js" type="text/javascript"></script>
    <script type="text/javascript" language="javascript">
        var timer_bindevent;
        $(function () {
            bindevent();

            $(".mainc").height($(window).height() - 36);

            $("#txtYjhgg").focus(function () {
                if ($(this).val() == "车况好，无事故，手续全，原车原图") {
                    $(this).removeClass("gray").val("");
                    $(this).unbind("focus");
                }
            });

            $("#rblXsz").click(function () {
                if ($("input[name='rblXsz']:checked").val() == "有")
                    $("#trXsz").show();
                else
                    $("#trXsz").hide();
            });
            $("#rblDjz").click(function () {
                if ($("input[name='rblDjz']:checked").val() == "有")
                    $("#trDjz").show();
                else
                    $("#trDjz").hide();
            });
            $("#rblGcfp").click(function () {
                if ($("input[name='rblGcfp']:checked").val() == "有")
                    $("#trGcfp").show();
                else
                    $("#trGcfp").hide();
            });
            $("#rblShzb").click(function () {
                if ($("input[name='rblShzb']:checked").val() == "商家延保")
                    $("#trShzb").show();
                else
                    $("#trShzb").hide();
            });

            $(".btnupd").each(function () {
                var button1 = $(this), interval1;
                new AjaxUpload(button1, {
                    action: '/cutimage.axd',
                    name: 'picfile',
                    responseType: 'json',
                    data: { action: 'jcbUpload' },
                    onSubmit: function (file, ext) {
                        if (!(ext && /^(jpg|png|jpeg|gif)$/i.test(ext))) {
                            alert('只能上传图片！');
                            return false;
                        }
                        button1.val('上传中');
                        this.disable();
                        interval1 = window.setInterval(function () {
                            var text = button1.val();
                            if (text.length < 13) {
                                button1.val(text + '.');
                            } else {
                                button1.val('上传中');
                            }
                        }, 200);
                    },
                    onComplete: function (file, response) {
                        button1.val('修改图片');
                        window.clearInterval(interval1);
                        this.enable();
                        $("img", button1.parent()).attr("src", response.src).attr("val", response.src);
                        $("input[type='hidden']", button1.parent()).val(response.src);
                    }
                });
            });
            $("#uploadbtclzp").each(function () {
                var imgpath_pics;
                var button1 = $(this), interval1;
                new AjaxUpload(button1, {
                    action: '/cutimage.axd',
                    name: 'picfile',
                    responseType: 'json',
                    data: { action: 'jcbUpload' },
                    onSubmit: function (file, ext) {
                        if (!(ext && /^(jpg|png|jpeg|gif)$/i.test(ext))) {
                            alert('只能上传图片！');
                            return false;
                        }
                        button1.val('上传中');
                        this.disable();
                        interval1 = window.setInterval(function () {
                            var text = button1.val();
                            if (text.length < 13) {
                                button1.val(text + '.');
                            } else {
                                button1.val('上传中');
                            }
                        }, 200);
                    },
                    onComplete: function (file, response) {
                        button1.val('修改照片');
                        window.clearInterval(interval1);
                        this.enable();
                        AddPic(response.src);
                    }
                });
            });
            $(".picDel").click(function () {
                DelPic(this);
            });
            $(".rbsifirstpic").click(function () {
                $(".rbsifirstpic").attr("checked", false);
                $(this).attr("checked", true);
                $(this).parent().parent().find("#hdnIsFirstpic" + $(this).attr("id").replace("rbIsFirstpic", "")).val("1");
            });
            $(".ddlpictype").change(function () {
                var curval = $(this).val();
                $(".ddlpictype").not(this).each(function () {
                    if ($(this).val() == curval)
                        $(this)[0].selectedIndex = 0;
                });
            });
        })

        function AddPic(url) {
            if (parseInt($("#hdnPiccount").val()) == 0) {
                $("#ulPics").html("");
            }
            $("#hdnPiccount").val(parseInt($("#hdnPiccount").val()) + 1);
            $("#hdnPicAddCount").val(parseInt($("#hdnPicAddCount").val()) + 1);
            $("#ulPics").append("<li>"
            + "<a href=\"javascript:void{0};\" title=\"删除\" class=\"picDel\"></a>"
            + "<img src=\"" + url + "\" alt=\"\" />"
            + "<label><input type=\"radio\" ID=\"rbIsFirstpic" + $("#hdnPicAddCount").val() + "\" name=\"rbIsFirstpic" + $("#hdnPicAddCount").val() + "\" class=\"rbsifirstpic\" />设置首图</label>"
            + "<select ID=\"ddlPicType" + $("#hdnPicAddCount").val() + "\" name=\"ddlPicType" + $("#hdnPicAddCount").val() + "\" class=\"ddlpictype\">" + $("#ddlPicType").html() + "</select>"
            + "<input type=\"hidden\" id=\"hdnPicUrl" + $("#hdnPicAddCount").val() + "\" name=\"hdnPicUrl" + $("#hdnPicAddCount").val() + "\" value=\"" + url + "\" />"
            + "<input type=\"hidden\" id=\"hdnIsFirstpic" + $("#hdnPicAddCount").val() + "\" name=\"hdnIsFirstpic" + $("#hdnPicAddCount").val() + "\" value=\"0\" /></li>");
            $(".picDel").unbind("click").click(function () {
                DelPic(this);
            });
            $(".rbsifirstpic").unbind("click").click(function () {
                $(".rbsifirstpic").attr("checked", false);
                $(this).attr("checked", true);
                $(this).parent().parent().find("#hdnIsFirstpic" + $(this).attr("id").replace("rbIsFirstpic","")).val("1");
            });
            $(".ddlpictype").unbind("change").change(function () {
                var curval = $(this).val();
                $(".ddlpictype").not(this).each(function () {
                    if ($(this).val() == curval)
                        $(this)[0].selectedIndex = 0;
                });
            });
        }
        function DelPic(obj) {
            $(obj).parent().remove();
        }
        function bindevent() {
            $("#btnCardeploy").unbind("click").click(function () {
                $(".cardeploy").toggleClass("hide");
                if ($(this).html() == "车辆配置+") {
                    $(this).html("车辆配置-");
                } else {
                    $(this).html("车辆配置+");
                }
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
    <div class="header">
        <h1>
            <%= GetInt("id") > 0 ? "编辑" : "添加" %>车源</h1>
        <span class="fr white"><a href="#">我的反馈</a> | <a href="#">意见反馈</a><input type="button"
            id="btnClose" class="hclose" /></span>
    </div>
    <div class="mainc">
        <div class="main">
            <form id="form1" runat="server">
            <asp:ScriptManager runat="server" ID="sm" EnablePartialRendering="true">
            </asp:ScriptManager>
            <div class="gray">
                车源信息填写越全面在车源上传到各网站时信息完整度越高，排位也越靠前。
            </div>
            <div class="ves">
                <div class="vtitle">
                    基本信息</div>
                <asp:UpdatePanel runat="server" ID="uplbaseinfo" UpdateMode="Conditional">
                    <ContentTemplate>
                        <table style="width: 100%;">
                            <thead>
                                <tr>
                                    <td>
                                        &nbsp;
                                    </td>
                                    <td>
                                    </td>
                                </tr>
                            </thead>
                            <tbody>
                                <tr>
                                    <td class="tr w160">
                                        车架号(VIN)：
                                    </td>
                                    <td>
                                        <input type="text" value="" class="srk5" runat="server" id="txtVINCode" name="txtVINCode" />
                                        <input type="button" runat="server" id="btnVINCode" name="btnVINCode" value="验证"
                                            class="an4" />
                                        <span class="gray">输入VIN码，一下41项将自动填入，省时省力</span>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                    </td>
                                    <td>
                                        <span class="gray lh24">如验证的车型不准确请手动改正</span>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="tr">
                                        <span class="red">*</span>车型：
                                    </td>
                                    <td>
                                        <asp:DropDownList runat="server" ID="ddlcChangs" CssClass="fl" AutoPostBack="true"
                                            OnSelectedIndexChanged="ddlcChangs_SelectedIndexChanged">
                                        </asp:DropDownList>
                                        <asp:DropDownList runat="server" ID="ddlcCxmc" AutoPostBack="true" OnSelectedIndexChanged="ddlcCxmc_SelectedIndexChanged">
                                        </asp:DropDownList>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="tr">
                                        <span class="red">*</span>排量：
                                    </td>
                                    <td>
                                        <asp:TextBox runat="server" ID="txtPailiang" CssClass="srk4"></asp:TextBox>
                                        L
                                        <asp:CheckBox runat="server" ID="cbxIswlzy" Text="涡轮增压" />
                                    </td>
                                </tr>
                                <tr>
                                    <td class="tr">
                                        <span class="red">*</span>变速箱：
                                    </td>
                                    <td>
                                        <asp:RadioButtonList runat="server" ID="rblBsx" RepeatDirection="Horizontal" BorderColor="White">
                                            <asp:ListItem Text="手动" Value="手动" Selected="True"></asp:ListItem>
                                            <asp:ListItem Text="自动" Value="自动"></asp:ListItem>
                                        </asp:RadioButtonList>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="tr">
                                        <span class="red">*</span>排放标准：
                                    </td>
                                    <td>
                                        <asp:DropDownList runat="server" ID="ddlPfbz">
                                            <asp:ListItem Text="-请选择-" Value=""></asp:ListItem>
                                            <asp:ListItem Text="国Ⅰ" Value="国Ⅰ"></asp:ListItem>
                                            <asp:ListItem Text="国Ⅱ" Value="国Ⅱ"></asp:ListItem>
                                            <asp:ListItem Text="国Ⅲ" Value="国Ⅲ"></asp:ListItem>
                                            <asp:ListItem Text="国Ⅳ" Value="国Ⅳ"></asp:ListItem>
                                            <asp:ListItem Text="国Ⅴ" Value="国Ⅴ"></asp:ListItem>
                                            <asp:ListItem Text="欧Ⅰ" Value="欧Ⅰ"></asp:ListItem>
                                            <asp:ListItem Text="欧Ⅱ" Value="欧Ⅱ"></asp:ListItem>
                                            <asp:ListItem Text="欧Ⅲ" Value="欧Ⅲ"></asp:ListItem>
                                            <asp:ListItem Text="欧Ⅳ" Value="欧Ⅳ"></asp:ListItem>
                                            <asp:ListItem Text="欧Ⅴ" Value="欧Ⅴ"></asp:ListItem>
                                        </asp:DropDownList>
                                        <asp:CheckBox runat="server" ID="cbxIsobd" Text="OBD" />
                                    </td>
                                </tr>
                                <tr>
                                    <td class="tr">
                                        <span class="red">*</span>外观颜色：
                                    </td>
                                    <td>
                                        <asp:DropDownList runat="server" ID="ddlWgys">
                                        </asp:DropDownList>
                                        <asp:TextBox runat="server" ID="txtWgys" CssClass="srk5" Visible="false"></asp:TextBox>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="tr">
                                        <span class="red">*</span>内饰颜色：
                                    </td>
                                    <td>
                                        <asp:DropDownList runat="server" ID="ddlNsys">
                                            <asp:ListItem Text="-选择颜色-" Value=""></asp:ListItem>
                                            <asp:ListItem Text="深内饰" Value="深内饰"></asp:ListItem>
                                            <asp:ListItem Text="浅内饰" Value="浅内饰"></asp:ListItem>
                                            <asp:ListItem Text="双色内饰" Value="双色内饰"></asp:ListItem>
                                        </asp:DropDownList>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="tr bold">
                                        <a href="javascript:void(0);" id="btnCardeploy" runat="server">车辆配置+</a>
                                    </td>
                                    <td>
                                        <%if (IsCardeploy)
                                          {%>
                                          <script type="text/javascript">
                                              $(function () {
                                                  $(".cardeploy").removeClass("hide");
                                              });
                                          </script>
                                        <%} %>
                                    </td>
                                </tr>
                                <tr class="cardeploy hide" id="trCardploydd1" runat="server">
                                    <td class="tr">
                                        大灯：
                                    </td>
                                    <td>
                                    </td>
                                </tr>
                                <tr class="cardeploy hide" id="trCardploydd2" runat="server">
                                    <td class="tr">
                                    </td>
                                    <td>
                                        <asp:CheckBoxList runat="server" ID="cblCardeploydd" RepeatDirection="Horizontal"
                                            BorderColor="White">
                                            <asp:ListItem Text="氙气大灯" Value="氙气大灯"></asp:ListItem>
                                            <asp:ListItem Text="大灯随动调节" Value="大灯随动调节"></asp:ListItem>
                                        </asp:CheckBoxList>
                                    </td>
                                </tr>
                                <tr class="cardeploy hide" id="trCardeployzy1" runat="server">
                                    <td class="tr">
                                        座椅：
                                    </td>
                                    <td>
                                    </td>
                                </tr>
                                <tr class="cardeploy hide" id="trCardeployzy2" runat="server">
                                    <td class="tr">
                                    </td>
                                    <td>
                                        <asp:CheckBoxList runat="server" ID="cblCardeployzy" RepeatDirection="Horizontal"
                                            BorderColor="White">
                                            <asp:ListItem Text="真皮/仿皮座椅" Value="真皮/仿皮座椅"></asp:ListItem>
                                            <asp:ListItem Text="前排座椅加热" Value="前排座椅加热"></asp:ListItem>
                                            <asp:ListItem Text="电动座椅记忆" Value="电动座椅记忆"></asp:ListItem>
                                            <asp:ListItem Text="座椅通风" Value="座椅通风"></asp:ListItem>
                                        </asp:CheckBoxList>
                                    </td>
                                </tr>
                                <tr class="cardeploy hide" id="trCardeployyx1" runat="server">
                                    <td class="tr">
                                        音响：
                                    </td>
                                    <td>
                                    </td>
                                </tr>
                                <tr class="cardeploy hide" id="trCardeployyx2" runat="server">
                                    <td class="tr">
                                    </td>
                                    <td>
                                        <asp:CheckBoxList runat="server" ID="cblCardeployyx" RepeatDirection="Horizontal"
                                            BorderColor="White">
                                            <asp:ListItem Text="CD/DVD" Value="CD/DVD"></asp:ListItem>
                                            <asp:ListItem Text="外接音源接口(AUX/USB/iPod等)" Value="外接音源接口(AUX/USB/iPod等)"></asp:ListItem>
                                        </asp:CheckBoxList>
                                    </td>
                                </tr>
                                <tr class="cardeploy hide" id="trCardeployaqqn1" runat="server">
                                    <td class="tr">
                                        安全气囊：
                                    </td>
                                    <td>
                                    </td>
                                </tr>
                                <tr class="cardeploy hide" id="trCardeployaqqn2" runat="server">
                                    <td class="tr">
                                    </td>
                                    <td>
                                        <asp:CheckBoxList runat="server" ID="cblCardeployaqqn" RepeatDirection="Horizontal"
                                            BorderColor="White">
                                            <asp:ListItem Text="驾驶座安全气囊" Value="驾驶座安全气囊"></asp:ListItem>
                                            <asp:ListItem Text="副驾驶安全气囊" Value="副驾驶安全气囊"></asp:ListItem>
                                            <asp:ListItem Text="前排侧气囊" Value="前排侧气囊"></asp:ListItem>
                                            <asp:ListItem Text="后排侧气囊" Value="后排侧气囊"></asp:ListItem>
                                        </asp:CheckBoxList>
                                    </td>
                                </tr>
                                <tr class="cardeploy hide" id="trCardeployqt1" runat="server">
                                    <td class="tr">
                                        其他：
                                    </td>
                                    <td>
                                    </td>
                                </tr>
                                <tr class="cardeploy hide" id="trCardeployqt2" runat="server">
                                    <td class="tr">
                                    </td>
                                    <td>
                                        <asp:CheckBoxList runat="server" ID="cblCardeployqt" RepeatDirection="Horizontal"
                                            BorderColor="White" RepeatColumns="5">
                                            <asp:ListItem Text="空气悬挂" Value="空气悬挂"></asp:ListItem>
                                            <asp:ListItem Text="定速巡航" Value="定速巡航"></asp:ListItem>
                                            <asp:ListItem Text="车身稳定控制(ESP/DSC/VSC等)" Value="车身稳定控制(ESP/DSC/VSC等)"></asp:ListItem>
                                            <asp:ListItem Text="无钥匙启动系统" Value="无钥匙启动系统"></asp:ListItem>
                                            <asp:ListItem Text="倒车雷达" Value="倒车雷达"></asp:ListItem>
                                            <asp:ListItem Text="倒车影像" Value="倒车影像"></asp:ListItem>
                                            <asp:ListItem Text="自动停车入位" Value="自动停车入位"></asp:ListItem>
                                            <asp:ListItem Text="电动天窗" Value="电动天窗"></asp:ListItem>
                                            <asp:ListItem Text="电动后舱/行李箱盖" Value="电动后舱/行李箱盖"></asp:ListItem>
                                            <asp:ListItem Text="方向盘换挡" Value="方向盘换挡"></asp:ListItem>
                                            <asp:ListItem Text="蓝牙/车载电话" Value="蓝牙/车载电话"></asp:ListItem>
                                            <asp:ListItem Text="GPS导航系统" Value="GPS导航系统"></asp:ListItem>
                                            <asp:ListItem Text="车载信息服务" Value="车载信息服务"></asp:ListItem>
                                            <asp:ListItem Text="自动空调" Value="自动空调"></asp:ListItem>
                                            <asp:ListItem Text="后排出风口" Value="后排出风口"></asp:ListItem>
                                        </asp:CheckBoxList>
                                    </td>
                                </tr>
                            </tbody>
                            <tfoot>
                                <tr>
                                    <td>
                                        &nbsp;
                                    </td>
                                    <td>
                                    </td>
                                </tr>
                            </tfoot>
                        </table>
                    </ContentTemplate>
                </asp:UpdatePanel>
            </div>
            <div class="ves">
                <div class="vtitle">
                    使用信息</div>
                <table style="width: 100%;">
                    <thead>
                        <tr>
                            <td>
                                &nbsp;
                            </td>
                            <td>
                            </td>
                        </tr>
                    </thead>
                    <tbody>
                        <tr>
                            <td class="tr w160">
                                <span class="red">*</span>表显里程：
                            </td>
                            <td>
                                <asp:TextBox runat="server" ID="txtBxlc" CssClass="srk5"></asp:TextBox>万公里
                            </td>
                        </tr>
                        <tr>
                            <td class="tr">
                                <span class="red">*</span>是否一手车：
                            </td>
                            <td>
                                <asp:RadioButtonList runat="server" ID="rblSfysc" RepeatDirection="Horizontal" BorderColor="White">
                                    <asp:ListItem Text="一手车" Value="一手车" Selected="True"></asp:ListItem>
                                    <asp:ListItem Text="曾交易过" Value="曾交易过"></asp:ListItem>
                                </asp:RadioButtonList>
                            </td>
                        </tr>
                        <tr>
                            <td class="tr">
                                <span class="red">*</span>车辆所在地：
                            </td>
                            <td>
                                <asp:UpdatePanel runat="server" ID="uplarea" UpdateMode="Conditional">
                                    <ContentTemplate>
                                        <asp:DropDownList runat="server" ID="ddlCarPromary" CssClass="fl" AutoPostBack="true"
                                            OnSelectedIndexChanged="ddlCarPromary_SelectedIndexChanged">
                                        </asp:DropDownList>
                                        <asp:DropDownList runat="server" ID="ddlCarCity">
                                        </asp:DropDownList>
                                    </ContentTemplate>
                                </asp:UpdatePanel>
                            </td>
                        </tr>
                        <tr>
                            <td class="tr">
                                <span class="red">*</span>使用性质：
                            </td>
                            <td>
                                <asp:DropDownList runat="server" ID="ddlSyxz">
                                    <asp:ListItem Text="非营运" Value="非营运" Selected="True"></asp:ListItem>
                                    <asp:ListItem Text="营运" Value="营运"></asp:ListItem>
                                    <asp:ListItem Text="营转非" Value="营转非"></asp:ListItem>
                                    <asp:ListItem Text="租赁" Value="租赁"></asp:ListItem>
                                    <asp:ListItem Text="单位用车" Value="单位用车"></asp:ListItem>
                                </asp:DropDownList>
                            </td>
                        </tr>
                        <tr>
                            <td class="tr">
                                有无重大事故：
                            </td>
                            <td>
                                <asp:RadioButtonList runat="server" ID="rblYwzdsg" RepeatDirection="Horizontal" BorderColor="White">
                                    <asp:ListItem Text="有" Value="有"></asp:ListItem>
                                    <asp:ListItem Text="无" Value="无" Selected="True"></asp:ListItem>
                                </asp:RadioButtonList>
                            </td>
                        </tr>
                        <tr>
                            <td class="tr">
                                外观成色：
                            </td>
                            <td>
                                <asp:RadioButtonList runat="server" ID="rblWgcs" RepeatDirection="Horizontal" BorderColor="White">
                                    <asp:ListItem Text="极新" Value="极新"></asp:ListItem>
                                    <asp:ListItem Text="较新" Value="较新" Selected="True"></asp:ListItem>
                                    <asp:ListItem Text="一般" Value="一般"></asp:ListItem>
                                    <asp:ListItem Text="较差" Value="较差"></asp:ListItem>
                                </asp:RadioButtonList>
                            </td>
                        </tr>
                        <tr>
                            <td class="tr">
                                内饰状态：
                            </td>
                            <td>
                                <asp:RadioButtonList runat="server" ID="rblNszt" RepeatDirection="Horizontal" BorderColor="White">
                                    <asp:ListItem Text="完好" Value="完好" Selected="True"></asp:ListItem>
                                    <asp:ListItem Text="有缺陷" Value="有缺陷"></asp:ListItem>
                                </asp:RadioButtonList>
                            </td>
                        </tr>
                        <tr>
                            <td class="tr">
                                仪表台状态：
                            </td>
                            <td>
                                <asp:DropDownList runat="server" ID="ddlYbtzt">
                                    <asp:ListItem Text="-请选择-" Value=""></asp:ListItem>
                                    <asp:ListItem Text="有异响" Value="有异响"></asp:ListItem>
                                    <asp:ListItem Text="无异响" Value="无异响" Selected="True"></asp:ListItem>
                                </asp:DropDownList>
                            </td>
                        </tr>
                        <tr>
                            <td class="tr">
                                座椅使用情况：
                            </td>
                            <td>
                                <asp:DropDownList runat="server" ID="ddlZysyqk">
                                    <asp:ListItem Text="-请选择-" Value="" Selected="True"></asp:ListItem>
                                    <asp:ListItem Text="真皮/仿皮座椅有磨损" Value="真皮/仿皮座椅有磨损"></asp:ListItem>
                                    <asp:ListItem Text="真皮/仿皮座椅无磨损" Value="真皮/仿皮座椅无磨损"></asp:ListItem>
                                    <asp:ListItem Text="布绒座椅较清洁" Value="布绒座椅较清洁"></asp:ListItem>
                                    <asp:ListItem Text="布绒座椅有污渍" Value="布绒座椅有污渍"></asp:ListItem>
                                </asp:DropDownList>
                            </td>
                        </tr>
                        <tr>
                            <td class="tr">
                                定期保养：
                            </td>
                            <td>
                                <asp:RadioButtonList runat="server" ID="rblDqby" RepeatDirection="Horizontal" BorderColor="White">
                                    <asp:ListItem Text="4s店定期保养" Value="4s店定期保养" Selected="True"></asp:ListItem>
                                    <asp:ListItem Text="非4s店定期保养" Value="非4s店定期保养"></asp:ListItem>
                                    <asp:ListItem Text="无定期保养" Value="无定期保养"></asp:ListItem>
                                </asp:RadioButtonList>
                            </td>
                        </tr>
                        <tr>
                            <td class="tr">
                                维修保养记录：
                            </td>
                            <td>
                                <asp:RadioButtonList runat="server" ID="rblWxbyjl" RepeatDirection="Horizontal" BorderColor="White">
                                    <asp:ListItem Text="齐全" Value="齐全" Selected="True"></asp:ListItem>
                                    <asp:ListItem Text="不齐全" Value="不齐全"></asp:ListItem>
                                </asp:RadioButtonList>
                            </td>
                        </tr>
                        <tr>
                            <td class="tr">
                                一句话广告：
                            </td>
                            <td>
                                <asp:TextBox runat="server" ID="txtYjhgg" CssClass="srk1 gray">车况好，无事故，手续全，原车原图</asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td class="tr">
                                真实油耗：
                            </td>
                            <td>
                                <asp:TextBox runat="server" ID="txtZsyh" CssClass="srk5"></asp:TextBox>
                                升/百公里
                            </td>
                        </tr>
                        <tr>
                            <td class="tr">
                                保养费用：
                            </td>
                            <td>
                                <asp:TextBox runat="server" ID="txtByfy" CssClass="srk5"></asp:TextBox>
                                元/年
                            </td>
                        </tr>
                    </tbody>
                    <tfoot>
                        <tr>
                            <td>
                                &nbsp;
                            </td>
                            <td>
                            </td>
                        </tr>
                    </tfoot>
                </table>
            </div>
            <div class="ves">
                <div class="vtitle">
                    牌照信息</div>
                <table style="width: 100%;">
                    <thead>
                        <tr>
                            <td>
                                &nbsp;
                            </td>
                            <td>
                            </td>
                        </tr>
                    </thead>
                    <tbody>
                        <tr>
                            <td class="tr w160">
                                车牌号：
                            </td>
                            <td>
                                <asp:TextBox runat="server" ID="txtCph" CssClass="srk3"></asp:TextBox>
                                <span class="gray">为您找车方便，不向外公布</span>
                            </td>
                        </tr>
                        <tr>
                            <td class="tr">
                                <span class="red">*</span>证件是否齐全：
                            </td>
                            <td>
                                <asp:RadioButtonList runat="server" ID="rblZjsfqq" RepeatDirection="Horizontal" BorderColor="White">
                                    <asp:ListItem Text="是" Value="是" Selected="True"></asp:ListItem>
                                    <asp:ListItem Text="否" Value="否"></asp:ListItem>
                                </asp:RadioButtonList>
                            </td>
                        </tr>
                        <tr>
                            <td class="tr">
                                行驶证：
                            </td>
                            <td>
                                <asp:RadioButtonList runat="server" ID="rblXsz" RepeatDirection="Horizontal" BorderColor="White">
                                    <asp:ListItem Text="有" Value="有"></asp:ListItem>
                                    <asp:ListItem Text="丢失" Value="丢失"></asp:ListItem>
                                    <asp:ListItem Text="补办中" Value="补办中"></asp:ListItem>
                                </asp:RadioButtonList>
                            </td>
                        </tr>
                        <tr runat="server" id="trXsz" class="hide">
                            <td>
                            </td>
                            <td>
                                <input type="button" id="uploadbtxsz" value="上传照片" class="an3 btnupd fl" />
                                <img src="../images/fm.jpg" alt="图片" id="imgxsz" class="pl10" style="width: 72px;
                                    height: 72px; border: 0px; padding: 0px;" runat="server" />
                                <input type="hidden" runat="server" id="hdnPicxsz" />
                            </td>
                        </tr>
                        <tr>
                            <td class="tr">
                                登记证：
                            </td>
                            <td>
                                <asp:RadioButtonList runat="server" ID="rblDjz" RepeatDirection="Horizontal" BorderColor="White">
                                    <asp:ListItem Text="有" Value="有"></asp:ListItem>
                                    <asp:ListItem Text="丢失" Value="丢失"></asp:ListItem>
                                    <asp:ListItem Text="补办中" Value="补办中"></asp:ListItem>
                                </asp:RadioButtonList>
                            </td>
                        </tr>
                        <tr runat="server" id="trDjz" class="hide">
                            <td>
                            </td>
                            <td>
                                <input type="button" id="uploadbtdjz" value="上传照片" class="an3 btnupd fl" />
                                <img src="../images/fm.jpg" alt="图片" id="imgdjz" class="pl10" style="width: 72px;
                                    height: 72px; border: 0px; padding: 0px;" runat="server" />
                                <input type="hidden" runat="server" id="hdnPicdjz" />
                            </td>
                        </tr>
                        <tr>
                            <td class="tr">
                                购车发票：
                            </td>
                            <td>
                                <asp:RadioButtonList runat="server" ID="rblGcfp" RepeatDirection="Horizontal" BorderColor="White">
                                    <asp:ListItem Text="有" Value="有"></asp:ListItem>
                                    <asp:ListItem Text="丢失" Value="丢失"></asp:ListItem>
                                    <asp:ListItem Text="补办中" Value="补办中"></asp:ListItem>
                                </asp:RadioButtonList>
                            </td>
                        </tr>
                        <tr runat="server" id="trGcfp" class="hide">
                            <td>
                            </td>
                            <td>
                                <input type="button" id="uploadbtgcfp" value="上传照片" class="an3 btnupd fl" />
                                <img src="../images/fm.jpg" alt="图片" id="imggcfp" class="pl10" style="width: 72px;
                                    height: 72px; border: 0px; padding: 0px;" runat="server" />
                                <input type="hidden" runat="server" id="hdnPicgcfp" />
                            </td>
                        </tr>
                        <tr>
                            <td class="tr">
                                购置税：
                            </td>
                            <td>
                                <asp:RadioButtonList runat="server" ID="rblGzs" RepeatDirection="Horizontal" BorderColor="White">
                                    <asp:ListItem Text="有" Value="有" Selected="True"></asp:ListItem>
                                    <asp:ListItem Text="丢失" Value="丢失"></asp:ListItem>
                                </asp:RadioButtonList>
                            </td>
                        </tr>
                        <tr>
                            <td class="tr">
                                是否可以外迁：
                            </td>
                            <td>
                                <asp:RadioButtonList runat="server" ID="rblSfkywq" RepeatDirection="Horizontal" BorderColor="White">
                                    <asp:ListItem Text="是" Value="是" Selected="True"></asp:ListItem>
                                    <asp:ListItem Text="否" Value="否"></asp:ListItem>
                                </asp:RadioButtonList>
                            </td>
                        </tr>
                        <tr>
                            <td class="tr">
                                <span class="red">*</span>首次上牌日期：
                            </td>
                            <td>
                                <asp:DropDownList runat="server" ID="ddlYearScsprq">
                                </asp:DropDownList>
                                <asp:DropDownList runat="server" ID="ddlMonthScsprq">
                                </asp:DropDownList>
                                <asp:CheckBox runat="server" ID="cbxIsWsp" Text="未上牌" />
                            </td>
                        </tr>
                        <tr>
                            <td class="tr">
                                <span class="red">*</span>年检有效期至：
                            </td>
                            <td>
                                <asp:DropDownList runat="server" ID="ddlYearNjyxq">
                                </asp:DropDownList>
                                <asp:DropDownList runat="server" ID="ddlMonthNjyxq">
                                </asp:DropDownList>
                                <asp:CheckBox runat="server" ID="cbxIsNjygq" Text="已过期" />
                            </td>
                        </tr>
                        <tr>
                            <td class="tr">
                                <span class="red">*</span>交强险有效期至：
                            </td>
                            <td>
                                <asp:DropDownList runat="server" ID="ddlYearJqxyxq">
                                </asp:DropDownList>
                                <asp:DropDownList runat="server" ID="ddlMonthJqxyxq">
                                </asp:DropDownList>
                                <asp:CheckBox runat="server" ID="cbxIsJqxygq" Text="已过期" />
                            </td>
                        </tr>
                        <tr>
                            <td class="tr">
                                <span class="red">*</span>商业险有效期至：
                            </td>
                            <td>
                                <asp:DropDownList runat="server" ID="ddlYearSyxyxq">
                                </asp:DropDownList>
                                <asp:DropDownList runat="server" ID="ddlMonthSyxyxq">
                                </asp:DropDownList>
                                <asp:CheckBox runat="server" ID="cbxIsSyxygq" Text="已过期" />
                            </td>
                        </tr>
                    </tbody>
                    <tfoot>
                        <tr>
                            <td>
                                &nbsp;
                            </td>
                            <td>
                            </td>
                        </tr>
                    </tfoot>
                </table>
            </div>
            <div class="ves">
                <div class="vtitle">
                    价格信息</div>
                <table style="width: 100%;">
                    <thead>
                        <tr>
                            <td>
                                &nbsp;
                            </td>
                            <td>
                            </td>
                        </tr>
                    </thead>
                    <tbody>
                        <tr>
                            <td class="tr w160">
                                <span class="red">*</span>销售信息：
                            </td>
                            <td>
                                <span class="red">预售价：</span><asp:TextBox runat="server" ID="txtYsj" CssClass="srk5"></asp:TextBox>
                                <span class="red">万元</span>
                                <asp:CheckBox runat="server" ID="cbxIsYsjbhghf" Text="包含过户费" />
                                <asp:CheckBox runat="server" ID="cbxIsYsjykj" Text="一口价" />
                                <asp:CheckBox runat="server" ID="cbxIsYsjkaj" Text="可按揭" />
                            </td>
                        </tr>
                        <tr>
                            <td class="tr">
                                售后质保：
                            </td>
                            <td>
                                <asp:RadioButtonList runat="server" ID="rblShzb" RepeatDirection="Horizontal" BorderColor="White">
                                    <asp:ListItem Text="原厂质保期内" Value="原厂质保期内"></asp:ListItem>
                                    <asp:ListItem Text="商家延保" Value="商家延保"></asp:ListItem>
                                    <asp:ListItem Text="无" Value="无"></asp:ListItem>
                                </asp:RadioButtonList>
                            </td>
                        </tr>
                        <tr id="trShzb" runat="server" class="hide">
                            <td class="tr">
                                商家延保内容：
                            </td>
                            <td>
                                <asp:TextBox runat="server" ID="txtSjybnry" CssClass="srk4"></asp:TextBox>
                                月
                                <asp:TextBox runat="server" ID="txtSjybnrgl" CssClass="srk4"></asp:TextBox>
                                万公里
                            </td>
                        </tr>
                        <tr>
                            <td class="tr">
                                出兑信息：
                            </td>
                            <td>
                                <span class="red">出兑价格：</span><asp:TextBox runat="server" ID="txtCdjg" CssClass="srk5"></asp:TextBox>
                                <span class="red">万元</span>
                                <asp:CheckBox runat="server" ID="cbxIsCdjgkyj" Text="可议价" />
                                <span class="gray">商户间倒车，出兑价格仅注册商可见</span>
                            </td>
                        </tr>
                    </tbody>
                    <tfoot>
                        <tr>
                            <td>
                                &nbsp;
                            </td>
                            <td>
                            </td>
                        </tr>
                    </tfoot>
                </table>
            </div>
            <div class="ves">
                <div class="vtitle">
                    车主自述</div>
                <table style="width: 100%;">
                    <tbody>
                        <tr>
                            <td class="tc">
                                <asp:TextBox runat="server" ID="txtCzzs" Rows="7" Style="width: 98%" TextMode="MultiLine"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <a class="ml20" href="javascript:void(0);">车主自述参考模版：</a>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <ul style="width: 80%; border: 1px solid #aeaeae; margin: 0 auto; padding: 0 10px;
                                    margin-bottom: 10px;">
                                    <li style="padding: 10px 10px 15px 10px; border-bottom: 1px dashed #aeaeae;"><strong>
                                        福特蒙迪欧</strong><br />
                                        <br />
                                        <span class="gray lh20">出售2005年福特蒙迪欧2.0Ghia-Ltd尊贵型，2.0排量，个人一手车，仅行驶7万公里，从买新车至今都在4S店进行保养，本车实拍照片，有保养记录，车况完美，喜欢这款车的朋友抓紧，车况绝对给您惊喜。</span>
                                    </li>
                                    <li style="padding: 10px 10px 15px 10px; border-bottom: 1px dashed #aeaeae;"><strong>
                                        大众帕萨特</strong><br />
                                        <br />
                                        <span class="gray lh20">大众帕萨特2007款2.0L手动基本型 ，2007年6月初次上牌，坚持4s店做保养，有4s店保养明细记录，没有任何事故，全险即将到期。两把可折叠遥控钥匙，其中一把非常新几乎没有使用。近期刚刚4s店做完保养，可随时驾驶。先到先得如果碰到爱车的人价格可以小议。</span>
                                    </li>
                                    <li style="padding: 10px 10px 15px 10px;"><strong>奥迪A6L</strong><br />
                                        <br />
                                        <span class="gray lh20">奥迪A6L2005款2.0T自动标准型,05年11月购买，2.0T排量动力非常强劲，并且非常省油。外观和内饰都非常新，自家爱车无事故，全部由4s店保养。具体配置有：2.0T涡轮增压、手自一体带S档、氙气大灯、大灯清洗、电动真皮座椅
                                            、天窗、BOSS音响、六碟CD、倒车雷达等。当时车价46万，现在转让格上多少可以商量，急于出手请尽快与本人联系。</span> </li>
                                </ul>
                            </td>
                        </tr>
                    </tbody>
                </table>
            </div>
            <div class="ves">
                <div class="vtitle">
                    车辆照片</div>
                <table style="width: 100%;">
                    <tbody>
                        <tr>
                            <td class="tr w100">
                                <input type="button" id="uploadbtclzp" value="上传照片" class="an3" />
                            </td>
                            <td>
                                图片要求：1.最大可上传5M图片；2.支持jpg/png/bmp格式；3.不能带其他网站的logo或无关信息
                            </td>
                        </tr>
                        <tr>
                            <td class="tr vt">
                                上传图片示例处：
                            </td>
                            <td>
                                <ul id="ulPics">
                                    <% if (rptPics.Items.Count == 0)
                                       { %>
                                    <li>
                                        <img src="../images/zq45d.png" alt="左前45度" />
                                        <span>左前45度</span> </li>
                                    <li>
                                        <img src="../images/yh45d.png" alt="右后45度" />
                                        <span>右后45度</span></li>
                                    <li>
                                        <img src="../images/cemian.png" alt="侧面" />
                                        <span>侧面</span></li>
                                    <li>
                                        <img src="../images/neishi.png" alt="内饰" />
                                        <span>内饰</span></li>
                                    <li>
                                        <img src="../images/fdj.png" alt="发动机舱" />
                                        <span>发动机舱</span></li>
                                    <%}%>
                                    <asp:Repeater runat="server" ID="rptPics" OnItemDataBound="rptPics_ItemDataBound">
                                        <ItemTemplate>
                                            <li><a href="javascript:void(0);" title="删除" class="picDel"></a>
                                                <img src="<%# Eval("PicUrl") %>" alt="<%# Eval("JcbPicType").ToString() %>" />
                                                <label><input type="radio" runat="server" id="rbIsFirstpic" name="rbIsFirstpic" class="rbsifirstpic" />设置首图</label>
                                                <asp:DropDownList runat="server" ID="ddlPicType" class="ddlpictype">
                                                </asp:DropDownList>
                                                <input type="hidden" runat="server" id="hdnPicUrl" />
                                                <input type="hidden" runat="server" id="hdnIsFirstpic" value="0" />
                                            </li>
                                        </ItemTemplate>
                                    </asp:Repeater>
                                </ul>
                                <input type="hidden" id="hdnPiccount" value="<%= rptPics.Items.Count%>" />
                                <input type="hidden" id="hdnPicAddCount" value="0" runat="server" />
                            </td>
                        </tr>
                    </tbody>
                    <tfoot>
                        <tr>
                            <td>
                                &nbsp;
                            </td>
                            <td>
                            </td>
                        </tr>
                    </tfoot>
                </table>
            </div>
            <div class="tc psubmit">
                 <asp:Button runat="server"  ID="btnSubmit" Text="保存车辆信息" CssClass="an1" OnClick="btnSubmit_Click" />
            </div>
            <div class="hide">
                <asp:DropDownList runat="server" ID="ddlPicType">
                </asp:DropDownList>
            </div>
            </form>
        </div>
    </div>
</body>
</html>
