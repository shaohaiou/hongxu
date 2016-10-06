<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="dailyreport.aspx.cs" Inherits="Hx.BackAdmin.dayreport.dailyreport" %>

<%@ Import Namespace="Hx.Components.Enumerations" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>日报录入</title>
    <link href=<%=ResourceServer%>/css/admin.css rel="stylesheet" type="text/css" />
    <script src=<%=ResourceServer%>/js/jquery-1.3.2.min.js type="text/javascript"></script>
    <script src=<%=ResourceServer%>/js/comm.js type="text/javascript"></script>
    <script src="/js/WdatePicker.js" type="text/javascript"></script>
    <script type="text/javascript">
        var timer_bindevent;
        $(function () {
            bindevent();

            //            $(".cwycdkdq").each(function () {
            //                if ($.trim($(this).val()) == "") {
            //                    var num = $(this).attr("id").replace("txtCWycdkdq", "");
            //                    $(this).val(num + "号").addClass("gray").addClass("tc").removeClass("tr");
            //                }
            //            });

            $("#btnSubmit").click(function () {
                return CheckForm();
            });
        });

        function bindevent() {
            if ($("#hdnallowmodify").length > 0) {
                $("#btnSubmit").attr("disabled", $("#hdnallowmodify").val() == "1" ? "" : "disabled")
                .attr("class", $("#hdnallowmodify").val() == "1" ? "an1" : "an1dis");
            }
            $("#txtDate").unbind("click");
            $("#txtDate").click(function () {
                WdatePicker({ 'readOnly': 'true', dateFmt: 'yyyy-MM-dd', maxDate: '<%=DateTime.Today.ToString("yyyy-MM-dd") %>' });
            });

            if ($(".countyhzhye").length > 0 && $(".countyhzhyesub").length > 0) {
                $(".countyhzhyesub").unbind("change");
                $(".countyhzhyesub").change(function () {
                    var countyhzhye = 0;
                    $(".countyhzhyesub").each(function () {
                        countyhzhye += $.trim($(this).val()) == "" ? 0 : parseFloat($(this).val());
                    });
                    $(".countyhzhye").val(countyhzhye.toFixed(2));
                });
            }

            if ($(".countycyhzhye").length > 0 && $(".countycyhzhyesub").length > 0) {
                $(".countycyhzhyesub").unbind("change");
                $(".countycyhzhyesub").change(function () {
                    var countycyhzhye = 0;
                    $(".countycyhzhyesub").each(function () {
                        countycyhzhye += $.trim($(this).val()) == "" ? 0 : parseFloat($(this).val());
                    });
                    $(".countycyhzhye").val(countycyhzhye.toFixed(2));
                });
            }

            if ($(".countbryjzf").length > 0 && $(".countbryjzfsub").length > 0) {
                $(".countbryjzfsub").unbind("change");
                $(".countbryjzfsub").change(function () {
                    var countbryjzf = 0;
                    $(".countbryjzfsub").each(function () {
                        countbryjzf += $.trim($(this).val()) == "" ? 0 : parseFloat($(this).val());
                    });
                    $(".countbryjzf").val(countbryjzf.toFixed(2));
                });
            }

            if ($(".countshybfwgmgs").length > 0 && $(".countshybfwgmgssub").length > 0) {
                $(".countshybfwgmgssub").unbind("change");
                $(".countshybfwgmgssub").change(function () {
                    var countshybfwgmgs = 0;
                    $(".countshybfwgmgssub").each(function () {
                        countshybfwgmgs += $.trim($(this).val()) == "" ? 0 : parseInt($(this).val());
                    });
                    $(".countshybfwgmgs").val(countshybfwgmgs);
                });
            }

            if ($(".countshybfwgmje").length > 0 && $(".countshybfwgmjesub").length > 0) {
                $(".countshybfwgmjesub").unbind("change");
                $(".countshybfwgmjesub").change(function () {
                    var countshybfwgmje = 0;
                    $(".countshybfwgmjesub").each(function () {
                        countshybfwgmje += $.trim($(this).val()) == "" ? 0 : parseFloat($(this).val());
                    });
                    $(".countshybfwgmje").val(countshybfwgmje.toFixed(2));
                });
            }

            if ($(".countxsybwycfwgmgs").length > 0 && $(".countxsybwycfwgmgssub").length > 0) {
                $(".countxsybwycfwgmgssub").unbind("change");
                $(".countxsybwycfwgmgssub").change(function () {
                    var countxsybwycfwgmgs = 0;
                    $(".countxsybwycfwgmgssub").each(function () {
                        countxsybwycfwgmgs += $.trim($(this).val()) == "" ? 0 : parseInt($(this).val());
                    });
                    $(".countxsybwycfwgmgs").val(countxsybwycfwgmgs);
                });
            }

            if ($(".countxsybwycfwgmje").length > 0 && $(".countxsybwycfwgmjesub").length > 0) {
                $(".countxsybwycfwgmjesub").unbind("change");
                $(".countxsybwycfwgmjesub").change(function () {
                    var countxsybwycfwgmje = 0;
                    $(".countxsybwycfwgmjesub").each(function () {
                        countxsybwycfwgmje += $.trim($(this).val()) == "" ? 0 : parseFloat($(this).val());
                    });
                    $(".countxsybwycfwgmje").val(countxsybwycfwgmje.toFixed(2));
                });
            }

            if ($("#hdnCWycdkdq").length > 0) {
                $(".cwycdkdq").unbind("change");
                $(".cwycdkdq").unbind("focus");
                $(".cwycdkdq").unbind("blur");
                $(".cwycdkdq").change(function () {
                    $("#hdnCWycdkdq").val($(".cwycdkdq").map(function () {
                        var num = $(this).attr("id").replace("txtCWycdkdq", "");
                        if ($.trim($(this).val()) == (num + "号")) return "";
                        return $(this).val();
                    }).get().join(","));
                });
                $(".cwycdkdq").focus(function () {
                    var num = $(this).attr("id").replace("txtCWycdkdq", "");
                    if ($.trim($(this).val()) == "" || $.trim($(this).val()) == (num + "号")) {
                        $(this).val("").removeClass("gray").removeClass("tc").addClass("tr");
                    }
                });
                $(".cwycdkdq").blur(function () {
                    var num = $(this).attr("id").replace("txtCWycdkdq", "");
                    if ($.trim($(this).val()) == "") {
                        $(this).val(num + "号").addClass("gray").addClass("tc").removeClass("tr");
                    }
                });
            }

            if ($(".remind").length > 0) {
                $(".remind").focus(function () {
                    $(this).val("");
                    $(this).removeClass("remind").removeClass("gray");
                    $(this).unbind("focus");
                });
            }

            if (timer_bindevent)
                clearTimeout(timer_bindevent);
            timer_bindevent = setTimeout(function () {
                bindevent();
            }, 500);
        }

        function CheckForm() {
            var pass = true;

            for (var i = 0; i < $(".required").length; i++) {
                var t = $($(".required")[i]);
                if ($.trim(t.val()) == "") {
                    t.focus();
                    prompts(t, "请填写此字段");
                    pass = false;
                    break;
                }
            }

            if (pass) {
                var reg = new RegExp("^(-?\\d+)(\\.\\d+)?$");
                for (var i = 0; i < $(".number").length; i++) {
                    var t = $($(".number")[i]);
                    if ($.trim(t.val()) != "" && !reg.test(t.val())) {
                        t.focus();
                        prompts(t, "此字段只能填写数字");
                        pass = false;
                        break;
                    }
                }
            }

            return pass;
        }
    </script>
</head>
<style type="text/css">
    .datatable
    {
        border-top: 1px solid #000;
        border-left: 1px solid #000;
    }
    .datatable tr
    {
        height: 28px;
    }
    .datatable td
    {
        border-right: 1px solid #000;
        border-bottom: 1px solid #000;
        padding: 1px 3px;
    }
    .datatablecw
    {
        border-top: 1px solid Orange;
        border-left: 1px solid Orange;
    }
    .datatablecw tr
    {
        height: 28px;
    }
    .datatablecw td
    {
        border-right: 1px solid Orange;
        border-bottom: 1px solid Orange;
        padding: 1px 3px;
    }
    .bggray
    {
        background-color: Orange;
    }
</style>
<body>
    <form id="form1" runat="server">
    <div class="ht_main">
        <ul class="xnav">
            <li <%if(CurrentDep==DayReportDep.销售部){ %>class="current" <%} %> <%= GetDepHide(DayReportDep.销售部) %>>
                <a href="?<%= CurrentQuery%>&dep=<%= (int)DayReportDep.销售部%>">销售部信息</a></li>
            <li <%if(CurrentDep==DayReportDep.售后部){ %>class="current" <%} %> <%= GetDepHide(DayReportDep.售后部) %>>
                <a href="?<%= CurrentQuery%>&dep=<%= (int)DayReportDep.售后部%>">售后部信息</a></li>
            <li <%if(CurrentDep==DayReportDep.市场部){ %>class="current" <%} %> <%= GetDepHide(DayReportDep.市场部) %>>
                <a href="?<%= CurrentQuery%>&dep=<%= (int)DayReportDep.市场部%>">市场部信息</a></li>
            <li <%if(CurrentDep==DayReportDep.财务部){ %>class="current" <%} %> <%= GetDepHide(DayReportDep.财务部) %>>
                <a href="?<%= CurrentQuery%>&dep=<%= (int)DayReportDep.财务部%>">财务部信息</a></li>
            <li <%if(CurrentDep==DayReportDep.行政部){ %>class="current" <%} %> <%= GetDepHide(DayReportDep.行政部) %>>
                <a href="?<%= CurrentQuery%>&dep=<%= (int)DayReportDep.行政部%>">行政部信息</a></li>
            <li <%if(CurrentDep==DayReportDep.精品部){ %>class="current" <%} %> <%= GetDepHide(DayReportDep.精品部) %>>
                <a href="?<%= CurrentQuery%>&dep=<%= (int)DayReportDep.精品部%>">精品部信息</a></li>
            <li <%if(CurrentDep==DayReportDep.客服部){ %>class="current" <%} %> <%= GetDepHide(DayReportDep.客服部) %>>
                <a href="?<%= CurrentQuery%>&dep=<%= (int)DayReportDep.客服部%>">客服部信息</a></li>
            <li <%if(CurrentDep==DayReportDep.二手车部){ %>class="current" <%} %> <%= GetDepHide(DayReportDep.二手车部) %>>
                <a href="?<%= CurrentQuery%>&dep=<%= (int)DayReportDep.二手车部%>">二手车部信息</a></li>
            <li <%if(CurrentDep==DayReportDep.金融部){ %>class="current" <%} %> <%= GetDepHide(DayReportDep.金融部) %>>
                <a href="?<%= CurrentQuery%>&dep=<%= (int)DayReportDep.金融部%>">金融部信息</a></li>
            <li <%if(CurrentDep==DayReportDep.DCC部){ %>class="current" <%} %> <%= GetDepHide(DayReportDep.DCC部) %>>
                <a href="?<%= CurrentQuery%>&dep=<%= (int)DayReportDep.DCC部%>">DCC部信息</a></li>
            <li <%if(CurrentDep==DayReportDep.无忧产品){ %>class="current" <%} %> <%= GetDepHide(DayReportDep.无忧产品) %>>
                <a href="?<%= CurrentQuery%>&dep=<%= (int)DayReportDep.无忧产品%>">无忧产品信息</a></li>
        </ul>
        <asp:ScriptManager runat="server" ID="sm">
        </asp:ScriptManager>
        <asp:UpdatePanel runat="server" ID="up1">
            <ContentTemplate>
                <table width="100%" border="0" cellspacing="0" cellpadding="0" class="biaoge3">
                    <caption class="bt2">
                        日报录入</caption>
                    <tbody>
                        <tr>
                            <td class="bg3 tr">
                                公司：
                            </td>
                            <td>
                                <asp:DropDownList runat="server" ID="ddlCorp" AutoPostBack="true" OnSelectedIndexChanged="ddlCorp_SelectedIndexChanged">
                                </asp:DropDownList>
                            </td>
                        </tr>
                        <tr>
                            <td class="bg3 tr">
                                日期：
                            </td>
                            <td>
                                <asp:TextBox runat="server" ID="txtDate" CssClass="srk6" OnTextChanged="txtDate_TextChanged"
                                    AutoPostBack="true"></asp:TextBox>
                            </td>
                        </tr>
                        <%if (CurrentDep == DayReportDep.销售部 || CurrentDep == DayReportDep.售后部)
                          {%>
                        <tr>
                            <td class="bg3 tr">
                                数据审核状态：
                            </td>
                            <td>
                                <asp:Label runat="server" ID="txtDailyReportCheckStatus"></asp:Label><span class="red">（目前由财务经理进行数据审核）</span>
                            </td>
                        </tr>
                        <tr>
                            <td class="bg3 tr">
                                审核备注：
                            </td>
                            <td>
                                <asp:Label runat="server" ID="txtDailyReportCheckRemark"></asp:Label>
                            </td>
                        </tr>
                        <%}%>
                        <%if (CurrentDep == DayReportDep.销售部)
                          {%>
                        <%if (CurrentUser.AllowYearGahterInput == "1")
                          { %>
                        <tr>
                            <td class="bg3 tr" style="background-color: Orange;">
                                本月整车实际销售额：
                            </td>
                            <td>
                                <asp:TextBox runat="server" ID="txtXSbyzcsjxse" CssClass="srk6 tr number"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td class="bg3 tr" style="background-color: Orange;">
                                本月整车预算销售额：
                            </td>
                            <td>
                                <asp:TextBox runat="server" ID="txtXSbyzcysxse" CssClass="srk6 tr number"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td class="bg3 tr" style="background-color: Orange;">
                                本月整车裸车实际毛利额：
                            </td>
                            <td>
                                <asp:TextBox runat="server" ID="txtXSbyzclcsjmle" CssClass="srk6 tr number"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td class="bg3 tr" style="background-color: Orange;">
                                本月整车裸车预算毛利额：
                            </td>
                            <td>
                                <asp:TextBox runat="server" ID="txtXSbyzclcysmle" CssClass="srk6 tr number"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td class="bg3 tr" style="background-color: Orange;">
                                本月厂方返利实际收入：
                            </td>
                            <td>
                                <asp:TextBox runat="server" ID="txtXSbycfflsjsr" CssClass="srk6 tr number"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td class="bg3 tr" style="background-color: Orange;">
                                本月厂方返利预算收入：
                            </td>
                            <td>
                                <asp:TextBox runat="server" ID="txtXSbycfflyssr" CssClass="srk6 tr number"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td class="bg3 tr" style="background-color: Orange;">
                                本月厂方金融手续费净收入：
                            </td>
                            <td>
                                <asp:TextBox runat="server" ID="txtXSbycfjrsxfjsr" CssClass="srk6 tr number"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td class="bg3 tr" style="background-color: Orange;">
                                本月美容交车净收入：
                            </td>
                            <td>
                                <asp:TextBox runat="server" ID="txtXSbymrjcjsr" CssClass="srk6 tr number"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td class="bg3 tr" style="background-color: Orange;">
                                本月精品毛利：
                            </td>
                            <td>
                                <asp:TextBox runat="server" ID="txtXSbyjpmlsr" CssClass="srk6 tr number"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td class="bg3 tr" style="background-color: Orange;">
                                本月精品预算毛利：
                            </td>
                            <td>
                                <asp:TextBox runat="server" ID="txtXSbyjpmlyssr" CssClass="srk6 tr number"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td class="bg3 tr" style="background-color: Orange;">
                                本月延保毛利：
                            </td>
                            <td>
                                <asp:TextBox runat="server" ID="txtXSbyybml" CssClass="srk6 tr number"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td class="bg3 tr" style="background-color: Orange;">
                                本月免费保养毛利：
                            </td>
                            <td>
                                <asp:TextBox runat="server" ID="txtXSbyzsmfbyml" CssClass="srk6 tr number"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td class="bg3 tr" style="background-color: Orange;">
                                本月其他收入：
                            </td>
                            <td>
                                <asp:TextBox runat="server" ID="txtXSbyqtsr" CssClass="srk6 tr number"></asp:TextBox>
                            </td>
                        </tr>
                        <%}else{ %>
                        <tr>
                            <td class="bg3 tr" style="background-color: Orange;">
                                在途车辆：
                            </td>
                            <td>
                                <asp:TextBox runat="server" ID="txtXSztcl" CssClass="srk6 tr number"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td class="bg3 tr" style="background-color: Orange;">
                                周转天数：
                            </td>
                            <td>
                                <asp:TextBox runat="server" ID="txtXSzzts" CssClass="srk6 tr number"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td class="bg3 tr" style="background-color: Orange;">
                                在库平均单台成本价：
                            </td>
                            <td>
                                <asp:TextBox runat="server" ID="txtXSclpjdj" CssClass="srk6 tr number"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td class="bg3 tr" style="background-color: Orange;">
                                厂家虚出台次：
                            </td>
                            <td>
                                <asp:TextBox runat="server" ID="txtXScjxctc" CssClass="srk6 tr number"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td class="bg3 tr" style="background-color: Orange;">
                                在库超3个月台次：
                            </td>
                            <td>
                                <asp:TextBox runat="server" ID="txtXSzkcsgytc" CssClass="srk6 tr number"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td class="bg3 tr" style="background-color: Orange;">
                                在库超6个月台次：
                            </td>
                            <td>
                                <asp:TextBox runat="server" ID="txtXSzkclgytc" CssClass="srk6 tr number"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td class="bg3 tr" style="background-color: Orange;">
                                在库超1年台次：
                            </td>
                            <td>
                                <asp:TextBox runat="server" ID="txtXSzkcyntc" CssClass="srk6 tr number"></asp:TextBox>
                            </td>
                        </tr>
                        <%} %>
                        <%}else if (CurrentDep == DayReportDep.售后部) {%>
                          <%if (CurrentUser.AllowYearGahterInput == "1")
                          { %>
                            <tr>
                                <td class="bg3 tr" style="background-color: Orange;">
                                    本月一般维修额：
                                </td>
                                <td>
                                    <asp:TextBox runat="server" ID="txtSHbyybwxe" CssClass="srk6 tr number"></asp:TextBox>
                                </td>
                            </tr>
                            <tr>
                                <td class="bg3 tr" style="background-color: Orange;">
                                    本月首保索赔额：
                                </td>
                                <td>
                                    <asp:TextBox runat="server" ID="txtSHbysbspe" CssClass="srk6 tr number"></asp:TextBox>
                                </td>
                            </tr>
                            <tr>
                                <td class="bg3 tr" style="background-color: Orange;">
                                    本月他品牌收入：
                                </td>
                                <td>
                                    <asp:TextBox runat="server" ID="txtSHbytppsr" CssClass="srk6 tr number"></asp:TextBox>
                                </td>
                            </tr>
                            <tr>
                                <td class="bg3 tr" style="background-color: Orange;">
                                    本月维修毛利额：
                                </td>
                                <td>
                                    <asp:TextBox runat="server" ID="txtSHbywxmle" CssClass="srk6 tr number"></asp:TextBox>
                                </td>
                            </tr>
                            <tr>
                                <td class="bg3 tr" style="background-color: Orange;">
                                    本月维修毛利率：
                                </td>
                                <td>
                                    <asp:TextBox runat="server" ID="txtSHbywxmll" CssClass="srk4 tr number"></asp:TextBox>%
                                </td>
                            </tr>
                            <tr>
                                <td class="bg3 tr" style="background-color: Orange;">
                                    本月废旧收入：
                                </td>
                                <td>
                                    <asp:TextBox runat="server" ID="txtSHbyfjsr" CssClass="srk6 tr number"></asp:TextBox>
                                </td>
                            </tr>
                            <tr>
                                <td class="bg3 tr" style="background-color: Orange;">
                                    本月含废旧的维修实际毛利率：
                                </td>
                                <td>
                                    <asp:TextBox runat="server" ID="txtSHbyhfjdwxsjmll" CssClass="srk4 tr number"></asp:TextBox>%
                                </td>
                            </tr>
                            <tr>
                                <td class="bg3 tr" style="background-color: Orange;">
                                    本月含废旧的维修预算毛利率：
                                </td>
                                <td>
                                    <asp:TextBox runat="server" ID="txtSHbyhfjdwxysmll" CssClass="srk4 tr number"></asp:TextBox>%
                                </td>
                            </tr>
                            <tr>
                                <td class="bg3 tr" style="background-color: Orange;">
                                    本月油漆收入：
                                </td>
                                <td>
                                    <asp:TextBox runat="server" ID="txtSHbyyqsr" CssClass="srk6 tr number"></asp:TextBox>
                                </td>
                            </tr>
                            <tr>
                                <td class="bg3 tr" style="background-color: Orange;">
                                    本月油漆成本：
                                </td>
                                <td>
                                    <asp:TextBox runat="server" ID="txtSHbyyqcb" CssClass="srk6 tr number"></asp:TextBox>
                                </td>
                            </tr>
                            <tr>
                                <td class="bg3 tr" style="background-color: Orange;">
                                    本月养护产品毛利率：
                                </td>
                                <td>
                                    <asp:TextBox runat="server" ID="txtSHbyyhcpmll" CssClass="srk4 tr number"></asp:TextBox>%
                                </td>
                            </tr>
                            <tr>
                                <td class="bg3 tr" style="background-color: Orange;">
                                    本月标准库存金额：
                                </td>
                                <td>
                                    <asp:TextBox runat="server" ID="txtSHbybzkcje" CssClass="srk6 tr number"></asp:TextBox>
                                </td>
                            </tr>
                            <tr>
                                <td class="bg3 tr" style="background-color: Orange;">
                                    本月期末实际库存额：
                                </td>
                                <td>
                                    <asp:TextBox runat="server" ID="txtSHbyqmsjkce" CssClass="srk6 tr number"></asp:TextBox>
                                </td>
                            </tr>
                            <tr>
                                <td class="bg3 tr" style="background-color: Orange;">
                                    其中一年以上库存额：
                                </td>
                                <td>
                                    <asp:TextBox runat="server" ID="txtSHqzynyskce" CssClass="srk6 tr number"></asp:TextBox>
                                </td>
                            </tr>
                            <tr>
                                <td class="bg3 tr" style="background-color: Orange;">
                                    本月标准的库存度：
                                </td>
                                <td>
                                    <asp:TextBox runat="server" ID="txtSHbybzdkcd" CssClass="srk6 tr number"></asp:TextBox>
                                </td>
                            </tr>
                            <tr>
                                <td class="bg3 tr" style="background-color: Orange;">
                                    本月实际库存度：
                                </td>
                                <td>
                                    <asp:TextBox runat="server" ID="txtSHbysjkcd" CssClass="srk6 tr number"></asp:TextBox>
                                </td>
                            </tr>
                            <tr>
                                <td class="bg3 tr" style="background-color: Orange;">
                                    本月配件毛利率：
                                </td>
                                <td>
                                    <asp:TextBox runat="server" ID="txtSHbypjmll" CssClass="srk4 tr number"></asp:TextBox>%
                                </td>
                            </tr>
                            <tr>
                                <td class="bg3 tr" style="background-color: Orange;">
                                    本月事故车毛利率：
                                </td>
                                <td>
                                    <asp:TextBox runat="server" ID="txtSHbysgcmll" CssClass="srk4 tr number"></asp:TextBox>%
                                </td>
                            </tr>
                            <tr>
                                <td class="bg3 tr" style="background-color: Orange;">
                                    本月一般维修毛利率：
                                </td>
                                <td>
                                    <asp:TextBox runat="server" ID="txtSHbyybwxmll" CssClass="srk4 tr number"></asp:TextBox>%
                                </td>
                            </tr>
                            <tr>
                                <td class="bg3 tr" style="background-color: Orange;">
                                    本月他牌车维修毛利率：
                                </td>
                                <td>
                                    <asp:TextBox runat="server" ID="txtSHbytppwxmll" CssClass="srk4 tr number"></asp:TextBox>%
                                </td>
                            </tr>
                            <tr>
                                <td class="bg3 tr" style="background-color: Orange;">
                                    本月本月续保返利收入：
                                </td>
                                <td>
                                    <asp:TextBox runat="server" ID="txtSHbyxbflsr" CssClass="srk6 tr number"></asp:TextBox>
                                </td>
                            </tr>
                            <tr>
                                <td class="bg3 tr" style="background-color: Orange;">
                                    本月续保平均单台净收入：
                                </td>
                                <td>
                                    <asp:TextBox runat="server" ID="txtSHbyxbpjdtjsr" CssClass="srk6 tr number"></asp:TextBox>
                                </td>
                            </tr>
                            <tr>
                                <td class="bg3 tr" style="background-color: Orange;">
                                    本月延保返利收入：
                                </td>
                                <td>
                                    <asp:TextBox runat="server" ID="txtSHbyybflsr" CssClass="srk6 tr number"></asp:TextBox>
                                </td>
                            </tr>
                            <tr>
                                <td class="bg3 tr" style="background-color: Orange;">
                                    本月延保平均单台净收入：
                                </td>
                                <td>
                                    <asp:TextBox runat="server" ID="txtSHbyybpjdtjsr" CssClass="srk6 tr number"></asp:TextBox>
                                </td>
                            </tr>
                            <tr>
                                <td class="bg3 tr" style="background-color: Orange;">
                                    本月导航升级业务平均单台收入：
                                </td>
                                <td>
                                    <asp:TextBox runat="server" ID="txtSHbydhsjywpjdtsr" CssClass="srk6 tr number"></asp:TextBox>
                                </td>
                            </tr>
                           <%} %>
                        <%}else if (CurrentDep == DayReportDep.精品部) {%>
                            <%if(CurrentUser.AllowYearGahterInput == "1"){ %>
                            <tr>
                                <td class="bg3 tr" style="background-color: Orange;">
                                    本月预算展厅精品毛利额：
                                </td>
                                <td>
                                    <asp:TextBox runat="server" ID="txtJPysztjpmle" CssClass="srk6 tr number"></asp:TextBox>
                                </td>
                            </tr>
                            <tr>
                                <td class="bg3 tr" style="background-color: Orange;">
                                    本月预算网点精品毛利额：
                                </td>
                                <td>
                                    <asp:TextBox runat="server" ID="txtJPyswdjpmle" CssClass="srk6 tr number"></asp:TextBox>
                                </td>
                            </tr>
                            <tr>
                                <td class="bg3 tr" style="background-color: Orange;">
                                    本月预算售后精品毛利额：
                                </td>
                                <td>
                                    <asp:TextBox runat="server" ID="txtJPysshjpmle" CssClass="srk6 tr number"></asp:TextBox>
                                </td>
                            </tr>
                            <tr>
                                <td class="bg3 tr" style="background-color: Orange;">
                                    本月预算展厅单车产值：
                                </td>
                                <td>
                                    <asp:TextBox runat="server" ID="txtJPysztdccz" CssClass="srk6 tr number"></asp:TextBox>
                                </td>
                            </tr>
                            <tr>
                                <td class="bg3 tr" style="background-color: Orange;">
                                    本月预算网点单台产值：
                                </td>
                                <td>
                                    <asp:TextBox runat="server" ID="txtJPyswddtcz" CssClass="srk6 tr number"></asp:TextBox>
                                </td>
                            </tr>
                            <tr>
                                <td class="bg3 tr" style="background-color: Orange;">
                                    本月预算售后单台产值：
                                </td>
                                <td>
                                    <asp:TextBox runat="server" ID="txtJPysshdtcz" CssClass="srk6 tr number"></asp:TextBox>
                                </td>
                            </tr>
                            <tr>
                                <td class="bg3 tr" style="background-color: Orange;">
                                    本月展厅实际精品产值：
                                </td>
                                <td>
                                    <asp:TextBox runat="server" ID="txtJPztsjjpcz" CssClass="srk6 tr number"></asp:TextBox>
                                </td>
                            </tr>
                            <tr>
                                <td class="bg3 tr" style="background-color: Orange;">
                                    本月展厅实际的毛利额：
                                </td>
                                <td>
                                    <asp:TextBox runat="server" ID="txtJPztsjmle" CssClass="srk6 tr number"></asp:TextBox>
                                </td>
                            </tr>
                            <tr>
                                <td class="bg3 tr" style="background-color: Orange;">
                                    本月网点精品毛利额：
                                </td>
                                <td>
                                    <asp:TextBox runat="server" ID="txtJPwdjpmle" CssClass="srk6 tr number"></asp:TextBox>
                                </td>
                            </tr>
                            <tr>
                                <td class="bg3 tr" style="background-color: Orange;">
                                    本月售后精品毛利额：
                                </td>
                                <td>
                                    <asp:TextBox runat="server" ID="txtJPshjpmle" CssClass="srk6 tr number"></asp:TextBox>
                                </td>
                            </tr>
                            <tr>
                                <td class="bg3 tr" style="background-color: Orange;">
                                    本月期末库存数：
                                </td>
                                <td>
                                    <asp:TextBox runat="server" ID="txtJPqmkcs" CssClass="srk6 tr number"></asp:TextBox>
                                </td>
                            </tr>
                            <tr>
                                <td class="bg3 tr" style="background-color: Orange;">
                                    3个月以上滞销库存：
                                </td>
                                <td>
                                    <asp:TextBox runat="server" ID="txtJPsgyyszxkc" CssClass="srk6 tr number"></asp:TextBox>
                                </td>
                            </tr>
                            <tr>
                                <td class="bg3 tr" style="background-color: Orange;">
                                    1年以上滞销库存：
                                </td>
                                <td>
                                    <asp:TextBox runat="server" ID="txtJPynyszxkc" CssClass="srk6 tr number"></asp:TextBox>
                                </td>
                            </tr>
                            <%} %>
                        <%} %>
                        <%if (CurrentDep == DayReportDep.财务部 && HasMonthlyTargetPower())
                          {%>
                        <tr>
                            <td class="bg3 tr" style="background-color: Orange;">
                                月初资金余额：
                            </td>
                            <td>
                                <asp:TextBox runat="server" ID="txtCWzjye" CssClass="srk6 tr number"></asp:TextBox>
                                万元
                            </td>
                        </tr>
                        <tr>
                            <td class="bg3 tr" style="background-color: Orange;">
                                月初POS未到帐：
                            </td>
                            <td>
                                <asp:TextBox runat="server" ID="txtCWposwdz" CssClass="srk6 tr number"></asp:TextBox>
                                万元
                            </td>
                        </tr>
                        <tr>
                            <td class="bg3 tr" style="background-color: Orange;">
                                月初银行帐户余额：
                            </td>
                            <td>
                                <asp:TextBox runat="server" ID="txtCWyhzhye" ReadOnly="true" CssClass="srk6 tr number countycyhzhye"></asp:TextBox>
                                万元
                            </td>
                        </tr>
                        <tr>
                            <td class="bg3 tr" style="background-color: Orange;">
                                其中农行：
                            </td>
                            <td>
                                <asp:TextBox runat="server" ID="txtCWnhzhye" CssClass="srk6 tr number countycyhzhyesub"></asp:TextBox>
                                万元
                            </td>
                        </tr>
                        <tr>
                            <td class="bg3 tr" style="background-color: Orange;">
                                中行：
                            </td>
                            <td>
                                <asp:TextBox runat="server" ID="txtCWzhzhye" CssClass="srk6 tr number countycyhzhyesub"></asp:TextBox>
                                万元
                            </td>
                        </tr>
                        <tr>
                            <td class="bg3 tr" style="background-color: Orange;">
                                工行：
                            </td>
                            <td>
                                <asp:TextBox runat="server" ID="txtCWghzhye" CssClass="srk6 tr number countycyhzhyesub"></asp:TextBox>
                                万元
                            </td>
                        </tr>
                        <tr>
                            <td class="bg3 tr" style="background-color: Orange;">
                                建行：
                            </td>
                            <td>
                                <asp:TextBox runat="server" ID="txtCWjianhzhye" CssClass="srk6 tr number countycyhzhyesub"></asp:TextBox>
                                万元
                            </td>
                        </tr>
                        <tr>
                            <td class="bg3 tr" style="background-color: Orange;">
                                交行：
                            </td>
                            <td>
                                <asp:TextBox runat="server" ID="txtCWjhzhye" CssClass="srk6 tr number countycyhzhyesub"></asp:TextBox>
                                万元
                            </td>
                        </tr>
                        <tr>
                            <td class="bg3 tr" style="background-color: Orange;">
                                民生：
                            </td>
                            <td>
                                <asp:TextBox runat="server" ID="txtCWmszhye" CssClass="srk6 tr number countycyhzhyesub"></asp:TextBox>
                                万元
                            </td>
                        </tr>
                        <tr>
                            <td class="bg3 tr" style="background-color: Orange;">
                                平安：
                            </td>
                            <td>
                                <asp:TextBox runat="server" ID="txtCWpazhye" CssClass="srk6 tr number countycyhzhyesub"></asp:TextBox>
                                万元
                            </td>
                        </tr>
                        <tr>
                            <td class="bg3 tr" style="background-color: Orange;">
                                中信：
                            </td>
                            <td>
                                <asp:TextBox runat="server" ID="txtCWzxzhye" CssClass="srk6 tr number countycyhzhyesub"></asp:TextBox>
                                万元
                            </td>
                        </tr>
                        <tr>
                            <td class="bg3 tr" style="background-color: Orange;">
                                华夏：
                            </td>
                            <td>
                                <asp:TextBox runat="server" ID="txtCWhxzhye" CssClass="srk6 tr number countycyhzhyesub"></asp:TextBox>
                                万元
                            </td>
                        </tr>
                        <tr>
                            <td class="bg3 tr" style="background-color: Orange;">
                                浙商：
                            </td>
                            <td>
                                <asp:TextBox runat="server" ID="txtCWzszhye" CssClass="srk6 tr number countycyhzhyesub"></asp:TextBox>
                                万元
                            </td>
                        </tr>
                        <tr>
                            <td class="bg3 tr" style="background-color: Orange;">
                                泰隆：
                            </td>
                            <td>
                                <asp:TextBox runat="server" ID="txtCWtlzhye" CssClass="srk6 tr number countycyhzhyesub"></asp:TextBox>
                                万元
                            </td>
                        </tr>
                        <tr>
                            <td class="bg3 tr" style="background-color: Orange;">
                                其他银行：
                            </td>
                            <td>
                                <asp:TextBox runat="server" ID="txtCWqtyhzhye" CssClass="srk6 tr number countycyhzhyesub"></asp:TextBox>
                                万元
                            </td>
                        </tr>
                        <tr>
                            <td class="bg3 tr" style="background-color: Orange;">
                                月初现金合计：
                            </td>
                            <td>
                                <asp:TextBox runat="server" ID="txtCWxjczhj" CssClass="srk6 tr number"></asp:TextBox>
                                万元
                            </td>
                        </tr>
                        <tr>
                            <td class="bg3 tr" style="background-color: Orange;">
                                月初留存现金：
                            </td>
                            <td>
                                <asp:TextBox runat="server" ID="txtCWlcxj" CssClass="srk6 tr number"></asp:TextBox>
                                万元
                            </td>
                        </tr>
                        <tr>
                            <td class="bg3 tr" style="background-color: Orange;">
                                银承、贷款到期：
                            </td>
                            <td>
                                <input type="hidden" runat="server" id="hdnCWycdkdq" />
                                <table width="600" border="0" cellspacing="0" cellpadding="0" class="datatablecw">
                                    <tr>
                                        <td>
                                            <asp:TextBox runat="server" ID="txtCWycdkdq1" class="srk7 tc cwycdkdq gray"></asp:TextBox>
                                        </td>
                                        <td>
                                            <asp:TextBox runat="server" ID="txtCWycdkdq2" class="srk7 tc cwycdkdq gray"></asp:TextBox>
                                        </td>
                                        <td>
                                            <asp:TextBox runat="server" ID="txtCWycdkdq3" class="srk7 tc cwycdkdq gray"></asp:TextBox>
                                        </td>
                                        <td>
                                            <asp:TextBox runat="server" ID="txtCWycdkdq4" class="srk7 tc cwycdkdq gray"></asp:TextBox>
                                        </td>
                                        <td>
                                            <asp:TextBox runat="server" ID="txtCWycdkdq5" class="srk7 tc cwycdkdq gray"></asp:TextBox>
                                        </td>
                                        <td>
                                            <asp:TextBox runat="server" ID="txtCWycdkdq6" class="srk7 tc cwycdkdq gray"></asp:TextBox>
                                        </td>
                                        <td>
                                            <asp:TextBox runat="server" ID="txtCWycdkdq7" class="srk7 tc cwycdkdq gray"></asp:TextBox>
                                        </td>
                                        <td>
                                            <asp:TextBox runat="server" ID="txtCWycdkdq8" class="srk7 tc cwycdkdq gray"></asp:TextBox>
                                        </td>
                                        <td>
                                            <asp:TextBox runat="server" ID="txtCWycdkdq9" class="srk7 tc cwycdkdq gray"></asp:TextBox>
                                        </td>
                                        <td>
                                            <asp:TextBox runat="server" ID="txtCWycdkdq10" class="srk7 tc cwycdkdq gray"></asp:TextBox>
                                        </td>
                                        <td>
                                            &nbsp;
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <asp:TextBox runat="server" ID="txtCWycdkdq11" class="srk7 tc cwycdkdq gray"></asp:TextBox>
                                        </td>
                                        <td>
                                            <asp:TextBox runat="server" ID="txtCWycdkdq12" class="srk7 tc cwycdkdq gray"></asp:TextBox>
                                        </td>
                                        <td>
                                            <asp:TextBox runat="server" ID="txtCWycdkdq13" class="srk7 tc cwycdkdq gray"></asp:TextBox>
                                        </td>
                                        <td>
                                            <asp:TextBox runat="server" ID="txtCWycdkdq14" class="srk7 tc cwycdkdq gray"></asp:TextBox>
                                        </td>
                                        <td>
                                            <asp:TextBox runat="server" ID="txtCWycdkdq15" class="srk7 tc cwycdkdq gray"></asp:TextBox>
                                        </td>
                                        <td>
                                            <asp:TextBox runat="server" ID="txtCWycdkdq16" class="srk7 tc cwycdkdq gray"></asp:TextBox>
                                        </td>
                                        <td>
                                            <asp:TextBox runat="server" ID="txtCWycdkdq17" class="srk7 tc cwycdkdq gray"></asp:TextBox>
                                        </td>
                                        <td>
                                            <asp:TextBox runat="server" ID="txtCWycdkdq18" class="srk7 tc cwycdkdq gray"></asp:TextBox>
                                        </td>
                                        <td>
                                            <asp:TextBox runat="server" ID="txtCWycdkdq19" class="srk7 tc cwycdkdq gray"></asp:TextBox>
                                        </td>
                                        <td>
                                            <asp:TextBox runat="server" ID="txtCWycdkdq20" class="srk7 tc cwycdkdq gray"></asp:TextBox>
                                        </td>
                                        <td>
                                            &nbsp;
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <asp:TextBox runat="server" ID="txtCWycdkdq21" class="srk7 tc cwycdkdq gray"></asp:TextBox>
                                        </td>
                                        <td>
                                            <asp:TextBox runat="server" ID="txtCWycdkdq22" class="srk7 tc cwycdkdq gray"></asp:TextBox>
                                        </td>
                                        <td>
                                            <asp:TextBox runat="server" ID="txtCWycdkdq23" class="srk7 tc cwycdkdq gray"></asp:TextBox>
                                        </td>
                                        <td>
                                            <asp:TextBox runat="server" ID="txtCWycdkdq24" class="srk7 tc cwycdkdq gray"></asp:TextBox>
                                        </td>
                                        <td>
                                            <asp:TextBox runat="server" ID="txtCWycdkdq25" class="srk7 tc cwycdkdq gray"></asp:TextBox>
                                        </td>
                                        <td>
                                            <asp:TextBox runat="server" ID="txtCWycdkdq26" class="srk7 tc cwycdkdq gray"></asp:TextBox>
                                        </td>
                                        <td>
                                            <asp:TextBox runat="server" ID="txtCWycdkdq27" class="srk7 tc cwycdkdq gray"></asp:TextBox>
                                        </td>
                                        <td>
                                            <asp:TextBox runat="server" ID="txtCWycdkdq28" class="srk7 tc cwycdkdq gray"></asp:TextBox>
                                        </td>
                                        <td>
                                            <asp:TextBox runat="server" ID="txtCWycdkdq29" class="srk7 tc cwycdkdq gray"></asp:TextBox>
                                        </td>
                                        <td>
                                            <asp:TextBox runat="server" ID="txtCWycdkdq30" class="srk7 tc cwycdkdq gray"></asp:TextBox>
                                        </td>
                                        <td>
                                            <asp:TextBox runat="server" ID="txtCWycdkdq31" class="srk7 tc cwycdkdq gray"></asp:TextBox>
                                        </td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                        <%} %>
                        <%if (CurrentDep == DayReportDep.行政部 && HasMonthlyTargetPower())
                          {%>
                        <tr>
                            <td class="bg3">
                            </td>
                            <td>
                                <table width="640" border="0" cellspacing="0" cellpadding="0" class="datatable">
                                    <tr class="tc bold" style="background-color: Orange;">
                                        <td class="w120">
                                            项目
                                        </td>
                                        <td class="w120">
                                            车牌、负责人
                                        </td>
                                        <td class="w60">
                                            第一周
                                        </td>
                                        <td class="w60">
                                            第二周
                                        </td>
                                        <td class="w60">
                                            第三周
                                        </td>
                                        <td class="w60">
                                            第四周
                                        </td>
                                        <td class="w120">
                                            上月未处理
                                        </td>
                                        <td class="w40">
                                            违章
                                        </td>
                                    </tr>
                                    <tr style="background-color: Yellow;">
                                        <td class="tc bold" rowspan="7" style="vertical-align: middle;">
                                            车辆违章次数
                                        </td>
                                        <td>
                                            <asp:TextBox runat="server" ID="txtXZcpfzr1" CssClass="srk6"></asp:TextBox>
                                        </td>
                                        <td>
                                            <asp:TextBox runat="server" ID="txtXZdyzclwzcs1" CssClass="srk7 number"></asp:TextBox>
                                        </td>
                                        <td>
                                            <asp:TextBox runat="server" ID="txtXZdezclwzcs1" CssClass="srk7 number"></asp:TextBox>
                                        </td>
                                        <td>
                                            <asp:TextBox runat="server" ID="txtXZdszclwzcs1" CssClass="srk7 number"></asp:TextBox>
                                        </td>
                                        <td>
                                            <asp:TextBox runat="server" ID="txtXZdsizclwzcs1" CssClass="srk7 number"></asp:TextBox>
                                        </td>
                                        <td>
                                            <asp:TextBox runat="server" ID="txtXZsywcl1" CssClass="srk6"></asp:TextBox>
                                        </td>
                                        <td>
                                            <asp:TextBox runat="server" ID="txtXZwz1" CssClass="srk7 number"></asp:TextBox>
                                        </td>
                                    </tr>
                                    <tr style="background-color: Yellow;">
                                        <td>
                                            <asp:TextBox runat="server" ID="txtXZcpfzr2" CssClass="srk6"></asp:TextBox>
                                        </td>
                                        <td>
                                            <asp:TextBox runat="server" ID="txtXZdyzclwzcs2" CssClass="srk7 number"></asp:TextBox>
                                        </td>
                                        <td>
                                            <asp:TextBox runat="server" ID="txtXZdezclwzcs2" CssClass="srk7 number"></asp:TextBox>
                                        </td>
                                        <td>
                                            <asp:TextBox runat="server" ID="txtXZdszclwzcs2" CssClass="srk7 number"></asp:TextBox>
                                        </td>
                                        <td>
                                            <asp:TextBox runat="server" ID="txtXZdsizclwzcs2" CssClass="srk7 number"></asp:TextBox>
                                        </td>
                                        <td>
                                            <asp:TextBox runat="server" ID="txtXZsywcl2" CssClass="srk6"></asp:TextBox>
                                        </td>
                                        <td>
                                            <asp:TextBox runat="server" ID="txtXZwz2" CssClass="srk7 number"></asp:TextBox>
                                        </td>
                                    </tr>
                                    <tr style="background-color: Yellow;">
                                        <td>
                                            <asp:TextBox runat="server" ID="txtXZcpfzr3" CssClass="srk6"></asp:TextBox>
                                        </td>
                                        <td>
                                            <asp:TextBox runat="server" ID="txtXZdyzclwzcs3" CssClass="srk7 number"></asp:TextBox>
                                        </td>
                                        <td>
                                            <asp:TextBox runat="server" ID="txtXZdezclwzcs3" CssClass="srk7 number"></asp:TextBox>
                                        </td>
                                        <td>
                                            <asp:TextBox runat="server" ID="txtXZdszclwzcs3" CssClass="srk7 number"></asp:TextBox>
                                        </td>
                                        <td>
                                            <asp:TextBox runat="server" ID="txtXZdsizclwzcs3" CssClass="srk7 number"></asp:TextBox>
                                        </td>
                                        <td>
                                            <asp:TextBox runat="server" ID="txtXZsywcl3" CssClass="srk6"></asp:TextBox>
                                        </td>
                                        <td>
                                            <asp:TextBox runat="server" ID="txtXZwz3" CssClass="srk7 number"></asp:TextBox>
                                        </td>
                                    </tr>
                                    <tr style="background-color: Yellow;">
                                        <td>
                                            <asp:TextBox runat="server" ID="txtXZcpfzr4" CssClass="srk6"></asp:TextBox>
                                        </td>
                                        <td>
                                            <asp:TextBox runat="server" ID="txtXZdyzclwzcs4" CssClass="srk7 number"></asp:TextBox>
                                        </td>
                                        <td>
                                            <asp:TextBox runat="server" ID="txtXZdezclwzcs4" CssClass="srk7 number"></asp:TextBox>
                                        </td>
                                        <td>
                                            <asp:TextBox runat="server" ID="txtXZdszclwzcs4" CssClass="srk7 number"></asp:TextBox>
                                        </td>
                                        <td>
                                            <asp:TextBox runat="server" ID="txtXZdsizclwzcs4" CssClass="srk7 number"></asp:TextBox>
                                        </td>
                                        <td>
                                            <asp:TextBox runat="server" ID="txtXZsywcl4" CssClass="srk6"></asp:TextBox>
                                        </td>
                                        <td>
                                            <asp:TextBox runat="server" ID="txtXZwz4" CssClass="srk7 number"></asp:TextBox>
                                        </td>
                                    </tr>
                                    <tr style="background-color: Yellow;">
                                        <td>
                                            <asp:TextBox runat="server" ID="txtXZcpfzr5" CssClass="srk6"></asp:TextBox>
                                        </td>
                                        <td>
                                            <asp:TextBox runat="server" ID="txtXZdyzclwzcs5" CssClass="srk7 number"></asp:TextBox>
                                        </td>
                                        <td>
                                            <asp:TextBox runat="server" ID="txtXZdezclwzcs5" CssClass="srk7 number"></asp:TextBox>
                                        </td>
                                        <td>
                                            <asp:TextBox runat="server" ID="txtXZdszclwzcs5" CssClass="srk7 number"></asp:TextBox>
                                        </td>
                                        <td>
                                            <asp:TextBox runat="server" ID="txtXZdsizclwzcs5" CssClass="srk7 number"></asp:TextBox>
                                        </td>
                                        <td>
                                            <asp:TextBox runat="server" ID="txtXZsywcl5" CssClass="srk6"></asp:TextBox>
                                        </td>
                                        <td>
                                            <asp:TextBox runat="server" ID="txtXZwz5" CssClass="srk7 number"></asp:TextBox>
                                        </td>
                                    </tr>
                                    <tr style="background-color: Yellow;">
                                        <td>
                                            <asp:TextBox runat="server" ID="txtXZcpfzr6" CssClass="srk6"></asp:TextBox>
                                        </td>
                                        <td>
                                            <asp:TextBox runat="server" ID="txtXZdyzclwzcs6" CssClass="srk7 number"></asp:TextBox>
                                        </td>
                                        <td>
                                            <asp:TextBox runat="server" ID="txtXZdezclwzcs6" CssClass="srk7 number"></asp:TextBox>
                                        </td>
                                        <td>
                                            <asp:TextBox runat="server" ID="txtXZdszclwzcs6" CssClass="srk7 number"></asp:TextBox>
                                        </td>
                                        <td>
                                            <asp:TextBox runat="server" ID="txtXZdsizclwzcs6" CssClass="srk7 number"></asp:TextBox>
                                        </td>
                                        <td>
                                            <asp:TextBox runat="server" ID="txtXZsywcl6" CssClass="srk6"></asp:TextBox>
                                        </td>
                                        <td>
                                            <asp:TextBox runat="server" ID="txtXZwz6" CssClass="srk7 number"></asp:TextBox>
                                        </td>
                                    </tr>
                                    <tr style="background-color: Yellow;">
                                        <td>
                                            <asp:TextBox runat="server" ID="txtXZcpfzr7" CssClass="srk6"></asp:TextBox>
                                        </td>
                                        <td>
                                            <asp:TextBox runat="server" ID="txtXZdyzclwzcs7" CssClass="srk7 number"></asp:TextBox>
                                        </td>
                                        <td>
                                            <asp:TextBox runat="server" ID="txtXZdezclwzcs7" CssClass="srk7 number"></asp:TextBox>
                                        </td>
                                        <td>
                                            <asp:TextBox runat="server" ID="txtXZdszclwzcs7" CssClass="srk7 number"></asp:TextBox>
                                        </td>
                                        <td>
                                            <asp:TextBox runat="server" ID="txtXZdsizclwzcs7" CssClass="srk7 number"></asp:TextBox>
                                        </td>
                                        <td>
                                            <asp:TextBox runat="server" ID="txtXZsywcl7" CssClass="srk6"></asp:TextBox>
                                        </td>
                                        <td>
                                            <asp:TextBox runat="server" ID="txtXZwz7" CssClass="srk7 number"></asp:TextBox>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="tl bold" colspan="2">
                                            迟到人数
                                        </td>
                                        <td>
                                            <asp:TextBox runat="server" ID="txtXZdyzcdrs" CssClass="srk7 number"></asp:TextBox>
                                        </td>
                                        <td>
                                            <asp:TextBox runat="server" ID="txtXZdezcdrs" CssClass="srk7 number"></asp:TextBox>
                                        </td>
                                        <td>
                                            <asp:TextBox runat="server" ID="txtXZdszcdrs" CssClass="srk7 number"></asp:TextBox>
                                        </td>
                                        <td>
                                            <asp:TextBox runat="server" ID="txtXZdsizcdrs" CssClass="srk7 number"></asp:TextBox>
                                        </td>
                                        <td colspan="2">
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="tl bold" colspan="2">
                                            请假人数
                                        </td>
                                        <td>
                                            <asp:TextBox runat="server" ID="txtXZdyzqjrs" CssClass="srk7 number"></asp:TextBox>
                                        </td>
                                        <td>
                                            <asp:TextBox runat="server" ID="txtXZdezqjrs" CssClass="srk7 number"></asp:TextBox>
                                        </td>
                                        <td>
                                            <asp:TextBox runat="server" ID="txtXZdszqjrs" CssClass="srk7 number"></asp:TextBox>
                                        </td>
                                        <td>
                                            <asp:TextBox runat="server" ID="txtXZdsizqjrs" CssClass="srk7 number"></asp:TextBox>
                                        </td>
                                        <td colspan="2">
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="tl bold" colspan="2">
                                            旷工人数
                                        </td>
                                        <td>
                                            <asp:TextBox runat="server" ID="txtXZdyzkgrs" CssClass="srk7 number"></asp:TextBox>
                                        </td>
                                        <td>
                                            <asp:TextBox runat="server" ID="txtXZdezkgrs" CssClass="srk7 number"></asp:TextBox>
                                        </td>
                                        <td>
                                            <asp:TextBox runat="server" ID="txtXZdszkgrs" CssClass="srk7 number"></asp:TextBox>
                                        </td>
                                        <td>
                                            <asp:TextBox runat="server" ID="txtXZdsizkgrs" CssClass="srk7 number"></asp:TextBox>
                                        </td>
                                        <td colspan="2">
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="tl bold" colspan="2">
                                            出差培训人次
                                        </td>
                                        <td>
                                            <asp:TextBox runat="server" ID="txtXZdyzccpxrc" CssClass="srk7 number"></asp:TextBox>
                                        </td>
                                        <td>
                                            <asp:TextBox runat="server" ID="txtXZdezccpxrc" CssClass="srk7 number"></asp:TextBox>
                                        </td>
                                        <td>
                                            <asp:TextBox runat="server" ID="txtXZdszccpxrc" CssClass="srk7 number"></asp:TextBox>
                                        </td>
                                        <td>
                                            <asp:TextBox runat="server" ID="txtXZdsizccpxrc" CssClass="srk7 number"></asp:TextBox>
                                        </td>
                                        <td colspan="2">
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="tl bold" colspan="2">
                                            安全事故损失额
                                        </td>
                                        <td>
                                            <asp:TextBox runat="server" ID="txtXZdyzaqsgsse" CssClass="srk7 number"></asp:TextBox>
                                        </td>
                                        <td>
                                            <asp:TextBox runat="server" ID="txtXZdezaqsgsse" CssClass="srk7 number"></asp:TextBox>
                                        </td>
                                        <td>
                                            <asp:TextBox runat="server" ID="txtXZdszaqsgsse" CssClass="srk7 number"></asp:TextBox>
                                        </td>
                                        <td>
                                            <asp:TextBox runat="server" ID="txtXZdsizaqsgsse" CssClass="srk7 number"></asp:TextBox>
                                        </td>
                                        <td colspan="2">
                                        </td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                        <tr>
                            <td class="bg3 tr gray">
                                注：
                            </td>
                            <td class="gray">
                                车辆违章包含公司所有试驾车和自备车<br />
                                当周违章车辆包含上周未处理的违章车辆<br />
                                如有多辆，都要填写
                            </td>
                        </tr>
                        <%}else if(CurrentDep == DayReportDep.无忧产品){ %>
                        <tr>
                            <td class="bg3 tr" style="background-color: Orange;">
                                本月机油套餐来厂使用车辆数：
                            </td>
                            <td>
                                <asp:TextBox runat="server" ID="txtNXCPbyjytclcsycls" CssClass="srk6 tr number"></asp:TextBox><span class="gray pl10">当月售后来厂车辆中来使用此产品车辆个数</span>
                            </td>
                        </tr>
                        <tr>
                            <td class="bg3 tr" style="background-color: Orange;">
                                本月机油套餐使用金额：
                            </td>
                            <td>
                                <asp:TextBox runat="server" ID="txtNXCPbyjytcsyje" CssClass="srk6 tr number"></asp:TextBox><span class="gray pl10">当月来厂车辆中来使用此产品与售后结算（合同价）金额（元）</span>
                            </td>
                        </tr>
                        <tr>
                            <td class="bg3 tr" style="background-color: Orange;">
                                本月划痕无忧服务到期个数：
                            </td>
                            <td>
                                <asp:TextBox runat="server" ID="txtNXCPbyhhwyfwdqgs" CssClass="srk6 tr number"></asp:TextBox><span class="gray pl10">当月购买此产品到期的车辆个数</span>
                            </td>
                        </tr>
                        <tr>
                            <td class="bg3 tr" style="background-color: Orange;">
                                本月划痕无忧服务到期金额：
                            </td>
                            <td>
                                <asp:TextBox runat="server" ID="txtNXCPbyhhwyfwdqje" CssClass="srk6 tr number"></asp:TextBox><span class="gray pl10">当月购买此产品到期的金额（元）</span>
                            </td>
                        </tr>
                        <tr>
                            <td class="bg3 tr" style="background-color: Orange;">
                                本月划痕无忧服务到期内赔付个数：
                            </td>
                            <td>
                                <asp:TextBox runat="server" ID="txtNXCPbyhhwyfwdqnpfgs" CssClass="srk6 tr number"></asp:TextBox><span class="gray pl10">当月到期购买此产品数量内总赔付个数</span>
                            </td>
                        </tr>
                        <tr>
                            <td class="bg3 tr" style="background-color: Orange;">
                                本月划痕无忧服务到期内赔付金额：
                            </td>
                            <td>
                                <asp:TextBox runat="server" ID="txtNXCPbyhhwyfwdqnpfje" CssClass="srk6 tr number"></asp:TextBox><span class="gray pl10">当月到期购买此产品数量内赔付总金额（元）</span>
                            </td>
                        </tr>
                        <tr>
                            <td class="bg3 tr" style="background-color: Orange;">
                                本月玻璃无忧服务到期个数：
                            </td>
                            <td>
                                <asp:TextBox runat="server" ID="txtNXCPbyblwyfwdqgs" CssClass="srk6 tr number"></asp:TextBox><span class="gray pl10">当月购买此产品到期的车辆个数</span>
                            </td>
                        </tr>
                        <tr>
                            <td class="bg3 tr" style="background-color: Orange;">
                                本月玻璃无忧服务到期金额：
                            </td>
                            <td>
                                <asp:TextBox runat="server" ID="txtNXCPbyblwyfwdqje" CssClass="srk6 tr number"></asp:TextBox><span class="gray pl10">当月购买此产品到期的金额（元）</span>
                            </td>
                        </tr>
                        <tr>
                            <td class="bg3 tr" style="background-color: Orange;">
                                本月玻璃无忧服务到期内赔付个数：
                            </td>
                            <td>
                                <asp:TextBox runat="server" ID="txtNXCPbyblwyfwdqnpfgs" CssClass="srk6 tr number"></asp:TextBox><span class="gray pl10">当月到期购买此产品数量内总赔付个数</span>
                            </td>
                        </tr>
                        <tr>
                            <td class="bg3 tr" style="background-color: Orange;">
                                本月玻璃无忧服务到期内赔付金额：
                            </td>
                            <td>
                                <asp:TextBox runat="server" ID="txtNXCPbyblwyfwdqnpfje" CssClass="srk6 tr number"></asp:TextBox><span class="gray pl10">当月到期购买此产品数量内赔付总金额（元）</span>
                            </td>
                        </tr>
                        <tr>
                            <td class="bg3 tr" style="background-color: Orange;">
                                本月延保服务到期个数：
                            </td>
                            <td>
                                <asp:TextBox runat="server" ID="txtNXCPbyybfwdqgs" CssClass="srk6 tr number"></asp:TextBox><span class="gray pl10">当月购买此产品到期的车辆个数</span>
                            </td>
                        </tr>
                        <tr>
                            <td class="bg3 tr" style="background-color: Orange;">
                                本月延保服务到期金额：
                            </td>
                            <td>
                                <asp:TextBox runat="server" ID="txtNXCPbyybfwdqje" CssClass="srk6 tr number"></asp:TextBox><span class="gray pl10">当月购买此产品到期的金额（元）</span>
                            </td>
                        </tr>
                        <tr>
                            <td class="bg3 tr" style="background-color: Orange;">
                                本月延保服务到期内赔付个数：
                            </td>
                            <td>
                                <asp:TextBox runat="server" ID="txtNXCPbyybfwdqnpfgs" CssClass="srk6 tr number"></asp:TextBox><span class="gray pl10">当月到期购买此产品数量内总赔付个数</span>
                            </td>
                        </tr>
                        <tr>
                            <td class="bg3 tr" style="background-color: Orange;">
                                本月延保服务到期内赔付金额：
                            </td>
                            <td>
                                <asp:TextBox runat="server" ID="txtNXCPbyybfwdqnpfje" CssClass="srk6 tr number"></asp:TextBox><span class="gray pl10">当月到期购买此产品数量内赔付总金额（元）</span>
                            </td>
                        </tr>
                        <%}else if(CurrentDep == DayReportDep.客服部 && HasMonthlyTargetPower()){ %>
                        <tr>
                            <td class="bg3 tr">
                                每月月底填写：
                            </td>
                            <td>
                                <table border="0" cellspacing="0" cellpadding="0" class="datatable">
                                    <tr class="tc bold" style="background-color: Orange;">
                                        <td class="w120">
                                            类型
                                        </td>
                                        <td class="w60">
                                            1年内
                                        </td>
                                        <td class="w60">
                                            2年内
                                        </td>
                                        <td class="w60">
                                            3年内
                                        </td>
                                        <td class="w60">
                                            3-5年内
                                        </td>
                                        <td class="w60">
                                            5年以上
                                        </td>
                                    </tr>
                                    <tr style="background-color: Yellow;">
                                        <td class="tc bold" style="background-color: Orange;">
                                            基盘保有客户量
                                        </td>
                                        <td>
                                            <asp:TextBox runat="server" ID="txtKFjpbylhl1year" CssClass="srk7 number"></asp:TextBox>
                                        </td>
                                        <td>
                                            <asp:TextBox runat="server" ID="txtKFjpbylhl2year" CssClass="srk7 number"></asp:TextBox>
                                        </td>
                                        <td>
                                            <asp:TextBox runat="server" ID="txtKFjpbylhl3year" CssClass="srk7 number"></asp:TextBox>
                                        </td>
                                        <td>
                                            <asp:TextBox runat="server" ID="txtKFjpbylhl3_5year" CssClass="srk7 number"></asp:TextBox>
                                        </td>
                                        <td>
                                            <asp:TextBox runat="server" ID="txtKFjpbylhl5year" CssClass="srk7 number"></asp:TextBox>
                                        </td>
                                    </tr>
                                    <tr style="background-color: Yellow;">
                                        <td class="tc bold" style="background-color: Orange;">
                                            有效管理内客户量
                                        </td>
                                        <td>
                                            <asp:TextBox runat="server" ID="txtKFyxglnkhl1year" CssClass="srk7 number"></asp:TextBox>
                                        </td>
                                        <td>
                                            <asp:TextBox runat="server" ID="txtKFyxglnkhl2year" CssClass="srk7 number"></asp:TextBox>
                                        </td>
                                        <td>
                                            <asp:TextBox runat="server" ID="txtKFyxglnkhl3year" CssClass="srk7 number"></asp:TextBox>
                                        </td>
                                        <td>
                                            <asp:TextBox runat="server" ID="txtKFyxglnkhl3_5year" CssClass="srk7 number"></asp:TextBox>
                                        </td>
                                        <td>
                                            <asp:TextBox runat="server" ID="txtKFyxglnkhl5year" CssClass="srk7 number"></asp:TextBox>
                                        </td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                        <%} %>
                        <%=GetTableStr()%>
                    </tbody>
                </table>
            </ContentTemplate>
        </asp:UpdatePanel>
        <table width="100%" border="0" cellspacing="0" cellpadding="0" class="biaoge3">
            <tbody>
                <tr>
                    <td class="bg1">
                    </td>
                    <td>
                        <asp:Button runat="server" ID="btnSubmit" CssClass="an1" OnClick="btnSubmit_Click"
                            Text="提交" />
                        <span class="gray">（</span><span class="red">所有项只能填写数字，不需要填写单位，</span><span class="gray">提交后将不能修改，请仔细核对数据后再提交）</span>
                        <br />
                        <span id="spMsg" class="red"></span>
                    </td>
                </tr>
            </tbody>
        </table>
    </div>
    </form>
</body>
</html>
