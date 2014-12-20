<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="dayreportuseredit.aspx.cs"
    Inherits="Hx.BackAdmin.dayreport.dayreportuseredit" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>日报用户权限设置</title>
    <link href="../css/admin.css" rel="stylesheet" type="text/css" />
    <script src="../js/jquery-1.3.2.min.js" type="text/javascript"></script>
    <script language="javascript" type="text/javascript">
        $(function () {
            $(".cbxDayReportDep").click(function () {
                $(this).parent().parent().parent().find(".cbxModule").attr("checked", $(this).attr("checked"));
                setmodulevalue();
                setdayreportdep();
            });
            $(".cbxModule").click(function () {
                setmodulevalue();

                $(this).parent().parent().parent().find(".cbxDayReportDep").each(function () {
                    var dayreportdep = $(this);
//                    dayreportdep.attr("checked", false);
                    $(this).parent().parent().parent().find(".cbxModule").each(function () {
                        if ($(this).attr("checked"))
                            dayreportdep.attr("checked", true);
                    });
                });

                setdayreportdep();
            });
            $("#cbxAllModule").click(function () {
                $(".cbxDayReportDep").attr("checked", $(this).attr("checked"));
                $(".cbxModule").attr("checked", $(this).attr("checked"));
                setmodulevalue();
                setdayreportdep();
            });
            $("#cbxAllDayReportCorp").click(function () {
                $(".cbxDayReportCorp").attr("checked", $(this).attr("checked"));
                setdayreportcorp();
            })
            $(".cbxDayReportCorp").click(function () {
                setdayreportcorp();
            });
            $(".cbxTargetDep").click(function () {
                settargetdep();
            });
            $("#cbxAllTargetDep").click(function () {
                $(".cbxTargetDep").attr("checked", $(this).attr("checked"));
                settargetdep();
            })
            $("#cbxAllTargetCorp").click(function () {
                $(".cbxTargetCorp").attr("checked", $(this).attr("checked"));
                settargetcorp();
            })
            $(".cbxTargetCorp").click(function () {
                settargetcorp();
            });
            $("#cbxAllDayReportViewCorp").click(function () {
                $(".cbxDayReportViewCorp").attr("checked", $(this).attr("checked"));
                setdayreportviewcorp();
            })
            $(".cbxDayReportViewCorp").click(function () {
                setdayreportviewcorp();
            });
            $("#cbxAllDayReportViewDep").click(function () {
                $(".cbxDayReportViewDep").attr("checked", $(this).attr("checked"));
                setdayreportviewdep();
            })
            $(".cbxDayReportViewDep").click(function () {
                setdayreportviewdep();
            });
            $("#cbxAllCRMReportInput").click(function () {
                $(".cbxCRMReportInput").attr("checked", $(this).attr("checked"));
                setcrmreportinput();
            })
            $(".cbxCRMReportInput").click(function () {
                setcrmreportinput();
            });
        });

        function setdayreportdep() {
            var dayreportdep = $(".cbxDayReportDep:checked").map(function () {
                return $(this).val();
            }).get().join('|');
            if (dayreportdep != '')
                dayreportdep = '|' + dayreportdep + '|'
            $("#hdnDayReportDep").val(dayreportdep);
        }

        function setmodulevalue() {
            var module = $(".cbxModule:checked").map(function () {
                return $(this).val();
            }).get().join('|');
            if (module != '')
                module = '|' + module + '|'
            $("#hdnModule").val(module);
        }

        function settargetdep() {
            var targetdep = $(".cbxTargetDep:checked").map(function () {
                return $(this).val();
            }).get().join('|');
            if (targetdep != '')
                targetdep = '|' + targetdep + '|'
            $("#hdnTargetDep").val(targetdep);
        }

        function setdayreportcorp() {
            var targetcorp = $(".cbxDayReportCorp:checked").map(function () {
                return $(this).val();
            }).get().join('|');
            if (targetcorp != '')
                targetcorp = '|' + targetcorp + '|'
            $("#hdnDayReportCorp").val(targetcorp);
        }

        function settargetcorp() {
            var targetcorp = $(".cbxTargetCorp:checked").map(function () {
                return $(this).val();
            }).get().join('|');
            if (targetcorp != '')
                targetcorp = '|' + targetcorp + '|'
            $("#hdnTargetCorp").val(targetcorp);
        }

        function setdayreportviewcorp() {
            var dayreportviewcorp = $(".cbxDayReportViewCorp:checked").map(function () {
                return $(this).val();
            }).get().join('|');
            if (dayreportviewcorp != '')
                dayreportviewcorp = '|' + dayreportviewcorp + '|'
            $("#hdnDayReportViewCorp").val(dayreportviewcorp);
        }

        function setdayreportviewdep() {
            var dayreportviewdep = $(".cbxDayReportViewDep:checked").map(function () {
                return $(this).val();
            }).get().join('|');
            if (dayreportviewdep != '')
                dayreportviewdep = '|' + dayreportviewdep + '|'
            $("#hdnDayReportViewDep").val(dayreportviewdep);
        }

        function setcrmreportinput() {
            var crmreportinput = $(".cbxCRMReportInput:checked").map(function () {
                return $(this).val();
            }).get().join('|');
            if (crmreportinput != '')
                crmreportinput = '|' + crmreportinput + '|'
            $("#hdnCRMReportInput").val(crmreportinput);
        }
    </script>
    <style type="text/css">
    .clpp ul{display:inline-block;width:160px;*display:inline;*zoom:1;vertical-align:top;margin-left:3px;}
    .nh{color:White;font-weight:bold;font-size:18px;background:#222;text-indent:5px;}
    label{line-height: 18px;display:inline-block;*display:inline;*zoom:1;}
    </style>
</head>
<body>
    <form id="form1" runat="server">
    <div class="ht_main">
        <table width="100%" border="0" cellspacing="0" cellpadding="0" class="biaoge3">
            <caption class="bt2">
                日报用户权限设置</caption>
            <tbody>
                <tr>
                    <td class="tr">
                        用户名：
                    </td>
                    <td>
                        <%= CurrentUser.UserName%>
                    </td>
                </tr>
                <tr>
                    <td class="tr">
                        所属公司及部门：
                    </td>
                    <td>
                        <%= CurrentUser.CorporationName%>
                        <%= CurrentUser.DayReportDep.ToString()%>
                    </td>
                </tr>
                <tr>
                    <td class="tr">
                        数据修改权限：
                    </td>
                    <td>
                        <asp:CheckBox runat="server" ID="cbxAllowModify" Checked="false" />
                    </td>
                </tr>
                <tr>
                    <td class="tr">
                        月报汇总权限：
                    </td>
                    <td>
                        <asp:CheckBox runat="server" ID="cbxReportGather" Checked="false" />
                    </td>
                </tr>
                <tr>
                    <td class="vt tr">
                        录入栏目权限：
                    </td>
                    <td class="clpp">
                        <label style="line-height: 18px; display: inherit;">
                            <input type="checkbox" id="cbxAllModule" class="fll" />全选</label>
                        <asp:Repeater runat="server" ID="rptDepartment" OnItemDataBound="rptDepartment_ItemDataBound" EnableViewState="false">
                            <ItemTemplate>
                                <ul>
                                    <li class="nh">
                                        <label>
                                            <input type="checkbox" class="cbxDayReportDep" <%#SetDayReportDep(Eval("Value").ToString()) %>
                                                value="<%# Eval("Value") %>" /><%# Eval("Name") %></label></li>
                                    <asp:Repeater runat="server" ID="rptDayReportModule">
                                        <ItemTemplate>
                                            <li>
                                                <label>
                                                    <input type="checkbox" class="cbxModule fll" value="<%# Eval("ID") %>" <%#SetModule(Eval("ID").ToString()) %> /><%# Eval("Name") %></label></li>
                                        </ItemTemplate>
                                    </asp:Repeater>
                                </ul>
                            </ItemTemplate>
                            <FooterTemplate>
                                </ul>
                            </FooterTemplate>
                        </asp:Repeater>
                    </td>
                </tr>
                <tr>
                    <td class="tr">
                        录入公司权限：
                    </td>
                    <td class="clpp">
                        <ul>
                            <li class="nh">
                                <label>
                                    <input type="checkbox" id="cbxAllDayReportCorp" class="fll" />全选</label></li>
                        </ul>
                    </td>
                </tr>
                <tr>
                    <td>
                    </td>
                    <td>
                        <asp:Repeater runat="server" ID="rptDayReportCorp">
                            <HeaderTemplate>
                                <ul>
                            </HeaderTemplate>
                            <ItemTemplate>
                                <li class="blockinline" style="width:230px;">
                                    <label>
                                        <input type="checkbox" class="cbxDayReportCorp fll" value="<%# Eval("ID") %>" <%#SetDayReportCorp(Eval("ID").ToString()) %> /><%# Eval("Name") %></label></li>
                            </ItemTemplate>
                            <FooterTemplate>
                                </ul></FooterTemplate>
                        </asp:Repeater>
                    </td>
                </tr>
                <tr>
                    <td class="tr">
                        月度目标公司权限：
                    </td>
                    <td class="clpp">
                        <ul>
                            <li class="nh">
                                <label>
                                    <input type="checkbox" id="cbxAllTargetCorp" class="fll" />全选</label></li>
                        </ul>
                    </td>
                </tr>
                <tr>
                    <td>
                    </td>
                    <td>
                        <asp:Repeater runat="server" ID="rptTargetCorp" EnableViewState="false">
                            <HeaderTemplate>
                                <ul>
                            </HeaderTemplate>
                            <ItemTemplate>
                                <li class="blockinline" style="width:230px;">
                                    <label>
                                        <input type="checkbox" class="cbxTargetCorp fll" value="<%# Eval("ID") %>" <%#SetTargetCorp(Eval("ID").ToString()) %> /><%# Eval("Name") %></label></li>
                            </ItemTemplate>
                            <FooterTemplate>
                                </ul></FooterTemplate>
                        </asp:Repeater>
                    </td>
                </tr>
                <tr>
                    <td class="bg4 tr">
                        月度目标部门权限：
                    </td>
                    <td class="clpp">
                        <ul>
                            <li class="nh">
                                <label>
                                    <input type="checkbox" id="cbxAllTargetDep" class="fll" />全选</label></li>
                        </ul>
                    </td>
                </tr>
                <tr>
                    <td>
                    </td>
                    <td>
                        <asp:Repeater runat="server" ID="rptTargetDep" EnableViewState="false">
                            <HeaderTemplate>
                                <ul>
                            </HeaderTemplate>
                            <ItemTemplate>
                                <li class="blockinline">
                                    <label>
                                        <input type="checkbox" class="cbxTargetDep fll" value="<%# Eval("Value") %>" <%#SetTargetDep(Eval("Value").ToString()) %> /><%# Eval("Name") %></label></li>
                            </ItemTemplate>
                            <FooterTemplate>
                                </ul>
                            </FooterTemplate>
                        </asp:Repeater>
                    </td>
                </tr>
                <tr>
                    <td class="bg4 tr vt">
                        月报查询公司权限：
                    </td>
                    <td class="clpp">
                        <ul>
                            <li class="nh">
                                <label>
                                    <input type="checkbox" id="cbxAllDayReportViewCorp" class="fll" />全选</label></li>
                        </ul>
                    </td>
                </tr>
                <tr>
                    <td>
                    </td>
                    <td>
                        <asp:Repeater runat="server" ID="rptDayReportViewCorp" EnableTheming="false">
                            <HeaderTemplate>
                                <ul>
                            </HeaderTemplate>
                            <ItemTemplate>
                                <li class="blockinline" style="width:230px;">
                                    <label>
                                        <input type="checkbox" class="cbxDayReportViewCorp fll" value="<%# Eval("ID") %>"
                                            <%#SetDayReportViewCorp(Eval("ID").ToString()) %> /><%# Eval("Name") %></label></li>
                            </ItemTemplate>
                            <FooterTemplate>
                                </ul></FooterTemplate>
                        </asp:Repeater>
                    </td>
                </tr>
                <tr>
                    <td class="bg4 tr vt">
                        月报查询部门权限：
                    </td>
                    <td class="clpp">
                        <ul>
                            <li class="nh">
                                <label>
                                    <input type="checkbox" id="cbxAllDayReportViewDep" class="fll" />全选</label></li>
                        </ul>
                    </td>
                </tr>
                <tr>
                    <td>
                    </td>
                    <td>
                        <asp:Repeater runat="server" ID="rptDayReportViewDep" EnableViewState="false">
                            <HeaderTemplate>
                                <ul>
                            </HeaderTemplate>
                            <ItemTemplate>
                                <li class="blockinline">
                                    <label>
                                        <input type="checkbox" class="cbxDayReportViewDep fll" value="<%# Eval("Value") %>"
                                            <%#SetDayReportViewDep(Eval("Value").ToString()) %> /><%# Eval("Name") %></label></li>
                            </ItemTemplate>
                            <FooterTemplate>
                                </ul>
                            </FooterTemplate>
                        </asp:Repeater>
                    </td>
                </tr>
                <tr>
                    <td class="tr">
                        CRM报表导出权限：
                    </td>
                    <td>
                        <asp:CheckBox runat="server" ID="cbxCRMReportExport" Checked="false" />
                    </td>
                </tr>
                <tr>
                    <td class="bg4 tr vt">
                        CRM报表录入权限：
                    </td>
                    <td class="clpp">
                        <ul>
                            <li class="nh">
                                <label>
                                    <input type="checkbox" id="cbxAllCRMReportInput" class="fll" />全选</label></li>
                        </ul>
                    </td>
                </tr>
                <tr>
                    <td>
                    </td>
                    <td>
                        <asp:Repeater runat="server" ID="rptCRMReportInput" EnableViewState="false">
                            <HeaderTemplate>
                                <ul>
                            </HeaderTemplate>
                            <ItemTemplate>
                                <li class="blockinline">
                                    <label>
                                        <input type="checkbox" class="cbxCRMReportInput fll" value="<%# Eval("Value") %>"
                                            <%#SetCRMReportInput(Eval("Value").ToString()) %> /><%# Eval("Text") %></label></li>
                            </ItemTemplate>
                            <FooterTemplate>
                                </ul>
                            </FooterTemplate>
                        </asp:Repeater>
                    </td>
                </tr>
                <tr>
                    <td>
                    </td>
                    <td>
                        <input type="hidden" runat="server" id="hdnModule" />
                        <input type="hidden" runat="server" id="hdnDayReportDep" />
                        <input type="hidden" runat="server" id="hdnDayReportCorp" />
                        <input type="hidden" runat="server" id="hdnTargetDep" />
                        <input type="hidden" runat="server" id="hdnTargetCorp" />
                        <input type="hidden" runat="server" id="hdnDayReportViewCorp" />
                        <input type="hidden" runat="server" id="hdnDayReportViewDep" />
                        <input type="hidden" runat="server" id="hdnCRMReportInput" />
                        <asp:Button runat="server" ID="btnSubmit" Text="保存" CssClass="an1" OnClick="btnSubmit_Click" />
                    </td>
                </tr>
            </tbody>
        </table>
    </div>
    </form>
</body>
</html>
